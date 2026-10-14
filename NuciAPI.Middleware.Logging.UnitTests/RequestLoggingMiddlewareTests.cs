using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using Moq;

using NuciLog.Core;

using NUnit.Framework;

namespace NuciAPI.Middleware.Logging.UnitTests
{
    [TestFixture]
    public sealed class RequestLoggingMiddlewareTests
    {
        private const int LastSuccessfulStatusCode = 299;
        private const int FirstNonSuccessfulStatusCode = 300;

        private Mock<ILogger> logger = null!;
        private IHost host = null!;
        private HttpClient client = null!;

        [SetUp]
        public async Task SetUp()
        {
            logger = new Mock<ILogger>();
            host = CreateHost(async context =>
            {
                await context.Response.WriteAsync("OK");
            });
            await host.StartAsync();
            client = host.GetTestClient();
        }

        [TearDown]
        public async Task TearDown()
        {
            client?.Dispose();
            if (host != null)
            {
                await host.StopAsync();
                host.Dispose();
            }
        }

        private IHost CreateHost(RequestDelegate terminalMiddleware)
        {
            return Host.CreateDefaultBuilder()
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseTestServer()
                    .ConfigureServices(services =>
                    {
                        services.AddSingleton(logger.Object);
                    })
                    .Configure(app =>
                    {
                        app.UseNuciApiRequestLogging();
                        app.Run(terminalMiddleware);
                    });
                })
                .Build();
        }

        private async Task ReplaceHostAsync(RequestDelegate terminalMiddleware)
        {
            await host.StopAsync();
            host.Dispose();
            client.Dispose();

            host = CreateHost(terminalMiddleware);
            await host.StartAsync();
            client = host.GetTestClient();
        }

        private async Task<HttpResponseMessage> SendRequestAsync(string method, string path, string body = "")
        {
            HttpRequestMessage request = new(new HttpMethod(method), path)
            {
                Content = string.IsNullOrEmpty(body) ? null : new StringContent(body)
            };
            request.Headers.Add("X-Forwarded-For", "203.0.113.50");
            return await client.SendAsync(request);
        }

        [Test]
        public async Task GivenASuccessfulRequest_WhenInvokingMiddleware_ThenStartedAndSuccessAreLogged()
        {
            using HttpResponseMessage response = await SendRequestAsync("POST", "/orders?page=1");

            Assert.Multiple(() =>
            {
                Assert.That(response.StatusCode, Is.EqualTo(System.Net.HttpStatusCode.OK));
                Assert.That(logger.Invocations.Count(x => x.Method.Name == nameof(ILogger.Info)), Is.EqualTo(2));
                Assert.That(logger.Invocations.Count(x => x.Method.Name == nameof(ILogger.Error)), Is.EqualTo(0));
            });
        }

        [Test]
        public async Task GivenAResponseWithTheLastSuccessfulStatusCode_WhenInvokingMiddleware_ThenSuccessIsLogged()
        {
            await ReplaceHostAsync(async context =>
            {
                context.Response.StatusCode = LastSuccessfulStatusCode;
                await context.Response.WriteAsync("OK");
            });

            using HttpResponseMessage response = await SendRequestAsync("GET", "/orders");

            Assert.Multiple(() =>
            {
                Assert.That(response.StatusCode, Is.EqualTo((System.Net.HttpStatusCode)LastSuccessfulStatusCode));
                Assert.That(logger.Invocations.Count(x => x.Method.Name == nameof(ILogger.Info)), Is.EqualTo(2));
                Assert.That(logger.Invocations.Count(x => x.Method.Name == nameof(ILogger.Error)), Is.EqualTo(0));
            });
        }

        [TestCase(200)]
        [TestCase(201)]
        [TestCase(202)]
        [TestCase(204)]
        [TestCase(206)]
        [TestCase(299)]
        public async Task GivenAResponseWithASuccessStatusCode_WhenInvokingMiddleware_ThenSuccessIsLogged(int statusCode)
        {
            await ReplaceHostAsync(async context =>
            {
                context.Response.StatusCode = statusCode;
                await context.Response.WriteAsync("Success");
            });

            using HttpResponseMessage response = await SendRequestAsync("GET", "/status");

            Assert.Multiple(() =>
            {
                Assert.That((int)response.StatusCode, Is.EqualTo(statusCode));
                Assert.That(logger.Invocations.Count(x => x.Method.Name == nameof(ILogger.Info)), Is.EqualTo(2));
                Assert.That(logger.Invocations.Count(x => x.Method.Name == nameof(ILogger.Error)), Is.EqualTo(0));
            });
        }

        [TestCase(100)]
        [TestCase(199)]
        [TestCase(300)]
        [TestCase(301)]
        [TestCase(307)]
        [TestCase(400)]
        [TestCase(404)]
        [TestCase(418)]
        [TestCase(499)]
        [TestCase(500)]
        [TestCase(503)]
        [TestCase(599)]
        public async Task GivenAResponseWithANonSuccessStatusCode_WhenInvokingMiddleware_ThenFailureIsLogged(int statusCode)
        {
            await ReplaceHostAsync(async context =>
            {
                context.Response.StatusCode = statusCode;
                await context.Response.WriteAsync("Failure");
            });

            using HttpResponseMessage response = await SendRequestAsync("GET", "/status");

            Assert.Multiple(() =>
            {
                Assert.That((int)response.StatusCode, Is.EqualTo(statusCode));
                Assert.That(logger.Invocations.Count(x => x.Method.Name == nameof(ILogger.Info)), Is.EqualTo(1));
                Assert.That(logger.Invocations.Count(x => x.Method.Name == nameof(ILogger.Error)), Is.EqualTo(1));
            });
        }

        [TestCase("GET")]
        [TestCase("POST")]
        [TestCase("PUT")]
        [TestCase("PATCH")]
        [TestCase("DELETE")]
        [TestCase("HEAD")]
        [TestCase("OPTIONS")]
        [TestCase("TRACE")]
        public async Task GivenARequestWithACommonHttpMethod_WhenInvokingMiddleware_ThenTheRequestSucceeds(string method)
        {
            using HttpResponseMessage response = await SendRequestAsync(method, "/methods");

            Assert.Multiple(() =>
            {
                Assert.That(response.StatusCode, Is.EqualTo(System.Net.HttpStatusCode.OK));
                Assert.That(logger.Invocations.Count(x => x.Method.Name == nameof(ILogger.Info)), Is.EqualTo(2));
                Assert.That(logger.Invocations.Count(x => x.Method.Name == nameof(ILogger.Error)), Is.EqualTo(0));
            });
        }

        [TestCase("/")]
        [TestCase("/orders")]
        [TestCase("/orders/123")]
        [TestCase("/orders/123/items")]
        [TestCase("/orders?status=pending")]
        [TestCase("/orders?page=1&pageSize=100")]
        [TestCase("/search?q=hello%20world")]
        public async Task GivenARequestWithAValidPath_WhenInvokingMiddleware_ThenTheRequestSucceeds(string path)
        {
            using HttpResponseMessage response = await SendRequestAsync("GET", path);

            Assert.Multiple(() =>
            {
                Assert.That(response.StatusCode, Is.EqualTo(System.Net.HttpStatusCode.OK));
                Assert.That(logger.Invocations.Count(x => x.Method.Name == nameof(ILogger.Info)), Is.EqualTo(2));
                Assert.That(logger.Invocations.Count(x => x.Method.Name == nameof(ILogger.Error)), Is.EqualTo(0));
            });
        }

        [TestCase("")]
        [TestCase("a")]
        [TestCase("order-id=123&status=pending")]
        [TestCase("{\"name\":\"Ada\"}")]
        [TestCase("line1\nline2")]
        public async Task GivenARequestWithARequestBody_WhenInvokingMiddleware_ThenTheRequestSucceeds(string body)
        {
            using HttpResponseMessage response = await SendRequestAsync("POST", "/orders", body);

            Assert.Multiple(() =>
            {
                Assert.That(response.StatusCode, Is.EqualTo(System.Net.HttpStatusCode.OK));
                Assert.That(logger.Invocations.Count(x => x.Method.Name == nameof(ILogger.Info)), Is.EqualTo(2));
                Assert.That(logger.Invocations.Count(x => x.Method.Name == nameof(ILogger.Error)), Is.EqualTo(0));
            });
        }

        [Test]
        public async Task GivenADownstreamException_WhenInvokingMiddleware_ThenFailureIsLoggedAndExceptionIsRethrown()
        {
            await ReplaceHostAsync(async context =>
            {
                throw new InvalidOperationException("boom");
            });

            InvalidOperationException exception = (await Assert.ThrowsAsync<InvalidOperationException>(async () => await SendRequestAsync("POST", "/orders?page=1")))!;

            Assert.That(exception.Message, Is.EqualTo("boom"));

            Assert.Multiple(() =>
            {
                Assert.That(logger.Invocations.Count(x => x.Method.Name == nameof(ILogger.Info)), Is.EqualTo(1));
                Assert.That(logger.Invocations.Count(x => x.Method.Name == nameof(ILogger.Error)), Is.EqualTo(1));
            });
        }

        [Test]
        public async Task GivenANonSuccessfulStatusCode_WhenInvokingMiddleware_ThenFailureIsLogged()
        {
            await ReplaceHostAsync(async context =>
            {
                context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                await context.Response.WriteAsync("Error");
            });

            using HttpResponseMessage response = await SendRequestAsync("POST", "/orders?page=1");

            Assert.Multiple(() =>
            {
                Assert.That(response.StatusCode, Is.EqualTo(System.Net.HttpStatusCode.InternalServerError));
                Assert.That(logger.Invocations.Count(x => x.Method.Name == nameof(ILogger.Info)), Is.EqualTo(1));
                Assert.That(logger.Invocations.Count(x => x.Method.Name == nameof(ILogger.Error)), Is.EqualTo(1));
            });
        }

        [Test]
        public async Task GivenAResponseWithTheFirstNonSuccessfulStatusCode_WhenInvokingMiddleware_ThenFailureIsLogged()
        {
            await ReplaceHostAsync(async context =>
            {
                context.Response.StatusCode = FirstNonSuccessfulStatusCode;
                await context.Response.WriteAsync("Redirect");
            });

            using HttpResponseMessage response = await SendRequestAsync("GET", "/orders");

            Assert.Multiple(() =>
            {
                Assert.That(response.StatusCode, Is.EqualTo(System.Net.HttpStatusCode.MultipleChoices));
                Assert.That(logger.Invocations.Count(x => x.Method.Name == nameof(ILogger.Info)), Is.EqualTo(1));
                Assert.That(logger.Invocations.Count(x => x.Method.Name == nameof(ILogger.Error)), Is.EqualTo(1));
            });
        }
    }
}
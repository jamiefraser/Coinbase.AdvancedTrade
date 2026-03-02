using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Coinbase.AdvancedTrade.Middleware
{
    /// <summary>
    /// Middleware for intercepting and modifying HTTP requests.
    /// </summary>
    public sealed class RequestMiddlewarePipeline
    {
        private readonly List<IRequestMiddleware> _middlewares;

        /// <summary>
        /// Initializes a new instance of the <see cref="RequestMiddlewarePipeline"/> class.
        /// </summary>
        public RequestMiddlewarePipeline()
        {
            _middlewares = new List<IRequestMiddleware>();
        }

        /// <summary>
        /// Adds middleware to the pipeline.
        /// </summary>
        /// <param name="middleware">The middleware to add.</param>
        public void Use(IRequestMiddleware middleware)
        {
            if (middleware == null)
                throw new ArgumentNullException(nameof(middleware));

            _middlewares.Add(middleware);
        }

        /// <summary>
        /// Executes the middleware pipeline for a request.
        /// </summary>
        /// <param name="context">The request context.</param>
        public async Task ExecuteAsync(RequestContext context)
        {
            foreach (var middleware in _middlewares)
            {
                await middleware.OnRequestAsync(context).ConfigureAwait(false);

                if (context.IsShortCircuited)
                    break;
            }
        }

        /// <summary>
        /// Executes the middleware pipeline for a response.
        /// </summary>
        /// <param name="context">The response context.</param>
        public async Task ExecuteResponseAsync(ResponseContext context)
        {
            for (var i = _middlewares.Count - 1; i >= 0; i--)
            {
                await _middlewares[i].OnResponseAsync(context).ConfigureAwait(false);
            }
        }
    }

    /// <summary>
    /// Interface for request middleware.
    /// </summary>
    public interface IRequestMiddleware
    {
        /// <summary>
        /// Called before a request is sent.
        /// </summary>
        /// <param name="context">The request context.</param>
        Task OnRequestAsync(RequestContext context);

        /// <summary>
        /// Called after a response is received.
        /// </summary>
        /// <param name="context">The response context.</param>
        Task OnResponseAsync(ResponseContext context);
    }

    /// <summary>
    /// Context for a request.
    /// </summary>
    public sealed class RequestContext
    {
        /// <summary>
        /// Gets or sets the HTTP method.
        /// </summary>
        public string Method { get; set; }

        /// <summary>
        /// Gets or sets the request path.
        /// </summary>
        public string Path { get; set; }

        /// <summary>
        /// Gets or sets the request headers.
        /// </summary>
        public Dictionary<string, string> Headers { get; set; }

        /// <summary>
        /// Gets or sets the request body.
        /// </summary>
        public object Body { get; set; }

        /// <summary>
        /// Gets or sets whether the pipeline should be short-circuited.
        /// </summary>
        public bool IsShortCircuited { get; set; }

        /// <summary>
        /// Gets the request metadata.
        /// </summary>
        public Dictionary<string, object> Metadata { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="RequestContext"/> class.
        /// </summary>
        public RequestContext()
        {
            Headers = new Dictionary<string, string>();
            Metadata = new Dictionary<string, object>();
        }
    }

    /// <summary>
    /// Context for a response.
    /// </summary>
    public sealed class ResponseContext
    {
        /// <summary>
        /// Gets or sets the request context.
        /// </summary>
        public RequestContext Request { get; set; }

        /// <summary>
        /// Gets or sets the HTTP status code.
        /// </summary>
        public int StatusCode { get; set; }

        /// <summary>
        /// Gets or sets the response headers.
        /// </summary>
        public Dictionary<string, string> Headers { get; set; }

        /// <summary>
        /// Gets or sets the response body.
        /// </summary>
        public object Body { get; set; }

        /// <summary>
        /// Gets or sets whether the response contains an error.
        /// </summary>
        public bool IsError { get; set; }

        /// <summary>
        /// Gets or sets the error exception, if any.
        /// </summary>
        public Exception Error { get; set; }

        /// <summary>
        /// Gets the response metadata.
        /// </summary>
        public Dictionary<string, object> Metadata { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ResponseContext"/> class.
        /// </summary>
        public ResponseContext()
        {
            Headers = new Dictionary<string, string>();
            Metadata = new Dictionary<string, object>();
        }
    }

    /// <summary>
    /// Middleware for logging requests and responses.
    /// </summary>
    public sealed class LoggingMiddleware : IRequestMiddleware
    {
        private readonly Action<string> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="LoggingMiddleware"/> class.
        /// </summary>
        /// <param name="logger">The logging action.</param>
        public LoggingMiddleware(Action<string> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public Task OnRequestAsync(RequestContext context)
        {
            _logger($"Request: {context.Method} {context.Path}");
            return Task.CompletedTask;
        }

        public Task OnResponseAsync(ResponseContext context)
        {
            _logger($"Response: {context.StatusCode} for {context.Request.Method} {context.Request.Path}");
            return Task.CompletedTask;
        }
    }

    /// <summary>
    /// Middleware for adding custom headers to requests.
    /// </summary>
    public sealed class HeadersMiddleware : IRequestMiddleware
    {
        private readonly Dictionary<string, string> _headers;

        /// <summary>
        /// Initializes a new instance of the <see cref="HeadersMiddleware"/> class.
        /// </summary>
        /// <param name="headers">The headers to add.</param>
        public HeadersMiddleware(Dictionary<string, string> headers)
        {
            _headers = headers ?? throw new ArgumentNullException(nameof(headers));
        }

        public Task OnRequestAsync(RequestContext context)
        {
            foreach (var header in _headers)
            {
                context.Headers[header.Key] = header.Value;
            }

            return Task.CompletedTask;
        }

        public Task OnResponseAsync(ResponseContext context)
        {
            return Task.CompletedTask;
        }
    }

    /// <summary>
    /// Middleware for measuring request performance.
    /// </summary>
    public sealed class PerformanceMiddleware : IRequestMiddleware
    {
        private readonly Action<string, TimeSpan> _metricsCallback;

        /// <summary>
        /// Initializes a new instance of the <see cref="PerformanceMiddleware"/> class.
        /// </summary>
        /// <param name="metricsCallback">Callback for performance metrics.</param>
        public PerformanceMiddleware(Action<string, TimeSpan> metricsCallback)
        {
            _metricsCallback = metricsCallback ?? throw new ArgumentNullException(nameof(metricsCallback));
        }

        public Task OnRequestAsync(RequestContext context)
        {
            context.Metadata["StartTime"] = DateTime.UtcNow;
            return Task.CompletedTask;
        }

        public Task OnResponseAsync(ResponseContext context)
        {
            if (context.Request.Metadata.TryGetValue("StartTime", out var startTimeObj) && 
                startTimeObj is DateTime startTime)
            {
                var duration = DateTime.UtcNow - startTime;
                _metricsCallback(context.Request.Path, duration);
            }

            return Task.CompletedTask;
        }
    }
}

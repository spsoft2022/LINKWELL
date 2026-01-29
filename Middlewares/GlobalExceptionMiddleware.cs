using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Net;

namespace LinkwellProductionSystem.Middlewares
{
    public class GlobalExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<GlobalExceptionMiddleware> _logger;
        private readonly IHostEnvironment _env;

        public GlobalExceptionMiddleware(
            RequestDelegate next,
            ILogger<GlobalExceptionMiddleware> logger,
            IHostEnvironment env)
        {
            _next = next;
            _logger = logger;
            _env = env;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unhandled exception");

                await WriteProblemDetailsAsync(context, ex);
            }
        }

        private async Task WriteProblemDetailsAsync(
            HttpContext context,
            Exception exception)
        {
            var problem = CreateProblemDetails(context, exception);

            context.Response.StatusCode = problem.Status ?? StatusCodes.Status500InternalServerError;
            context.Response.ContentType = "application/problem+json";

            await context.Response.WriteAsJsonAsync(problem);
        }

        private ProblemDetails CreateProblemDetails(
            HttpContext context,
            Exception exception)
        {
            var problem = new ProblemDetails
            {
                Instance = context.Request.Path
            };

            switch (exception)
            {
                case SqlException sqlEx:
                    problem.Status = StatusCodes.Status500InternalServerError;
                    problem.Title = "Database error";
                    problem.Detail = _env.IsDevelopment()
                        ? sqlEx.Message
                        : "A database error occurred.";
                    problem.Type = "https://httpstatuses.com/500";
                    break;

                case UnauthorizedAccessException:
                    problem.Status = StatusCodes.Status401Unauthorized;
                    problem.Title = "Unauthorized";
                    problem.Detail = "You are not authorized to perform this action.";
                    problem.Type = "https://httpstatuses.com/401";
                    break;

                case ArgumentException argEx:
                    problem.Status = StatusCodes.Status400BadRequest;
                    problem.Title = "Invalid request";
                    problem.Detail = argEx.Message;
                    problem.Type = "https://httpstatuses.com/400";
                    break;

                default:
                    problem.Status = StatusCodes.Status500InternalServerError;
                    problem.Title = "Internal Server Error";
                    problem.Detail = _env.IsDevelopment()
                        ? exception.Message
                        : "An unexpected error occurred.";
                    problem.Type = "https://httpstatuses.com/500";
                    break;
            }

            // RFC 7807 allows extensions
            problem.Extensions["traceId"] = context.TraceIdentifier;
            problem.Extensions["timestamp"] = DateTime.UtcNow;

            return problem;
        }
    }
}

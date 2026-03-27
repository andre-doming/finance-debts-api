namespace finance.debts.api.Middlewares
{
    public class CorrelationIdMiddleware
    {
        private readonly RequestDelegate _next;

        public CorrelationIdMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            var header = context.Request.Headers["correlationId"];

            if (!Guid.TryParse(header, out var correlationId))
            {
                correlationId = Guid.NewGuid();
            }

            context.Items["CorrelationId"] = correlationId;

            await _next(context);
        }
    }
}

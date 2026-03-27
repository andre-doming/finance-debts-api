using Microsoft.AspNetCore.Mvc;
using finance.debts.api.Services;

namespace finance.debts.api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DebtsController : ControllerBase
    {
        private readonly DebtService _service;
        private readonly ILogger<DebtsController> _logger;

        public DebtsController(DebtService service, ILogger<DebtsController> logger)
        {
            _service = service;
            _logger = logger;
        }

        [HttpPost("{id}/process")]
        public async Task<IActionResult> ProcessDebt(int id)
        {
            /* var correlationIdHeader = HttpContext.Request.Headers["correlationId"];

            Guid correlationId;

            if (!string.IsNullOrEmpty(correlationIdHeader) && Guid.TryParse(correlationIdHeader, out var parsed))
                correlationId = parsed;
            else
                correlationId = Guid.NewGuid();*/

            var correlationId = (Guid)HttpContext.Items["CorrelationId"];

            _logger.LogInformation("CorrelationId recebido: {CorrelationId}", correlationId);

            var result = await _service.ProcessDebtAsync(id, correlationId);

            return Ok(new
            {
                message = result,
                debtId = id
            });
        }
    }
}
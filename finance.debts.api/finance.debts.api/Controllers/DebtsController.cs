using Microsoft.AspNetCore.Mvc;
using finance.debts.api.Services;

namespace finance.debts.api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DebtsController : ControllerBase
{
    private readonly DebtService _service;

    public DebtsController(DebtService service)
    {
        _service = service;
    }

    [HttpPost("{id}/process")]
    public async Task<IActionResult> ProcessDebt(int id)
    {
        var result = await _service.ProcessDebtAsync(id);

        return Ok(new
        {
            message = result,
            debtId = id
        });
    }
}
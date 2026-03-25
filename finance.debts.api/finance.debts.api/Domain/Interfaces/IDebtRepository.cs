using finance.debts.api.Domain.Entities;

namespace finance.debts.api.Domain.Interfaces;

public interface IDebtRepository
{
    Task<Debt?> GetByIdAsync(int id);
    Task UpdateAsync(Debt debt);
}
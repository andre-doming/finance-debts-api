using finance.debts.api.Domain.Entities;

namespace finance.debts.api.Domain.Interfaces
{
    public interface IProcessingLogRepository
    {
        Task AddAsync(ProcessingLog log);
    }
}

using finance.debts.domain.Entities;
using finance.debts.domain.Interfaces;

namespace finance.debts.api.Services
{
    public class DebtService
    {
        private readonly IDebtRepository _repository;

        public DebtService(IDebtRepository repository)
        {
            _repository = repository;
        }

        private readonly IProcessingLogRepository _logRepository;
        public DebtService(IDebtRepository repository, IProcessingLogRepository logRepository)
        {
            _repository = repository;
            _logRepository = logRepository;
        }

        public async Task<string> ProcessDebtAsync(int id, Guid correlationId)
        {
            try
            {
                if (id <= 0)
                    throw new ArgumentException("DebtId inválido");

                var debt = await _repository.GetByIdAsync(id);

                if (debt is null)
                    throw new KeyNotFoundException("Dívida não encontrada");

                if (correlationId == Guid.Empty)
                    throw new ArgumentException("CorrelationId é obrigatório");

                debt.Process(correlationId);

                var updated = await _repository.TryProcessAsync(debt);

                if (!updated)
                {
                    return $"Debt {id} already processed";
                }

                await _logRepository.AddAsync(new ProcessingLog
                {
                    DebtId = debt.DebtId,
                    StatusId = (int)debt.StatusId,
                    Message = "Divida processada com sucesso",
                    CorrelationId = correlationId,
                    CreatedAt = DateTime.UtcNow
                });

                return $"Debt {id} processed successfully";
            }
            catch (Exception ex)
            {
                await _logRepository.AddAsync(new ProcessingLog
                {
                    DebtId = id,
                    StatusId = -1,
                    Message = ex.Message,
                    CreatedAt = DateTime.UtcNow
                });

                throw;
            }
        }
    }
}
using finance.debts.api.Domain.Entities;
using finance.debts.api.Domain.Enums;
using finance.debts.api.Domain.Interfaces;

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

        public async Task<string> ProcessDebtAsync(int id, Guid? correlationId)
        {
            try
            {
                if (id <= 0)
                    throw new ArgumentException("DebtId inválido");

                var debt = await _repository.GetByIdAsync(id);

                if (debt is null)
                    throw new KeyNotFoundException("Dívida não encontrada");

                // atualiza status
                debt.StatusId = ProcessingStatus.Processed;
                debt.AmountPaid= debt.AmountDue;
                debt.PaymentDate = DateTime.UtcNow;
                debt.CorrelationId = correlationId;

                // 💾 salva no banco
                //var updated = await _repository.TryProcessAsync(debt);

                //await _repository.UpdateAsync(debt);

                var updated = await _repository.TryProcessAsync(debt);

                if (!updated)
                {
                    return $"Debt {id} already processed";
                }

                // 🟡 👉 AQUI ENTRA O LOG
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

        //Service fazia tudo sozinho
        //Service → chama Repository → usa Domain
    }
}
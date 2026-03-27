using Dapper;
using finance.debts.api.Infrastructure.Data.Dtos;
using finance.debts.domain.Interfaces;
using finance.debts.domain.Entities;
using finance.debts.domain.Enums;
using Microsoft.Data.SqlClient;


namespace finance.debts.api.Infrastructure.Repositories
{

    public class DebtRepository : IDebtRepository
    {
        private readonly string _connectionString;

        public DebtRepository(IConfiguration config)
        {
            _connectionString = config.GetConnectionString("DefaultConnection");
        }

        public async Task<Debt?> GetByIdAsync(int id)
        {
            using var connection = new SqlConnection(_connectionString);

            var sql = @"
            SELECT 
                debt_id AS DebtId,
                client_id AS ClientId,
                amount_due AS AmountDue,
                status_id AS StatusId,
                created_at AS CreatedAt
            FROM Debts
            WHERE debt_id = @Id";

            var dto = await connection.QueryFirstOrDefaultAsync<DebtDto>(sql, new { Id = id });

            if (dto is null)
                return null;

            var debt = new Debt(
                dto.DebtId,
                dto.ClientId,
                dto.AmountDue,
                correlationId: null
            );

            if (dto.StatusId == (int)ProcessingStatus.Processed)
            {
                debt.Process(Guid.NewGuid());
            }

            return debt;
        }
        public async Task<bool> TryProcessAsync(Debt debt)
        {
            using var connection = new SqlConnection(_connectionString);

            var sql = @"
            UPDATE Debts
            SET status_id = @ProcessedStatus,
                amount_paid = @AmountPaid,
                payment_date = @PaymentDate,
                correlation_id = @CorrelationId
            WHERE debt_id = @DebtId
                AND status_id != @ProcessedStatus";

            var rows = await connection.ExecuteAsync(sql, new
            {
                ProcessedStatus = ProcessingStatus.Processed,
                debt.AmountPaid,
                debt.PaymentDate,
                debt.DebtId,
                CorrelationId = debt.CorrelationId
            });

            return rows > 0;
        }
    }
}
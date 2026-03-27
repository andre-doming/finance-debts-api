using Dapper;
using Microsoft.Data.SqlClient;
using finance.debts.domain.Entities;
using finance.debts.domain.Interfaces;

namespace finance.debts.api.Infrastructure.Repositories
{
    public class ProcessingLogRepository : IProcessingLogRepository
    {
        private readonly string _connectionString;

        public ProcessingLogRepository(IConfiguration config)
        {
            _connectionString = config.GetConnectionString("DefaultConnection");
        }

        public async Task AddAsync(ProcessingLog log)
        {
            using var connection = new SqlConnection(_connectionString);

            var sql = @"
            INSERT INTO processing_logs (debt_id, status_id, message, correlation_id, created_at)
            VALUES (@DebtId, @StatusId, @Message, @CorrelationId, @CreatedAt)";

            await connection.ExecuteAsync(sql, log);
        }
    }
}

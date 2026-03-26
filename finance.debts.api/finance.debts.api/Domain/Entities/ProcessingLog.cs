namespace finance.debts.api.Domain.Entities
{
    public class ProcessingLog
    {
        public int LogId { get; set; }
        public int DebtId { get; set; }
        public int StatusId { get; set; }
        public string Message { get; set; } = string.Empty;
        public Guid? CorrelationId { get; set; } 
        public DateTime CreatedAt { get; set; }
    }
}

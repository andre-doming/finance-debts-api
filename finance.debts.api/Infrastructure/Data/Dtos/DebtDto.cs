namespace finance.debts.api.Infrastructure.Data.Dtos
{
    public class DebtDto
    {
        public int DebtId { get; set; }
        public int ClientId { get; set; }
        public decimal AmountDue { get; set; }
        public decimal AmountPaid { get; set; }
        public int StatusId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime PaymentDate { get; set; }
        public Guid CorrelationId { get; set; }
    }
}

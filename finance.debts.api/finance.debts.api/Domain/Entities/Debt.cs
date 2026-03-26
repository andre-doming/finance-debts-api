using finance.debts.api.Domain.Enums;

namespace finance.debts.api.Domain.Entities;

public class Debt
{
    public bool IsProcessed => StatusId == ProcessingStatus.Processed;

    public int DebtId { get; set; }
    public int ClientId { get; set; }
    public decimal AmountDue { get; set; }
    public decimal? AmountPaid { get; set; }
    public ProcessingStatus StatusId { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? PaymentDate { get; set; }
    public Guid? CorrelationId { get; set; }

    // regra de negócio
    public void Process()
    {
        if (IsProcessed) 
            throw new InvalidOperationException("Dívida já processada");

        StatusId = ProcessingStatus.Processed;
        AmountPaid = AmountDue;
        PaymentDate = DateTime.UtcNow;
    }
}
using Sequel.Attributes;

namespace RecordSales.Models
{
    public class CashFlowModel
    {
        public int NewId { get; set; }

        public string Code { get; set; }

        public int Id { get; set; }

        public int TransactionTypeId { get; set; }

        public decimal Amount { get; set; }

        public string Description { get; set; }

        public string Notes { get; set; }
    }
}

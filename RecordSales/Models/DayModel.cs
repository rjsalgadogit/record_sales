using System.Collections.Generic;

namespace RecordSales.Models
{
    public class DayModel
    {
        public int Id { get; set; }

        public string Code { get; set; }

        public int Year { get; set; }

        public int Month { get; set; }

        public int Day { get; set; }

        public int TransactionTypeId { get; set; }

        public string DayText { get; set; }

        public string MonthText { get; set; }

        public decimal Amount { get; set; }

        public decimal TotalSales { get; set; }

        public bool IsExpenses { get; set; }

        public List<CashFlowModel> Expenses { get; set; }

        public List<CashFlowModel> Additionals { get; set; }
    }
}

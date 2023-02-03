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

        public string TotalSalesText { get; set; }

        public bool IsExpenses { get; set; }

        public List<CashFlowModel> Expenses { get; set; }

        public List<CashFlowModel> Additionals { get; set; }

        public List<CashFlowModel> ExpensesToBeDeleted { get; set; }

        public List<CashFlowModel> AdditionalsToBeDeleted { get; set; }

        public DayModel()
        {
            Expenses = new List<CashFlowModel>();
            Additionals = new List<CashFlowModel>();
            ExpensesToBeDeleted = new List<CashFlowModel>();
            AdditionalsToBeDeleted = new List<CashFlowModel>();
        }
    }
}

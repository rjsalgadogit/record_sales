namespace RecordSales.Models
{
    public class DayModel
    {
        public int Year { get; set; }

        public int Month { get; set; }

        public int Day { get; set; }

        public int TransactionTypeId { get; set; }

        public string DayText { get; set; }

        public string MonthText { get; set; }

        public decimal BasedSales { get; set; }

        public decimal TotalSales { get; set; }
    }
}

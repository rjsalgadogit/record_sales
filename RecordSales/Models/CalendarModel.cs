using System.Collections.Generic;

namespace RecordSales.Models
{
    public class CalendarModel
    {
        public string MonthYear { get; set; }

        public List<WeekModel> Weeks { get; set; }
    }
}

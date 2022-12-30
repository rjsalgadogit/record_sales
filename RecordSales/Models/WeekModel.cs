using System;

namespace RecordSales.Models
{
    public class WeekModel
    {
        public int SundayDay{ get; set; }

        public int MondayDay { get; set; }

        public int TuesdayDay { get; set; }

        public int WednesdayDay { get; set; }

        public int ThursdayDay { get; set; }

        public int FridayDay { get; set; } 

        public int SaturdayDay { get; set; }

        public DateTime? Sunday { get; set; }

        public DateTime? Monday { get; set; }

        public DateTime? Tuesday { get; set; }

        public DateTime? Wednesday { get; set; }

        public DateTime? Thursday { get; set; }

        public DateTime? Friday { get; set; }

        public DateTime? Saturday { get; set; }
    }
}

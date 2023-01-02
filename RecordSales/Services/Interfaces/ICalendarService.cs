using RecordSales.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;

namespace RecordSales.Services.Interfaces
{
    public interface ICalendarService
    {
        public Task<List<WeekModel>> MakeCalendar(int year, int month, DayOfWeek endOfWeek);
    }
}

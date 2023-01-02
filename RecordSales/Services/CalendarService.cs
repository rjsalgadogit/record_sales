using RecordSales.Models;
using RecordSales.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RecordSales.Services
{
    public class CalendarService : ICalendarService
    {
        public async Task<List<WeekModel>> MakeCalendar(int year, int month, DayOfWeek endOfWeek)
        {
            var listOfDates = await GetDates(year, month);
            var weekends = listOfDates.Where(x => x.DayOfWeek == endOfWeek).ToList();

            //Creating a list of weeks
            var listCounter = 0;
            var weekCounter = 0;
            var weekList = new List<WeekModel>();
            var weekModel = new WeekModel();

            foreach (var date in listOfDates)
            {
                switch (date.DayOfWeek)
                {
                    case DayOfWeek.Sunday:
                        weekModel.Sunday = date;
                        weekModel.SundayDay = date.Day;
                        break;

                    case DayOfWeek.Monday:
                        weekModel.Monday = date;
                        weekModel.MondayDay = date.Day;
                        break;

                    case DayOfWeek.Tuesday:
                        weekModel.Tuesday = date;
                        weekModel.TuesdayDay = date.Day;
                        break;

                    case DayOfWeek.Wednesday:
                        weekModel.Wednesday = date;
                        weekModel.WednesdayDay = date.Day;
                        break;

                    case DayOfWeek.Thursday:
                        weekModel.Thursday = date;
                        weekModel.ThursdayDay = date.Day;
                        break;

                    case DayOfWeek.Friday:
                        weekModel.Friday = date;
                        weekModel.FridayDay = date.Day;
                        break;

                    case DayOfWeek.Saturday:
                        weekModel.Saturday = date;
                        weekModel.SaturdayDay = date.Day;
                        break;
                }

                // if date is equal(=) to endOfWeek then reset values and add the week(weekModel) to weekList
                if (weekCounter <= (weekends.Count - 1) && date.DayOfWeek == weekends[weekCounter].DayOfWeek)
                {
                    weekCounter++;
                    weekList.Add(weekModel);
                    weekModel = new WeekModel();
                }
                else if (weekCounter > (weekends.Count - 1) && listCounter == (listOfDates.Count - 1))
                {
                    weekList.Add(weekModel);
                }

                listCounter++;
            }

            return weekList;
        }

        private async Task<List<DateTime>> GetDates(int year, int month)
        {
            var listOfDates = Enumerable.Range(1, DateTime.DaysInMonth(year, month))  // Days: 1, 2 ... 31 etc.
                    .Select(day => new DateTime(year, month, day)) // Map each day to a date
                    .ToList(); // Load dates into a list

            return await Task.FromResult(listOfDates);
        }
    }
}

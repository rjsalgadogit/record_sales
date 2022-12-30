using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RecordSales.Models;
using RecordSales.Models.Enums;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace RecordSales.Controllers
{
	public class HomeController : Controller
	{
		private readonly ILogger<HomeController> _logger;

		public HomeController(ILogger<HomeController> logger)
		{
			_logger = logger;
		}

		public IActionResult Index()
		{            
            ViewBag.MonthInt = DateTime.Now.Month;
            ViewBag.YearInt = DateTime.Now.Year;
            ViewBag.MonthYear = $"{DateTime.Now.ToString("MMMM")} {DateTime.Now.ToString("yyyy")}";

            return View();
		}

		public IActionResult Privacy()
		{
			return View();
		}

		[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
		public IActionResult Error()
		{
			return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
		}

        public async Task<IActionResult> DayModal()
        {
            return PartialView();
        }

        public async Task<string> GetCalendar(int year, int month)
        {
            var monthYear = new DateTime(year, month, 1);

            var result = new JsonResultModel { ErrorMessage = string.Empty, };

            var calendarModel = new CalendarModel 
            { 
                MonthYear = $"{monthYear.ToString("MMMM")} {monthYear.ToString("yyyy")}",
                Weeks = MakeCalendar(year, month, DayOfWeek.Saturday)
            };

            result.Collection = calendarModel;

            return JsonConvert.SerializeObject(await Task.FromResult(result));
        }

        #region Private Methods

        private List<WeekModel> MakeCalendar(int year, int month, DayOfWeek endOfWeek)
        {
            var listOfDates = GetDates(year, month);
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

        private List<DateTime> GetDates(int year, int month)
		{
            return Enumerable.Range(1, DateTime.DaysInMonth(year, month))  // Days: 1, 2 ... 31 etc.
                    .Select(day => new DateTime(year, month, day)) // Map each day to a date
                    .ToList(); // Load dates into a list
        }		

        #endregion
    }
}

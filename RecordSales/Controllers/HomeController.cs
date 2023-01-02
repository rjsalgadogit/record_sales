using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RecordSales.DBModels.StoredProcedures;
using RecordSales.Models;
using RecordSales.Models.Enums;
using RecordSales.Services.Interfaces;
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
        private readonly ICalendarService _calendarService;
        private readonly ICashFlowService _cashFlowService;

        public HomeController(ILogger<HomeController> logger
            , ICalendarService calendarService
            , ICashFlowService cashFlowService)
        {
            _logger = logger;
            _calendarService = calendarService;
            _cashFlowService = cashFlowService;
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

        public IActionResult DayModal(DayModel viewModel)
        {
            var today = new DateTime(viewModel.Year, viewModel.Month, viewModel.Day);
            viewModel.DayText = today.ToString("dddd");
            viewModel.MonthText = today.ToString("MMM");

            return PartialView(viewModel);
        }

        public async Task<IActionResult> SaveData(DayModel viewModel)
        {
            await _cashFlowService.UpdateCashFlow(new UpdateCashFlow
            {
                Id = $"{viewModel.Year}-{viewModel.Month}-{viewModel.Day}",
                TransactionTypeId = 1,
                Amount = viewModel.BasedSales,
                Description = "Benta ng Shop",
                User = "system"
            });

            return Json("");
        }

        public async Task<string> GetCalendar(int year, int month)
        {
            var monthYear = new DateTime(year, month, 1);   //default value for current month and year

            var result = new JsonResultModel { ErrorMessage = string.Empty, };

            var calendarModel = new CalendarModel 
            { 
                MonthYear = $"{monthYear.ToString("MMMM")} {monthYear.ToString("yyyy")}",
                Weeks =  await _calendarService.MakeCalendar(year, month, DayOfWeek.Saturday)
            };

            result.Collection = calendarModel;

            return JsonConvert.SerializeObject(result);
        }
    }
}

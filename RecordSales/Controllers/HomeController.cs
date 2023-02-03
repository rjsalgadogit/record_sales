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
using System.Drawing;
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

        public async Task<IActionResult> _DayModal(DayModel viewModel)
        {
            var today = new DateTime(viewModel.Year, viewModel.Month, viewModel.Day);
            var result = await _cashFlowService.GetCashFlowAsync(new GetCashFlow { Code = $"{viewModel.Year}-{viewModel.Month}-{viewModel.Day}" });

            //for display
            if (result != null)
            {
                viewModel = result;                
            }

            viewModel.Day = today.Day;
            viewModel.DayText = today.ToString("dddd");
            viewModel.MonthText = today.ToString("MMM");

            return PartialView(await Task.FromResult(viewModel));
        }

        public async Task<IActionResult> _ExpenseItemView(CashFlowModel viewModel)
        {
            return PartialView(await Task.FromResult(viewModel));
        }

        public async Task<IActionResult> _AdditionalItemView(CashFlowModel viewModel)
        {
            return PartialView(await Task.FromResult(viewModel));
        }

        public async Task<IActionResult> SaveData(DayModel viewModel)
        {
            var code = $"{viewModel.Year}-{viewModel.Month}-{viewModel.Day}";

            var result = await _cashFlowService.UpdateCashFlowAsync(new UpdateCashFlow
            {
                Id = viewModel.Id,
                Code = code,
                TransactionTypeId = (int)TransactionEnum.Sales,
                Amount = viewModel.Amount,
                Description = "Benta ng Shop",
                User = "system"
            });

            foreach (var item in viewModel.Expenses)
            {
                var expresult = await _cashFlowService.UpdateCashFlowAsync(new UpdateCashFlow
                {
                    Id = item.Id,
                    Code = code,
                    TransactionTypeId = (int)TransactionEnum.Expenses,
                    Amount = item.Amount * -1,
                    Description = item.Description,
                    User = "system"
                });
            }

            foreach (var item in viewModel.Additionals)
            {
                var addresult = await _cashFlowService.UpdateCashFlowAsync(new UpdateCashFlow
                {
                    Id = item.Id,
                    Code = code,
                    TransactionTypeId = (int)TransactionEnum.Additional,
                    Amount = item.Amount,
                    Description = item.Description,
                    User = "system"
                });
            }

            foreach (var item in viewModel.ExpensesToBeDeleted)
            {
                await _cashFlowService.DeleteCashFlowAsync(new DeleteCashFlow { Id = item.Id });
            }

            foreach (var item in viewModel.AdditionalsToBeDeleted)
            {
                await _cashFlowService.DeleteCashFlowAsync(new DeleteCashFlow { Id = item.Id });
            }

            return Json(result);
        }

        public async Task<IActionResult> GenerateCellHtml(int year, int month, int day)
        {
            var model = await _cashFlowService.GetSalesAmountAsync(new GetSalesAmount { Code = $"{year}-{month}-{day}" });

            if (model.TotalSales > 0)
                return Json(model.TotalSalesText);

            return Json(null);
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

        #region Default Methods

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        #endregion
    }
}

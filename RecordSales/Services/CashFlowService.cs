using RecordSales.DBModels.StoredProcedures;
using RecordSales.Models;
using RecordSales.Models.Enums;
using RecordSales.Models.Extensions;
using RecordSales.Services.Interfaces;
using Sequel.Service.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RecordSales.Services
{
    public class CashFlowService : ICashFlowService
    {
        private readonly ISequelService<UpdateCashFlow> _updateCashFlow;
        private readonly ISequelService<GetSalesAmount> _getSalesAmount;
        private readonly ISequelService<GetCashFlow> _getCashFlow;
        private readonly ISequelService<DeleteCashFlow> _deleteCashFlow;

        public CashFlowService(ISequelService<UpdateCashFlow> updateCashFlow
            , ISequelService<GetSalesAmount> getSalesAmount
            , ISequelService<GetCashFlow> getCashFlow
            , ISequelService<DeleteCashFlow> deleteCashFlow)
        {
            _updateCashFlow = updateCashFlow;
            _getSalesAmount = getSalesAmount;
            _getCashFlow = getCashFlow;
            _deleteCashFlow = deleteCashFlow;
        }

        public async Task<JsonResultModel> UpdateCashFlowAsync(UpdateCashFlow updateCashFlow)
        {
            var result = new JsonResultModel
            {
                ErrorMessage = string.Empty,
            };

            try
            {
                await _updateCashFlow.PerformSPFromSequelClientAsync(updateCashFlow);

                result.IsSuccess = true;
            }
            catch (Exception ex)
            {
                result.ErrorMessage = ex.Message;
            }

            return result;
        }

        public async Task<DayModel> GetSalesAmountAsync(GetSalesAmount getSales)
        {
            var result = await _getSalesAmount.GetSPResultsFromSequelClientAsync(getSales);

            return result.Select(i => i.ToViewModel()).FirstOrDefault();
        }

        public async Task<DayModel> GetCashFlowAsync(GetCashFlow getCashFlow)
        {
            var result = await _getCashFlow.GetSPResultsFromSequelClientAsync(getCashFlow);

            var transaction1 = result.Where(x => x.TransactionTypeId == 1).ToList();    //sales
            var transaction2 = result.Where(x => x.TransactionTypeId == 2).ToList();    //expenses
            var transaction3 = result.Where(x => x.TransactionTypeId == 3).ToList();    //additional

            if (result.Count > 0 || result.Any())
            {
                return new DayModel
                {
                    Id = transaction1.FirstOrDefault().Id,
                    Code = transaction1.FirstOrDefault().Code,
                    Amount = transaction1.FirstOrDefault().Amount,
                    TransactionTypeId = (int)TransactionEnum.Sales,
                    Expenses = transaction2.Select(i => i.ToViewModel()).ToList(),
                    Additionals = transaction3.Select(i => i.ToViewModel()).ToList()
                };
            }

            return null;
        }

        public async Task DeleteCashFlowAsync(DeleteCashFlow deleteCashFlow)
        {
            await _deleteCashFlow.PerformSPFromSequelClientAsync(deleteCashFlow);
        }
    }
}

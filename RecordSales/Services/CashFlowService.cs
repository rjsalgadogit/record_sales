using RecordSales.DBModels.StoredProcedures;
using RecordSales.Models;
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
        private readonly ISequelService<GetSales> _getSales;

        public CashFlowService(ISequelService<UpdateCashFlow> updateCashFlow
            , ISequelService<GetSales> getSales)
        {
            _updateCashFlow = updateCashFlow;
            _getSales = getSales;
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

        public async Task<DayModel> GetSalesAsync(GetSales getSales)
        {
            var result = await _getSales.GetSPResultsFromSequelClientAsync(getSales);

            return result.Select(i => i.ToViewModel()).FirstOrDefault();
        }
    }
}

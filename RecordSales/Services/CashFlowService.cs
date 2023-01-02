using RecordSales.DBModels.StoredProcedures;
using RecordSales.Models;
using RecordSales.Services.Interfaces;
using Sequel.Service.Interfaces;
using System;
using System.Threading.Tasks;

namespace RecordSales.Services
{
    public class CashFlowService : ICashFlowService
    {
        private readonly ISequelService<UpdateCashFlow> _updateCashFlow;

        public CashFlowService(ISequelService<UpdateCashFlow> updateCashFlow)
        {
            _updateCashFlow = updateCashFlow;
        }

        public async Task<JsonResultModel> UpdateCashFlow(UpdateCashFlow updateCashFlow)
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
    }
}

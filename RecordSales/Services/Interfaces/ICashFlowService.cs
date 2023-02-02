using RecordSales.DBModels.StoredProcedures;
using RecordSales.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RecordSales.Services.Interfaces
{
    public interface ICashFlowService
    {
        public Task<JsonResultModel> UpdateCashFlowAsync(UpdateCashFlow updateCashFlow);

        public Task<DayModel> GetSalesAmountAsync(GetSalesAmount getSales);

        public Task<DayModel> GetCashFlowAsync(GetCashFlow getCashFlow);
    }
}

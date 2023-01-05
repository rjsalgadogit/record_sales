using RecordSales.DBModels.StoredProcedures;
using RecordSales.Models;
using System.Threading.Tasks;

namespace RecordSales.Services.Interfaces
{
    public interface ICashFlowService
    {
        public Task<JsonResultModel> UpdateCashFlowAsync(UpdateCashFlow updateCashFlow);

        Task<DayModel> GetSalesAsync(GetSales getSales);
    }
}

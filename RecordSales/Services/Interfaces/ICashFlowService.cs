using RecordSales.DBModels.StoredProcedures;
using RecordSales.Models;
using System.Threading.Tasks;

namespace RecordSales.Services.Interfaces
{
    public interface ICashFlowService
    {
        public Task<JsonResultModel> UpdateCashFlow(UpdateCashFlow updateCashFlow);
    }
}

using RecordSales.DBModels.StoredProcedures;
using RecordSales.Models.Enums;

namespace RecordSales.Models.Extensions
{
    public static class Extensions
    {
        public static DayModel ToViewModel(this GetSalesAmount model) 
            => new DayModel
            {
                TotalSales= model.TotalSales
            };

        public static CashFlowModel ToViewModel(this GetCashFlow model)
            => new CashFlowModel
            {
                Id = model.Id,
                Code = model.Code,
                TransactionTypeId = model.TransactionTypeId,
                Amount = model.TransactionTypeId == (int)TransactionEnum.Expenses ?
                    model.Amount * -1 :     //convert negative to positive for expenses
                    model.Amount,
                Description = model.Description,
                Notes = model.Notes
            };
    }
}

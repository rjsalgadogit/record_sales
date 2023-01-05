using RecordSales.DBModels.StoredProcedures;

namespace RecordSales.Models.Extensions
{
    public static class Extensions
    {
        public static DayModel ToViewModel(this GetSales model) 
            => new DayModel
            {
                TotalSales= model.TotalSales
            };
    }
}

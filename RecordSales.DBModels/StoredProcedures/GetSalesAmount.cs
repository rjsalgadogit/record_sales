using Sequel.Attributes;
using Sequel.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace RecordSales.DBModels.StoredProcedures
{
    public class GetSalesAmount : ModelBaseSqlStoredProcedure
    {
        [Param]
        public string Code { get; set; }

        [Read]
        public decimal TotalSales { get; set; }
    }
}

using Sequel.Attributes;
using Sequel.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace RecordSales.DBModels.StoredProcedures
{
    public class GetSales : ModelBaseSqlStoredProcedure
    {
        [Param]
        public string Id { get; set; }

        [Read]
        public decimal TotalSales { get; set; }
    }
}

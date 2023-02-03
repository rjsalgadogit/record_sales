using Sequel.Attributes;
using Sequel.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace RecordSales.DBModels.StoredProcedures
{
    public class DeleteCashFlow : ModelBaseSqlStoredProcedure
    {
        [Param]
        public int Id { get; set; }
    }
}

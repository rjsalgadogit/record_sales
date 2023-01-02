using Sequel.Attributes;
using Sequel.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace RecordSales.DBModels.StoredProcedures
{
    public class UpdateCashFlow : ModelBaseSqlStoredProcedure
    {
        [Param]
        public string Id { get; set; }

        [Param]
        public int TransactionTypeId { get; set; }

        [Param]
        public decimal Amount { get; set; }

        [Param]
        public string Description { get; set; }

        [Param]
        public string Notes { get; set; }

        [Param]
        public string User { get; set; }
    }
}

using Sequel.Attributes;
using Sequel.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace RecordSales.DBModels.StoredProcedures
{
    public class GetCashFlow : ModelBaseSqlStoredProcedure
    {
        public string Code { get; set; }

        [Read]
        public int Id { get; set; }

        [Read]
        public int TransactionTypeId { get; set; }

        [Read]
        public decimal Amount { get; set; }

        [Read]
        public string Description { get; set; }

        [Read]
        public string Notes { get; set; }
    }
}

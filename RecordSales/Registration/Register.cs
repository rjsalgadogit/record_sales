﻿using Microsoft.Extensions.DependencyInjection;
using RecordSales.DBModels.StoredProcedures;
using RecordSales.SequelConnection;
using RecordSales.Services;
using RecordSales.Services.Interfaces;
using Sequel.Service;
using Sequel.Service.Interfaces;

namespace RecordSales.Registration
{
    public static class Register
    {
        public static void RegisterService(IServiceCollection services)
        {
            services.AddTransient(typeof(ISequelConnection), typeof(AppSequelConnection));

            #region SP

            services.AddTransient(typeof(ISequelService<UpdateCashFlow>), typeof(SequelService<UpdateCashFlow>));
            services.AddTransient(typeof(ISequelService<GetSalesAmount>), typeof(SequelService<GetSalesAmount>));
            services.AddTransient(typeof(ISequelService<GetCashFlow>), typeof(SequelService<GetCashFlow>));
            services.AddTransient(typeof(ISequelService<DeleteCashFlow>), typeof(SequelService<DeleteCashFlow>));

            #endregion

            #region Services

            services.AddTransient(typeof(ICalendarService), typeof(CalendarService));
            services.AddTransient(typeof(ICashFlowService), typeof(CashFlowService));

            #endregion
        }
    }
}

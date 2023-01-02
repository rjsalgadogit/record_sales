using Microsoft.Extensions.DependencyInjection;
using RecordSales.Services;
using RecordSales.Services.Interfaces;

namespace RecordSales.Registration
{
    public static class Register
    {
        public static void RegisterService(IServiceCollection services)
        {
            services.AddTransient(typeof(ICalendarService), typeof(CalendarService));
        }
    }
}

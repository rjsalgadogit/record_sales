using Microsoft.Extensions.Configuration;
using Sequel.Service.Interfaces;

namespace RecordSales.SequelConnection
{
    public class AppSequelConnection : ISequelConnection
    {
        private string _connectionString = string.Empty;

        public AppSequelConnection(IConfiguration config)
        {
            _connectionString = config.GetValue<string>("ConnectionStrings:AppData", "");
        }

        public string ConnectionString => _connectionString;
    }
}

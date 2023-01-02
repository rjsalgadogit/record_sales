using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Sequel.Service.Interfaces
{
	public interface ISequelService<T>
	{
		Task<List<T>> ReadFromSequelClientAsync(string query, SqlParameter[] parameters);

		Task<int> ExecuteToSequelClientAsync(string query, SqlParameter[] parameters);

		Task<int> InsertToSequelClientAsync(T objectInstance);

		Task<T> InsertToSequelClientWithRecordReturnAsync(T objectInstance);

		Task<int> InsertToSequelClientAsync(T objectInstance, bool OverruleIdentity);

		Task<int> InsertRangeToSequelClientAsync(List<T> objectInstanceList);

		Task<List<T>> InsertRangeToSequelClientWithRecordReturnAsync(List<T> objectInstance);

		Task<int> InsertRangeToSequelClientAsync(List<T> objectInstanceList, bool OverruleIdentity);

		Task<int> UpdateToSequelClientAsync(T objectInstance);

		Task<int> UpdateRangeToSequelClientAsync(List<T> objectInstanceList);

		Task<int> DeleteFromSequelClientAsync(T objectInstance);

		Task<int> DeleteRangeFromSequelClientAsync(List<T> objectInstanceList);

		Task<Int64> GetScalarFromSequelClientAsync(string query, SqlParameter[] parameters);

		Task<List<T>> GetSPResultsFromSequelClientAsync(T spModel);

		Task<List<U>> GetSPResultsFromSequelClientAsync<U>(T objectInstance);

		Task<Int32> PerformSPFromSequelClientAsync(T objectInstance);
	}
}

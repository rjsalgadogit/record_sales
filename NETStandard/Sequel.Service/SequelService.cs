using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using Sequel.Attributes;
using Sequel.Service.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Sequel.Service
{
	public class SequelService<T> : ISequelService<T>
	{
		/// <summary>
		/// The sequel service base service implement on .net standard
		/// </summary>
		private readonly SequelBaseService<T> _sequelBaseBaseService;

		private readonly ISequelConnection _sequelConnection;

		/// <summary>
		/// The logger
		/// </summary>
		private readonly ILogger _logger;

		public SequelService(ILoggerFactory logger,
			ISequelConnection sequelConnection)
		{
			_sequelConnection = sequelConnection;
			_sequelBaseBaseService = new SequelBaseService<T>(sequelConnection.ConnectionString);
			if (logger != null)
				_logger = logger.CreateLogger(this.GetType().Namespace.ToString() + "." + this.GetType().Name);
		}

		public async Task<List<T>> ReadFromSequelClientAsync(string query, SqlParameter[] parameters)
		{
			List<T> results = new List<T>();

			try
			{
				results = await _sequelBaseBaseService.ReadFromSequelClientAsync(query, parameters);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex.Message);

				throw;
			}

			return results;
		}

		public async Task<int> ExecuteToSequelClientAsync(string query, SqlParameter[] parameters)
		{
			int result = 0;

			try
			{
				result = await _sequelBaseBaseService.ExecuteToSequelClientAsync(query, parameters);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex.Message);

				throw;
			}

			return result;
		}

		public async Task<int> InsertToSequelClientAsync(T objectInstance)
		{
			int countInserted;

			try
			{
				countInserted = await _sequelBaseBaseService.InsertToSequelClientAsync(objectInstance);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex.Message);

				throw;
			}

			return countInserted;
		}

		public async Task<T> InsertToSequelClientWithRecordReturnAsync(T objectInstance)
		{
			T record;

			try
			{
				record = await _sequelBaseBaseService.InsertToSequelClientWithRecordReturnAsync(objectInstance);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex.Message);

				throw;
			}

			return record;
		}

		public async Task<int> InsertToSequelClientAsync(T objectInstance, bool OverruleIdentity)
		{
			int countInserted;

			try
			{
				countInserted = await _sequelBaseBaseService.InsertToSequelClientAsync(objectInstance, OverruleIdentity);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex.Message);

				throw;
			}

			return countInserted;
		}

		public async Task<int> InsertRangeToSequelClientAsync(List<T> objectInstanceList)
		{
			int countInserted = 0;

			try
			{
				countInserted = await _sequelBaseBaseService.InsertRangeToSequelClientAsync(objectInstanceList);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex.Message);

				throw;
			}

			return countInserted;

		}

		public async Task<List<T>> InsertRangeToSequelClientWithRecordReturnAsync(List<T> objectInstance)
		{
			List<T> records;

			try
			{
				records = await _sequelBaseBaseService.InsertRangeToSequelClientWithRecordReturnAsync(objectInstance);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex.Message);

				throw;
			}

			return records;
		}

		public async Task<int> InsertRangeToSequelClientAsync(List<T> objectInstanceList, bool OverruleIdentity)
		{
			int countInserted = 0;

			try
			{
				countInserted = await _sequelBaseBaseService.InsertRangeToSequelClientAsync(objectInstanceList, OverruleIdentity);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex.Message);

				throw;
			}

			return countInserted;

		}

		public async Task<int> UpdateToSequelClientAsync(T objectInstance)
		{

			int countUpdated;

			try
			{
				countUpdated = await _sequelBaseBaseService.UpdateToSequelClientAsync(objectInstance);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex.Message);

				throw;
			}

			return countUpdated;
		}

		public async Task<int> UpdateRangeToSequelClientAsync(List<T> objectInstanceList)
		{
			int countUpdated = 0;

			try
			{
				countUpdated = await _sequelBaseBaseService.UpdateRangeToSequelClientAsync(objectInstanceList);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex.Message);

				throw;
			}

			return countUpdated;
		}

		public async Task<int> DeleteFromSequelClientAsync(T objectInstance)
		{

			int countDeleted;

			try
			{
				countDeleted = await _sequelBaseBaseService.DeleteFromSequelClientAsync(objectInstance);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex.Message);

				throw;
			}

			return countDeleted;
		}

		public async Task<int> DeleteRangeFromSequelClientAsync(List<T> objectInstanceList)
		{
			int countDeleted = 0;

			try
			{
				countDeleted = await _sequelBaseBaseService.DeleteRangeFromSequelClientAsync(objectInstanceList);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex.Message);

				throw;
			}

			return countDeleted;

		}

		public async Task<Int64> GetScalarFromSequelClientAsync(string query, SqlParameter[] parameters)
		{

			Int64 result = 0;

			try
			{
				result = await _sequelBaseBaseService.GetScalarFromSequelClientAsync(query, parameters);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex.Message);

				throw;
			}

			return result;
		}

		public async Task<List<T>> GetSPResultsFromSequelClientAsync(T sPModel)
		{
			List<T> results = new List<T>();

			try
			{

				using (SqlConnection connection = new SqlConnection(_sequelConnection.ConnectionString))
				{
					if (connection.State != ConnectionState.Open)
						connection.Open();

					using (SqlCommand command = _sequelBaseBaseService.GenerateSPCommand(sPModel))
					{
						command.Connection = connection;

						SqlDataReader reader = await command.ExecuteReaderAsync();

						while (reader.Read())
						{
							var item = Activator.CreateInstance(typeof(T));

							PropertyInfo[] properties = typeof(T).GetRuntimeProperties().ToArray();
							foreach (PropertyInfo property in properties)
							{
								if (property.GetCustomAttributes<ParamAttribute>().ToList().FirstOrDefault() == null)
								{
									PropertyInfo propertyInfo = item.GetType().GetProperty(property.Name);
									if (propertyInfo != null)
									{
										//System.Diagnostics.Debug.WriteLine(property.Name);
										if (reader.GetValue(reader.GetOrdinal(property.Name)) != DBNull.Value)
											propertyInfo.SetValue(item, reader.GetValue(reader.GetOrdinal(property.Name)), null);
									}
								}
							}

							results.Add((T)item);
						}

						reader.Close();
						command.Parameters.Clear();
					}
					connection.Close();

				}
			}
			catch
			{
				throw;
			}

			return results;
		}

		public async Task<List<U>> GetSPResultsFromSequelClientAsync<U>(T objectInstance)
		{
			List<U> results = new List<U>();

			try
			{
				results = await _sequelBaseBaseService.GetSPResultsFromSequelClientAsync<U>(objectInstance);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex.Message);

				throw;
			}
			return results;
		}

		public async Task<Int32> PerformSPFromSequelClientAsync(T objectInstance)
		{
			int countInserted = 0;
			try
			{
				countInserted = await _sequelBaseBaseService.PerformSPFromSequelClientAsync(objectInstance);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex.Message);

				throw;
			}
			return countInserted;
		}
	}
}

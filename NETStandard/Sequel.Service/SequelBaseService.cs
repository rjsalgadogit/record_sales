using Microsoft.Data.SqlClient;
using Sequel.Attributes;
using Sequel.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Sequel.Service
{
	public class SequelBaseService<T>
	{
		private string _connectionString { get; set; }
		public SequelBaseService(string connectionString)
		{
			_connectionString = connectionString;
		}

		#region "Utility Functions"

		public SqlCommand GenerateSPCommand(T objectInstance)
		{
			using (SqlConnection connection = new SqlConnection(_connectionString))
			{
				if (connection.State != ConnectionState.Open)
					connection.Open();

				return GenerateSPCommand(connection, objectInstance);
			}
		}

		public SqlCommand GenerateSPCommand(SqlConnection connection, T objectInstance)
		{
			PropertyInfo[] properties;

			string commandText = GenerateInsertCommandText(objectInstance, out properties, false);

			SqlCommand command = new SqlCommand(commandText, connection);

			if (objectInstance is ModelBaseSqlStoredProcedure)
			{
				command.CommandType = CommandType.StoredProcedure;
			}

			foreach (PropertyInfo property in properties)
			{
				PropertyInfo propertyInfo = objectInstance.GetType().GetProperty(property.Name);
				if (propertyInfo != null)
				{
					//skip fields to be excluded
					if (propertyInfo.GetCustomAttributes<ReadAttribute>().ToList().FirstOrDefault() == null)
					{
						FieldInfo field = objectInstance.GetType().GetField(property.Name, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
						PropertyInfo instancePropertyInfo = objectInstance.GetType().GetProperty(property.Name);
						if (instancePropertyInfo.GetValue(objectInstance) != null)
							command.Parameters.Add(new SqlParameter(property.Name, instancePropertyInfo.GetValue(objectInstance)));
						else
							command.Parameters.Add(new SqlParameter(property.Name, DBNull.Value));
					}
				}
			}

			return command;
		}

		public static SqlCommand GenerateCommand(SqlConnection connection, PropertyInfo[] properties, string commandText, T objectInstance, bool insert, bool OverruleIdentity)
		{
			SqlCommand command = new SqlCommand(commandText, connection);

			if (objectInstance is ModelBaseSqlStoredProcedure)
			{
				command.CommandType = CommandType.StoredProcedure;
			}

			foreach (PropertyInfo property in properties)
			{
				PropertyInfo propertyInfo = objectInstance.GetType().GetProperty(property.Name);
				if (propertyInfo != null)
				{
					//skip fields to be excluded
					if (propertyInfo.GetCustomAttributes<ReadAttribute>().ToList().FirstOrDefault() == null)
					{
						//if the command is an insert and the key is an identity key, exclude that property
						if (insert == false || propertyInfo.GetCustomAttributes<IdentityKeyAttribute>().ToList().FirstOrDefault() == null || OverruleIdentity == true)
						{
							FieldInfo field = objectInstance.GetType().GetField(property.Name, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
							PropertyInfo instancePropertyInfo = objectInstance.GetType().GetProperty(property.Name);
							if (instancePropertyInfo.GetValue(objectInstance) != null)
								command.Parameters.Add(new SqlParameter(property.Name, instancePropertyInfo.GetValue(objectInstance)));
							else
								command.Parameters.Add(new SqlParameter(property.Name, DBNull.Value));
						}
					}
				}
			}

			return command;
		}

		private static string GenerateInsertCommandText(object item, out PropertyInfo[] properties, bool OverruleIdentity)
		{
			StringBuilder fieldCollection = new StringBuilder();
			StringBuilder parameterCollection = new StringBuilder();

			TableNameAttribute tableNameAttribute = typeof(T).GetCustomAttributes<TableNameAttribute>().ToList().FirstOrDefault();

			string tablename = item.GetType().Name;
			if (tableNameAttribute != null)
				tablename = tableNameAttribute.TableName;

			properties = typeof(T).GetRuntimeProperties().ToArray();

			foreach (PropertyInfo property in properties)
			{
				PropertyInfo propertyInfo = item.GetType().GetProperty(property.Name);
				if (propertyInfo != null)
				{
					if (propertyInfo.GetCustomAttributes<ReadAttribute>().ToList().FirstOrDefault() == null &&
											(propertyInfo.GetCustomAttributes<IdentityKeyAttribute>().ToList().FirstOrDefault() == null || OverruleIdentity))
					{
						if (fieldCollection.Length > 0)
							fieldCollection.Append(",");

						fieldCollection.Append(property.Name);

						if (parameterCollection.Length > 0)
							parameterCollection.Append(",");

						parameterCollection.Append(string.Format("@{0}", property.Name));
					}
				}
			}

			if (item is ModelBaseSqlStoredProcedure)
			{
				return tablename;
			}
			else
			{
				string commandText = "INSERT INTO {0}({1}) VALUES ({2})";
				if (OverruleIdentity)
					commandText = string.Format("SET IDENTITY_INSERT {0} ON;", tablename) + commandText + string.Format("; SET IDENTITY_INSERT {0} OFF", tablename);

				return string.Format(commandText, tablename, fieldCollection.ToString(), parameterCollection.ToString());
			}
		}

		private static string GenerateInsertCommandTextWithRecordReturn(object item, out PropertyInfo[] properties)
		{
			string keyField = "";

			StringBuilder fieldCollection = new StringBuilder();
			StringBuilder parameterCollection = new StringBuilder();

			TableNameAttribute tableNameAttribute = typeof(T).GetCustomAttributes<TableNameAttribute>().ToList().FirstOrDefault();

			string tablename = item.GetType().Name;
			if (tableNameAttribute != null)
				tablename = tableNameAttribute.TableName;


			properties = typeof(T).GetRuntimeProperties().ToArray();
			foreach (PropertyInfo property in properties)
			{
				PropertyInfo propertyInfo = item.GetType().GetProperty(property.Name);
				if (propertyInfo != null)
				{
					if (propertyInfo.GetCustomAttributes<ReadAttribute>().ToList().FirstOrDefault() == null &&
											(propertyInfo.GetCustomAttributes<IdentityKeyAttribute>().ToList().FirstOrDefault() == null))
					{
						if (fieldCollection.Length > 0)
							fieldCollection.Append(",");

						fieldCollection.Append(property.Name);

						if (parameterCollection.Length > 0)
							parameterCollection.Append(",");

						parameterCollection.Append(string.Format("@{0}", property.Name));
					}

					if (propertyInfo.GetCustomAttributes<IdentityKeyAttribute>().ToList().FirstOrDefault() != null)
						keyField = property.Name;

				}
			}


			string commandText = "INSERT INTO {0}({1}) VALUES ({2}); SELECT * FROM {0} WHERE {3}=@@IDENTITY";

			return string.Format(commandText, tablename, fieldCollection.ToString(), parameterCollection.ToString(), keyField);
		}

		private static string GenerateUpdateCommandText(T objectInstance, out PropertyInfo[] properties)
		{
			StringBuilder fieldCollection = new StringBuilder();
			StringBuilder keyCollection = new StringBuilder();

			TableNameAttribute tableNameAttribute = typeof(T).GetCustomAttributes<TableNameAttribute>().ToList().FirstOrDefault();

			string tablename = objectInstance.GetType().Name;
			if (tableNameAttribute != null)
				tablename = tableNameAttribute.TableName;

			properties = typeof(T).GetRuntimeProperties().ToArray();
			foreach (PropertyInfo property in properties)
			{
				PropertyInfo propertyInfo = objectInstance.GetType().GetProperty(property.Name);
				if (propertyInfo != null)
				{
					if (propertyInfo.GetCustomAttributes<ReadAttribute>().ToList().FirstOrDefault() == null)
					{
						if (propertyInfo.GetCustomAttributes<KeyAttribute>().ToList().FirstOrDefault() != null)
						{
							//if it is a key, add it to the key collection
							if (keyCollection.Length > 0)
								keyCollection.Append(" AND ");

							keyCollection.Append(property.Name);
							keyCollection.Append("=");
							keyCollection.Append("@" + property.Name);
						}
						else
						{
							//otherwise add it to the field collection
							if (fieldCollection.Length > 0)
								fieldCollection.Append(",");

							fieldCollection.Append(property.Name);
							fieldCollection.Append("=");
							fieldCollection.Append("@" + property.Name);

						}
					}
				}
			}

			return string.Format("UPDATE {0} SET {1} WHERE {2}", tablename, fieldCollection, keyCollection);
		}

		private static string GenerateDeleteCommandText(T objectInstance, out PropertyInfo[] properties)
		{
			StringBuilder keyCollection = new StringBuilder();

			TableNameAttribute tableNameAttribute = typeof(T).GetCustomAttributes<TableNameAttribute>().ToList().FirstOrDefault();

			string tablename = objectInstance.GetType().Name;
			if (tableNameAttribute != null)
				tablename = tableNameAttribute.TableName;

			properties = typeof(T).GetRuntimeProperties().ToArray();
			foreach (PropertyInfo property in properties)
			{
				PropertyInfo propertyInfo = objectInstance.GetType().GetProperty(property.Name);
				if (propertyInfo != null)
				{
					if (propertyInfo.GetCustomAttributes<ReadAttribute>().ToList().FirstOrDefault() == null)
					{
						if (propertyInfo.GetCustomAttributes<KeyAttribute>().ToList().FirstOrDefault() != null)
						{
							//if it is a key, add it to the key collection
							if (keyCollection.Length > 0)
								keyCollection.Append(" AND ");

							keyCollection.Append(property.Name);
							keyCollection.Append("=");
							keyCollection.Append("@" + property.Name);
						}
					}
				}
			}

			return string.Format("DELETE FROM {0} WHERE {1}", tablename, keyCollection);
		}

		#endregion "Utility Functions"

		public async Task<List<T>> ReadFromSequelClientAsync(string query, SqlParameter[] parameters)
		{
			List<T> results = new List<T>();

			try
			{

				using (SqlConnection connection = new SqlConnection(_connectionString))
				{
					if (connection.State != ConnectionState.Open)
						connection.Open();

					using (SqlCommand command = new SqlCommand(query, connection))
					{
						command.CommandType = CommandType.Text;

						if (parameters != null && parameters.Length > 0)
						{
							command.Parameters.AddRange(parameters.ToArray());
						}

						SqlDataReader reader = await command.ExecuteReaderAsync(CommandBehavior.CloseConnection);

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

		/// <summary>
		/// This method allows to update or delete records according to certain conditions and will avoid having to load a record in order to update it or delete it
		/// </summary>
		/// <param name="query"></param>
		/// <param name="parameters"></param>
		/// <returns></returns>
		public async Task<int> ExecuteToSequelClientAsync(string query, SqlParameter[] parameters)
		{
			int countExecuted;

			try
			{

				using (SqlConnection connection = new SqlConnection(_connectionString))
				{
					if (connection.State != ConnectionState.Open)
						connection.Open();

					using (SqlCommand command = new SqlCommand(query, connection))
					{
						command.CommandType = CommandType.Text;

						if (parameters != null && parameters.Length > 0)
						{
							command.Parameters.AddRange(parameters.ToArray());
						}

						countExecuted = await command.ExecuteNonQueryAsync();


						command.Parameters.Clear();
					}
					connection.Close();
				}
			}
			catch
			{
				throw;
			}

			return countExecuted;
		}

		public async Task<int> InsertToSequelClientAsync(T objectInstance)
		{
			int countInserted;

			try
			{

				using (SqlConnection connection = new SqlConnection(_connectionString))
				{
					if (connection.State != ConnectionState.Open)
						connection.Open();

					PropertyInfo[] properties;

					string insertCommand = GenerateInsertCommandText(objectInstance, out properties, false);

					using (SqlCommand command = GenerateCommand(connection, properties, insertCommand, objectInstance, true, false))
					{
						countInserted = await command.ExecuteNonQueryAsync();
						command.Parameters.Clear();
					}
					connection.Close();
				}

			}
			catch
			{
				throw;
			}

			return countInserted;
		}

		public async Task<T> InsertToSequelClientWithRecordReturnAsync(T objectInstance)
		{
			List<T> results = new List<T>();

			try
			{

				using (SqlConnection connection = new SqlConnection(_connectionString))
				{
					if (connection.State != ConnectionState.Open)
						connection.Open();

					PropertyInfo[] properties;

					string insertCommand = GenerateInsertCommandTextWithRecordReturn(objectInstance, out properties);

					using (SqlCommand command = GenerateCommand(connection, properties, insertCommand, objectInstance, true, false))
					{
						SqlDataReader reader = await command.ExecuteReaderAsync(CommandBehavior.CloseConnection);

						while (reader.Read())
						{
							var item = Activator.CreateInstance(typeof(T));

							foreach (PropertyInfo property in properties)
							{
								//exclude properties such as calculated fields
								if (property.GetCustomAttributes<ParamAttribute>().ToList().FirstOrDefault() == null && property.GetCustomAttributes<ReadAttribute>().ToList().FirstOrDefault() == null)
								{
									PropertyInfo propertyInfo = item.GetType().GetProperty(property.Name);
									if (propertyInfo != null)
									{
										System.Diagnostics.Debug.WriteLine(property.Name);
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

			return results.FirstOrDefault();
		}

		public async Task<int> InsertToSequelClientAsync(T objectInstance, bool OverruleIdentity)
		{
			int countInserted;

			try
			{

				using (SqlConnection connection = new SqlConnection(_connectionString))
				{
					if (connection.State != ConnectionState.Open)
						connection.Open();

					PropertyInfo[] properties;

					string insertCommand = GenerateInsertCommandText(objectInstance, out properties, OverruleIdentity);

					using (SqlCommand command = GenerateCommand(connection, properties, insertCommand, objectInstance, true, OverruleIdentity))
					{
						countInserted = await command.ExecuteNonQueryAsync();
						command.Parameters.Clear();
					}
					connection.Close();
				}

			}
			catch
			{
				throw;
			}

			return countInserted;
		}

		public async Task<int> InsertRangeToSequelClientAsync(List<T> objectInstanceList)
		{
			int countInserted = 0;

			try
			{
				if (objectInstanceList.Count > 0)
				{
					using (SqlConnection connection = new SqlConnection(_connectionString))
					{
						if (connection.State != ConnectionState.Open)
							connection.Open();

						PropertyInfo[] properties;
						string insertCommand = GenerateInsertCommandText(objectInstanceList.FirstOrDefault(), out properties, false);

						foreach (T objectInstance in objectInstanceList)
						{
							using (SqlCommand command = GenerateCommand(connection, properties, insertCommand, objectInstance, true, false))
							{

								countInserted += await command.ExecuteNonQueryAsync();

								command.Parameters.Clear();
							}
						}

						connection.Close();
					}
				}
			}
			catch
			{
				throw;
			}

			return countInserted;

		}

		public async Task<List<T>> InsertRangeToSequelClientWithRecordReturnAsync(List<T> objectInstances)
		{
			List<T> results = new List<T>();

			try
			{

				using (SqlConnection connection = new SqlConnection(_connectionString))
				{
					foreach (var objectInstance in objectInstances)
					{
						if (connection.State != ConnectionState.Open)
							connection.Open();

						PropertyInfo[] properties;

						string insertCommand = GenerateInsertCommandTextWithRecordReturn(objectInstance, out properties);

						using (SqlCommand command = GenerateCommand(connection, properties, insertCommand, objectInstance, true, false))
						{
							SqlDataReader reader = await command.ExecuteReaderAsync(CommandBehavior.CloseConnection);

							while (reader.Read())
							{
								var item = Activator.CreateInstance(typeof(T));

								foreach (PropertyInfo property in properties)
								{
									//exclude properties such as calculated fields
									if (property.GetCustomAttributes<ParamAttribute>().ToList().FirstOrDefault() == null && property.GetCustomAttributes<ReadAttribute>().ToList().FirstOrDefault() == null)
									{
										PropertyInfo propertyInfo = item.GetType().GetProperty(property.Name);
										if (propertyInfo != null)
										{
											System.Diagnostics.Debug.WriteLine(property.Name);
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

		public async Task<int> InsertRangeToSequelClientAsync(List<T> objectInstanceList, bool OverruleIdentity)
		{
			int countInserted = 0;

			try
			{
				if (objectInstanceList.Count > 0)
				{
					using (SqlConnection connection = new SqlConnection(_connectionString))
					{
						if (connection.State != ConnectionState.Open)
							connection.Open();

						PropertyInfo[] properties;
						string insertCommand = GenerateInsertCommandText(objectInstanceList.FirstOrDefault(), out properties, OverruleIdentity);

						foreach (T objectInstance in objectInstanceList)
						{
							using (SqlCommand command = GenerateCommand(connection, properties, insertCommand, objectInstance, true, OverruleIdentity))
							{

								countInserted += await command.ExecuteNonQueryAsync();

								command.Parameters.Clear();
							}
						}

						connection.Close();
					}
				}
			}
			catch
			{
				throw;
			}

			return countInserted;

		}

		public async Task<int> UpdateToSequelClientAsync(T objectInstance)
		{

			int countUpdated;

			try
			{

				using (SqlConnection connection = new SqlConnection(_connectionString))
				{
					if (connection.State != ConnectionState.Open)
						connection.Open();

					PropertyInfo[] properties;

					string updateCommand = GenerateUpdateCommandText(objectInstance, out properties);

					using (SqlCommand command = GenerateCommand(connection, properties, updateCommand, objectInstance, false, false))
					{

						countUpdated = await command.ExecuteNonQueryAsync();

						command.Parameters.Clear();
					}

					connection.Close();
				}
			}
			catch
			{
				throw;
			}

			return countUpdated;
		}

		public async Task<int> UpdateRangeToSequelClientAsync(List<T> objectInstanceList)
		{
			int countUpdated = 0;

			try
			{
				if (objectInstanceList.Count > 0)
				{
					using (SqlConnection connection = new SqlConnection(_connectionString))
					{
						if (connection.State != ConnectionState.Open)
							connection.Open();

						PropertyInfo[] properties;

						string updateCommand = GenerateUpdateCommandText(objectInstanceList.FirstOrDefault(), out properties);

						foreach (T objectInstance in objectInstanceList)
						{
							SqlCommand command = GenerateCommand(connection, properties, updateCommand, objectInstance, false, false);

							countUpdated += await command.ExecuteNonQueryAsync();

							command.Parameters.Clear();
						}

						connection.Close();
					}
				}
			}
			catch
			{
				throw;
			}

			return countUpdated;
		}

		public async Task<int> DeleteFromSequelClientAsync(T objectInstance)
		{

			int countDeleted;
			try
			{

				using (SqlConnection connection = new SqlConnection(_connectionString))
				{
					if (connection.State != ConnectionState.Open)
						connection.Open();

					PropertyInfo[] properties;

					string deleteCommand = GenerateDeleteCommandText(objectInstance, out properties);

					using (SqlCommand command = GenerateCommand(connection, properties, deleteCommand, objectInstance, false, false))
					{

						countDeleted = await command.ExecuteNonQueryAsync();

						command.Parameters.Clear();
					}
					connection.Close();
				}
			}
			catch
			{
				throw;
			}

			return countDeleted;
		}

		public async Task<int> DeleteRangeFromSequelClientAsync(List<T> objectInstanceList)
		{
			int countDeleted = 0;

			try
			{

				using (SqlConnection connection = new SqlConnection(_connectionString))
				{
					if (connection.State != ConnectionState.Open)
						connection.Open();

					PropertyInfo[] properties;

					foreach (T objectInstance in objectInstanceList)
					{
						string deleteCommand = GenerateDeleteCommandText(objectInstance, out properties);

						using (SqlCommand command = GenerateCommand(connection, properties, deleteCommand, objectInstance, false, false))
						{
							countDeleted += await command.ExecuteNonQueryAsync();

							command.Parameters.Clear();
						}
					}

					connection.Close();
				}
			}
			catch
			{
				throw;
			}

			return countDeleted;

		}

		public async Task<Int64> GetScalarFromSequelClientAsync(string query, SqlParameter[] parameters)
		{
			Int64 result = 0;

			try
			{

				using (SqlConnection connection = new SqlConnection(_connectionString))
				{
					if (connection.State != ConnectionState.Open)
						connection.Open();

					using (SqlCommand command = new SqlCommand(query, connection))
					{
						command.CommandType = CommandType.Text;

						if (parameters != null && parameters.Length > 0)
						{
							command.Parameters.AddRange(parameters.ToArray());
						}

						var value = await command.ExecuteScalarAsync();

						if (value != null)
							result = (int)value;

						command.Parameters.Clear();
					}

					connection.Close();
				}
			}
			catch
			{
				throw;
			}

			return result;
		}

		public async Task<List<U>> GetSPResultsFromSequelClientAsync<U>(T objectInstance)
		{
			List<U> results = new List<U>();
			using (SqlConnection connection = new SqlConnection(_connectionString))
			{
				if (connection.State != ConnectionState.Open)
					connection.Open();

				using (SqlCommand command = GenerateSPCommand(connection, objectInstance))
				{
					SqlDataReader reader = await command.ExecuteReaderAsync(CommandBehavior.CloseConnection);
					while (reader.Read())
					{
						var item = Activator.CreateInstance(typeof(U));

						PropertyInfo[] properties = typeof(U).GetRuntimeProperties().ToArray();
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

						results.Add((U)item);
					}
					reader.Close();
					command.Parameters.Clear();

				}
				connection.Close();
			}

			return results;
		}

		public async Task<Int32> PerformSPFromSequelClientAsync(T objectInstance)
		{
			int countInserted = 0;

			try
			{
				using (SqlConnection connection = new SqlConnection(_connectionString))
				{
					if (connection.State != ConnectionState.Open)
						connection.Open();

					using (SqlCommand command = GenerateSPCommand(connection, objectInstance))
					{
						countInserted = await command.ExecuteNonQueryAsync();

						command.Parameters.Clear();
					}

					connection.Close();
				}
			}
			catch
			{
				throw;
			}

			return countInserted;

		}
	}
}

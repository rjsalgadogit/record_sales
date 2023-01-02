using System;
using System.Collections.Generic;
using System.Text;

namespace Sequel.Attributes
{
	public class TableNameAttribute : Attribute
	{
		public string _tableName = string.Empty;

		public TableNameAttribute(string tableName)
		{
			_tableName = tableName;
		}

		public string TableName
		{
			get { return _tableName; }
		}
	}
}

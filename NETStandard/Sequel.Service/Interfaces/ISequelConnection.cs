using System;
using System.Collections.Generic;
using System.Text;

namespace Sequel.Service.Interfaces
{
	public interface ISequelConnection
	{
		string ConnectionString { get; }
	}
}

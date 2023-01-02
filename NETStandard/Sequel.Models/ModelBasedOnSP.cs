using System;
using System.Collections.Generic;
using System.Text;

namespace Sequel.Models
{
	public class ModelBasedOnSP : ModelBaseSql
	{
		/// <summary>
		/// Gets or sets the id of the record on the old SharePoint 2013 system
		/// </summary>
		public int? SP2013Id { get; set; }
	}
}

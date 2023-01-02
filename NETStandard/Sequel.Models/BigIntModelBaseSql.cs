using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Xml.Linq;

namespace Sequel.Models
{
	public class BigIntModelBaseSql : BigIntModelFoundation
	{
		/// <summary>
		/// Gets the creation date
		/// </summary>
		[DisplayFormat(DataFormatString = "{0:MMM dd, yyyy HH:mm:ss}")]
		[Display(Name = "Created on")]
		public DateTime CreationDate { get; set; }

		/// <summary>
		/// Gets or sets the userId of the person that created
		/// </summary>
		[Display(Name = "Created by")]
		public string CreationUserId { get; set; }

		/// <summary>
		/// Gets or sets the modification date 
		/// </summary>
		[DisplayFormat(DataFormatString = "{0:MMM dd, yyyy HH:mm:ss}")]
		[Display(Name = "Last Modified on")]
		public DateTime ModificationDate { get; set; }

		/// <summary>
		/// Gets or sets the userId of the person that modified 
		/// </summary>
		[Display(Name = "Modified by")]
		public string ModificationUserId { get; set; }
	}
}

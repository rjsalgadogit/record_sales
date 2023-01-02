using Sequel.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Sequel.Models
{
	public class BigIntModelFoundation
	{
		[Key]
		[IdentityKey]
		public Int64 id { get; set; }
	}
}

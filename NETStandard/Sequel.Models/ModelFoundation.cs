using Sequel.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Sequel.Models
{
	public class ModelFoundation
	{
		[Key]
		[IdentityKey]
		public int id { get; set; }
	}
}

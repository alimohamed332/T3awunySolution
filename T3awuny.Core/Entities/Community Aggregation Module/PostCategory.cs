using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace T3awuny.Core.Entities.Community_Aggregation_Module
{
	public class PostCategory :BaseEntity
	{
		public int Id { get; set; }
		public string Type { get; set; } = default!;
	}
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace T3awuny.Core.Entities.Community_Aggregation_Module
{
	public class PostFile : BaseEntity
	{
		public string Id { get; set; } = Guid.NewGuid().ToString();
		public string FileUrl { get; set; } = default!;
		public DateTime CreatedAt { get; private set; } = DateTime.Now;

		//Forigen Keys 
		public string PostId { get; set; }
	}
}

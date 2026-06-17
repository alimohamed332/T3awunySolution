using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using T3awuny.Core.Entities.UserModule;

namespace T3awuny.Core.Entities.Community_Aggregation_Module
{
	public class CommentLikes  :BaseEntity
	{
		public string Id { get; set; } = Guid.NewGuid().ToString();
		public DateTime InteractedAt { get; private set; } = DateTime.Now;

		//Forigen Keys 
		public string CommentId { get; set; } = default!;
		[ForeignKey(nameof(ApplicationUser.Id))]
		public string UserId { get; set; } = default!;
	}
}

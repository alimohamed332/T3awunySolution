using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using T3awuny.Core.Entities.UserModule;

namespace T3awuny.Core.Entities.Community_Aggregation_Module
{
	public class Post : BaseEntity
	{
		public string Id { get; set; } = Guid.NewGuid().ToString();
		public DateTime CreatedAt { get; private set; } = DateTime.Now;
		public string Title { get; set; } =String.Empty;
		public string Description { get; set; } = default!;
		//Navigation Properties
		public Category? Category { get; set; }
		public ApplicationUser? Publisher { get; set; }
		public IReadOnlyList<PostFile>? Files { get; set; }
		public IReadOnlyList<PostShare>? Shares { get; set; }
		public IReadOnlyList<PostLike>? Likes { get; set; }
		public IReadOnlyList<Comment>? Comments { get; set; }
		//Forigen Keys
		public int CategoryId { get; set; }
		[ForeignKey(nameof(ApplicationUser.Id))]
		public string UserId { get; set; } = default!;
		//Helper Properties
		[NotMapped]
		public TimeSpan CreatedSince => DateTime.Now - CreatedAt;
		

	}
}

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using T3awuny.Core.Entities.UserModule;

namespace T3awuny.Core.Entities.Community_Aggregation_Module
{
	public class Comment : BaseEntity
	{
		public string Id { get; set; } = Guid.NewGuid().ToString();
		public string Description { get; set; } = default!;
		public DateTime CreatedAt { get; private set; } = DateTime.Now;
		public IReadOnlyList<CommentLikes>? Likes { get; set; }
		public IReadOnlyList<Comment>? SubComments { get; set; }

		//Forigen Keys
		public string PostId { get; set; } = default!;
		[ForeignKey(nameof(Comment.Id))]
		public string? ParentCommentId {  get; set; }
		[ForeignKey(nameof(ApplicationUser.Id))]
		public string UserId { get; set; } = default!;
	}
}

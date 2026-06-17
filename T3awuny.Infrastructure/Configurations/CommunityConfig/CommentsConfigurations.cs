using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using T3awuny.Core.Entities.Community_Aggregation_Module;

namespace T3awuny.Infrastructure.Configurations.CommunityConfig
{
	internal class CommentsConfigurations : IEntityTypeConfiguration<Comment>
	{
		public void Configure(EntityTypeBuilder<Comment> builder)
		{
			builder.HasMany(Comment => Comment.Likes)
				.WithOne()
				.HasForeignKey(Like => Like.CommentId);

			//Comment that have comments

			builder.HasMany(Comment => Comment.SubComments)
				.WithOne()
				.HasForeignKey(Comment => Comment.ParentCommentId)
				.IsRequired(false);
		}
	}
}

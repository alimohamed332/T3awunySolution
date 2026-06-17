using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using T3awuny.Core.Entities.Community_Aggregation_Module;

namespace T3awuny.Infrastructure.Configurations.CommunityConfig
{
	internal class PostConfigurations : IEntityTypeConfiguration<Post>
	{
		public void Configure(EntityTypeBuilder<Post> builder)
		{
			// relation between user and posts [1:m]
			builder.HasOne(post => post.Publisher)
				.WithMany()
				.HasForeignKey(post => post.UserId)
				.IsRequired()
				.OnDelete(DeleteBehavior.NoAction);
			// relation between category and posts [1:m]
			builder.HasOne(post => post.Category)
				.WithMany()
				.HasForeignKey(post => post.CategoryId);
			// relation between posts and files  [m:1]
			builder.HasMany(post => post.Files)
				.WithOne()
				.HasForeignKey(File => File.PostId);
			// relation between Shares and post [m:1]
			builder.HasMany(post => post.Shares)
				.WithOne()
				.HasForeignKey(Share => Share.PostId);
			// relation between Likes and post [m:1]
			builder.HasMany(post => post.Likes)
				.WithOne()
				.HasForeignKey(Like => Like.PostId);
			// relation between Likes and post [m:1]
			builder.HasMany(post => post.Comments)
				.WithOne()
				.HasForeignKey(Comment => Comment.PostId);
		}
	}
}

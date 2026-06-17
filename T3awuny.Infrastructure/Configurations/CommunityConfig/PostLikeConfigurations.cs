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
	internal class PostLikeConfigurations : IEntityTypeConfiguration<PostLike>
	{
		void IEntityTypeConfiguration<PostLike>.Configure(EntityTypeBuilder<PostLike> builder)
		{
			builder.HasOne(Likes => Likes.User)
				.WithMany()
				.HasForeignKey(like => like.UserId)
				.OnDelete(DeleteBehavior.NoAction);
		}
	}
}

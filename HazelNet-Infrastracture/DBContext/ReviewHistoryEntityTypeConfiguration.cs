using HazelNet_Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HazelNet_Infrastracture.DBContext;

public class ReviewHistoryEntityTypeConfiguration:  IEntityTypeConfiguration<ReviewHistory>
{
    public void Configure(EntityTypeBuilder<ReviewHistory> builder)
    {
        builder.HasKey(h => h.Id);
        builder.Property(h => h.Id).ValueGeneratedNever();

        builder.HasMany(h => h.ReviewLogs)
            .WithOne(l => l.ReviewHistory)
            .HasForeignKey(l => l.ReviewHistoryId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
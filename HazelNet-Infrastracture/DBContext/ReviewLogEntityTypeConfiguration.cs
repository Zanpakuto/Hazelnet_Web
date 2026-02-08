using HazelNet_Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HazelNet_Infrastracture.DBContext;

public class ReviewLogEntityTypeConfiguration :  IEntityTypeConfiguration<ReviewLog>
{
    public void Configure(EntityTypeBuilder<ReviewLog> builder)
    {
        builder.HasKey(r => r.Id);
        builder.Property(r => r.Id).ValueGeneratedNever();

        builder.HasOne(r => r.ReviewHistory)
            .WithMany(l => l.ReviewLogs)
            .HasForeignKey(r => r.ReviewHistoryId);
    }
}
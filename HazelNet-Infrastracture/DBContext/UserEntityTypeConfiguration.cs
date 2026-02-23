using HazelNet_Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HazelNet_Infrastracture.DBContext;

public class UserEntityTypeConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.HasKey(c => c.Id);
        builder.Property(c => c.Id).ValueGeneratedOnAdd();
        builder.Property(c => c.Username)
            .HasMaxLength(30)
            .IsRequired();
        
        builder.HasMany(c => c.Decks)
            .WithOne(d => d.User)
            .HasForeignKey(d => d.UserId);
    }
}
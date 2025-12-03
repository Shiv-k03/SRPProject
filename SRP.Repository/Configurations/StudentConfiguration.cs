using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SRP.Repository.Entities;

namespace StudentReportPortal.Infrastructure.Configurations
{
    public class StudentConfiguration : IEntityTypeConfiguration<Student>
    {
        public void Configure(EntityTypeBuilder<Student> builder)
        {
            builder.ToTable("Students");
            builder.HasKey(s => s.StudentId);

            builder.Property(s => s.RollNumber)
                .IsRequired()
                .HasMaxLength(20);

            builder.Property(s => s.PhoneNumber)
                .HasMaxLength(15);

            builder.Property(s => s.Address)
                .HasMaxLength(200);

            builder.HasIndex(s => s.RollNumber).IsUnique();

            builder.HasMany(s => s.StudentSubjects)
                .WithOne(ss => ss.Student)
                .HasForeignKey(ss => ss.StudentId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(s => s.Marks)
                .WithOne(m => m.Student)
                .HasForeignKey(m => m.StudentId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
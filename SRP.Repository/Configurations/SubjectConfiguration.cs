using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SRP.Repository.Entities;

namespace StudentReportPortal.Infrastructure.Configurations
{
    public class SubjectConfiguration : IEntityTypeConfiguration<Subject>
    {
        public void Configure(EntityTypeBuilder<Subject> builder)
        {
            builder.ToTable("Subjects");
            builder.HasKey(s => s.SubjectId);

            builder.Property(s => s.SubjectName).IsRequired().HasMaxLength(100);
            builder.Property(s => s.SubjectCode).IsRequired().HasMaxLength(20);
            builder.Property(s => s.Description).HasMaxLength(500);

            builder.HasIndex(s => s.SubjectCode).IsUnique();

            builder.HasMany(s => s.TeacherSubjects)
                .WithOne(ts => ts.Subject)
                .HasForeignKey(ts => ts.SubjectId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(s => s.StudentSubjects)
                .WithOne(ss => ss.Subject)
                .HasForeignKey(ss => ss.SubjectId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(s => s.Marks)
                .WithOne(m => m.Subject)
                .HasForeignKey(m => m.SubjectId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
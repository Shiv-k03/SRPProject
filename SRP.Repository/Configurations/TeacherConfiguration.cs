using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SRP.Repository.Entities;

namespace StudentReportPortal.Infrastructure.Configurations
{
    public class TeacherConfiguration : IEntityTypeConfiguration<Teacher>
    {
        public void Configure(EntityTypeBuilder<Teacher> builder)
        {
            builder.ToTable("Teachers");

            builder.HasKey(t => t.TeacherId);

            builder.Property(t => t.EmployeeCode)
                .IsRequired()
                .HasMaxLength(20);

            builder.Property(t => t.Qualification)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(t => t.PhoneNumber)
                .HasMaxLength(15);

            builder.HasIndex(t => t.EmployeeCode).IsUnique();

            builder.HasMany(t => t.TeacherSubjects)
                .WithOne(ts => ts.Teacher)
                .HasForeignKey(ts => ts.TeacherId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
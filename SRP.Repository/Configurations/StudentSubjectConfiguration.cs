using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SRP.Repository.Entities;

namespace StudentReportPortal.Infrastructure.Configurations
{
    public class StudentSubjectConfiguration : IEntityTypeConfiguration<StudentSubject>
    {
        public void Configure(EntityTypeBuilder<StudentSubject> builder)
        {
            builder.ToTable("StudentSubjects");
            builder.HasKey(ss => ss.StudentSubjectId);

            builder.Property(ss => ss.Status)
                .IsRequired()
                .HasMaxLength(20);

            builder.HasIndex(ss => new 
            {
                ss.StudentId,
                ss.SubjectId,
                ss.Semester 
            });
        }
    }
}
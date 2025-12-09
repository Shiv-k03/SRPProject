using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SRP.Repository.Entities;

namespace StudentReportPortal.Infrastructure.Configurations
{
    public class TeacherSubjectConfiguration : IEntityTypeConfiguration<TeacherSubject>
    {
        public void Configure(EntityTypeBuilder<TeacherSubject> builder)
        {
            builder.ToTable("TeacherSubjects");
            builder.HasKey(ts => ts.TeacherSubjectId);

            builder.Property(ts => ts.AcademicYear)
                .IsRequired()
                .HasMaxLength(10);

            builder.HasIndex(ts => new
            {
                ts.TeacherId,
                ts.SubjectId,
                ts.Semester,
                ts.AcademicYear
            });
        }
    }
}


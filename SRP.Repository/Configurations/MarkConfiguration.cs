using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SRP.Repository.Entities;

namespace StudentReportPortal.Infrastructure.Configurations
{
    public class MarkConfiguration : IEntityTypeConfiguration<Mark>
    {
        public void Configure(EntityTypeBuilder<Mark> builder)
        {
            builder.ToTable("Marks");

            builder.HasKey(m => m.MarkId);

            builder.Property(m => m.ObtainedMarks)
                .HasColumnType("decimal(5,2)");

            builder.Property(m => m.TotalMarks)
                .HasColumnType("decimal(5,2)");

            builder.Property(m => m.ExamType)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(m => m.Grade)
                .IsRequired()
                .HasMaxLength(5);

            builder.Property(m => m.Remarks)
                .HasMaxLength(200);

            builder.HasIndex(m => new 
            {
                m.StudentId, 
                m.SubjectId, 
                m.ExamType 
            });
        }
    }
}
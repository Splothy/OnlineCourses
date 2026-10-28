using Infrastructure.Persistence.Converters;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OnlineCourses.Domain.Entities;

namespace Infrastructure.Persistence.Configurations;

public sealed class LessonConfiguration : IEntityTypeConfiguration<Lesson>
{
    public void Configure(EntityTypeBuilder<Lesson> builder)
    {
        builder.ToTable("lessons");
        builder.HasKey(lesson => lesson.Id);
        builder.Property(lesson => lesson.Id).HasConversion(new LessonIdConverter()).ValueGeneratedNever();
        builder.Property(lesson => lesson.CourseId).HasConversion(new CourseIdConverter()).IsRequired();
        builder.Property(lesson => lesson.Title).HasMaxLength(160).IsRequired();
        builder.Property(lesson => lesson.Order).IsRequired();
        builder.Property(lesson => lesson.ContentType).HasMaxLength(40).IsRequired();
        builder.Property(lesson => lesson.CreatedAt)
            .HasConversion(new DateTimeUtcConverter())
            .HasDefaultValueSql("timezone('utc', now())")
            .IsRequired();
        builder.HasIndex(lesson => new { lesson.CourseId, lesson.Order }).IsUnique();
    }
}

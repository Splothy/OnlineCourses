using Infrastructure.Persistence.Converters;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OnlineCourses.Domain.Entities;

namespace Infrastructure.Persistence.Configurations;

public sealed class CourseConfiguration : IEntityTypeConfiguration<Course>
{
    public void Configure(EntityTypeBuilder<Course> builder)
    {
        builder.ToTable("courses");
        builder.HasKey(course => course.Id);
        builder.Property(course => course.Id).HasConversion(new CourseIdConverter()).ValueGeneratedNever();
        builder.Property(course => course.Title).HasMaxLength(160).IsRequired();
        builder.HasIndex(course => course.Title).IsUnique();
        builder.Property(course => course.Description).HasMaxLength(4000).IsRequired();
        builder.Property(course => course.Level).HasMaxLength(40).IsRequired();
        builder.Property(course => course.DurationInMinutes).IsRequired();
        builder.Property(course => course.CreatedAt)
            .HasConversion(new DateTimeUtcConverter())
            .HasDefaultValueSql("timezone('utc', now())")
            .IsRequired();

        builder.HasMany(course => course.Lessons)
            .WithOne()
            .HasForeignKey(lesson => lesson.CourseId)
            .OnDelete(DeleteBehavior.Cascade);
        builder.Navigation(course => course.Lessons).UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}

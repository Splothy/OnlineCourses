using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using OnlineCourses.Domain.Entities;

namespace Infrastructure.Persistence.Converters;

public sealed class CourseIdConverter() : ValueConverter<CourseId, Guid>(
    id => id.Value,
    value => CourseId.From(value));

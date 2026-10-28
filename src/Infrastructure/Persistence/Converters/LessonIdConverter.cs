using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using OnlineCourses.Domain.Entities;

namespace Infrastructure.Persistence.Converters;

public sealed class LessonIdConverter() : ValueConverter<LessonId, Guid>(
    id => id.Value,
    value => LessonId.From(value));

using Application.Abstractions.Persistence;
using Application.Courses.Exceptions;
using LanguageExt;
using Microsoft.EntityFrameworkCore;
using OnlineCourses.Domain.Entities;

namespace Infrastructure.Persistence.Repositories;

public sealed class LessonRepository(ApplicationDbContext context) : ILessonRepository, ILessonQueries
{
    public async Task<Option<Lesson>> GetById(CourseId courseId, LessonId lessonId, CancellationToken cancellationToken)
    {
        var lesson = await context.Lessons.FirstOrDefaultAsync(
            item => item.CourseId == courseId && item.Id == lessonId, cancellationToken);
        return lesson is null ? Option<Lesson>.None : Option<Lesson>.Some(lesson);
    }

    public async Task<Option<Lesson>> GetByOrder(CourseId courseId, int order, CancellationToken cancellationToken)
    {
        var lesson = await context.Lessons.FirstOrDefaultAsync(
            item => item.CourseId == courseId && item.Order == order, cancellationToken);
        return lesson is null ? Option<Lesson>.None : Option<Lesson>.Some(lesson);
    }

    public async Task<IReadOnlyList<Lesson>> GetByCourseId(CourseId courseId, CancellationToken cancellationToken) =>
        await context.Lessons.AsNoTracking().Where(item => item.CourseId == courseId)
            .OrderBy(item => item.Order).ToListAsync(cancellationToken);

    public async Task<Either<CourseException, Lesson>> Add(Lesson lesson, CancellationToken cancellationToken)
    {
        try
        {
            await context.Lessons.AddAsync(lesson, cancellationToken);
            await context.SaveChangesAsync(cancellationToken);
            return lesson;
        }
        catch (Exception exception)
        {
            return new CourseUnhandledException(lesson.CourseId, exception);
        }
    }

    public async Task<Either<CourseException, Lesson>> Update(Lesson lesson, CancellationToken cancellationToken)
    {
        try
        {
            context.Lessons.Update(lesson);
            await context.SaveChangesAsync(cancellationToken);
            return lesson;
        }
        catch (Exception exception)
        {
            return new CourseUnhandledException(lesson.CourseId, exception);
        }
    }
}

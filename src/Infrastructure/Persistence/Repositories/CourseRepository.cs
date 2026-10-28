using System.Net.Mime;
using Application.Abstractions.Persistence;
using Application.Courses.Exceptions;
using LanguageExt;
using Microsoft.EntityFrameworkCore;
using OnlineCourses.Domain.Entities;

namespace Infrastructure.Persistence.Repositories;

public sealed class CourseRepository(ApplicationDbContext context) : ICourseRepository, ICourseQueries
{
    public async Task<IReadOnlyList<Course>> GetAll(CancellationToken cancellationToken) =>
        await context.Courses.Include(course => course.Lessons).AsNoTracking().ToListAsync(cancellationToken);

    public async Task<Option<Course>> GetById(CourseId id, CancellationToken cancellationToken)
    {
        var course = await context.Courses.Include(item => item.Lessons)
            .FirstOrDefaultAsync(item => item.Id == id, cancellationToken);
        return course is null ? Option<Course>.None : Option<Course>.Some(course);
    }

    public async Task<Option<Course>> GetByTitle(string title, CancellationToken cancellationToken)
    {
        var course = await context.Courses.FirstOrDefaultAsync(
            item => item.Title.ToLower() == title.Trim().ToLower(), cancellationToken);
        return course is null ? Option<Course>.None : Option<Course>.Some(course);
    }

    public async Task<Either<CourseException, Course>> Add(Course course, CancellationToken cancellationToken)
    {
        try
        {
            await context.Courses.AddAsync(course, cancellationToken);
            await context.SaveChangesAsync(cancellationToken);
            return course;
        }
        catch (Exception exception)
        {
            return new CourseUnhandledException(course.Id, exception);
        }
    }

    public async Task<Either<CourseException, Course>> Update(Course course, CancellationToken cancellationToken)
    {
        try
        {
            context.Courses.Update(course);
            await context.SaveChangesAsync(cancellationToken);
            return course;
        }
        catch (Exception exception)
        {
            return new CourseUnhandledException(course.Id, exception);
        }
    }
}

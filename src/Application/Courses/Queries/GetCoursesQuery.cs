using MediatR;
using OnlineCourses.Domain.Entities;

namespace Application.Courses.Queries;

public sealed record GetCoursesQuery : IRequest<IReadOnlyList<Course>>;
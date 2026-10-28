using Api.Dtos;
using Application.Courses.Commands;
using Application.Courses.Exceptions;
using Application.Courses.Queries;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using OnlineCourses.Domain.Entities;

namespace Api.Controllers;

[ApiController]
[Route("courses")]
public sealed class CoursesController(ISender sender) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<CourseResponse>>> GetAll(
        CancellationToken cancellationToken)
    {
        var courses = await sender.Send(new GetCoursesQuery(), cancellationToken);
        return Ok(courses.Select(ToResponse).ToList());
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<CourseResponse>> GetById(
        Guid id,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(new GetCourseByIdQuery(CourseId.From(id)), cancellationToken);

        return result.Match<ActionResult<CourseResponse>>(
            course => Ok(ToResponse(course)),
            () => NotFound());
    }

    [HttpPost]
    public async Task<ActionResult<CourseResponse>> Create(
        CreateCourseRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var result = await sender.Send(
                new CreateCourseCommand(
                    request.Title,
                    request.Description,
                    request.Level,
                    request.DurationInMinutes),
                cancellationToken);

            return result.Match<ActionResult<CourseResponse>>(
                course => CreatedAtAction(
                    nameof(GetById),
                    new { id = course.Id.Value },
                    ToResponse(course)),
                error => ToErrorResult(error));
        }
        catch (ValidationException exception)
        {
            return BadRequest(new { errors = exception.Errors.Select(x => x.ErrorMessage) });
        }
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<CourseResponse>> Update(
        Guid id,
        UpdateCourseRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var result = await sender.Send(
                new UpdateCourseCommand(
                    CourseId.From(id),
                    request.Title,
                    request.Description,
                    request.Level,
                    request.DurationInMinutes),
                cancellationToken);

            return result.Match<ActionResult<CourseResponse>>(
                course => Ok(ToResponse(course)),
                error => ToErrorResult(error));
        }
        catch (ValidationException exception)
        {
            return BadRequest(new { errors = exception.Errors.Select(x => x.ErrorMessage) });
        }
    }

    private ActionResult<CourseResponse> ToErrorResult(CourseException error) =>
        error switch
        {
            CourseNotFoundException => NotFound(new { error.Message }),
            CourseAlreadyExistsException => Conflict(new { error.Message }),
            _ => StatusCode(StatusCodes.Status500InternalServerError, new { error.Message })
        };

    private static CourseResponse ToResponse(Course course) =>
        new(
            course.Id.Value,
            course.Title,
            course.Description,
            course.Level,
            course.DurationInMinutes,
            course.CreatedAt,
            course.Lessons
                .OrderBy(lesson => lesson.Order)
                .Select(lesson => new LessonResponse(
                    lesson.Id.Value,
                    lesson.Title,
                    lesson.Order,
                    lesson.ContentType,
                    lesson.CreatedAt))
                .ToList());
}

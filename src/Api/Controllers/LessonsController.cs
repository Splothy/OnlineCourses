using Api.Dtos;
using Application.Courses.Exceptions;
using Application.Lessons.Commands;
using Application.Lessons.Queries;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using OnlineCourses.Domain.Entities;

namespace Api.Controllers;

[ApiController]
[Route("courses/{courseId:guid}/lessons")]
public sealed class LessonsController(ISender sender) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<LessonResponse>>> GetByCourse(
        Guid courseId,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(
            new GetLessonsByCourseQuery(CourseId.From(courseId)), cancellationToken);
        if (result.IsNone)
            return NotFound();

        var lessons = result.IfNone(() => throw new InvalidOperationException());
        return Ok(lessons.Select(ToResponse).ToList());
    }

    [HttpGet("{lessonId:guid}")]
    public async Task<ActionResult<LessonResponse>> GetById(
        Guid courseId,
        Guid lessonId,
        CancellationToken cancellationToken)
    {
        var lesson = await sender.Send(
            new GetLessonByIdQuery(CourseId.From(courseId), LessonId.From(lessonId)), cancellationToken);
        return lesson.Match<ActionResult<LessonResponse>>(
            value => Ok(ToResponse(value)),
            () => NotFound());
    }

    [HttpPost]
    public async Task<ActionResult<LessonResponse>> Create(
        Guid courseId,
        CreateLessonRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var result = await sender.Send(new CreateLessonCommand(
                CourseId.From(courseId), request.Title, request.Order, request.ContentType), cancellationToken);
            return result.Match<ActionResult<LessonResponse>>(
                lesson => CreatedAtAction(nameof(GetById),
                    new { courseId, lessonId = lesson.Id.Value }, ToResponse(lesson)),
                ToErrorResult);
        }
        catch (ValidationException exception)
        {
            return BadRequest(new { errors = exception.Errors.Select(error => error.ErrorMessage) });
        }
    }

    [HttpPut("{lessonId:guid}")]
    public async Task<ActionResult<LessonResponse>> Update(
        Guid courseId,
        Guid lessonId,
        UpdateLessonRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var result = await sender.Send(new UpdateLessonCommand(
                CourseId.From(courseId), LessonId.From(lessonId), request.Title, request.Order, request.ContentType),
                cancellationToken);
            return result.Match<ActionResult<LessonResponse>>(
                lesson => Ok(ToResponse(lesson)),
                ToErrorResult);
        }
        catch (ValidationException exception)
        {
            return BadRequest(new { errors = exception.Errors.Select(error => error.ErrorMessage) });
        }
    }

    [HttpDelete("{lessonId:guid}")]
    public async Task<ActionResult> Delete(
        Guid courseId,
        Guid lessonId,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(
            new DeleteLessonCommand(CourseId.From(courseId), LessonId.From(lessonId)),
            cancellationToken);

        return result.Match<ActionResult>(
            _ => NoContent(),
            ToActionResult);
    }

    private ActionResult ToActionResult(CourseException error) =>
        error switch
        {
            CourseNotFoundException or LessonNotFoundException => NotFound(new { error.Message }),
            LessonAlreadyExistsException => Conflict(new { error.Message }),
            _ => StatusCode(StatusCodes.Status500InternalServerError, new { error.Message })
        };

    private ActionResult<LessonResponse> ToErrorResult(CourseException error) =>
        error switch
        {
            CourseNotFoundException or LessonNotFoundException => NotFound(new { error.Message }),
            LessonAlreadyExistsException => Conflict(new { error.Message }),
            _ => StatusCode(StatusCodes.Status500InternalServerError, new { error.Message })
        };

    private static LessonResponse ToResponse(Lesson lesson) =>
        new(lesson.Id.Value, lesson.Title, lesson.Order, lesson.ContentType, lesson.CreatedAt);
}

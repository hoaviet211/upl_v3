using System.Security.Claims;
using Academy.Crm.Api.Controllers;
using Academy.Crm.Api.Mapping;
using Academy.Crm.Core.Entities;
using Academy.Crm.Infra.Repositories;
using AutoMapper;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Xunit;

namespace Academy.Crm.Tests;

public class SelfEnrollTests
{
    [Fact]
    public async Task SelfEnroll_Uses_Current_Student()
    {
        using var db = DbTestHelper.CreateSqliteInMemory();
        var uow = new UnitOfWork(db);
        var student = new Student { StudentCode = "S1", FirstName = "A", LastName = "B", PhoneNumber = "1" };
        var course = new Course { CourseCode = "C1", CourseName = "Course 1", ProgrammeId = 1, StartDate = new DateOnly(2025,1,1), EndDate = new DateOnly(2025,1,2) };
        await db.Students.AddAsync(student);
        await db.Courses.AddAsync(course);
        await db.SaveChangesAsync();

        var mapper = new MapperConfiguration(cfg => cfg.AddProfile<ApiMappingProfile>()).CreateMapper();
        var controller = new EnrollmentsController(uow, mapper)
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = new ClaimsPrincipal(new ClaimsIdentity(new[]
                    {
                        new Claim("studentId", student.Id.ToString())
                    }, "TestAuth"))
                }
            }
        };

        var res = await controller.SelfEnroll(course.Id, default);
        var ok = res.Result as OkObjectResult;
        ok.Should().NotBeNull();
        dynamic payload = ok!.Value!;
        ((int)payload.StudentId).Should().Be(student.Id);
        ((int)payload.CourseId).Should().Be(course.Id);
    }
}


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

public class CoursesPagingTests
{
    [Fact]
    public async Task Paging_Returns_Total_Page_PageSize()
    {
        using var db = DbTestHelper.CreateSqliteInMemory();
        var uow = new UnitOfWork(db);
        for (int i = 1; i <= 25; i++)
        {
            await db.Courses.AddAsync(new Course { CourseCode = $"C{i}", CourseName = $"Course {i}", ProgrammeId = 1, StartDate = new DateOnly(2025,1,1), EndDate = new DateOnly(2025,12,31) });
        }
        await db.SaveChangesAsync();

        var mapper = new MapperConfiguration(cfg => cfg.AddProfile<ApiMappingProfile>()).CreateMapper();
        var controller = new CoursesController(uow, mapper)
        {
            ControllerContext = new ControllerContext { HttpContext = new DefaultHttpContext() }
        };
        var result = await controller.List(page: 2, pageSize: 10, sort: null, q: null);
        var ok = result.Result as OkObjectResult;
        ok.Should().NotBeNull();
        dynamic payload = ok!.Value!;
        ((int)payload.Paging.Total).Should().Be(25);
        ((int)payload.Paging.Page).Should().Be(2);
        ((int)payload.Paging.PageSize).Should().Be(10);
    }
}


using Academy.Crm.Core.Entities;
using Academy.Crm.Core.Enums;
using Academy.Crm.Infra.Repositories;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Academy.Crm.Tests;

public class EnrollmentTests
{
    [Fact]
    public async Task Cannot_Create_Duplicate_Enrollment()
    {
        using var db = DbTestHelper.CreateSqliteInMemory();
        var uow = new UnitOfWork(db);
        var student = new Student { StudentCode = "S1", FirstName = "A", LastName = "B", PhoneNumber = "1" };
        var course = new Course { CourseCode = "C1", CourseName = "Course 1", ProgrammeId = 1, StartDate = new DateOnly(2025,1,1), EndDate = new DateOnly(2025,1,2) };
        await db.Students.AddAsync(student);
        await db.Courses.AddAsync(course);
        await db.SaveChangesAsync();

        await uow.Enrollments.AddAsync(new Enrollment { StudentId = student.Id, CourseId = course.Id, Status = EnrollmentStatus.Pending });
        await uow.SaveChangesAsync();

        await uow.Invoking(u => u.Enrollments.AddAsync(new Enrollment { StudentId = student.Id, CourseId = course.Id, Status = EnrollmentStatus.Pending }))
            .Should().NotThrowAsync();

        await uow.Invoking(u => u.SaveChangesAsync())
            .Should().ThrowAsync<DbUpdateException>();
    }
}


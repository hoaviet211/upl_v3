using Academy.Crm.Core.Entities;
using Academy.Crm.Core.Enums;
using Academy.Crm.Infra.Db;
using Microsoft.EntityFrameworkCore;

namespace Academy.Crm.Infra.Seed;

public static class DataSeeder
{
    public static async Task SeedAsync(AppDbContext db, Func<string, string> hashPassword, CancellationToken ct = default)
    {
        await db.Database.MigrateAsync(ct);

        if (!await db.Programmes.AnyAsync(ct))
        {
            var programme = new Programme
            {
                ProgrammeName = "NCLD",
                AdmissionCycle = "K71-K72"
            };

            var course1 = new Course
            {
                CourseCode = "NCLD-K71",
                CourseName = "NCLD Khoá 71",
                StartDate = new DateOnly(2025, 1, 1),
                EndDate = new DateOnly(2025, 3, 31),
            };
            var course2 = new Course
            {
                CourseCode = "NCLD-K72",
                CourseName = "NCLD Khoá 72",
                StartDate = new DateOnly(2025, 4, 1),
                EndDate = new DateOnly(2025, 6, 30),
            };
            programme.Courses.Add(course1);
            programme.Courses.Add(course2);
            await db.Programmes.AddAsync(programme, ct);
            await db.SaveChangesAsync(ct);

            foreach (var c in new[] { course1, course2 })
            {
                for (int i = 1; i <= 6; i++)
                {
                    db.ClassSessions.Add(new ClassSession
                    {
                        CourseId = c.Id,
                        SessionCode = $"{c.CourseCode}-S{i:00}",
                        Title = $"Buổi {i}",
                        StartTime = DateTime.UtcNow.AddDays(i),
                        EndTime = DateTime.UtcNow.AddDays(i).AddHours(2),
                        DurationMinutes = 120,
                        Mode = ClassSessionMode.Offline,
                        Status = ClassSessionStatus.Planned
                    });
                }
            }
            await db.SaveChangesAsync(ct);
        }

        if (!await db.Students.AnyAsync(ct))
        {
            var student = new Student
            {
                StudentCode = "thanhtam",
                FirstName = "Tam",
                LastName = "Thanh",
                PhoneNumber = "0900000000",
                FullName = "Thanh Tam"
            };
            await db.Students.AddAsync(student, ct);
            await db.SaveChangesAsync(ct);

            await db.UserAccounts.AddRangeAsync(new[]
            {
                new UserAccount { UserName = "admin", PasswordHash = hashPassword("testing"), Role = "Admin" },
                new UserAccount { UserName = "thanhtam", PasswordHash = hashPassword("testing"), Role = "Student", StudentId = student.Id }
            }, ct);

            await db.SaveChangesAsync(ct);
        }
    }
}


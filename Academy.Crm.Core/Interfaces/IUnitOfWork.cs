using Academy.Crm.Core.Entities;

namespace Academy.Crm.Core.Interfaces;

public interface IUnitOfWork : IAsyncDisposable
{
    IRepository<Programme> Programmes { get; }
    IRepository<Course> Courses { get; }
    IRepository<ClassSession> ClassSessions { get; }
    IRepository<Student> Students { get; }
    IRepository<IdCard> IdCards { get; }
    IRepository<Enrollment> Enrollments { get; }
    IRepository<Attendance> Attendances { get; }

    Task<int> SaveChangesAsync(CancellationToken ct = default);
}


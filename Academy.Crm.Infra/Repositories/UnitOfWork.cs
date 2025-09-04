using Academy.Crm.Core.Entities;
using Academy.Crm.Core.Interfaces;
using Academy.Crm.Infra.Db;

namespace Academy.Crm.Infra.Repositories;

public class UnitOfWork : IUnitOfWork
{
    private readonly AppDbContext _db;

    public UnitOfWork(AppDbContext db)
    {
        _db = db;
        Programmes = new Repository<Programme>(_db);
        Courses = new Repository<Course>(_db);
        ClassSessions = new Repository<ClassSession>(_db);
        Students = new Repository<Student>(_db);
        IdCards = new Repository<IdCard>(_db);
        Enrollments = new Repository<Enrollment>(_db);
        Attendances = new Repository<Attendance>(_db);
        UserAccounts = new Repository<UserAccount>(_db);
    }

    public IRepository<Programme> Programmes { get; }
    public IRepository<Course> Courses { get; }
    public IRepository<ClassSession> ClassSessions { get; }
    public IRepository<Student> Students { get; }
    public IRepository<IdCard> IdCards { get; }
    public IRepository<Enrollment> Enrollments { get; }
    public IRepository<Attendance> Attendances { get; }
    public IRepository<UserAccount> UserAccounts { get; }

    public Task<int> SaveChangesAsync(CancellationToken ct = default)
        => _db.SaveChangesAsync(ct);

    public ValueTask DisposeAsync() => _db.DisposeAsync();
}

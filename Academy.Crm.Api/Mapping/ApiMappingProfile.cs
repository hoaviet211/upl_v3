using Academy.Crm.Api.DTOs;
using Academy.Crm.Core.Entities;
using AutoMapper;

namespace Academy.Crm.Api.Mapping;

public class ApiMappingProfile : Profile
{
    public ApiMappingProfile()
    {
        CreateMap<Programme, ProgrammeDto>().ReverseMap();
        CreateMap<Course, CourseDto>().ReverseMap();
        CreateMap<ClassSession, ClassSessionDto>().ReverseMap();
        CreateMap<Student, StudentDto>().ReverseMap();
        CreateMap<IdCard, IdCardDto>().ReverseMap();
        CreateMap<Enrollment, EnrollmentDto>().ReverseMap();
        CreateMap<Attendance, AttendanceDto>().ReverseMap();
    }
}


using FluentValidation;
using FluentValidation.AspNetCore;
using SRP.Business.Interfaces;
using SRP.Business.Services;
using SRP.Business.Validators;
using SRP.Repository.Interfaces;
using SRP.Repository.Repositories;

namespace SRP.API.Extensions
{
    public static class ServiceExtensionsID
    {
        public static void AddApplicationServices(this IServiceCollection services)
        {
            // Register Services
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IDepartmentService, DepartmentService>();
            services.AddScoped<ITokenService, TokenService>();
            services.AddScoped<ITeacherService, TeacherService>();
            services.AddScoped<IStudentService, StudentService>();
            services.AddScoped<ISubjectService, SubjectService>();
            services.AddScoped<IMarkService, MarkService>();
            services.AddScoped<IUserService, UserService>();
        }

        public static void AddInfrastructureServices(this IServiceCollection services)
        {
            // Register Repositories
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IDepartmentRepository, DepartmentRepository>();
            services.AddScoped<ITeacherRepository, TeacherRepository>();
            services.AddScoped<IStudentRepository, StudentRepository>();
            services.AddScoped<ISubjectRepository, SubjectRepository>();
            services.AddScoped<ITeacherSubjectRepository, TeacherSubjectRepository>();
            services.AddScoped<IStudentSubjectRepository, StudentSubjectRepository>();
            services.AddScoped<IMarkRepository, MarkRepository>();
        }

        public static void AddFluentValidationServices(this IServiceCollection services)
        {
            services.AddFluentValidationAutoValidation();
            services.AddFluentValidationClientsideAdapters();
            services.AddValidatorsFromAssemblyContaining<LoginRequestValidator>();
        }

      
    }
}

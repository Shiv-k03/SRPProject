using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using SRP.API.Extensions;
using SRP.API.Middleware;
using SRP.API.Mapper;
using SRP.Repository.Context;
using System.Text;
using AutoMapper;
//using AutoMapperExtensions = AutoMapper.Extensions.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// Configure Database
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Add Application Services
builder.Services.AddApplicationServices();

// Add Infrastructure Services (Repositories)
builder.Services.AddInfrastructureServices();

// Add FluentValidation
builder.Services.AddFluentValidationServices();

// Add AutoMapper
builder.Services.AddAutoMapper(typeof(SRP.API.Mapper.AutoMappingProfile));
//builder.Services.AddAutoMapper(typeof(AutoMappingProfile).Assembly);

// Configure JWT Authentication
var jwtSettings = builder.Configuration.GetSection("JwtSettings");
var secretKey = jwtSettings["SecretKey"];

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings["Issuer"],
        ValidAudience = jwtSettings["Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)),
        ClockSkew = TimeSpan.Zero
    };
});

// Add Authorization
builder.Services.AddAuthorization();

// Configure Swagger
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Student Report Portal API",
        Version = "v1",
        Description = "API for Student Report Portal Management System",
        Contact = new OpenApiContact
        {
            Name = "Student Report Portal",
            Email = "support@srp.com"
        }
    });

    // Add JWT Authentication to Swagger
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Enter 'Bearer' [space] and then your token in the text input below.\n\nExample: 'Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...'",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

// Configure CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

//--------------------------------------------------------------------------------------------------------------------------------------
// Build the app
//--------------------------------------------------------------------------------------------------------------------------------------
var app = builder.Build();

//--------------------------------------------------------------------------------------------------------------------------------------
// Configure the HTTP request pipeline
//--------------------------------------------------------------------------------------------------------------------------------------
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Student Report Portal API v1");
        c.RoutePrefix = string.Empty; // Set Swagger UI at the app's root
    });
}

//--------------------------------------------------------------------------------------------------------------------------------------
// Use Custom Exception Handling Middleware
//--------------------------------------------------------------------------------------------------------------------------------------
app.UseMiddleware<ExceptionHandlingMiddleware>();

app.UseHttpsRedirection();

app.UseCors("AllowAll");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();



//using Microsoft.AspNetCore.Authentication.JwtBearer;
//using Microsoft.EntityFrameworkCore;
//using Microsoft.IdentityModel.Tokens;
//using SRP.API.Extensions;
//using SRP.Repository.Context;
//using System.Text;

//var builder = WebApplication.CreateBuilder(args);

////--------------------------------------------------------------------------------------------------------------------------------------
////Add DbContext
////--------------------------------------------------------------------------------------------------------------------------------------
//builder.Services.AddDbContext<ApplicationDbContext>(options =>
//    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));


////--------------------------------------------------------------------------------------------------------------------------------------
////  Add Controllers 
////--------------------------------------------------------------------------------------------------------------------------------------
//builder.Services.AddControllers();

////--------------------------------------------------------------------------------------------------------------------------------------
//// Add Swagger
////--------------------------------------------------------------------------------------------------------------------------------------
//builder.Services.AddEndpointsApiExplorer();
//builder.Services.AddSwaggerGen();

////ServiceExtensionsID.IServiceCollection(builder.Services);

////--------------------------------------------------------------------------------------------------------------------------------------
//// Add CORS
////--------------------------------------------------------------------------------------------------------------------------------------
//builder.Services.AddCors(options =>
//{
//    options.AddPolicy("AllowAll", policy =>
//    {
//        policy.AllowAnyOrigin()
//              .AllowAnyMethod()
//              .AllowAnyHeader();
//    });
//});


////--------------------------------------------------------------------------------------------------------------------------------------
//// Configure AutoMapper
////--------------------------------------------------------------------------------------------------------------------------------------
//builder.Services.AddAutoMapper(typeof(SRP.API.Mapper.AutoMappingProfile));

////--------------------------------------------------------------------------------------------------------------------------------------
//// JWT Authentication
////--------------------------------------------------------------------------------------------------------------------------------------
//var jwtSettings = builder.Configuration.GetSection("JwtSettings");
//var secretKey = jwtSettings["Secret"];

//builder.Services.AddAuthentication(options =>
//{
//    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
//    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
//})
//.AddJwtBearer(options =>
//{
//    options.TokenValidationParameters = new TokenValidationParameters
//    {
//        ValidateIssuer = true,
//        ValidateAudience = true,
//        ValidateLifetime = true,
//        ValidateIssuerSigningKey = true,
//        ValidIssuer = jwtSettings["Issuer"],
//        ValidAudience = jwtSettings["Audience"],
//        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey!))
//    };
//});

//builder.Services.AddAuthorization();

//var app = builder.Build();

////--------------------------------------------------------------------------------------------------------------------------------------
//// Configure the HTTP request pipeline
////--------------------------------------------------------------------------------------------------------------------------------------
//if (app.Environment.IsDevelopment())
//{
//    app.UseSwagger();
//    app.UseSwagger();
//}

//app.UseHttpsRedirection();
//app.UseCors("AllowAll");
//app.UseAuthentication();
//app.UseAuthorization();
//app.MapControllers();

//app.Run();
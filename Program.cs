using InheritanceDemo.Mapping;
using Mapster;
using MapsterMapper;
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using InheritanceDemo.Data;
using Microsoft.AspNetCore.Builder;
using FluentValidation;
using SharpGrip.FluentValidation.AutoValidation.Mvc.Extensions;
using InheritanceDemo.Models;
using InheritanceDemo.DTOs;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<SkoleContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("SqlServer")));

builder.Services.AddCors(options =>
{
    options.AddPolicy("ReactClient", policy =>
    {
        policy.SetIsOriginAllowed((host) => true)
        .AllowAnyHeader()
        .AllowAnyMethod()
        .AllowCredentials();
    });
});

// Add services to the container.
TypeAdapterConfig typeadapterconfig = TypeAdapterConfig.GlobalSettings;
MapsterConfig.Register(typeadapterconfig);
typeadapterconfig.Scan(Assembly.GetExecutingAssembly());

builder.Services.AddSingleton(typeadapterconfig);
builder.Services.AddScoped<IMapper, ServiceMapper>();
// Afhængig af Mapster.DependencyInjection nuget pakken

// 1. Registrer validatorer (fra DependencyInjectionExtensions pakken)
builder.Services.AddValidatorsFromAssemblyContaining<AnsatCreateDtoValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<StudentCreateDtoValidator>();

// 2. Aktiver automatisk validering (fra SharpGrip pakken)
builder.Services.AddFluentValidationAutoValidation();

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment())
//{
//    app.MapOpenApi();
//}

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

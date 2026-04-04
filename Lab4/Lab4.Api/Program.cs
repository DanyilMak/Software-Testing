using FluentValidation.AspNetCore;
using Lab4.Api;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddFluentValidation(fv =>
{
    fv.RegisterValidatorsFromAssemblyContaining<CreateStudentRequestValidator>();
});

var app = builder.Build();

app.MapControllers();

app.Run();

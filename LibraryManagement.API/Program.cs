using DotNetEnv;
using Microsoft.EntityFrameworkCore;
using LibraryManagement.Infrastructure.Data;
using LibraryManagement.Infrastructure.Repositories;
using LibraryManagement.Application.Services;
using LibraryManagement.Application.Interfaces;





Env.Load();

var builder = WebApplication.CreateBuilder(args);

var connectionString = Environment.GetEnvironmentVariable("DB_CONNECTION");

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(connectionString));

builder.Services.AddControllers();

builder.Services.AddCors();  

builder.Services.AddOpenApi();

builder.Services.AddScoped<IUserRepository, UserRepository>();

builder.Services.AddScoped<UserService>();

var app = builder.Build();

app.UseCors(builder=>builder.WithOrigins("http://localhost:4200").AllowAnyMethod().AllowAnyHeader());

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.MapControllers();  

app.Run();
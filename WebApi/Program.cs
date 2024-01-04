using Application.Commands;
using Application.Queries;
using Infrastructure.Persistence;
using Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
ConfigurationManager configurationManager = builder.Configuration;
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

string connectionString = "Data Source=localhost;Initial Catalog=ProjectSetup;Integrated Security=True;MultipleActiveResultSets=True;TrustServerCertificate=True;";

var type = typeof(Infrastructure.Persistence.Migrations.Module);
var assembly = type.Assembly;
var assemblyName = assembly.FullName;

builder.Services.AddDbContext<DatabaseContext>(options =>
{
    options.UseSqlServer(connectionString, e =>
    {
        e.MigrationsAssembly(assemblyName);
        e.CommandTimeout(360);
        e.EnableRetryOnFailure(5, TimeSpan.FromSeconds(3), null);
    });
});
builder.Services.AddRepositories();
    
builder.Services.AddApplicationQueryServices();
builder.Services.AddApplicationCommandServices();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
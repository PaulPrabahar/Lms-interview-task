using BuildingBlocks.Behaviours;
using BuildingBlocks.Exceptions;
using Carter;
using FluentValidation;
using LMSInterviewTask.Api.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
var assembly = typeof(Program).Assembly;
//application registration
builder.Services.AddMediatR(config =>
{
    config.RegisterServicesFromAssemblies(assembly);
    config.AddOpenBehavior(typeof(ValidationBehaviour<,>));
    config.AddOpenBehavior(typeof(LoggingBehaviour<,>));
});
builder.Services.AddValidatorsFromAssembly(assembly);
builder.Services.AddCarter();
builder.Services.AddDbContext<LmsContext>(opts => 
opts.UseSqlite(builder.Configuration.GetConnectionString("Database")));
builder.Services.AddExceptionHandler<CustomExceptionHandler>();
var app = builder.Build();

//app.UseMigration();

app.UseExceptionHandler(options => { });
app.UseDefaultFiles();   // finds index.html automatically
app.UseStaticFiles();    // allows serving .html, .js, .css
app.MapCarter();

app.Run();

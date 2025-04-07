using Microsoft.EntityFrameworkCore;
using WorkayCalendarApi.Services.Interfaces;
using WorkayCalendarApi.Services.Implementations;
using WorkayCalendarApi.Repositories.Interfaces;
using WorkayCalendarApi.Repositories.Implementations;
using WorkdayCalendarApi.Entities;
using WorkdayCalendarApi.Profiles;

var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;

// Add Entity Framework Core
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DbConnectionString")));
// Add CORS policy
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngularDevClient", policy =>
    {
        policy.WithOrigins("http://localhost:4200")
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials(); // if needed
    });
});


ConfigureServices(services);
builder.Services.AddControllers();
builder.Services.AddAutoMapper(typeof(MappingProfile).Assembly);


builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//To do Authentication ,May be use Auth0 m2m/user token,Check how long it is free
// Add CORS services

var app = builder.Build();
app.UseCors("AllowAngularDevClient");
// Auto-apply migrations at startup
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    dbContext.Database.EnsureCreated();  // Ensures the database is created
    //dbContext.Database.Migrate();  // Automatically applies pending migrations
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();

static void ConfigureServices(IServiceCollection services)
{
    // services
    services.AddScoped<IWorkdayService, WorkdayService>();
    services.AddScoped<IHolidayService, HolidayService>();
    // repos
    services.AddScoped<IHolidayRepository, HolidayRepository>();
}

using SmartTaskerMini.Core.Application;
using SmartTaskerMini.Core.Domain;
using SmartTaskerMini.Core.Infrastructure;
using SmartTaskerMini.WebApp.Middleware;

var builder = WebApplication.CreateBuilder(args);

// Initialize configuration
SmartTaskerMini.Core.Application.ConfigurationManager.SetStorageType("SQL");

// Add services
builder.Services.AddControllersWithViews();
builder.Services.AddScoped<ITaskRepository>(provider => 
    RepositoryFactory.CreateTaskRepository());
builder.Services.AddScoped<TaskService>();
builder.Services.AddScoped<ReportService>();

// Configure logging
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();

var app = builder.Build();

// Configure middleware pipeline
app.UseMiddleware<SecurityHeadersMiddleware>();
app.UseMiddleware<PerformanceMiddleware>();
app.UseMiddleware<RequestLoggingMiddleware>();
app.UseMiddleware<ErrorHandlingMiddleware>();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}
else
{
    app.UseDeveloperExceptionPage();
}

app.UseStaticFiles();
app.UseRouting();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Tasks}/{action=Index}/{id?}");

app.Run();
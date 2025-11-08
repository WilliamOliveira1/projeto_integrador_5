using Microsoft.EntityFrameworkCore;
using PIV.ClientApp.src.services.Prevision;
using PIV.Data;
using PIV.interfaces;
using PIV.Models;
using PIV.Services;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddScoped<IRainfallDataFromCsv, RainfallDataFromCsv>();
builder.Services.AddScoped<ISaveDataFromCsvService, SaveDataFromCsvService>();
builder.Services.AddScoped<ISaveDataFromSensor, SaveDataFromSensor>();
builder.Services.AddScoped<IPrevisionService, PrevisionService>();
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseMySql(connectionString, new MySqlServerVersion(new Version(6, 0, 2))));
builder.Services.AddHttpClient();
builder.Services.AddControllersWithViews();

builder.Services.Configure<OpenAIOptions>(builder.Configuration.GetSection("OpenAI"));
builder.Services.Configure<Firebase>(builder.Configuration.GetSection("Firebase"));

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseMySql(
        connectionString,
        ServerVersion.AutoDetect(connectionString),
        mySqlOptions => mySqlOptions.EnableRetryOnFailure(
            maxRetryCount: 5,
            maxRetryDelay: TimeSpan.FromSeconds(10),
            errorNumbersToAdd: null
        )
    )
);

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();


app.MapControllerRoute(
    name: "default",
    pattern: "{controller}/{action=Index}/{id?}");

app.MapFallbackToFile("index.html"); ;

app.Run();

using TeamLongestPeriod.Models;
using TeamLongestPeriod.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorPages();
builder.Services.AddDbContext<TeamLongestPeriodDbContext>();
builder.Services.AddScoped<ITeamLongestPeriodService, TeamLongestPeriodService>();

var app = builder.Build();

if(!app.Environment.IsDevelopment())
{
	app.UseExceptionHandler("/Error");
}

app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();
app.MapRazorPages();

app.Run();

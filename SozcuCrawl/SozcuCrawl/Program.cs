using SozcuCrawl.Extensions;
using SozcuCrawl.Service;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();

builder.Services.ConfigureElasticService(builder.Configuration);
builder.Services.ConfigureElasticsearchService();
builder.Services.ConfigureScrapperService();
builder.Services.ConfigureDataCoordinatorService();
builder.Services.AddMemoryCache();
builder.Services.ConfigureDataRefresherService();

builder.Services.AddHttpClient();
var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var dataCoordinator = scope.ServiceProvider.GetRequiredService<DataCoordinator>();
    await dataCoordinator.InitializeDataAsync();
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
	app.UseExceptionHandler("/Error");
	// The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
	app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapRazorPages();

app.Run();

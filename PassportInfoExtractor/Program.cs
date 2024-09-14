using Microsoft.Extensions.Configuration;
using PassportInfoExtractor.Components;
using PassportInfoExtractor.Data;
using PassportInfoExtractor.Data.DataAccess;
using PassportInfoExtractor.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// Configure database connection
builder.Services.AddSingleton<IDbConnection, DbConnection>();

// Register data access classes
builder.Services.AddSingleton<IPassportData, PassportData>();
builder.Services.AddSingleton<IPassportGroupData, PassportGroupData>();

// Register services
builder.Services.AddScoped<IDatabaseService, DatabaseService>();
builder.Services.AddScoped<IDocumentIntelligenceService, DocumentIntelligenceService>();
builder.Services.AddSingleton<AzureBlobService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();


app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();

using ProjetoEM.DataBase;
using ProjetoEM.Interfaces;
using ProjetoEM.Repositories;
using ProjetoEM.Services;
using System.Globalization;
using Microsoft.AspNetCore.Localization;

var builder = WebApplication.CreateBuilder(args);

var supportedCultures = new[] { new CultureInfo("pt-BR") };
builder.Services.Configure<RequestLocalizationOptions>(options =>
{
    options.DefaultRequestCulture = new RequestCulture("pt-BR");
    options.SupportedCultures = supportedCultures;
    options.SupportedUICultures = supportedCultures;
});

// Add services to the container.
builder.Services.AddControllersWithViews()
    .AddDataAnnotationsLocalization();


string connectionString = builder.Configuration.GetConnectionString("FirebirdConnection");
builder.Services.AddSingleton(new FireBirdConnection(connectionString));

// Repositories
builder.Services.AddScoped<IAlunoRepository, AlunoRepository>();
builder.Services.AddScoped<ICidadeRepository, CidadeRepository>();

// Services
builder.Services.AddScoped<RelatorioService>();

var app = builder.Build();

app.UseRequestLocalization();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Aluno}/{action=Index}/{id?}");

app.Run();

using EM.Repository.Banco;
using EM.Domain.Interface;
using EM.Repository;
using EM.Web.Services;
using System.Globalization;
using Microsoft.AspNetCore.Localization;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

CultureInfo[] supportedCultures = new[] { new CultureInfo("pt-BR") };
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
builder.Services.AddScoped<IAlunoRepository, RepositorioAluno>();
builder.Services.AddScoped<ICidadeRepository, RepositorioCidade>();

// Services
builder.Services.AddScoped<RelatorioService>();

WebApplication app = builder.Build();

app.UseRequestLocalization();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Aluno/Index");
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

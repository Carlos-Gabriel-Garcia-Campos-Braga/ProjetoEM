using EM.Domain.Interface;
using EM.MontadorRelatorio.Interface;
using EM.MontadorRelatorio.Services;
using EM.Repository;
using EM.Repository.Banco;
using Microsoft.AspNetCore.Localization;
using System.Globalization;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

CultureInfo[] supportedCultures = [new CultureInfo("pt-BR")];
builder.Services.Configure<RequestLocalizationOptions>(options =>
{
    options.DefaultRequestCulture = new RequestCulture("pt-BR");
    options.SupportedCultures = supportedCultures;
    options.SupportedUICultures = supportedCultures;
});

builder.Services.AddControllersWithViews()
    .AddDataAnnotationsLocalization();

string? connectionString = builder.Configuration.GetConnectionString("FirebirdConnection");
if (string.IsNullOrWhiteSpace(connectionString))
{
    throw new InvalidOperationException("Conex�o com banco de dados n�o encontrada.");
}
builder.Services.AddSingleton(new FireBirdConnection(connectionString));

builder.Services.AddScoped<IAlunoRepository, RepositorioAluno>();
builder.Services.AddScoped<ICidadeRepository, RepositorioCidade>();

builder.Services.AddScoped<IRelatorioService, RelatorioAlunoService>();

WebApplication app = builder.Build();

app.UseRequestLocalization();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Aluno/Index");
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

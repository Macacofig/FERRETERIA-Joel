using FERRETERIA__Joel.Factories;
using FERRETERIA__Joel.Helpers;
using FERRETERIA__Joel.Models;
using FERRETERIA__Joel.Repositories;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorPages();

UrlProtector.Inicializar(
    builder.Configuration["UrlProteccion:Clave"]
        ?? throw new InvalidOperationException(
            "Falta la clave 'UrlProteccion:Clave' en la configuración."));


builder.Services.AddSingleton<IDbConnectionFactory, MySqlConnectionFactory>();


builder.Services.AddScoped<RepositoryCreator<Producto>, ProductoRepositoryCreator>();
builder.Services.AddScoped<RepositoryCreator<Categoria>, CategoriaRepositoryCreator>();
builder.Services.AddScoped<RepositoryCreator<Proveedor>, ProveedorRepositoryCreator>();
builder.Services.AddScoped<RepositoryCreator<Empleado>, EmpleadoRepositoryCreator>();
builder.Services.AddScoped<RepositoryCreator<HistoricoPrecio>, HistoricoPrecioRepositoryCreator>();

var app = builder.Build();


if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();
app.MapRazorPages()
   .WithStaticAssets();

app.Run();
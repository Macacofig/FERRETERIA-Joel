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


builder.Services.AddScoped<RepositoryCreator<IRepository<Producto>>, ProductoRepositoryCreator>();
builder.Services.AddScoped<RepositoryCreator<IRepository<Categoria>>, CategoriaRepositoryCreator>();
builder.Services.AddScoped<RepositoryCreator<IRepository<Proveedor>>, ProveedorRepositoryCreator>();
builder.Services.AddScoped<RepositoryCreator<IRepository<Empleado>>, EmpleadoRepositoryCreator>();
builder.Services.AddScoped<RepositoryCreator<IRepository<HistoricoPrecio>>, HistoricoPrecioRepositoryCreator>();
builder.Services.AddScoped<IModificacionRepository<Producto>, MySqlProductoRepository>();
builder.Services.AddScoped<IModificacionRepository<Categoria>, MySqlCategoriaRepository>();
builder.Services.AddScoped<IModificacionRepository<Proveedor>, MySqlProveedorRepository>();
builder.Services.AddScoped<IModificacionRepository<Empleado>, MySqlEmpleadoRepository>();
builder.Services.AddScoped<MySqlHistoricoPrecioRepository>();

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
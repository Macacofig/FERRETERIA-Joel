using FERRETERIA__Joel.Factories;
using FERRETERIA__Joel.Helpers;
using FERRETERIA__Joel.Models;
using FERRETERIA__Joel.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();

UrlProtector.Inicializar(
    builder.Configuration["UrlProteccion:Clave"]
        ?? throw new InvalidOperationException(
            "Falta la clave 'UrlProteccion:Clave' en la configuración."));

// Patrones de diseño (SOLID):
// - IDbConnectionFactory: Factory Method para crear conexiones MySQL.
// - RepositoryCreator<T>: Factory Method para crear los repositorios (Concrete Products).
// - Singleton: una única instancia del factory de conexiones, administrada por DI.
builder.Services.AddSingleton<IDbConnectionFactory, MySqlConnectionFactory>();

// Concrete Creators del Factory Method de repositorios.
builder.Services.AddScoped<RepositoryCreator<Producto>, ProductoRepositoryCreator>();
builder.Services.AddScoped<RepositoryCreator<Categoria>, CategoriaRepositoryCreator>();
builder.Services.AddScoped<RepositoryCreator<Proveedor>, ProveedorRepositoryCreator>();
builder.Services.AddScoped<RepositoryCreator<Empleado>, EmpleadoRepositoryCreator>();
builder.Services.AddScoped<RepositoryCreator<HistoricoPrecio>, HistoricoPrecioRepositoryCreator>();

// Funcionalidades específicas de cada entidad, separadas del CRUD genérico (ICRUD<T>).
builder.Services.AddScoped<IProductoRepositoryFunctions, MySqlProductoRepository>();
builder.Services.AddScoped<ICategoriaRepositoryFunctions, MySqlCategoriaRepository>();
builder.Services.AddScoped<IProveedorRepositoryFunctions, MySqlProveedorRepository>();
builder.Services.AddScoped<IHistoricoPrecioRepositoryFunctions, MySqlHistoricoPrecioRepository>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();
app.MapRazorPages()
   .WithStaticAssets();

app.Run();
using FERRETERIA__Joel.Factories;
using FERRETERIA__Joel.Models;
using FERRETERIA__Joel.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();

// Patrones de diseño (SOLID):
// - IDbConnectionFactory: Factory Method para crear conexiones MySQL.
// - RepositoryCreator<T>: Factory Method para crear los repositorios (Concrete Products).
// - Singleton: una única instancia del factory de conexiones, administrada por DI.
builder.Services.AddSingleton<IDbConnectionFactory, MySqlConnectionFactory>();

// Concrete Creators del Factory Method de repositorios.
// El Producto de cada uno ya combina ICRUD<T> con las funciones especiales
// de esa entidad, así que no hace falta registrar las funciones aparte.
builder.Services.AddScoped<RepositoryCreator<IProductoRepository>, ProductoRepositoryCreator>();
builder.Services.AddScoped<RepositoryCreator<ICategoriaRepository>, CategoriaRepositoryCreator>();
builder.Services.AddScoped<RepositoryCreator<IProveedorRepository>, ProveedorRepositoryCreator>();
builder.Services.AddScoped<RepositoryCreator<ICRUD<Empleado>>, EmpleadoRepositoryCreator>();
builder.Services.AddScoped<RepositoryCreator<IHistoricoPrecioRepository>, HistoricoPrecioRepositoryCreator>();

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
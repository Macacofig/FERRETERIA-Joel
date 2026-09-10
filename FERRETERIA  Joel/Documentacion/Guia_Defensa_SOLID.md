# Guía de Defensa — Ferretería Joel (SOLID, Clean Code, Arquitectura, DI)

Guía de estudio para responder preguntas del panel. Cada respuesta tiene la
fórmula: **concepto → ejemplo en el proyecto → ubicación exacta**.

---

## 1. ¿Qué arquitectura seguimos y por qué separamos los archivos así?

**Respuesta corta:**
Seguimos una **arquitectura en capas por funcionalidad (folder-by-feature)** con
separación de responsabilidades, sobre **ASP.NET Core Razor Pages**. Dividimos el
proyecto en carpetas, cada una con una responsabilidad:

| Carpeta | Responsabilidad |
|---------|-----------------|
| `Models/` | Clases que representan las tablas (Categoria, Producto, Proveedor, Empleado, HistoricoPrecio) |
| `Repositories/` | Acceso a datos: `Interfaz*` (contrato) + `MySql*` (implementación con SQL) |
| `Validaciones/` | Reglas de negocio (cada clase valida una entidad) |
| `Pages/` | Página Razor = archivo `.cshtml` (vista) + `.cshtml.cs` (Page Model / controlador) |
| `wwwroot/` | Archivos estáticos (CSS, JS, imágenes) |
| `Program.cs` | Registro de servicios e **inyección de dependencias** (composición) |

**¿Por qué?**
- Cada capa tiene **una sola razón para cambiar** (responsabilidad única).
- La página ya **no sabe ni le importa** cómo se guardan los datos (MySQL).
- Si mañana cambiamos de base de datos, solo tocamos `Repositories/MySql*` y el
  registro en `Program.cs`. Las vistas no cambian.
- Es más fácil **mantener** y **probar** (puedo probar las páginas con
  repositorios falsos).

---

## 2. ¿Qué es una interfaz? ¿Por qué las usamos?

**Respuesta corta:**
Una **interfaz** es un **contrato**: declara *qué* debe hacer una clase, pero no
* cómo*. Es como "la lista de compromisos" de una clase.

Ejemplo real — `Repositories/IProductoRepository.cs:3-18`:

```csharp
public interface IProductoRepository
{
    List<Models.Producto> ObtenerTodos();
    Models.Producto? ObtenerPorId(int idProducto);
    int Insertar(Models.Producto producto);
    void Actualizar(Models.Producto producto);
    void CambiarEstado(int idProducto);
    bool ExisteCodigo(string codigo, int? idProductoExcluir = null);
    bool ExisteCategoriaActiva(short idCategoria);
}
```

La interfaz **solo dice** "todo el que me implemente debe tener `ObtenerTodos()`,
`Insertar()`...". La implementación concreta (`Repositories/MySqlProductoRepository.cs`)
es la que escribe el color SQL con ADO .NET.

**¿Por qué las usamos?**
- Permiten depender de **abstracciones** y no de clases concretas (**DIP**).
- Cuando el Page Model inyecta `IProductoRepository`, la página no necesita
  conocer MySQL; cualquier clase que cumpla el contrato la sustituye (**LSP**).
- Facilita pruebas: puedo crear un `ProductoRepositoryFalso` (en memoria) para
  hacer pruebas unitarias sin base de datos.

---

## 3. ¿Qué es inyección de dependencias (DI) y dónde la hacemos?

**Respuesta corta:**
DI es entregarle a una clase **las cosas que necesita** (sus dependencias) en
lugar de que ella misma las cree con `new`. En nuestro proyecto:

**Paso 1 — Registro (composición):** en `Program.cs:8-13` registramos cada
interfaz con su implementación:

```csharp
builder.Services.AddScoped<ICategoriaRepository, MySqlCategoriaRepository>();
builder.Services.AddScoped<IProductoRepository, MySqlProductoRepository>();
builder.Services.AddScoped<IProveedorRepository, MySqlProveedorRepository>();
builder.Services.AddScoped<IEmpleadoRepository, MySqlEmpleadoRepository>();
builder.Services.AddScoped<IHistoricoPrecioRepository, MySqlHistoricoPrecioRepository>();
```

- `AddScoped<T>` = se crea una instancia por request HTTP (ciclo de vida).
- `Program.cs` es el **Composition Root**: único lugar donde se conocen
  implementaciones concretas.

**Paso 2 — Consumo (constructor):** el Page Model recibe las dependencias por
constructor y el contenedor se las inyecta. `Pages/ProductoNuevo.cshtml.cs:30-42`:

```csharp
public ProductoNuevoModel(
    IProductoRepository productoRepository,
    ICategoriaRepository categoriaRepository,
    IEmpleadoRepository empleadoRepository,
    IHistoricoPrecioRepository historicoPrecioRepository,
    ILogger<ProductoNuevoModel> logger)
{
    _productoRepository = productoRepository;
    _categoriaRepository = categoriaRepository;
    _empleadoRepository = empleadoRepository;
    _historicoPrecioRepository = historicoPrecioRepository;
    _logger = logger;
}
```

**¿Por qué no `new MySqlProductoRepository()` dentro de la página?**
- La página dependería de la clase concreta (violaría DIP).
- Difícil de probar (siempre pegaría a MySQL).
- Acoplada: cambiar BD = cambiar la página.
Por eso el comentario en `Program.cs:8` literalmente dice
*"Inyección de dependencias para Categoría (SOLID - Inversión de dependencias)"*.

---

## 4. Aplicación de cada principio SOLID (con ejemplos exactos)

### S — Single Responsibility (Responsabilidad Única)
*"Una clase debe tener una sola razón para cambiar."*

**¿Dónde?** Separamos responsabilidades:
- **Persistir** → `Repositories/` (ej. `Repositories/MySqlProductoRepository.cs:6`)
- **Validar** → `Validaciones/ProductoValidaciones.cs:3` (métodos como
  `EsPrecioValido`, `EsCodigoValido`, `EsUnidadMedidaValida`)
- **Orquestar HTTP** → Page Model. `Pages/ProductoNuevo.cshtml.cs:49-58`:
  `OnPost()` solo llama `NormalizarDatos()` → `Validar()` → si hay errores
  vuelve a la página → si no, guarda y redirige. No hace SQL.
- **Mapear fila→modelo** → método privado `MapearProducto`
  (`Repositories/MySqlProductoRepository.cs:325-371`). Un solo lugar.

**Clave para defender:** *"Cada clase tiene un solo motivo por el cual podría
ser modificada: la validación cambia en Validaciones, la consulta en el
Repository, la lógica HTTP en el Page Model."*

### O — Open/Closed (Abierto/Cerrado)
*"Abierto a la extensión, cerrado a la modificación."*

**Respuesta:** Todas las páginas dependen de interfaces
(`IProductoRepository`, `ICategoriaRepository`…). Si quiero:

- Usar otra base de datos → creo `SqlServerProductoRepository` e **únicamente**
  cambio el registro en `Program.cs:9-13`. No modifico ninguna página.
- Hacer pruebas rápidas → creo `ProductoRepositoryEnMemoria`.

Las páginas **no se tocan** para agregar funcionalidad nueva: están cerradas a
la modificación y el sistema abierto a extensión.

### L — Sustitución de Liskov (LSP)
*"Cualquier implementación debe poder sustituir a la interfaz sin romper el
programa."*

**Respuesta:** `Repositories/MySqlProductoRepository.cs` implementa completo el
contrato de `IProductoRepository.cs:3-18`. Cualquier otra clase que implemente la
misma interfaz respetando las mismas reglas funciona en `Pages/Productos.cshtml.cs`
que consume `ObtenerTodos()`. El programa espera **el contrato**, no la clase.

### I — Segregación de Interfaces (ISP)
*"Interfaces pequeñas y específicas; nadie usa métodos que no necesita."*

**Respuesta:** No existen interfaces "gordas". Ejemplo perfecto —
`Repositories/IEmpleadoRepository.cs` tiene **un solo método** (`ObtenerActivos()`),
y `Repositories/IHistoricoPrecioRepository.cs:5-11` tiene solo 4 métodos:

```csharp
List<HistoricoPrecio> ObtenerPorProducto(int idProducto);
void Insertar(HistoricoPrecio historicoPrecio);
HistoricoPrecio? ObtenerPrecioVigente(int idProducto);
void CerrarPrecioVigente(int idProducto);
```

Un Page Model que solo necesita el catálogo de empleados no recibe una interfaz
con métodos de pagos o reportes que no usa.

### D — Inversión de Dependencias (DIP)
*"Depender de abstracciones, no de clases concretas."*

**Respuesta:** Los módulos de alto nivel (páginas) dependen de las **interfaces**
y no de las clases concretas MySQL:
- Registro: `Program.cs:9-13`
- La página **jamás** instancia `new MySqlProductoRepository()`. Recibe
  `IProductoRepository` por constructor (`Pages/ProductoNuevo.cshtml.cs:30-42`).

---

## 5. ¿Qué es Clean Code y dónde se aplica?

Clean Code = código **legible, sin duplicación y con intención clara**.
Ejemplos en nuestro proyecto:

| Práctica | Ejemplo (ubicación) |
|----------|---------------------|
| Nombres con intención | `CerrarPrecioVigente()`, `ExisteNit()`, `MapearProducto()` |
| Métodos pequeños / una tarea | `NormalizarDatos()`, `Validar()`, `CargarCatalogos()` en `Pages/ProductoNuevo.cshtml.cs` |
| Sin duplicación (DRY) | `MapearProducto` centraliza el mapeo (`MySqlProductoRepository.cs:325-371`) |
| Lectura tipada del reader | `GetInt32`, `GetDecimal`, `GetDateTime` (sin `ToString()` adivino) |
| Manejo seguro de nulos | `reader["Descripcion"] == DBNull.Value ? null : ...` (`MySqlProductoRepository.cs:341-344`) |
| Seguridad de recursos | `using MySqlConnection`, `using MySqlCommand`, `using MySqlDataReader` |
| Errores manejados | `try/catch` + `ILogger.LogError` + mensaje genérico al usuario |
| Constantes SQL | `const string query = "..."` |

---

## 6. Historia de datos / Histórico de precios (esssss: ejemplo pedido)

**¿Cómo funciona el histórico?** ⬅ Pregunta casi segura.

1. **Al crear un producto**: se inserta el producto y se registra su **primera
   versión** con motivo *"Precio inicial"* (`Pages/ProductoNuevo.cshtml.cs`,
   método `Insertar()`).
2. **Al actualizar** (`Pages/ProductoEditar.cshtml.cs`, método `Actualizar()`):
   se obtiene el precio vigente; si el nuevo precio **cambió**, se cierra el
   vigente (`CerrarPrecioVigente`) y se inserta el nuevo con motivo
   *"Cambio de precio"*.
3. **Siempre hay un solo precio vigente**: el registro con
   `FechaFinVigencia = NULL`.
4. **Vista del histórico**: `Pages/ProductoHistoricoPrecio.cshtml` (muestra
   lista: Precio, Inicio/Fin de vigencia, Motivo, Empleado responsable).

**Secretos SQL** destacables que conviene saber mencionar:
- `HistoricoPrecio.Precio` se almacena con `DECIMAL(10,2)`.
- `CerrarPrecioVigente` hace: `UPDATE ... SET FechaFinVigencia = NOW() WHERE
  IdProducto = @id AND FechaFinVigencia IS NULL`.

---

## 7. Otras preguntas clásicas y sus respuestas

### ¿Por qué eliminación lógica y no física (DELETE)?
Porque una ferretería necesita **auditoría**: si elimino un proveedor con
`DELETE`, el NIT quedaría libre para ser reutilizado y perderíamos el historial.
Por eso `Producto`/`Proveedor` tienen `CambiarEstado()` que alterna
`Estado=1`(activo) / `Estado=0`(inactivo) con un `CASE`. Además la llave foránea
de Categoría↔Producto se protege.

### ¿Cómo validas los datos antes de insertar?
Dos capas:
1. **Lógica de negocio** en `Validaciones/*` (`EsPrecioValido` = precio > 0,
   `EsCodigoValido` = no vacío y ≤ 30 caracteres, etc.).
2. **Integridad de la BD**: aunque se intente ingresar un código/NIT repetido,
   la **constraint única** de MySQL lanza el error 1062 y lo capturamos en el
   `catch` para mostrar un mensaje claro al usuario.

### ¿Dónde está la cadena de conexión? (seguridad)
En `appsettings.json` (y `appsettings.Development.json`), leída mediante
`builder.Configuration.GetConnectionString("MySqlConnection")`, nunca escrita en
el código. Nuestros repositorios lanzan lo siguiente si falta (code defensivo):
`InvalidOperationException("No se encontró la cadena de conexión MySqlConnection.")`.

### ¿Qué hace GetConnectionString y de dónde sale `IConfiguration`?
`IConfiguration` es inyectado por el contenedor de DI; internamente lee los
archivos `appsettings*.json` y registros de configuración.

### ¿Qué es `MySqlDataReader`?
Un lector **solo hacia adelante** (forward-only) que devuelve la fila actual de
la consulta; con `MapearProducto(reader)` transformamos cada fila a un `Producto`.

### ¿Para qué sirve `ILogger`?
Para **registrar errores técnicos** con contexto (id del registro, excepción)
sin exponerlos al usuario. Es la forma recomendada por ASP.NET Core: se inyecta,
no se crea.

### ¿Qué es un Page Model?
`Pages/*.cshtml.cs` = la "capa controladora" de cada página Razor. Maneja
`OnGet()` (mostrar) y `OnPost()` (enviar formulario), usa `[BindProperty]` para
enlazar los campos del formulario y se inyectan sus dependencias.

### ¿Cómo se ve el flujo de una petición (request → DB)?
`HTTP GET/POST` → Razor Page `.cshtml` (vista) → `.cshtml.cs` (Page Model) →
`I*Repository` (interfaz) → `MySql*Repository` (SQL con ADO .NET) → MySQL → devuelve modelos → se re-renderiza `.cshtml`.

### ¿Por qué usar `short`/`int` en las claves y `decimal` en precios?
Para no dejar que el dinero se maneje con `float/double` (podrían acumular
errores de redondeo). `PrecioVenta` y `PorcentajeGanancia` son `decimal`, y los
`Id*` son tipos enteros que reflejan su tamaño real en la tabla.

### ¿Por qué usamos Razor Pages y no MVC clásico?
Porque es la evolución oficial de Microsoft para páginas simples con señal:
Page Model integrado, sin tener que declarar controladores/views explícitos.
Elegimos lo que mejor encaja para este tipo de sistema administrativo.

### ¿Qué pasaría si quitas el `using`? (recursos)
Los objetos conectados (`MySqlConnection`, `MySqlCommand`, `MySqlDataReader`)
quedan abiertos hasta que el GC los cierre, problema de **fuga de conexiones**:
a la larga se agota el pool de MySQL.

### ¿Cómo ordenamos el repositorio Git?
Ramas temáticas (`validaciones-categoria`, `datos-historicos`, `pagehome`,
`Crud-de-Productos`, `crud-categoria`, `CRUD-PROVEEDORES`) + pull requests a
`master`, commits con prefijos (`feat:`, `fix:`, `refactor:`) — ver sección 4 de
`Informe_SOLID_FerreteriaJoel.md`.

### ¿Esto es "n" capas o "clean architecture"?
Es una **arquitectura en capas orientada a Razor Pages**: Presentación (Pages) →
Lógica (Page Model + Validaciones) → Acceso a datos (Repositories) → Base de
datos. Usamos los **principios** de Clean Architecture (DIP, límites de
dependencia) dentro de una estructura simple, no una implementación formal de
Clean/Hexagonal.

---

## 8. Los 3 diagramas del informe (para tener mentalmente)

1. **Modelos / Repos / Validaciones**: Cómo `I*Repository` ← implementa `MySql*Repository`,
   y Page Models dependen de las interfaces.
2. **Páginas (Page Models)**: Cada página apunta a sus repositorios; `ProductoNuevo`
   y `ProductoEditar` son las dos que tocan `IHistoricoPrecioRepository`.
3. **Histórico**: `Producto 1 → 0..* HistoricoPrecio`, con `ObtenerPrecioVigente`
   y `CerrarPrecioVigente`.

**Tip para la defensa:** si te piden "¿dónde se aplica X?" nombra en este orden:
**concepto (1 frase) → archivo exacto (`Pages/ProductoNuevo.cshtml.cs:30-42`) →
frase que cierra**: *"por eso dependemos de IU, no de MySQL, y por eso lo
podemos probar fácilmente."*
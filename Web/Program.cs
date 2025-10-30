// using Business.AutoMapper;
// using CloudinaryDotNet;
// using Data.SeedData.SeederHelpers;
// using System.Reflection;
// using Web.Extensions;


// var builder = WebApplication.CreateBuilder(args);

// // =============== [ SERVICES ] ===============

// // Middleware
// builder.Services.AddHttpContextAccessor(); 

// // DbContext
// builder.Services.AddPersistence(builder.Configuration); // Extension

// // DataINIT
// builder.Services.AddDataSeeders(); // Extension

// // Controllers
// builder.Services.AddControllers();

// // AutoMapper
// builder.Services.AddAutoMapper(typeof(GeneralMapper));

// // Swagger 
// builder.Services.AddSwaggerDocumentation();
// builder.Services.AddSwaggerWithJwtSupport(); // Extension

// builder.Services.AddMemoryCache();

// // JWT 
// builder.Services.AddJwtAuthentication(builder.Configuration); // Extension

// // CORS 
// builder.Services.AddCustomCors(builder.Configuration); // Extension

// // Entities
// builder.Services.AddEntitiesServices(); //Extension

// // BUSINESS + SERVICES EXTRAS
// builder.Services.AddBusinessServices(builder.Configuration); // Extension


// // =============== [ Cloudinary Config ] ===============
// var account = new Account(
//     builder.Configuration["Cloudinary:CloudName"],
//     builder.Configuration["Cloudinary:ApiKey"],
//     builder.Configuration["Cloudinary:ApiSecret"]
// );

// var cloudinary = new Cloudinary(account);
// builder.Services.AddSingleton(cloudinary);

// // =============== [ Build App ] ===============
// var app = builder.Build();

// // Swagger en desarrollo
// if (app.Environment.IsDevelopment())
// {
//     app.UseSwagger();
//     app.UseSwaggerUI(c =>
//     {
//         c.SwaggerEndpoint("/swagger/v1/swagger.json", "Mi API v1");
//         c.DocExpansion(Swashbuckle.AspNetCore.SwaggerUI.DocExpansion.None);
//         // None -> todo colapsado
//         // List -> lista de endpoints visible, pero sin detalles
//         // Full -> todo expandido
//     });

//     using var scope = app.Services.CreateScope();
//     var config = scope.ServiceProvider.GetRequiredService<IConfiguration>();
//     await SeederExecutor.SeedAllAsync(scope.ServiceProvider, config);
// }

// if (app.Environment.IsProduction())
// {
//     app.UseSwagger();
//     app.UseSwaggerUI();
// }

// app.UseHttpsRedirection();
// app.UseCors();
// app.UseAuthentication();
// app.UseAuthorization();
// app.MapControllers();
// app.Run();

using Business.AutoMapper;
using CloudinaryDotNet;
using Data.SeedData.SeederHelpers;
using Web.Extensions;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

// ===================================================
// 🧱 [1] CONFIGURACIÓN DE SERVICIOS
// ===================================================

// Middleware básico
builder.Services.AddHttpContextAccessor();

// DbContext + Repositorios
builder.Services.AddPersistence(builder.Configuration);

// Inicialización de datos (Seeders)
builder.Services.AddDataSeeders();

// Controladores
builder.Services.AddControllers();

// AutoMapper
builder.Services.AddAutoMapper(typeof(GeneralMapper));

// Swagger
builder.Services.AddSwaggerDocumentation();
builder.Services.AddSwaggerWithJwtSupport();

// Cache y JWT
builder.Services.AddMemoryCache();
builder.Services.AddJwtAuthentication(builder.Configuration);

// CORS (usará OrígenesPermitidos desde appsettings.json o AllowAll si falta)
builder.Services.AddCustomCors(builder.Configuration);

// Registrar servicios de entidades y capa de negocio
builder.Services.AddEntitiesServices();
builder.Services.AddBusinessServices(builder.Configuration);

// ===================================================
// ☁️ [2] CONFIGURACIÓN DE CLOUDINARY
// ===================================================
var account = new Account(
    builder.Configuration["Cloudinary:CloudName"],
    builder.Configuration["Cloudinary:ApiKey"],
    builder.Configuration["Cloudinary:ApiSecret"]
);
builder.Services.AddSingleton(new Cloudinary(account));

// ===================================================
// 🚀 [3] CONSTRUIR APP
// ===================================================
var app = builder.Build();

// ===================================================
// 🧩 [4] SWAGGER — Habilitado para todos los entornos
// ===================================================
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Codexy API v1");
    c.RoutePrefix = "swagger"; // URL → /swagger
    c.DocExpansion(Swashbuckle.AspNetCore.SwaggerUI.DocExpansion.None);
});

// ===================================================
// 🧪 [5] SEEDERS (solo en Development o Staging si lo deseas)
// ===================================================
if (app.Environment.IsDevelopment() || app.Environment.EnvironmentName == "qa")
{
    using var scope = app.Services.CreateScope();
    var config = scope.ServiceProvider.GetRequiredService<IConfiguration>();
    await SeederExecutor.SeedAllAsync(scope.ServiceProvider, config);
}

// ===================================================
// ⚙️ [6] PIPELINE DE MIDDLEWARES
// ===================================================

// HTTPS solo en producción (para evitar errores en Docker)
if (app.Environment.IsProduction())
{
    app.UseHttpsRedirection();
}

app.UseCors();               // Configurado en AddCustomCors()
app.UseAuthentication();     // JWT
app.UseAuthorization();
app.MapControllers();

app.Run();

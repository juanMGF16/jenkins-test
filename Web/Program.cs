using Business.AutoMapper;
using CloudinaryDotNet;
using Data.SeedData.SeederHelpers;
using System.Reflection;
using Web.Extensions;


var builder = WebApplication.CreateBuilder(args);

// =============== [ SERVICES ] ===============

// Middleware
builder.Services.AddHttpContextAccessor(); 

// DbContext
builder.Services.AddPersistence(builder.Configuration); // Extension

// DataINIT
builder.Services.AddDataSeeders(); // Extension

// Controllers
builder.Services.AddControllers();

// AutoMapper
builder.Services.AddAutoMapper(typeof(GeneralMapper));

// Swagger 
builder.Services.AddSwaggerDocumentation();
builder.Services.AddSwaggerWithJwtSupport(); // Extension

builder.Services.AddMemoryCache();

// JWT 
builder.Services.AddJwtAuthentication(builder.Configuration); // Extension

// CORS 
builder.Services.AddCustomCors(builder.Configuration); // Extension

// Entities
builder.Services.AddEntitiesServices(); //Extension

// BUSINESS + SERVICES EXTRAS
builder.Services.AddBusinessServices(builder.Configuration); // Extension


// =============== [ Cloudinary Config ] ===============
var account = new Account(
    builder.Configuration["Cloudinary:CloudName"],
    builder.Configuration["Cloudinary:ApiKey"],
    builder.Configuration["Cloudinary:ApiSecret"]
);

var cloudinary = new Cloudinary(account);
builder.Services.AddSingleton(cloudinary);

// =============== [ Build App ] ===============
var app = builder.Build();

// Swagger en desarrollo
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Mi API v1");
        c.DocExpansion(Swashbuckle.AspNetCore.SwaggerUI.DocExpansion.None);
        // None -> todo colapsado
        // List -> lista de endpoints visible, pero sin detalles
        // Full -> todo expandido
    });

    using var scope = app.Services.CreateScope();
    var config = scope.ServiceProvider.GetRequiredService<IConfiguration>();
    await SeederExecutor.SeedAllAsync(scope.ServiceProvider, config);
}

if (app.Environment.IsProduction())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();
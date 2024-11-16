//var builder = WebApplication.CreateBuilder(args);

//// Add services to the container.

//builder.Services.AddControllers();
//// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
//builder.Services.AddOpenApi();
using ChallengerTest.Data;
using ChallengerTest.Repositories;
using ChallengerTest.Repositories.Command;
using ChallengerTest.Repositories.Query;
using ChallengerTest.Services;
using Microsoft.EntityFrameworkCore;
using Nest; // Cliente de Elasticsearch

var builder = WebApplication.CreateBuilder(args);

// Configurar cadena de conexión a SQL Server
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Registrar repositorios y Unit of Work
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IPermissionQueryRepository, PermissionQueryRepository>();
builder.Services.AddScoped<IPermissionCommandRepository, PermissionCommandRepository>();

// Configurar Elasticsearch
var settings = new ConnectionSettings(new Uri("http://localhost:9200")).DefaultIndex("permissions");
var elasticClient = new ElasticClient(settings);
builder.Services.AddSingleton<IElasticClient>(elasticClient);
builder.Services.AddScoped<IElasticsearchService, ElasticsearchService>();

// Agregar servicios de controladores
builder.Services.AddControllers();

// Agregar Swagger (opcional para documentación)
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configurar middleware
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();

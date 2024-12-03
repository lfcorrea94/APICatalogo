using APICatalogo.Context;
using APICatalogo.DTOs.Mappings;
using APICatalogo.Extensions;
using APICatalogo.Filters;
using APICatalogo.Logging;
using APICatalogo.Repositories;
using APICatalogo.Repositories.Interface;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;
using AutoMapper;
using Microsoft.Extensions.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Define nos controladores as configura��es necess�rias para que ele seja utilizado corretamente
builder.Services.AddControllers().AddJsonOptions(options =>
    options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles); // Ignorar refer�ncias c�clicas no Json
builder.Services.AddControllers(options =>
{
    options.Filters.Add(typeof(ApiExceptionFilter));
})
.AddJsonOptions(options =>
{
    options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
})
.AddNewtonsoftJson();


// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Obtem a string de conex�o
string mySqlConnection = builder.Configuration.GetConnectionString("DefaultConnection");
// Inst�ncia de builder para definir o provedor, registrar e definir o contexto registrando os servi�os no container DI
builder.Services.AddDbContext<AppDbContext>( options=>options.UseMySql(mySqlConnection,
                                            ServerVersion.AutoDetect(mySqlConnection)) );

// Registro do servi�o no container
builder.Services.AddScoped<ICategoriaRepository, CategoriaRepository>();
builder.Services.AddScoped<IProdutoRepository, ProdutoRepository>();
builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();  

// Configurando o provedor de log customizado definindo o n�vem m�nimo como information
builder.Logging.AddProvider(new CustomLoggerProvider(new CustomLoggerProviderConfiguration
{
    LogLevel = LogLevel.Information
}));

builder.Services.AddAutoMapper(typeof(ProdutoDTOMappingProfile));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    // Configura middleware de tratamento de exce��es global
    app.ConfigureExceptionHandler();
}

app.UseHttpsRedirection();

app.UseAuthorization();

// Inclui o middleware de roteamento sem fazer suposi��o de roteamento.
// Faz com que dependa do roteamento dos atributos que utilizamos no controllador. � usado para mapear qualquer atributo
// que existe nos controlladores utilizando os atributos Route
app.MapControllers();

app.Run();

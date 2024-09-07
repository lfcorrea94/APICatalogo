using APICatalogo.Context;
using APICatalogo.Extensions;
using APICatalogo.Logging;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers().AddJsonOptions(options =>
    options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles); // Ignorar referências cíclicas no Json

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Obtem a string de conexão
string mySqlConnection = builder.Configuration.GetConnectionString("DefaultConnection");
// Instância de builder para definir o provedor, registrar e definir o contexto registrando os serviços no container DI
builder.Services.AddDbContext<AppDbContext>( options=>options.UseMySql(mySqlConnection,
                                            ServerVersion.AutoDetect(mySqlConnection)) );

// Configurando o provedor de log customizado definindo o nívem mínimo como information
builder.Logging.AddProvider(new CustomLoggerProvider(new CustomLoggerProviderConfiguration
{
    LogLevel = LogLevel.Information
}));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    // Configura middleware de tratamento de exceções global
    app.ConfigureExceptionHandler();
}

app.UseHttpsRedirection();

app.UseAuthorization();

// Inclui o middleware de roteamento sem fazer suposição de roteamento.
// Faz com que dependa do roteamento dos atributos que utilizamos no controllador. É usado para mapear qualquer atributo
// que existe nos controlladores utilizando os atributos Route
app.MapControllers();

app.Run();

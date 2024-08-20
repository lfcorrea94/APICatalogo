using APICatalogo.Context;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers().AddJsonOptions(options =>
    options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles); // Ignorar refer�ncias c�clicas no Json

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Obtem a string de conex�o
string mySqlConnection = builder.Configuration.GetConnectionString("DefaultConnection");
// Inst�ncia de builder para definir o provedor, registrar e definir o contexto registrando os servi�os no container DI
builder.Services.AddDbContext<AppDbContext>( options=>options.UseMySql(mySqlConnection,
                                            ServerVersion.AutoDetect(mySqlConnection)) );

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

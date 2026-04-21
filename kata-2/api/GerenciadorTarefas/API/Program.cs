using GerenciadorTarefas.Domain.Interfaces;
using GerenciadorTarefas.Infrastructure.Data;
using GerenciadorTarefas.Infrastructure.Repositories;

using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "Painel de Tarefas API", Version = "v1" });
});

builder.Services.AddDbContext<TarefaContexto>(opcoes =>
    opcoes.UseSqlite("Data Source=tarefas.db"));

builder.Services.AddScoped<ITarefaRepositorio, TarefaRepositorio>();

builder.Services.AddCors(opcoes =>
    opcoes.AddDefaultPolicy(p => p.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod()));

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Painel de Tarefas v1"));
}

app.UseCors();
app.MapControllers();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<TarefaContexto>();
    db.Database.EnsureCreated();
}

app.Run();
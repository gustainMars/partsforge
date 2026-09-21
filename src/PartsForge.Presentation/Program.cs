using Microsoft.EntityFrameworkCore;
using PartsForge.Application.Interfaces;
using PartsForge.Application.UseCases.Receitas.ConsultarViabilidade;
using PartsForge.Infrastructure.Persistence;
using PartsForge.Infrastructure.Persistence.Repositories;
using PartsForge.Presentation.ExceptionHandling;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<PartsForgeDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("Default")));

builder.Services.AddScoped<IReceitaRepository, ReceitaRepository>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

builder.Services.AddMediatR(cfg => 
    cfg.RegisterServicesFromAssembly(typeof(ConsultarViabilidadeQuery).Assembly));

builder.Services.AddExceptionHandler<ApiExceptionHandler>();
builder.Services.AddProblemDetails();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseExceptionHandler();

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

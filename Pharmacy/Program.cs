using Pharmacy_.Interfaces;
using Pharmacy_.MiddleWares;
using Pharmacy_.Repositories;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(opt => 
    opt.AddDefaultPolicy(b => b.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin()));

// Add items to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<IPhotoService, PhotoService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseMiddleware<ExceptionHandlingMiddleware>();

app.UseHttpsRedirection();

app.UseAuthorization();

app.UseCors();

app.MapControllers();

app.Run();

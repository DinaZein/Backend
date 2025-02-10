using EmployeeManagementAPI.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddCors(options =>
{
  options.AddPolicy("AllowFrontend",
      policy =>
      {
        policy.WithOrigins("http://localhost:3000") // React Frontend URL
                .AllowAnyMethod()
                .AllowAnyHeader();
      });
});


builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();


if (app.Environment.IsDevelopment())
{
  app.UseSwagger();
  app.UseSwaggerUI();
}



app.UseCors("AllowFrontend");


app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();

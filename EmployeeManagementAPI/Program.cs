using EmployeeManagementAPI.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// ✅ 1. Add Database Connection (SQL Server)
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// ✅ 2. Enable CORS (Allow Frontend to Access API)
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

// ✅ 3. Add Controllers
builder.Services.AddControllers();

// ✅ 4. Enable Swagger for API Documentation
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// ✅ 5. Enable Swagger UI in Development Mode
if (app.Environment.IsDevelopment())
{
  app.UseSwagger();
  app.UseSwaggerUI();
}

// ✅ 6. Enable CORS Middleware (Must Be Before Authorization)
app.UseCors("AllowFrontend");

// ✅ 7. Middleware Setup
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();

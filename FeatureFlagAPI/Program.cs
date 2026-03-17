using FeatureFlagAPI.Interfaces;
using FeatureFlagAPI.Repositories;
using FeatureFlagAPI.Services;
using Microsoft.Data.SqlClient;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IRoleRepository, RoleRepository>();
builder.Services.AddScoped<IFeatureRepository, FeatureRepository>();  // ? NEW
builder.Services.AddScoped<IFeaturePermissionRepository, FeaturePermissionRepository>();
builder.Services.AddScoped<IRoleService, RoleService>();
builder.Services.AddScoped<IUser, UserService>();
builder.Services.AddScoped<IFeatureService, FeatureService>();
builder.Services.AddScoped<IRegionService, RegionService>();
builder.Services.AddScoped<IRegionRepository, RegionRepository>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngular", policy =>
    {
        policy.WithOrigins("http://localhost:4200")
              .AllowAnyHeader()
              .AllowAnyMethod().AllowCredentials();
    });
});
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
try
{
    using var conn = new SqlConnection(connectionString);
    conn.Open();
    Console.WriteLine("? Database connection successful!");
}
catch (Exception ex)
{
    Console.WriteLine($"? Database connection failed: {ex.Message}");
}
var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("AllowAngular");
app.UseAuthorization();

app.MapControllers();

app.Run();

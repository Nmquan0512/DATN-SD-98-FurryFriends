using FurryFriends.API.Data;
using FurryFriends.API.Repositories;
using FurryFriends.API.Repository;
using FurryFriends.API.Repository.IRepository;
using FurryFriends.API.Services.IServices;
using FurryFriends.API.Services;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;
using FurryFriends.API.Repositories.IRepositories;
using Microsoft.Extensions.FileProviders;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
        options.JsonSerializerOptions.WriteIndented = true;
    });
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHttpContextAccessor();

// Add DbContext
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
// Add Repository Services
builder.Services.AddScoped<IHoaDonRepository, HoaDonRepository>();

// Add CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        builder =>
        {
            builder.AllowAnyOrigin()
                   .AllowAnyMethod()
                   .AllowAnyHeader();
        });
});
builder.Services.AddScoped<IChatLieuRepository, ChatLieuRepository>();
// Trong FurryFriends.API.Program.cs
builder.Services.AddScoped<IChatLieuService, ChatLieuService>();
builder.Services.AddScoped<IThanhPhanRepository, ThanhPhanRepository>();
// Trong FurryFriends.API.Program.cs
builder.Services.AddScoped<IThanhPhanService, ThanhPhanService>();
builder.Services.AddScoped<IThuongHieuService, ThuongHieuService>();
// Trong FurryFriends.API.Program.cs
builder.Services.AddScoped<IThuongHieuRepository, ThuongHieuRepository>();
builder.Services.AddScoped<IMauSacRepository, MauSacRepository>();
// Trong FurryFriends.API.Program.cs
builder.Services.AddScoped<IMauSacService, MauSacService>();
builder.Services.AddScoped<IKichCoService, KichCoService>();
// Trong FurryFriends.API.Program.cs
builder.Services.AddScoped<IKichCoRepository, KichCoRepository>();
builder.Services.AddScoped<IAnhService, AnhService>();
// Trong FurryFriends.API.Program.cs
builder.Services.AddScoped<IAnhRepository, AnhRepository>();
builder.Services.AddScoped<ISanPhamRepository, SanPhamRepository>();
// Trong FurryFriends.API.Program.cs
builder.Services.AddScoped<ISanPhamService, SanPhamService>();
var app = builder.Build();
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();


// Use CORS
app.UseCors("AllowAll");
app.UseStaticFiles();
// Add static files middleware
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(
        Path.Combine(Directory.GetCurrentDirectory(), "wwwroot")),
    RequestPath = ""
});

app.UseAuthorization();

app.MapControllers();

app.Run();

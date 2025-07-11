using FurryFriends.API.Data;
using FurryFriends.API.Models;
using FurryFriends.API.Repositories;
using FurryFriends.API.Repository;
using FurryFriends.API.Repository.IRepository;
using FurryFriends.API.Services.IServices;
using FurryFriends.API.Services;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;

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

// Add DbContext
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Add Repository Services
builder.Services.AddScoped<IHoaDonRepository, HoaDonRepository>();
builder.Services.AddScoped<IGiamGiaRepository, GiamGiaRepository>();
builder.Services.AddScoped<IKhachHangRepository, KhachHangRepository>();
builder.Services.AddScoped<IChucVuRepository, ChucVuRepository>();
builder.Services.AddScoped<IDiaChiKhachHangRepository, DiaChiKhachHangRepository>();
builder.Services.AddScoped<IKhachHangRepository, KhachHangRepository>();
builder.Services.AddScoped<IVoucherRepository, VoucherRepository>();
builder.Services.AddScoped<ITaiKhoanRepository, TaiKhoanRepository>();
builder.Services.AddScoped<IThongTinCaNhanService, ThongTinCaNhanService>();
builder.Services.AddScoped<INhanVienRepository, NhanVIenRepository>();
builder.Services.AddScoped<IThongBaoRepository, ThongBaoRepository>();
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

// Add static files middleware
app.UseStaticFiles();

app.UseAuthorization();

app.MapControllers();

app.Run();

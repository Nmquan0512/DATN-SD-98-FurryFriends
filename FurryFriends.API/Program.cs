using FurryFriends.API.Data;
using FurryFriends.API.Models;
using FurryFriends.API.Repositories;
using FurryFriends.API.Repository;
using FurryFriends.API.Repository.IRepository;
using FurryFriends.API.Services;
using FurryFriends.API.Services.IServices;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using System.Text.Json.Serialization;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;

try
{
    var builder = WebApplication.CreateBuilder(args);

    // Add services to the container.
    builder.Services.AddControllers()
        .AddJsonOptions(options =>
        {
            options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
            options.JsonSerializerOptions.WriteIndented = true;
        });
    
    // Thêm validation services từ nhánh HEAD
    builder.Services.Configure<ApiBehaviorOptions>(options =>
    {
        options.SuppressModelStateInvalidFilter = false;
    });

    // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();
    builder.Services.AddHttpContextAccessor();

    // Add DbContext
    builder.Services.AddDbContext<AppDbContext>(options =>
        options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

    // Thêm Authentication và JWT Bearer từ nhánh origin/BanHanglan1sua
    builder.Services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
        };
    });

    // Add all Repository and Service dependencies
    builder.Services.AddScoped<IHoaDonRepository, HoaDonRepository>();
    builder.Services.AddScoped<IGiamGiaRepository, GiamGiaRepository>();
    builder.Services.AddScoped<IKhachHangRepository, KhachHangRepository>();
    builder.Services.AddScoped<IChucVuRepository, ChucVuRepository>();
    builder.Services.AddScoped<IDiaChiKhachHangRepository, DiaChiKhachHangRepository>();
    builder.Services.AddScoped<IVoucherRepository, VoucherRepository>();
    builder.Services.AddScoped<ITaiKhoanRepository, TaiKhoanRepository>();
    builder.Services.AddScoped<INhanVienRepository, NhanVIenRepository>();
    builder.Services.AddScoped<IThongBaoRepository, ThongBaoRepository>();
    builder.Services.AddScoped<IChatLieuRepository, ChatLieuRepository>();
    builder.Services.AddScoped<IChatLieuService, ChatLieuService>();
    builder.Services.AddScoped<IThanhPhanRepository, ThanhPhanRepository>();
    builder.Services.AddScoped<IThanhPhanService, ThanhPhanService>();
    builder.Services.AddScoped<IThuongHieuService, ThuongHieuService>();
    builder.Services.AddScoped<IThuongHieuRepository, ThuongHieuRepository>();
    builder.Services.AddScoped<IMauSacRepository, MauSacRepository>();
    builder.Services.AddScoped<IMauSacService, MauSacService>();
    builder.Services.AddScoped<IKichCoService, KichCoService>();
    builder.Services.AddScoped<ISanPhamService, SanPhamService>();
    builder.Services.AddAutoMapper(typeof(Program).Assembly);
    builder.Services.AddScoped<IKichCoRepository, KichCoRepository>();
    builder.Services.AddScoped<IAnhService, AnhService>();
    builder.Services.AddScoped<IAnhRepository, AnhRepository>();
    builder.Services.AddScoped<ISanPhamRepository, SanPhamRepository>();
    builder.Services.AddScoped<ISanPhamChiTietRepository, SanPhamChiTietRepository>();
    builder.Services.AddScoped<ISanPhamChiTietService, SanPhamChiTietService>();
    builder.Services.AddScoped<IThongTinCaNhanService, ThongTinCaNhanService>();
    builder.Services.AddScoped<IDotGiamGiaSanPhamRepository, DotGiamGiaSanPhamRepository>();
    builder.Services.AddScoped<IGiamGiaService, GiamGiaService>();
    builder.Services.AddScoped<IBanHangRepository, BanHangRepository>();
    builder.Services.AddScoped<IBanHangService, BanHangService>();
    // Thêm từ nhánh HEAD
    builder.Services.AddScoped<IGioHangRepository, GioHangRepository>();
    builder.Services.AddScoped<IHinhThucThanhToanRepository, HinhThucThanhToanRepository>();
    builder.Services.AddScoped<VoucherCalculationService>();

    // Add CORS policy cho phép web admin truy cập API
    builder.Services.AddCors(options =>
    {
        options.AddPolicy("AllowWebAdmin",
            policy =>
            {
                policy.WithOrigins("https://localhost:7102")
                      .AllowAnyHeader()
                      .WithMethods("GET", "POST", "DELETE", "OPTIONS");
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
    app.UseCors("AllowWebAdmin");
    app.UseStaticFiles();
    
    // Add static files middleware
    app.UseStaticFiles(new StaticFileOptions
    {
        FileProvider = new PhysicalFileProvider(
            Path.Combine(Directory.GetCurrentDirectory(), "wwwroot")),
        RequestPath = ""
    });

    app.UseAuthentication(); // Thêm authentication trước authorization
    app.UseAuthorization();

    app.MapControllers();

    app.Run();
}
catch (Exception ex)
{
    // Bắt bất kỳ lỗi nào xảy ra trong quá trình khởi tạo và ghi ra console
    Console.ForegroundColor = ConsoleColor.Red;
    Console.WriteLine("!!!!!! A FATAL EXCEPTION OCCURRED DURING STARTUP !!!!!!");
    Console.WriteLine(ex.ToString());
    Console.ResetColor();
    // Giữ cho cửa sổ console không bị đóng ngay lập tức
    Console.WriteLine("Press any key to exit...");
    Console.ReadKey();
}
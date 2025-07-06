using FurryFriends.API.Repository;
using FurryFriends.API.Repository.IRepository;
using FurryFriends.Web.Service.IService;
using FurryFriends.Web.Service;
using FurryFriends.Web.Services;
using FurryFriends.Web.Services.IService;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication.Facebook;
using FurryFriends.API.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddHttpClient<IHoaDonService, HoaDonService>(client =>
{
    client.BaseAddress = new Uri("https://localhost:7289/");
    client.Timeout = TimeSpan.FromSeconds(30);
});

builder.Services.AddHttpClient<IGiamGiaService, GiamGiaService>(client =>
{
    client.BaseAddress = new Uri("https://localhost:7289/api/");
    client.Timeout = TimeSpan.FromSeconds(30);
});

builder.Services.AddHttpClient<IDotGiamGiaService, DotGiamGiaService>(client =>
{
    client.BaseAddress = new Uri("https://localhost:7289/api/");
    client.Timeout = TimeSpan.FromSeconds(30);
});
builder.Services.AddHttpClient<ISanPhamRepository, SanPhamRepository>(client =>
{
    client.BaseAddress = new Uri("https://localhost:7289/api/");
    client.Timeout = TimeSpan.FromSeconds(30);
});
builder.Services.AddHttpClient<IDiaChiKhachHangService, DiaChiKhachHangService>(client =>
{
    client.BaseAddress = new Uri("https://localhost:7289/");
    client.Timeout = TimeSpan.FromSeconds(30);
});
builder.Services.AddHttpClient<IKhachHangService, KhachHangService>(client =>
{
    client.BaseAddress = new Uri("https://localhost:7289/");
    client.Timeout = TimeSpan.FromSeconds(30);
});
builder.Services.AddHttpClient<IChucVuService, ChucVuService>(client =>
{
    client.BaseAddress = new Uri("https://localhost:7289/api/"); // Địa chỉ API, thay đổi port nếu cần
    client.DefaultRequestHeaders.Add("Accept", "application/json");
    client.Timeout = TimeSpan.FromSeconds(30);
});
builder.Services.AddHttpClient<ITaiKhoanService, TaiKhoanService>(client =>
{
    client.BaseAddress = new Uri("https://localhost:7289/api/"); // Địa chỉ API, thay đổi port nếu cần
    client.DefaultRequestHeaders.Add("Accept", "application/json");
    client.Timeout = TimeSpan.FromSeconds(30);
});
builder.Services.AddHttpClient<INhanVienService, NhanVienService>(client =>
{
    client.BaseAddress = new Uri("https://localhost:7289/api/"); // Địa chỉ API, thay đổi port nếu cần
    client.DefaultRequestHeaders.Add("Accept", "application/json");
    client.Timeout = TimeSpan.FromSeconds(30);
});
builder.Services.AddHttpClient<IVoucherService, VoucherService>(client =>
{
    client.BaseAddress = new Uri("https://localhost:7289/");
    client.DefaultRequestHeaders.Add("Accept", "application/json");
    client.Timeout = TimeSpan.FromSeconds(30);
});
builder.Services.AddHttpClient<IChatLieuService, ChatLieuService>(client =>
{
    client.BaseAddress = new Uri("https://localhost:7289/");
});
builder.Services.AddHttpClient<IThanhPhanService, ThanhPhanService>(client =>
{
    client.BaseAddress = new Uri("https://localhost:7289/");
});
builder.Services.AddHttpClient<IThuongHieuService, ThuongHieuService>(client =>
{
    client.BaseAddress = new Uri("https://localhost:7289/");
});
builder.Services.AddHttpClient<IMauSacService, MauSacService>(client =>
{
    client.BaseAddress = new Uri("https://localhost:7289/");
});
builder.Services.AddHttpClient<IKichCoService, KichCoService>(client =>
{
    client.BaseAddress = new Uri("https://localhost:7289/");
});


// Register SanPhamService for Dashboard
builder.Services.AddScoped<ISanPhamService, SanPhamService>();

// Thêm cấu hình xác thực Google và Facebook
builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = GoogleDefaults.AuthenticationScheme;
})
.AddCookie()
.AddGoogle(options =>
{
    options.ClientId = "968410379877-vk3bu6n1711b6ip9756ranke5uc7rvmd.apps.googleusercontent.com";
    options.ClientSecret = "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";
    options.CallbackPath = "/DangKy/DangNhapGoogleCallback";
});

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromHours(2);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

builder.Services.AddHttpContextAccessor();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.UseSession();

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllerRoute(
        name: "Areas",
        pattern: "{area:exists}/{controller=Dashboard}/{action=Index}/{id?}");

    endpoints.MapControllerRoute(
        name: "default",
        pattern: "{controller=Home}/{action=Index}/{id?}");
});

app.Run();

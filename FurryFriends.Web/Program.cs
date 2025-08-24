using FurryFriends.Web.Service;
using FurryFriends.Web.Service.IService;
using FurryFriends.Web.Services;
using FurryFriends.Web.Services.Handlers;
using FurryFriends.Web.Services.IService;
using FurryFriends.Web.Services.IServices;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Facebook;
using Microsoft.AspNetCore.Authentication.Google;
using System.Security.Claims;

var builder = WebApplication.CreateBuilder(args);

// Cấu hình logging
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();

// Add services to the container.
builder.Services.AddControllersWithViews();



// Đăng ký HttpMessageHandler
builder.Services.AddScoped<AuthHeaderHandler>();

// Đăng ký các service với HttpClient
builder.Services.AddScoped<IVnPayService, VnPayService>();
builder.Services.AddScoped<IEmailNotificationService, EmailNotificationService>();
builder.Services.AddScoped<DiscountCalculationService>();
builder.Services.AddScoped<IPhieuHoanTraService, PhieuHoanTraService>();

builder.Services.AddHttpClient<IPhieuHoanTraService, PhieuHoanTraService>(client =>
{
	client.BaseAddress = new Uri("https://localhost:7289/"); // URL API của bạn
});

builder.Services.AddHttpClient<IHoaDonService, HoaDonService>(client =>
{
    client.BaseAddress = new Uri("https://localhost:7289/");
    client.Timeout = TimeSpan.FromSeconds(30);
});

builder.Services.AddHttpClient<IGiamGiaService, GiamGiaService>(client =>
{
    client.BaseAddress = new Uri("https://localhost:7289/");
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
    client.BaseAddress = new Uri("https://localhost:7289/api/");
    client.DefaultRequestHeaders.Add("Accept", "application/json");
    client.Timeout = TimeSpan.FromSeconds(30);
});
builder.Services.AddHttpClient<ITaiKhoanService, TaiKhoanService>(client =>
{
    client.BaseAddress = new Uri("https://localhost:7289/api/");
    client.DefaultRequestHeaders.Add("Accept", "application/json");
    client.Timeout = TimeSpan.FromSeconds(30);
});
builder.Services.AddHttpClient<INhanVienService, NhanVienService>(client =>
{
    client.BaseAddress = new Uri("https://localhost:7289/api/");
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
builder.Services.AddHttpClient<IThongTinCaNhanService, ThongTinCaNhanService>(client =>
{
    client.BaseAddress = new Uri("https://localhost:7289/");
});
builder.Services.AddHttpClient<IGioHangService, GioHangService>(client =>
{
    client.BaseAddress = new Uri("https://localhost:7289/");
});
builder.Services.AddHttpClient<IHinhThucThanhToanService, HinhThucThanhToanService>(client =>
{
    client.BaseAddress = new Uri("https://localhost:7289/");
});
builder.Services.AddHttpClient<IAnhService, AnhService>(client =>
{
    client.BaseAddress = new Uri("https://localhost:7289/");
});

builder.Services.AddHttpClient<ISanPhamChiTietService, SanPhamChiTietService>(client =>
{
    client.BaseAddress = new Uri("https://localhost:7289/");
});

builder.Services.AddHttpClient<ISanPhamService, SanPhamService>(client =>
{
    client.BaseAddress = new Uri("https://localhost:7289/");
});

// Sử dụng AddHttpMessageHandler để thêm AuthHeaderHandler
builder.Services.AddHttpClient<IBanHangService, BanHangService>(client =>
{
    client.BaseAddress = new Uri("https://localhost:7289/");
}).AddHttpMessageHandler<AuthHeaderHandler>();

// Thêm cấu hình xác thực Google và Facebook
builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = GoogleDefaults.AuthenticationScheme;
})
        .AddCookie(options =>
        {
            options.Cookie.Name = "FurryFriends.Auth";
            options.Cookie.HttpOnly = true;
            options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
            options.Cookie.SameSite = SameSiteMode.Lax;
            options.ExpireTimeSpan = TimeSpan.FromHours(2);
            options.SlidingExpiration = true;
        })
.AddGoogle(options =>
{
    options.ClientId = "968410379877-vk3bu6n1711b6ip9756ranke5uc7rvmd.apps.googleusercontent.com";
    options.ClientSecret = "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";
    options.CallbackPath = "/DangKy/signin-google";
    options.SaveTokens = true;
    options.CorrelationCookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
    options.CorrelationCookie.SameSite = SameSiteMode.Lax;
    options.CorrelationCookie.Expiration = TimeSpan.FromMinutes(30);
    options.Events = new Microsoft.AspNetCore.Authentication.OAuth.OAuthEvents
    {
        OnRemoteFailure = context =>
        {
            if (context.Failure?.Message?.Contains("oauth state was missing or invalid") == true)
            {
                context.HandleResponse();
                context.Response.Redirect("/DangKy?error=oauth_state_invalid");
                return Task.CompletedTask;
            }
            
            context.HandleResponse();
            context.Response.Redirect("/DangKy?error=google_auth_failed");
            return Task.CompletedTask;
        },
        OnTicketReceived = async context =>
        {
            var claims = context.Principal.Identities.FirstOrDefault()?.Claims;
            var email = claims?.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
            var name = claims?.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value;
            var picture = claims?.FirstOrDefault(c => c.Type == "urn:google:picture")?.Value;
            
            if (string.IsNullOrEmpty(email))
            {
                context.Response.Redirect("/DangKy?error=google_auth_failed_no_email");
                context.HandleResponse();
                return;
            }
            
            // Prevent default redirect and redirect to our processing action with query parameters
            context.HandleResponse();
            var redirectUrl = $"/DangKy/ProcessGoogleLogin?email={Uri.EscapeDataString(email)}&name={Uri.EscapeDataString(name ?? "")}&picture={Uri.EscapeDataString(picture ?? "")}";
            context.Response.Redirect(redirectUrl);
            await Task.CompletedTask;
        }
    };
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
    app.UseHsts();
}



app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseSession();

app.UseAuthentication();
app.UseAuthorization();

app.UseCookiePolicy(new CookiePolicyOptions
{
    MinimumSameSitePolicy = SameSiteMode.Lax,
    HttpOnly = Microsoft.AspNetCore.CookiePolicy.HttpOnlyPolicy.Always,
    Secure = CookieSecurePolicy.SameAsRequest
});

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

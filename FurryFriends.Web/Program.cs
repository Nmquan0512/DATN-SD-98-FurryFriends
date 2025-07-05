using FurryFriends.Web.Services;
using FurryFriends.Web.Service.IService;
using FurryFriends.Web.Service;
using FurryFriends.Web.Services;
using FurryFriends.Web.Services.IService;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllersWithViews();

// Cấu hình HttpClient cho các Service dùng để gọi API
builder.Services.AddHttpClient<IKhachHangService, KhachHangService>(static client =>
{
    client.BaseAddress = new Uri("https://localhost:7289/");
});

builder.Services.AddHttpClient<IHoaDonService, HoaDonService>(client =>
{
    client.BaseAddress = new Uri("https://localhost:7289/");
});
builder.Services.AddHttpClient<IDiaChiKhachHangService, DiaChiKhachHangService>(client =>
{
    client.BaseAddress = new Uri("https://localhost:7289/");
});
builder.Services.AddHttpClient<IVoucherService, VoucherService>(client =>
{
    client.BaseAddress = new Uri("https://localhost:7289/");
});
builder.Services.AddHttpClient<IVoucherService, VoucherService>(client =>
{
    client.BaseAddress = new Uri("https://localhost:7289/");
});
var app = builder.Build();

// Configure the HTTP request pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

// Điều hướng nếu dùng Area
app.UseEndpoints(endpoints =>
{
    endpoints.MapControllerRoute(
        name: "Areas",
        pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

    endpoints.MapControllerRoute(
        name: "default",
        pattern: "{controller=Home}/{action=Index}/{id?}");
});

app.Run();

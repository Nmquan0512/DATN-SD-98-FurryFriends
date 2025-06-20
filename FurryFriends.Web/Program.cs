using FurryFriends.API.Repository.IRepository;
using FurryFriends.API.Repository;
using FurryFriends.Web.Services;
using FurryFriends.Web.Services.IService;
using FurryFriends.API.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers().AddJsonOptions(x =>
{
	x.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.Preserve;
	x.JsonSerializerOptions.WriteIndented = true;
});
// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddHttpClient<IHoaDonService, HoaDonService>(client =>
{
    client.BaseAddress = new Uri("https://localhost:7289/");
});
builder.Services.AddHttpClient<IChucVuService, ChucVuService>(client =>
{
	client.BaseAddress = new Uri("https://localhost:7289/api/"); // Địa chỉ API, thay đổi port nếu cần
	client.DefaultRequestHeaders.Add("Accept", "application/json");
});
builder.Services.AddHttpClient<ITaiKhoanService, TaiKhoanService>(client =>
{
	client.BaseAddress = new Uri("https://localhost:7289/api/"); // Địa chỉ API, thay đổi port nếu cần
	client.DefaultRequestHeaders.Add("Accept", "application/json");
});
builder.Services.AddHttpClient<INhanVienService, NhanVienService>(client =>
{
	client.BaseAddress = new Uri("https://localhost:7289/api/"); // Địa chỉ API, thay đổi port nếu cần
	client.DefaultRequestHeaders.Add("Accept", "application/json");
});


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

app.UseAuthorization();

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

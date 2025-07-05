using FurryFriends.Web.Services;
using FurryFriends.Web.Services.IService;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddHttpClient<IHoaDonService, HoaDonService>(client =>
{
    client.BaseAddress = new Uri("https://localhost:7289/");
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

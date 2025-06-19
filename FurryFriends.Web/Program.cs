using FurryFriends.API.Repository;
using FurryFriends.API.Repository.IRepository;
using FurryFriends.Web.Services;
using FurryFriends.Web.Services.IService;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddHttpClient<IGiamGiaService, GiamGiaService>(client =>
{
    client.BaseAddress = new Uri("https://localhost:7289/api/");
});

builder.Services.AddHttpClient<IDotGiamGiaService, DotGiamGiaService>(client =>
{
    client.BaseAddress = new Uri("https://localhost:7289/api/");
});
builder.Services.AddHttpClient<ISanPhamRepository, SanPhamRepository>(client =>
{
    client.BaseAddress = new Uri("https://localhost:7289/api/");
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

app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();

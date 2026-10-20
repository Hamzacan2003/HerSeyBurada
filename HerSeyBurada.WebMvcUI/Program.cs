using HerSeyBurada.Business.Abstract;
using HerSeyBurada.Business.Concrete;
using HerSeyBurada.Core.Entities;
using HerSeyBurada.DataAccess.Abstract;
using HerSeyBurada.DataAccess.Concrete;
using HerSeyBurada.WebMvcUI.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// 1. DbContext
builder.Services.AddDbContext<HerSeyBuradaDbContext>(options =>
    options.UseNpgsql(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        b => b.MigrationsAssembly("HerSeyBurada.DataAccess")
    ));

// 2. Repositories & Services
builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
builder.Services.AddScoped<IKullaniciService, KullaniciManager>();
builder.Services.AddScoped<IKuponService, KuponManager>();

// 3. Email & Session Kayıtları (EKLENDİ)
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddSession(); // TempData ve Session takibi için gerekli

// 4. Cookie Authentication
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/security/girisyap";
        options.LogoutPath = "/security/cikis";
        options.AccessDeniedPath = "/security/girisyap";
        options.Cookie.Name = "HerSeyBurada.Auth";
        options.ExpireTimeSpan = TimeSpan.FromDays(7);
    });

builder.Services.AddControllersWithViews();

var app = builder.Build();

// 5. Seed Data (Admin ve Satici)
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<HerSeyBuradaDbContext>();
    context.Database.EnsureCreated();

    if (!context.Kullanicilar.Any(k => k.Eposta == "hamzacana98@gmail.com"))
    {
        var admin = new Kullanici
        {
            Id = Guid.NewGuid(),
            Ad = "Hamza Can",
            Soyad = "Altıntop",
            Eposta = "hamzacana98@gmail.com",
            SifreHash = "hamzcan.1",
            Rol = "Admin"
        };
        context.Kullanicilar.Add(admin);
    }

    if (!context.Kullanicilar.Any(k => k.Eposta == "satici@herseyburada.com"))
    {
        var saticiUser = new Kullanici
        {
            Id = Guid.NewGuid(),
            Ad = "Firma 1",
            Soyad = "Mağaza",
            Eposta = "satici@herseyburada.com",
            SifreHash = "123456",
            Rol = "Satici"
        };
        context.Kullanicilar.Add(saticiUser);

        var magaza = new Magaza
        {
            Id = Guid.NewGuid(),
            KullaniciId = saticiUser.Id,
            MagazaAdi = "Teknoloji Ve Giyim Mağazası",
            VergiNo = "1234567890",
            OnaylandiMi = true
        };
        context.Magazalar.Add(magaza);
    }

    context.SaveChanges();
}

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.UseSession(); // EKLENDİ (Routing'den sonra, Auth'dan önce)

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Urun}/{action=Liste}/{id?}");

app.Run();
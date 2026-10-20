using HerSeyBurada.Business.Abstract;
using HerSeyBurada.Business.Concrete;
using HerSeyBurada.Core.Entities;
using HerSeyBurada.DataAccess.Abstract;
using HerSeyBurada.DataAccess.Concrete;
using HerSeyBurada.WebMvcUI.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using Npgsql;

var builder = WebApplication.CreateBuilder(args);

// 1. Connection String Okuma ve Npgsql Uyumluluğu
string? rawConnectionString = Environment.GetEnvironmentVariable("ConnectionStrings__DefaultConnection")
                             ?? builder.Configuration.GetConnectionString("DefaultConnection");

string formattedConnectionString = rawConnectionString ?? "";

// Eğer URI formatında (postgresql://) gelmişse Npgsql'in patlamaması için Key-Value formatına dönüştür
if (!string.IsNullOrEmpty(rawConnectionString) && rawConnectionString.StartsWith("postgresql://", StringComparison.OrdinalIgnoreCase))
{
    try
    {
        var uri = new Uri(rawConnectionString);
        var userInfo = uri.UserInfo.Split(':');

        var builderDb = new NpgsqlConnectionStringBuilder
        {
            Host = uri.Host,
            Port = uri.Port > 0 ? uri.Port : 5432,
            Username = userInfo[0],
            Password = userInfo.Length > 1 ? Uri.UnescapeDataString(userInfo[1]) : "",
            Database = uri.AbsolutePath.TrimStart('/'),
            SslMode = SslMode.Require,
            TrustServerCertificate = true
        };
        formattedConnectionString = builderDb.ConnectionString;
    }
    catch (Exception ex)
    {
        Console.WriteLine($"[CONN STRING HATA]: Connection string dönüştürülürken hata oluştu: {ex.Message}");
    }
}

// 2. DbContext Tanımı
builder.Services.AddDbContext<HerSeyBuradaDbContext>(options =>
    options.UseNpgsql(
        formattedConnectionString,
        b => b.MigrationsAssembly("HerSeyBurada.DataAccess")
    ));

// 3. Repositories & Services
builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
builder.Services.AddScoped<IKullaniciService, KullaniciManager>();
builder.Services.AddScoped<IKuponService, KuponManager>();

// 4. Email & Session
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddSession();

// 5. Cookie Authentication
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

// 6. Seed Data (Veritabanı Oluşturma ve İlk Kullanıcılar)
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<HerSeyBuradaDbContext>();

    // Veritabanını oluştur
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
app.UseSession();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Urun}/{action=Liste}/{id?}");

app.Run();
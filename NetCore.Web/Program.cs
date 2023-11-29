using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.EntityFrameworkCore;
using NetCore.Services.Data;
using NetCore.Services.Interfaces;
using NetCore.Services.Svcs;
using NetCore.Utilites.Utils;

var builder = WebApplication.CreateBuilder(args);

string defaultConnString = builder.Configuration.GetConnectionString(name:"DefaultConnection");
string dbFirstConnString = builder.Configuration.GetConnectionString(name: "DBFirstDBConnection");
// Add services to the container.
builder.Services.AddControllersWithViews();

#region Service 의존성 주입 

Common.SetDataProtection(builder.Services, @"D:\DataProtector\", "NetCore", Enums.CryptoType.CngCbc);
// IUser 인터페이스에 UserService 클래스 인스턴스를 주입
// 의존성 주입을 사용하기 위해서 서비스로 등록을 하는 시스템

builder.Services.AddScoped<IUser, UserService>();

// 데이터베이스 접속 정보, Migration 프로젝트를 지정
builder.Services.AddDbContext<CodeFirstDbContext>(options =>
{
    options.UseSqlServer(connectionString: defaultConnString,
        sqlServerOptionsAction: mig => mig.MigrationsAssembly(assemblyName: "NetCore.Migrations"));
});
builder.Services.AddDbContext<DBFirstDbContext>(options =>
        options.UseSqlServer(connectionString: dbFirstConnString));

//// 닷넷코어는 MVC 패턴을 사용하기 위해 의존성 주입을 사용해야하기 떄문에 MVC 서비스를 등록
//builder.Services.AddMvc();
builder.Services.AddControllersWithViews();
// 신원 보증과 승인 권한
builder.Services.AddAuthentication(defaultScheme: CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.AccessDeniedPath = "/Membership/Forbidden"; // 권한이 없을시 튀겨 나가 접속되는 페이지
        options.LogoutPath = "/Membership/Login";
    });
builder.Services.AddAuthorization();
builder.Services.AddHttpContextAccessor();

#endregion

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
////강의내용
//신원보증만
app.UseAuthentication();

// 승인권한을 사용하기 위해 추가됨.
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();

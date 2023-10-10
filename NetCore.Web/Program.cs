using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using NetCore.Services.Data;
using NetCore.Services.Interfaces;
using NetCore.Services.Svcs;
using System.Configuration;

var builder = WebApplication.CreateBuilder(args);

string connString = builder.Configuration.GetConnectionString(name:"DefaultConnection");
// Add services to the container.
builder.Services.AddControllersWithViews();

#region Service 의존성 주입 
// IUser 인터페이스에 UserService 클래스 인스턴스를 주입
// 의존성 주입을 사용하기 위해서 서비스로 등록을 하는 시스템
builder.Services.AddScoped<IUser, UserService>();
// 데이터베이스 접속 정보, Migration 프로젝트를 지정
builder.Services.AddDbContext<CodeFirstDbContext>(options =>
{
    options.UseSqlServer(connectionString: connString,
        sqlServerOptionsAction:mig => mig.MigrationsAssembly(assemblyName: "NetCore.Migrations"));
});
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

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();

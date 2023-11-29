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

#region Service ������ ���� 

Common.SetDataProtection(builder.Services, @"D:\DataProtector\", "NetCore", Enums.CryptoType.CngCbc);
// IUser �������̽��� UserService Ŭ���� �ν��Ͻ��� ����
// ������ ������ ����ϱ� ���ؼ� ���񽺷� ����� �ϴ� �ý���

builder.Services.AddScoped<IUser, UserService>();

// �����ͺ��̽� ���� ����, Migration ������Ʈ�� ����
builder.Services.AddDbContext<CodeFirstDbContext>(options =>
{
    options.UseSqlServer(connectionString: defaultConnString,
        sqlServerOptionsAction: mig => mig.MigrationsAssembly(assemblyName: "NetCore.Migrations"));
});
builder.Services.AddDbContext<DBFirstDbContext>(options =>
        options.UseSqlServer(connectionString: dbFirstConnString));

//// ����ھ�� MVC ������ ����ϱ� ���� ������ ������ ����ؾ��ϱ� ������ MVC ���񽺸� ���
//builder.Services.AddMvc();
builder.Services.AddControllersWithViews();
// �ſ� ������ ���� ����
builder.Services.AddAuthentication(defaultScheme: CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.AccessDeniedPath = "/Membership/Forbidden"; // ������ ������ Ƣ�� ���� ���ӵǴ� ������
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
////���ǳ���
//�ſ�������
app.UseAuthentication();

// ���α����� ����ϱ� ���� �߰���.
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();

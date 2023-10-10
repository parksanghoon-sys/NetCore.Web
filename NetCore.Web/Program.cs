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

#region Service ������ ���� 
// IUser �������̽��� UserService Ŭ���� �ν��Ͻ��� ����
// ������ ������ ����ϱ� ���ؼ� ���񽺷� ����� �ϴ� �ý���
builder.Services.AddScoped<IUser, UserService>();
// �����ͺ��̽� ���� ����, Migration ������Ʈ�� ����
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

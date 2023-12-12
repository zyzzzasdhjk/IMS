using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using IMS.Data;
using IMS.Service.DataBase;
using IMS.Service.TaskService;
using IMS.Service.TeamServices;
using IMS.Service.UserServices;
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddEntityFrameworkStores<ApplicationDbContext>();
builder.Services.AddControllersWithViews();

// IOC
builder.Services.AddSingleton<IRelationalDataBase, MysqlDataBase>();
builder.Services.AddSingleton<INosqlDataBase, MongoDataBase>();
builder.Services.AddSingleton<IUserService, UserService>();
builder.Services.AddSingleton<ITeamSqlService, TeamMysqlService>();
builder.Services.AddSingleton<ITaskSqlService, TaskMysqlService>();

// CORS 跨域
builder.Services.AddCors(policy =>
{
    policy.AddPolicy("CorsPolicy", opt => opt
        .AllowAnyOrigin()
        .AllowAnyHeader()
        .AllowAnyMethod()
        .WithExposedHeaders("X-Pagination"));
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

// 跨域
app.UseCors("CorsPolicy");

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();

app.Run();
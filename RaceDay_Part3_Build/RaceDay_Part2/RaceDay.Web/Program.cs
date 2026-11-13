var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllersWithViews();
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(o => { o.Cookie.Name = ".RaceDay.Web.Session"; o.Cookie.HttpOnly = true; o.Cookie.IsEssential = true; o.IdleTimeout = TimeSpan.FromHours(4); });
builder.Services.AddHttpClient("RaceDayApi", c => c.BaseAddress = new Uri(builder.Configuration["Api:BaseUrl"] ?? "https://localhost:7001/"));
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<RaceDay.Web.Services.IHttpContextAccessorAccessor, RaceDay.Web.Services.HttpContextAccessorAccessor>();
builder.Services.AddScoped<RaceDay.Web.Services.ApiClient>();
var app = builder.Build();
if (!app.Environment.IsDevelopment()) { app.UseExceptionHandler("/Home/Error"); app.UseHsts(); }
app.UseHttpsRedirection(); app.UseStaticFiles(); app.UseRouting(); app.UseSession();
app.MapControllerRoute(name: "default", pattern: "{controller=Home}/{action=Index}/{id?}");
app.Run();

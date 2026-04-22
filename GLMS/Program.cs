using Microsoft.EntityFrameworkCore;
using GLMS.Data;
using GLMS.AppServices;
using GLMS.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Register DbContext
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Register AppServices
builder.Services.AddScoped<IClientAppService, ClientAppService>();
builder.Services.AddScoped<IContractAppService, ContractAppService>();
builder.Services.AddScoped<IServiceRequestAppService, ServiceRequestAppService>();

// Register FileService
builder.Services.AddScoped<IFileService, FileService>();

// Register CurrencyService with HttpClient
builder.Services.AddHttpClient<ICurrencyService, CurrencyService>();

var app = builder.Build();

// Seed database
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    DbInitializer.Seed(dbContext);
}

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

// Configure default MVC route
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();

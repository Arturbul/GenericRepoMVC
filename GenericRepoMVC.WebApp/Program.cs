using GenericRepoMVC.Domain.Data;
using GenericRepoMVC.WebApp.Mappers;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

//Db context
builder.Services.AddDbContext<GenericRepoMVCContext>(
    opt => opt.UseSqlServer(builder.Configuration.GetConnectionString("GenericRepoMVCDb"))
);

//DI
GenericRepoMVC.Servicies.Dependencies.Register(builder.Services);

//Swagger
builder.Services.AddSwaggerGen(options =>
{
    List<string> xmlFiles = Directory.GetFiles(AppContext.BaseDirectory, "*.xml", SearchOption.TopDirectoryOnly).ToList();
    xmlFiles.ForEach(xmlFile => options.IncludeXmlComments(xmlFile));
});

builder.Services.AddAutoMapper(typeof(PersonProfile));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

//swagger
if (builder.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options => // UseSwaggerUI is called only in Development.
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
        options.RoutePrefix = string.Empty;
    });
}

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();

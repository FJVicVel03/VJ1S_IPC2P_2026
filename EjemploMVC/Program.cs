using Microsoft.EntityFrameworkCore;
using EjemploMVC.Data;
using EjemploMVC.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();

// Seed database and ensure created
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<ApplicationDbContext>();
        context.Database.EnsureCreated();
        
        if (!context.Products.Any())
        {
            context.Products.AddRange(
                new Product { Nombre = "Laptop HP Pavilion", Precio = 799.99m, Descripcion = "Laptop de 15 pulgadas, AMD Ryzen 5, 8GB RAM, 512GB SSD", Stock = 10 },
                new Product { Nombre = "Mouse Inalámbrico Logitech", Precio = 24.99m, Descripcion = "Mouse ergonómico inalámbrico con receptor USB", Stock = 50 },
                new Product { Nombre = "Teclado Mecánico Corsair", Precio = 89.99m, Descripcion = "Teclado mecánico retroiluminado RGB, interruptores Cherry MX Red", Stock = 15 }
            );
            context.SaveChanges();
        }
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "Ocurrió un error al sembrar la base de datos.");
    }
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Productos}/{action=Index}/{id?}")
    .WithStaticAssets();


app.Run();

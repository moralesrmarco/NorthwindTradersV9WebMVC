using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using NorthwindTradersV9BLL;
using NorthwindTradersV9Common;
using NorthwindTradersV9DAL;
using NorthwindTradersV9DAL.Helpers;
using NorthwindTradersV9DAL.Infrastructure;
using NorthwindTradersV9Entities;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddDistributedMemoryCache();

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

builder.Services.AddAuthentication(
    CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.Cookie.Name = "NorthwindTraders.Auth";
        options.ExpireTimeSpan = TimeSpan.FromDays(30);
        options.SlidingExpiration = true;
        options.LoginPath = "/Account/Login";
        options.AccessDeniedPath = "/Account/AccessDenied";
    });

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("PermisoEmpleados", policy =>
        policy.RequireClaim(
            "Permiso",
            Permisos.Empleados.ToString()));

    options.AddPolicy("PermisoClientes", policy =>
        policy.RequireClaim(
            "Permiso",
            Permisos.Clientes.ToString()));

    options.AddPolicy("PermisoProveedores", policy =>
        policy.RequireClaim(
            "Permiso",
            Permisos.Proveedores.ToString()));

    options.AddPolicy("PermisoClientesProveedores", policy =>
    {
        policy.RequireAssertion(context =>
            context.User.HasClaim(
                "Permiso",
                Permisos.Clientes.ToString()) ||

            context.User.HasClaim(
                "Permiso",
                Permisos.Proveedores.ToString()));
    });

    options.AddPolicy("PermisoCategorias", policy =>
        policy.RequireClaim(
            "Permiso",
            Permisos.Categorias.ToString()));

    options.AddPolicy("PermisoProductos", policy =>
        policy.RequireClaim(
            "Permiso",
            Permisos.Productos.ToString()));

    options.AddPolicy("PermisoCategoriasProductos", policy =>
    {
        policy.RequireAssertion(context =>
            context.User.HasClaim(
                "Permiso",
                Permisos.Categorias.ToString()) ||

            context.User.HasClaim(
                "Permiso",
                Permisos.Productos.ToString()));
    });

    options.AddPolicy("PermisoProveedoresProductos", policy =>
    {
        policy.RequireAssertion(context =>
            context.User.HasClaim(
                "Permiso",
                Permisos.Proveedores.ToString()) ||

            context.User.HasClaim(
                "Permiso",
                Permisos.Productos.ToString()));
    });

    options.AddPolicy("PermisoVentas", policy =>
        policy.RequireClaim(
            "Permiso",
            Permisos.Ventas.ToString()));

    options.AddPolicy("PermisoGraficas", policy =>
        policy.RequireClaim(
            "Permiso",
            Permisos.Graficas.ToString()));

    options.AddPolicy("PermisoAdministracion", policy =>
        policy.RequireClaim(
            "Permiso",
            Permisos.Administracion.ToString()));

    options.AddPolicy("PermisoTableroAltaDireccion", policy =>
        policy.RequireClaim(
            "Permiso",
            Permisos.TableroAltaDireccion.ToString()));

    options.AddPolicy("PermisoTableroVendedores", policy =>
        policy.RequireClaim(
            "Permiso",
            Permisos.TableroVendedores.ToString()));

    // Toda la aplicación requiere autenticación por defecto.
    options.FallbackPolicy =
        new AuthorizationPolicyBuilder()
            .RequireAuthenticatedUser()
            .Build();
});

builder.Services.Configure<AppSettings>(
    builder.Configuration.GetSection("AppSettings"));

builder.Services.AddScoped<IDbConnectionFactory, DbConnectionFactory>();

builder.Services.AddScoped<IUsuarioDAL, UsuarioDAL>();
builder.Services.AddScoped<UsuarioBLL>();

builder.Services.AddScoped<IPermisoDAL, PermisoDAL>();
builder.Services.AddScoped<PermisoBLL>();

builder.Services.AddScoped<IEmpleadoDAL, EmpleadoDAL>();
builder.Services.AddScoped<EmpleadoBLL>();

builder.Services.AddScoped<ComboDataHelper>();

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
app.UseSession();
app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.Run();

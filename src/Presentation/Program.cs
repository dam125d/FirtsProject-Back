using System.Text;
using Intap.FirstProject.Application;
using Intap.FirstProject.Infrastructure;
using Intap.FirstProject.Infrastructure.Persistence;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Scalar.AspNetCore;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.AddControllers();
builder.Services.AddOpenApi();

builder.Services.AddCors(options =>
{
    options.AddPolicy("FrontendPolicy", policy =>
        policy
            .WithOrigins(
                "http://localhost:49260",
                "https://localhost:49260",
                "http://localhost:4200",
                "https://localhost:4200")
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials());
});

string secretKey = builder.Configuration["Jwt:SecretKey"]
    ?? throw new InvalidOperationException("JWT SecretKey not configured.");

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)),
        };

        // Read JWT from cookie
        options.Events = new JwtBearerEvents
        {
            OnMessageReceived = context =>
            {
                string? token = context.Request.Cookies["intap-auth"];
                if (!string.IsNullOrEmpty(token))
                    context.Token = token;
                return Task.CompletedTask;
            },
        };
    });

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("view_users",    p => p.RequireClaim("permission", "view_users"));
    options.AddPolicy("create_users",  p => p.RequireClaim("permission", "create_users"));
    options.AddPolicy("edit_users",    p => p.RequireClaim("permission", "edit_users"));
    options.AddPolicy("delete_users",  p => p.RequireClaim("permission", "delete_users"));

    options.AddPolicy("view_projects",   p => p.RequireClaim("permission", "view_projects"));
    options.AddPolicy("create_projects", p => p.RequireClaim("permission", "create_projects"));
    options.AddPolicy("edit_projects",   p => p.RequireClaim("permission", "edit_projects"));
    options.AddPolicy("delete_projects", p => p.RequireClaim("permission", "delete_projects"));

    options.AddPolicy("view_tasks",   p => p.RequireClaim("permission", "view_tasks"));
    options.AddPolicy("create_tasks", p => p.RequireClaim("permission", "create_tasks"));
    options.AddPolicy("edit_tasks",   p => p.RequireClaim("permission", "edit_tasks"));
    options.AddPolicy("delete_tasks", p => p.RequireClaim("permission", "delete_tasks"));

    options.AddPolicy("view_roles",   p => p.RequireClaim("permission", "view_roles"));
    options.AddPolicy("create_roles", p => p.RequireClaim("permission", "create_roles"));
    options.AddPolicy("edit_roles",   p => p.RequireClaim("permission", "edit_roles"));
    options.AddPolicy("delete_roles", p => p.RequireClaim("permission", "delete_roles"));

    options.AddPolicy("view_categorias",   p => p.RequireClaim("permission", "view_categorias"));
    options.AddPolicy("create_categorias", p => p.RequireClaim("permission", "create_categorias"));
    options.AddPolicy("edit_categorias", p => p.RequireClaim("permission", "edit_categorias"));
    options.AddPolicy("delete_categorias", p => p.RequireClaim("permission", "delete_categorias"));

    options.AddPolicy("view_subcategorias", p => p.RequireClaim("permission", "view_subcategorias"));
    options.AddPolicy("create_subcategorias", p => p.RequireClaim("permission", "create_subcategorias"));
    options.AddPolicy("edit_subcategorias", p => p.RequireClaim("permission", "edit_subcategorias"));
    options.AddPolicy("delete_subcategorias", p => p.RequireClaim("permission", "delete_subcategorias"));

    options.AddPolicy("view_tipos_proyectos",   p => p.RequireClaim("permission", "view_tipos_proyectos"));
    options.AddPolicy("create_tipos_proyectos", p => p.RequireClaim("permission", "create_tipos_proyectos"));
    options.AddPolicy("edit_tipos_proyectos", p => p.RequireClaim("permission", "edit_tipos_proyectos"));
    options.AddPolicy("delete_tipos_proyectos", p => p.RequireClaim("permission", "delete_tipos_proyectos"));

    options.AddPolicy("view_tipos_tareas",   p => p.RequireClaim("permission", "view_tipos_tareas"));
    options.AddPolicy("create_tipos_tareas", p => p.RequireClaim("permission", "create_tipos_tareas"));
    options.AddPolicy("edit_tipos_tareas",   p => p.RequireClaim("permission", "edit_tipos_tareas"));
    options.AddPolicy("delete_tipos_tareas", p => p.RequireClaim("permission", "delete_tipos_tareas"));

    options.AddPolicy("empleados:read",   p => p.RequireClaim("permission", "empleados:read"));
    options.AddPolicy("empleados:create", p => p.RequireClaim("permission", "empleados:create"));
    options.AddPolicy("empleados:update", p => p.RequireClaim("permission", "empleados:update"));
    options.AddPolicy("empleados:delete", p => p.RequireClaim("permission", "empleados:delete"));
});

WebApplication app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference(options => options.WithTitle("Intap API"));
}

app.UseCors("FrontendPolicy");
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

// Seed database
await DataSeeder.SeedAsync(app.Services);

app.Run();

// Required for WebApplicationFactory<Program> in E2E tests
public partial class Program
{
    protected Program() { }
}

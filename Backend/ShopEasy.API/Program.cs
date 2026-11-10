using Microsoft.Extensions.FileProviders;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using ShopEasy.API.Middleware;
using ShopEasy.BC.Services;
using ShopEasy.BC.Settings;
using ShopEasy.DA.Data;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("ShopEasy");

if (string.IsNullOrWhiteSpace(connectionString))
{
    Console.Error.WriteLine(
        "Falta ConnectionStrings:ShopEasy. Configurala en appsettings.Development.json o en User Secrets.");
    return 1;
}

builder.Services.AgregarBaseDeDatos(connectionString);

var jwtKey = builder.Configuration["Jwt:Key"] ?? string.Empty;

if (string.IsNullOrWhiteSpace(jwtKey) || Encoding.UTF8.GetByteCount(jwtKey) < 32)
{
    Console.Error.WriteLine("Jwt:Key debe existir y tener al menos 32 caracteres.");
    return 1;
}

var jwtSettings = new JwtSettings
{
    Key = jwtKey,
    Issuer = builder.Configuration["Jwt:Issuer"] ?? "ShopEasy.API",
    Audience = builder.Configuration["Jwt:Audience"] ?? "ShopEasy.Frontend",
    ExpiresMinutes = int.TryParse(builder.Configuration["Jwt:ExpiresMinutes"], out var minutos) &&
                     minutos > 0
        ? minutos
        : 60
};

builder.Services.AddSingleton(jwtSettings);

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.MapInboundClaims = false;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = jwtSettings.Issuer,
            ValidateAudience = true,
            ValidAudience = jwtSettings.Audience,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),
            ValidateLifetime = true,
            ClockSkew = TimeSpan.FromSeconds(30),
            NameClaimType = "name",
            RoleClaimType = "role"
        };
    });

builder.Services.AddAuthorization();

builder.Services.AddCors(options =>
{
    options.AddPolicy("Frontend", policy =>
    {
        policy
            .WithOrigins(
                "http://localhost:5500",
                "http://127.0.0.1:5500",
                "http://localhost:3000",
                "http://localhost:5173")
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

builder.Services
    .AddControllers()
    .ConfigureApiBehaviorOptions(options =>
    {
        options.InvalidModelStateResponseFactory = _ =>
            new BadRequestObjectResult(new
            {
                status = StatusCodes.Status400BadRequest,
                message = "El cuerpo de la solicitud no es valido."
            });
    });

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "ShopEasy CRM API",
        Version = "v1"
    });

    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Escribe el token que devuelve /api/auth/login."
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<UserService>();
builder.Services.AddScoped<DashboardService>();

var app = builder.Build();

// Ruta física del FrontEnd
var frontendPath = Path.GetFullPath(
    Path.Combine(
        app.Environment.ContentRootPath,
        "..",
        "..",
        "FrontEnd"
    )
);

// Configurar archivos estáticos del FrontEnd
var frontendProvider = new PhysicalFileProvider(frontendPath);

// Permitir que index.html sea la página inicial
app.UseDefaultFiles(new DefaultFilesOptions
{
    FileProvider = frontendProvider
});

// Servir HTML, CSS y JavaScript
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = frontendProvider
});

app.UseMiddleware<ExceptionMiddleware>();

app.UseStatusCodePages(async context =>
{
    var response = context.HttpContext.Response;

    if (response.HasStarted || response.ContentLength > 0)
    {
        return;
    }

    var mensaje = response.StatusCode switch
    {
        StatusCodes.Status401Unauthorized => "Debes iniciar sesion o enviar un token valido.",
        StatusCodes.Status403Forbidden => "No tienes permisos para realizar esta operacion.",
        StatusCodes.Status404NotFound => "El recurso solicitado no existe.",
        StatusCodes.Status405MethodNotAllowed => "El metodo HTTP no esta permitido.",
        _ => "La solicitud no pudo ser procesada."
    };

    await response.WriteAsJsonAsync(new
    {
        status = response.StatusCode,
        message = mensaje
    });
});

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "ShopEasy CRM API v1");
    });
}
else
{
    app.UseHttpsRedirection();
}

app.UseCors("Frontend");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();

return 0;

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using rentline_backend;
using rentline_backend.Services;
using rentline_backend.Domain.Enums;
using rentline_backend.Data;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// Configure DbContext. Connection string should be defined in appsettings.json under ConnectionStrings:Default.
builder.Services.AddDbContext<RentlineDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("Default")));

// Bind JwtSettings and register token services.
builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("Jwt"));
builder.Services.AddSingleton<IJwtTokenService, JwtTokenService>();
builder.Services.AddSingleton<IPasswordHasher, BCryptPasswordHasher>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo { Title = "Rentline API", Version = "v1" });
    var securityScheme = new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Description = "JWT Authorization header using the Bearer scheme."
    };
    c.AddSecurityDefinition("Bearer", securityScheme);
    c.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        { securityScheme, new string[] {} }
    });
});

// Configure authentication using JWT.
var jwtSettings = new JwtSettings();
builder.Configuration.GetSection("Jwt").Bind(jwtSettings);
var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Key));

builder.Services
    .AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtSettings.Issuer,
            ValidAudience = jwtSettings.Audience,
            IssuerSigningKey = key
        };
    });

// Configure authorization policies.
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("OrgMember", policy => policy.RequireClaim("orgId"));
    options.AddPolicy("OwnerOrManager", policy => policy.RequireRole(
        Role.Landlord.ToString(),
        Role.AgencyAdmin.ToString(),
        Role.Manager.ToString()));
    options.AddPolicy("MaintenanceOrManager", policy => policy.RequireRole(
        Role.Maintenance.ToString(),
        Role.Landlord.ToString(),
        Role.AgencyAdmin.ToString(),
        Role.Manager.ToString()));
    options.AddPolicy("TenantOnly", policy => policy.RequireRole(Role.Tenant.ToString()));
});

builder.Services.AddCors(o =>
{
    o.AddPolicy("DevCors", p =>
        p.WithOrigins("http://localhost:5173")
         .AllowAnyHeader()
         .AllowAnyMethod()
         .AllowCredentials());
});

var app = builder.Build();

app.UseCors("DevCors");
app.UseAuthentication();

// Configure the HTTP request pipeline.
app.UseRouting();

app.Use(async (ctx, next) =>
{
    var db = ctx.RequestServices.GetRequiredService<RentlineDbContext>();
    var orgClaim = ctx.User?.Claims?.FirstOrDefault(c => c.Type == "orgId")?.Value;
    db.CurrentOrgId = Guid.TryParse(orgClaim, out var g) ? g : null;
    await next();
});


app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Rentline API v1");
});

app.UseAuthorization();

app.MapControllers();

app.Run();
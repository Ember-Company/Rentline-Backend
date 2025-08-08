
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Text;
using rentline_backend.Data;
using rentline_backend.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDbContext>(opt =>
    opt.UseNpgsql(builder.Configuration.GetConnectionString("Default")));

builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<InviteService>();
builder.Services.AddSingleton<CloudinaryService>();

builder.Services.AddControllers().AddJsonOptions(o => o.JsonSerializerOptions.ReferenceHandler =
        System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles);
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

// JWT
var jwtKey = builder.Configuration["Jwt:Key"] ?? throw new Exception("Missing Jwt:Key");
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),
            ValidateIssuer = false,
            ValidateAudience = false,
            NameClaimType = ClaimTypes.NameIdentifier,
            RoleClaimType = ClaimTypes.Role
        };
    });

// Authorization policies
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("OrgMember", p => p.RequireClaim("orgId"));
    options.AddPolicy("OwnerOrManager", p => p.RequireAssertion(ctx =>
        ctx.User.IsInRole("Landlord") || ctx.User.IsInRole("AgencyAdmin") || ctx.User.IsInRole("Manager")));
    options.AddPolicy("MaintenanceOrManager", p => p.RequireAssertion(ctx =>
        ctx.User.IsInRole("Maintenance") || ctx.User.IsInRole("Landlord") || ctx.User.IsInRole("AgencyAdmin") || ctx.User.IsInRole("Manager")));
    options.AddPolicy("TenantOnly", p => p.RequireRole("Tenant"));
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

// Set CurrentOrgId from claims per request
app.Use(async (ctx, next) =>
{
    var db = ctx.RequestServices.GetRequiredService<AppDbContext>();
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

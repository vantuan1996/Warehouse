using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;

var builder = WebApplication.CreateBuilder(args);

// 1. Load config files
builder.Configuration
        .SetBasePath(Directory.GetCurrentDirectory())
        .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
        .AddJsonFile("ocelot.json", optional: false, reloadOnChange: true)
        .AddEnvironmentVariables();

// 2. CORS Policy cho React / Frontend
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.SetIsOriginAllowed(_ => true)
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

// 3. JWT Authentication Setup tại Gateway
var secretKey = builder.Configuration["Jwt:SecretKey"] ?? "Chuoi_Secret_Key_Mac_Dinh_Sieu_Bao_Mat_123456!";
var keyBytes = Encoding.UTF8.GetBytes(secretKey);

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer("Bearer", options =>
{
    options.RequireHttpsMetadata = false;
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(keyBytes),
        ValidateIssuer = false,
        ValidateAudience = false,
        ValidateLifetime = true,
        ClockSkew = TimeSpan.FromMinutes(1)
    };

    // Tự động nhận diện Token từ Header hoặc danh sách các Cookie
    options.Events = new JwtBearerEvents
    {
        OnMessageReceived = context =>
        {
            // 1. Đọc từ Header "Authorization: Bearer <token>"
            string? authHeader = context.Request.Headers["Authorization"].FirstOrDefault();
            if (!string.IsNullOrEmpty(authHeader) && authHeader.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
            {
                context.Token = authHeader.Substring("Bearer ".Length).Trim();
                return Task.CompletedTask;
            }

            // 2. Nếu Header không có, quét qua tất cả các tên Cookie có thể có
            var possibleCookieNames = new[] { "token", "accessToken", "access_token" };
            foreach (var cookieName in possibleCookieNames)
            {
                if (context.Request.Cookies.TryGetValue(cookieName, out var cookieToken) && !string.IsNullOrEmpty(cookieToken))
                {
                    context.Token = cookieToken;
                    break; // Bắt được token rồi thì dừng vòng lặp
                }
            }

            return Task.CompletedTask;
        }
    };
});

// 4. Add Ocelot
builder.Services.AddOcelot(builder.Configuration);

var app = builder.Build();

// ⚠️ Thứ tự Middleware bắt buộc: CORS -> Auth -> Ocelot
app.UseCors("AllowAll");

app.UseAuthentication();
app.UseAuthorization();

await app.UseOcelot();

app.Run();
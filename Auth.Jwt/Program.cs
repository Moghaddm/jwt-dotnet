using System.Text;
using Auth.Jwt.Models;
using Auth.Jwt.Services;
using Auth.Jwt.Services.User;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IJwtAuthenticationManager, JwtAuthenticationManager>();
var jsonTokenConfiguration = builder.Configuration.GetSection("JWT").Get<JwtTokenConfiguration>();
builder.Services.AddSingleton(jsonTokenConfiguration!);

builder.Services
    .AddAuthentication(schemes =>
    {
        schemes.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        schemes.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        schemes.DefaultForbidScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.Audience = jsonTokenConfiguration!.Audience;
        options.RequireHttpsMetadata = true;
        options.SaveToken = true;
        options.TokenValidationParameters =
            new Microsoft.IdentityModel.Tokens.TokenValidationParameters
            {
                ValidateIssuer = false,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(
                    Encoding.ASCII.GetBytes(jsonTokenConfiguration.Secret)
                ),
                ValidAudience = jsonTokenConfiguration.Audience,
                ValidateAudience = true,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.FromMinutes(1)
            };
    });

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();


using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Net;
using System.Net.Mail;
using System.Text;
using T3awuny.Application.Contracts;
using T3awuny.Application.Helpers;
using T3awuny.Application.JwtFeatures;
using T3awuny.Application.Services;
using T3awuny.Core.Entities;
using T3awuny.Core.Repository.Contracts;
using T3awuny.Infrastructure.Data;
using T3awuny.Infrastructure.Repositories;
using T3awuny.Infrastructure.Services;
using T3awunyWebService.Helpers;

namespace T3awunyWebService
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
            #region Swagger Registration
            builder.Services.AddOpenApi();
            builder.Services.AddSwaggerGen(swagger =>
            {
                //This is to generate the Default UI of Swagger Documentation    
                swagger.SwaggerDoc("v1", new OpenApiInfo
                {
                     Version = "v1",
                     Title = "ASP.NET 9 Web API",
                     Description = " T3awuny Web Service"
                });
                // To Enable authorization using Swagger (JWT)    
                swagger.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "Enter 'Bearer' [space] and then your valid token in the text input below.\r\n\r\nExample: \"Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9\"",
                });
                swagger.AddSecurityRequirement(new OpenApiSecurityRequirement
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
                      new string[] {}
                   }
                });
            });
            //builder.Services.AddEndpointsApiExplorer();
            #endregion

            #region Register DbContext Service
            builder.Services.AddDbContext<T3awunyDbContext>(options =>
                {
                    options.UseSqlServer(builder.Configuration.GetConnectionString("MonsterConnection"));
                });
            #endregion

            #region Cors Settings
            builder.Services.AddCors(options =>
            {
               options.AddPolicy("Allow", policy =>
               {
                   policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
               });
            }); 
            #endregion

            #region Identity and Jwt registration and configuration
            #region Identity service Registriation
            builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<T3awunyDbContext>()
                .AddDefaultTokenProviders(); ;
            #endregion

            #region JWT and Google Login Configurations
            // To read JWT from appsettings for direct use
            builder.Services.Configure<JWT>(builder.Configuration.GetSection("JWT"));
            // configure JWT settings
            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme; //CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;  //GoogleDefaults.AuthenticationScheme;
            })
             .AddCookie("External", options =>
             {
                    options.Cookie.SameSite = SameSiteMode.None;
                    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
             })
               .AddGoogle(options =>
               {
                   var googleAuthSection = builder.Configuration.GetSection("Authentication:Google");
               
                   options.ClientId = googleAuthSection["ClientId"]
                       ?? throw new InvalidOperationException("Google ClientId not found.");
               
                   options.ClientSecret = googleAuthSection["ClientSecret"]
                       ?? throw new InvalidOperationException("Google ClientSecret not found.");
               
                   options.CallbackPath = "/signin-google";
                   options.SignInScheme = "External";
               })
                .AddJwtBearer(op =>
            {
                op.RequireHttpsMetadata = false;
                op.SaveToken = false;
                op.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = builder.Configuration["JWT:Issuer"],
                    ValidAudience = builder.Configuration["JWT:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JWT:Key"]!)),
                    ClockSkew = TimeSpan.Zero
                };
            });
            #endregion

            #region Register JwtHandler and RefreshTokenHandler
            builder.Services.AddSingleton<JwtHandler>();
            builder.Services.AddSingleton<RefreshTokenHandler>();
            #endregion

            #endregion

            #region Register Auth Service
            builder.Services.AddScoped<IAuthService, AuthService>();
            #endregion

            #region Register the auto mapper service
            //builder.Services.AddAutoMapper(M => M.AddProfile(new MappingProfiles()));
            //equal
            builder.Services.AddAutoMapper(typeof(MappingProfiles));
            #endregion

            #region Email Service Register and Configurations 
            builder.Services.AddScoped<IEmailService, EmailService>();

            var emailSettings = builder.Configuration.GetSection("EmailSettings");

            builder.Services
            .AddFluentEmail(emailSettings["From"])
            .AddSmtpSender(() => new SmtpClient(emailSettings["Host"])
            {
                Port = int.Parse(emailSettings["Port"]!),
                Credentials = new NetworkCredential(
                    emailSettings["Username"],
                    emailSettings["Password"]),
                EnableSsl = bool.Parse(emailSettings["EnableSsl"]!)
            });
            #endregion

            #region Register the file storage service
            var webRootPath = builder.Environment.WebRootPath    // if wwwroot exists
                              ?? builder.Environment.ContentRootPath; // fallback

            //builder.Services.AddInfrastructure(builder.Configuration, webRootPath);
            //builder.Services.AddApplication();
            builder.Services.AddScoped<IFileStorageService, LocalFileStorageService>(provider => new LocalFileStorageService(webRootPath));
            #endregion

            #region Register the generic repo service
            builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
            #endregion

            #region Register the address  service
            builder.Services.AddScoped<IAddressService, AddressService>();
            #endregion
            #region Register the NominatimGeocodingService
            //builder.Services.AddScoped<IGeocodingService, NominatimGeocodingService>();
            builder.Services.AddHttpClient<IGeocodingService, NominatimGeocodingService>();
            #endregion


            var app = builder.Build();

            #region Create Scope for app registered services and inject the T3awunyDbContext explicitly to apply any pending migrations and do data seeding for the application
            var scope = app.Services.CreateScope();
            var services = scope.ServiceProvider;
            var _dbContext = services.GetRequiredService<T3awunyDbContext>();
            var loggerFactory = services.GetRequiredService<ILoggerFactory>();
            var logger = loggerFactory.CreateLogger<Program>();
            try
            {
                _dbContext.Database.Migrate();  //update database by appliying any pending migrations if exists, if not it will do nothing
                await T3awunyContextSeed.SeedRolesAsync(_dbContext); // data seeding
                await T3awunyContextSeed.SeedAdminAsync(_dbContext); // data seeding
            }
            catch (Exception ex)
            {
                //var logger = loggerFactory.CreateLogger<Program>();
                logger.LogError(ex, "An error occured during applying the migration");
            }
            #endregion 

            // Configure the HTTP request pipeline.
            //if (app.Environment.IsDevelopment())
            //{
                //app.MapOpenApi();
                app.UseSwagger();
                app.UseSwaggerUI();
            //}

            app.UseHttpsRedirection();

            app.UseCors("Allow");
            app.UseAuthentication();
            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}

using FluentValidation.AspNetCore;
using MediatR;
using Microservices.API.Security.Aplication;
using Microservices.API.Security.Core.Dto;
using Microservices.API.Security.Core.Entities;
using Microservices.API.Security.Core.Persistence;
using Microservices.API.Security.Infrastructure;
using Microservices.API.Security.Infrastructure.JwtLogic;
using Microservices.API.Security.Middleware;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System;
using System.Text;

namespace Microservices.API.Security
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddCors(o => o.AddPolicy("corsAPP", builder =>
            {
                builder.WithOrigins("*").
                        AllowAnyMethod().
                        AllowAnyHeader();
            }));

            services.AddDbContext<SecurityDbContext>(opt =>
            {
                opt.UseSqlServer(Configuration.GetConnectionString("connectionDB"),
                    sqlServerOptionsAction: sqlOptions =>
                    {
                        sqlOptions.EnableRetryOnFailure(
                        maxRetryCount: 10,
                        maxRetryDelay: TimeSpan.FromSeconds(30),
                        errorNumbersToAdd: null
                        );
                    });
            });
            services.AddHttpContextAccessor();

            services.AddControllers(opt =>
            {
                var policy = new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build();
                opt.Filters.Add(new AuthorizeFilter(policy));

            }).AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.IgnoreNullValues = true;

            }).AddFluentValidation(x => x.RegisterValidatorsFromAssemblyContaining<Register>());

            var builder = services.AddIdentityCore<Users>();
            var identityBuilder = new IdentityBuilder(builder.UserType, builder.Services);
            identityBuilder.AddRoles<IdentityRole>();
            identityBuilder.AddClaimsPrincipalFactory<UserClaimsPrincipalFactory<Users, IdentityRole>>();
            identityBuilder.AddEntityFrameworkStores<SecurityDbContext>();
            identityBuilder.AddSignInManager<SignInManager<Users>>();
            services.AddMediatR(typeof(Register.UserRegisterCommand).Assembly);
            services.AddAutoMapper(typeof(MappingProfile));
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Microservices.API.Security", Version = "v1" });
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
                });
                c.AddSecurityRequirement(new OpenApiSecurityRequirement {
                   {
                        new OpenApiSecurityScheme{
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        }, Array.Empty<string>()
                    }

                });
            });


            services.AddScoped<IJwtGenerator, JwtGenerator>();
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("this is secret password for jwt in example microservices"));
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(opt =>
            {
                opt.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = key,
                    ValidateAudience = false,
                    ValidateIssuer = false
                };
            });
            services.AddTransient<IUserSession, UserSession>();
            services.TryAddSingleton<ISystemClock, SystemClock>();

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {

            app.UseMiddleware<HandlerErrorMiddleware>();

            if (env.IsDevelopment())
            {
                // app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Microservices.API.Security v1"));
            }

            app.UseRouting();
            app.UseCors("corsAPP");

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}

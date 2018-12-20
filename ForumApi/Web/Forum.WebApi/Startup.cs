﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Forum.Data;
using Forum.Data.Common;
using Forum.Data.Common.Interfaces;
using Forum.Data.DataTransferObjects;
using Forum.Data.Models.Users;
using Forum.Services.Data;
using Forum.Services.Data.Interfaces;
using Forum.Services.Data.Utils;
using Forum.WebApi.Middleware;
using Forum.WebApi.Utils;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Forum.WebApi
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
            services.AddDbContext<ForumContext>(options =>
            {
                options.UseSqlServer(
                    this.Configuration.GetConnectionString("DefaultConnection"));
            });

            services.AddAutoMapper(config =>
            {
                config.AddProfile(new MapInitialization());
            });

            var jwtSettingsSection = this.Configuration.GetSection("JwtSettings");

            services.Configure<JwtSettings>(jwtSettingsSection);
            services.Configure<FacebookSettings>(
                this.Configuration.GetSection("Authentication").GetSection("Facebook"));
            services.Configure<GeoLocationSettings>(
                this.Configuration.GetSection("GeoLocation"));

            var jwtSettings = jwtSettingsSection.Get<JwtSettings>();
            var key = Encoding.ASCII.GetBytes(jwtSettings.Secret);

            services
                .AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddJwtBearer(options =>
                {
                    options.RequireHttpsMetadata = false;
                    options.SaveToken = true;
                    options.TokenValidationParameters = new TokenValidationParameters()
                    {
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(key),
                        ValidateIssuer = false,
                        ValidateAudience = false
                    };
                });

            services.AddScoped(typeof(IRepository<>), typeof(Repository<>));

            services.AddScoped<IAccountService, AccountService>();
            services.AddScoped<IExternalAccountService, ExternalAccountService>();
            services.AddScoped<ICategoryService, CategoryService>();

            services.AddIdentity<User, IdentityRole>(options =>
                {
                    options.User.RequireUniqueEmail = true;
                    options.SignIn.RequireConfirmedEmail = false;
                    options.Password.RequireDigit = false;
                    options.Password.RequireLowercase = false;
                    options.Password.RequireUppercase = false;
                    options.Password.RequireNonAlphanumeric = false;
                    options.Password.RequiredUniqueChars = 0;
                    options.Password.RequiredLength = 6;
                })
                .AddEntityFrameworkStores<ForumContext>()
                .AddDefaultTokenProviders();

            services.AddCors();

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            using (var serviceScope = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                var context = serviceScope.ServiceProvider.GetService<ForumContext>();

                if (env.IsDevelopment())
                {
                    context.Database.Migrate();
                }
                var roleManager = serviceScope.ServiceProvider.GetService<RoleManager<IdentityRole>>();
                var userManager = serviceScope.ServiceProvider.GetService<UserManager<User>>();
                var accountService = serviceScope.ServiceProvider.GetService<IAccountService>();
                var logger = serviceScope.ServiceProvider.GetService<ILogger<IDatabaseInitializer>>();
                var userRepository = serviceScope.ServiceProvider.GetService<IRepository<User>>();

                new DatabaseInitiliazer().Seed(roleManager, userManager, Configuration, accountService, logger, userRepository).Wait();
            }

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            if (env.EnvironmentName == "Testing")
            {
                app.UseMiddleware<FakeRemoteIpAddressMiddleware>();
            }

            app.UseCors(builder => builder
                .AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader()
                .AllowCredentials());

            app.UseHttpsRedirection();
            app.UseMvc();
        }
    }
}

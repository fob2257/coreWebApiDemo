using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using AutoMapper;
using Swashbuckle.AspNetCore.Swagger;

using coreWebApiDemo.Models.DAL;
using coreWebApiDemo.Models.DTO;
using coreWebApiDemo.Models.DAL.Entities;
using coreWebApiDemo.Business.Services;
using coreWebApiDemo.Business.Helpers;

namespace coreWebApiDemo
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
            // DB Context
            services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));

            // Automapper Service
            services.AddAutoMapper(options =>
            {
                options.CreateMap<AuthorDTO_POST, Author>();
                options.CreateMap<AuthorDTO_PUT, Author>();
                options.CreateMap<BookDTO_POST, Book>();
            });

            // Encryptation Service
            services.AddDataProtection();

            // Swagger Service
            services.AddSwaggerGen(config =>
            {
                config.SwaggerDoc("v1", new Info
                {
                    Title = "My Web API",
                    Version = "v1",
                    Description = "This is a description"
                });

                config.SwaggerDoc("v2", new Info
                {
                    Title = "My Web API v2",
                    Version = "v2",
                    Description = "This is a description for version 2"
                });
            });

            // Caching Service
            services.AddResponseCaching();

            // Cors Service
            services.AddCors();

            // Authentication Service
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(Configuration["JWT:key"])),
                    ClockSkew = TimeSpan.Zero
                });

            // Identity Service
            services.AddIdentity<User, IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            // Services
            services.AddTransient<IDIService, DIService>();
            services.AddTransient<IHashService, HashService>();
            services.AddScoped<MyActionFilter>();
            //services.AddTransient<Microsoft.Extensions.Hosting.IHostedService, WriteToFileHostedService>();
            //services.AddTransient<Microsoft.Extensions.Hosting.IHostedService, ConsumeScopedServiceHostedService>();

            services
                .AddMvc(options =>
                {
                    options.Filters.Add(new MyExceptionFilter());
                    options.Conventions.Add(new ApiExplorerGroupPerVersionConvention());
                })
                .SetCompatibilityVersion(CompatibilityVersion.Version_2_2)
                .AddJsonOptions(options => options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            // Swagger JSON
            app.UseSwagger();

            // Swagger UI Assets
            app.UseSwaggerUI(config =>
            {
                config.SwaggerEndpoint("/swagger/v1/swagger.json", "My API v1");
                config.SwaggerEndpoint("/swagger/v2/swagger.json", "My API v2");
            });

            app.UseHttpsRedirection();

            // Caching
            app.UseResponseCaching();

            // Authentication
            app.UseAuthentication();

            // Cors
            app.UseCors(builder =>
            builder
            .AllowAnyOrigin()
            .AllowAnyHeader()
            .AllowAnyMethod());

            app.UseMvc();
        }
    }
}

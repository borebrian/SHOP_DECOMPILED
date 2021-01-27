using Lubes.DBContext;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SHOP_DECOMPILED
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
                services.AddHttpContextAccessor();
                services.AddControllersWithViews();
                services.AddControllersWithViews();
                _ = services.AddRazorPages().AddRazorRuntimeCompilation();
                services.AddSession();
                //services.AddDbContext<ApplicationDBContext>(options => options.UseMySql(absConnectionString));
                services.AddHttpContextAccessor();
                services.AddSession(options => {
                    options.IdleTimeout = TimeSpan.FromMinutes(10);

                });
                services.AddDbContext<ApplicationDBContext>(
        options => options.UseMySql(Configuration.GetConnectionString("Default")));
                services.AddTransient<MySqlConnection>(_ => new MySqlConnection(Configuration["Default"]));
                var key = Encoding.ASCII.GetBytes("YourKey-2374-OFFKDI940NG7:56753253-tyuw-5769-0921-kfirox29zoxv");
                services.AddAuthentication(x =>
                {
                    x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddJwtBearer(x =>
                {
                    x.RequireHttpsMetadata = false;
                    x.SaveToken = true;
                    x.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(key),
                        ValidateIssuer = false,
                        ValidateAudience = false,

                        RequireExpirationTime = true,
                        ValidateLifetime = true,
                        ClockSkew = TimeSpan.Zero
                    };
                });
            }

            // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
            public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
            {
                if (env.IsDevelopment())
                {
                    app.UseDeveloperExceptionPage();
                }
                else
                {
                    //app.UseExceptionHandler("/Home/Error");
                    app.UseDeveloperExceptionPage();

                    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                    app.UseHsts();
                }
                app.UseSession();
                app.Use(async (context, next) =>
                {
                    var JWToken = context.Session.GetString("JWToken");
                    if (!string.IsNullOrEmpty(JWToken))
                    {
                        context.Request.Headers.Add("Authorization", "Bearer " + JWToken);
                    }
                    await next();
                });
                app.UseHttpsRedirection();
                //redirect to login id not autenticated or invalid page..
                app.UseStatusCodePagesWithReExecute("/Log_in/Log_in", "?statusCode={0}");
                app.UseStaticFiles();
                app.UseAuthentication();

                app.UseRouting();
                app.UseAuthorization();

                app.UseEndpoints(endpoints =>
                {//si utarudisha tu
                    endpoints.MapControllerRoute(
                        name: "default",
                        pattern: "{controller=Log_in}/{action=Log_in}/{id?}");
                });
            }
        }
    }


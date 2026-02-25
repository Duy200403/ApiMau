using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Swashbuckle.AspNetCore.SwaggerGen;
using FluentValidation.AspNetCore;
using ApiWebsite.Mapping;
using ApiWebsite.Model;
using Microsoft.Extensions.FileProviders;
using ApiWebsite.Core.Base;
using System.Collections.Generic;
using ApiWebsite.Model.Interface;
using ApiWebsite.Helper.Middleware;
using System.Text.Json.Serialization;
using System.IO;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.DependencyInjection;
using ApiWebsite.Core.Services;

namespace ApiWebsite
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }
        readonly string allowSpecificOrigins = "_allowSpecificOrigins";

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddCors(options =>
             {
                 options.AddPolicy(allowSpecificOrigins, builder =>
                    builder
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .AllowAnyOrigin()
                    );
             });
            // services.AddDbContext<ApplicationDbContext>(options =>
            //     {
            //         options.UseSqlite(Configuration.GetConnectionString("DefaultConnection"));
            //     }
            // );

            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));

            services.AddStackExchangeRedisCache(options =>
            {
                options.Configuration = Configuration.GetConnectionString("RedisConnection");
            });

            services.AddControllers();
            services.AddApiVersioning(config =>
          {
              config.DefaultApiVersion = new ApiVersion(1, 0);
              config.AssumeDefaultVersionWhenUnspecified = true;
              config.ReportApiVersions = true;
          }); // đánh version cho api.

            services.AddVersionedApiExplorer(options =>
             {
                 options.GroupNameFormat = "'v'VVV";
                 options.SubstituteApiVersionInUrl = true;
             });
            services.AddControllers().AddJsonOptions(o =>
           {
               o.JsonSerializerOptions.IgnoreNullValues = true;
               o.JsonSerializerOptions.Converters.Add(new System.Text.Json.Serialization.JsonStringEnumConverter());
           }); // Enum được dùng theo kiểu string
               // nếu dùng cho 
            services.Configure<IISServerOptions>(options =>
            {
                options.MaxRequestBodySize = int.MaxValue;
            });
            services.Configure<FormOptions>(x =>
            {
                x.ValueLengthLimit = int.MaxValue;
                x.MultipartBodyLengthLimit = int.MaxValue; // if don't set default value is: 128 MB
                x.MultipartHeadersLengthLimit = int.MaxValue;
            });
            services.Configure<KestrelServerOptions>(options =>
             {
                 options.Limits.MaxRequestBodySize = int.MaxValue; // if don't set default value is: 30 MB
             });
            services.AddSwaggerGen(options => options.OperationFilter<SwaggerDefaultValues>());
            services.AddTransient<IConfigureOptions<SwaggerGenOptions>, ConfigureSwaggerOptions>();
            services.Configure<List<VirtualPathConfig>>(Configuration.GetSection(nameof(VirtualPathConfig)));
            services.Configure<JwtIssuerOptions>(Configuration.GetSection(nameof(JwtIssuerOptions)));
            services.AddScoped<IJwtIssuerOptions>(sp => sp.GetRequiredService<IOptions<JwtIssuerOptions>>().Value);
            // Adding the Unit of work to the DI container
            services.AddMvc().AddFluentValidation(fv => fv.RegisterValidatorsFromAssemblyContaining<AccountProfile>()); // cái này chỉ cần 1 cái đại diện thôi, không phải khai báo tất cả
            services.AddUnitOfWork();
            services.AddAllServices(); // khai báo các service tự động, ko cần add tay ở đây nữa
            services.AddLogging();
            services.AddAutoMapper(typeof(Startup));

            // services.AddControllers().AddJsonOptions(x =>
            //   x.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.Preserve
            //   // x.JsonSerializerOptions.ReferenceHandler = null
            //   );

            // update booking status
            services.AddCronJob<CronJobUpdateDonViDeNghiService>(c =>
           {
               c.TimeZoneInfo = TimeZoneInfo.Local;
            //    c.CronExpression = @"*/15 * * * *";
               c.CronExpression = @"* * * * *";
           });
              
              services.AddControllers()
                .AddNewtonsoftJson(options =>
                {
                    // cái này để lúc return ra có cả nested data
                    options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Serialize;
                    // options.SerializerSettings.PreserveReferencesHandling = Newtonsoft.Json.PreserveReferencesHandling.Objects;
                    // Add StringEnumConverter to the Converters collection
                    options.SerializerSettings.Converters.Add(new Newtonsoft.Json.Converters.StringEnumConverter());
                });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IApiVersionDescriptionProvider provider, ApplicationDbContext dataContext)
        {
            dataContext.Database.Migrate();

            // if (env.IsDevelopment())
            // {
            app.UseDeveloperExceptionPage();
            app.UseSwagger();
            app.UseSwaggerUI(
            options =>
            {
                // build a swagger endpoint for each discovered API version  
                foreach (var description in provider.ApiVersionDescriptions)
                {
                    options.SwaggerEndpoint($"/swagger/{description.GroupName}/swagger.json", description.GroupName.ToUpperInvariant());
                }
            });
            // }

            // app.UseHttpsRedirection();
            app.UseCors(allowSpecificOrigins);
            app.UseRouting();
            app.UseMiddleware<JwtMiddleware>();
            app.UseAuthorization();

            // app.UseEndpoints(endpoints =>
            // {
            //   endpoints.MapControllers();
            // });
            app.UseEndpoints(endpoints =>
                 {
                     endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller}/{action=Index}/{id?}");
                 });
            // khi nao dung thi bo comment liên quan đến upload file, image
            var vitualPath = Configuration.GetSection(nameof(VirtualPathConfig)).Get<List<VirtualPathConfig>>();
            string path = System.IO.Directory.GetCurrentDirectory();
            vitualPath.ForEach(f =>
            {
                app.UseStaticFiles(new StaticFileOptions()
                {
                    FileProvider = new PhysicalFileProvider(path + "\\" + f.RealPath),
                    RequestPath = f.RequestPath
                });
            });

            var task = Task.Run(async () => await Core.Services.SeedDataService.SeedAccount(app.ApplicationServices));
        }

    }
}
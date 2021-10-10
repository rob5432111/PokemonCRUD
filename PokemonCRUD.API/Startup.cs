using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using PokemonCRUD.Core.Interfaces;
using PokemonCRUD.Core.Models;
using PokemonCRUD.Repository.Data;
using PokemonCRUD.Services;
using System;
using Serilog;
using FluentValidation;
using FluentValidation.AspNetCore;
using PokemonCRUD.Core.Validators;

namespace PokemonCRUD.API
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }       

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            var appSettings = new AppSettings();
            Configuration.Bind(appSettings);

            //Setting dependency injection configurations
            services.Configure<AppSettings>(Configuration);


            services.AddControllers();

            //Add logging
            services.AddLogging(loggingBuilder =>
                loggingBuilder.AddSerilog(dispose: true));
         
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Version = "v1",
                    Title = "Pokemon API",
                    Description = "CRUD Operations for a Pokemon DB",
                    Contact = new OpenApiContact
                    {
                        Name = "Roberto Pozo",
                        Email = "roberto.pozo.andrade@gmail.com",
                        Url = new Uri("https://github.com/rob5432111")
                    }
                });
            });
            services.AddMvc()
                .AddFluentValidation();

            //Scoped services
            services.AddScoped<IPokemonRepository, PokemonRepository>();

            //Transient services
            services.AddTransient<IPokemonService, PokemonService>();
            services.AddTransient<IConfigurationService, ConfigurationService>();
            services.AddTransient<IValidator<Pokemon>, PokemonValidator>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseSwagger();

            app.UseSwaggerUI();

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

        }
    }
}

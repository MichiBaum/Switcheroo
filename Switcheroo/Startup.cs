﻿using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Switcheroo {
    public class Startup {
        
        public IServiceProvider ServiceProvider { get; }
        public IConfiguration Configuration { get; }
        
        public Startup(IConfiguration configuration, IServiceProvider serviceProvider) {
            Configuration = configuration;
            ServiceProvider = serviceProvider;
        }


        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddTransient(typeof(MainWindow));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app)
        {
            var mainWindow = ServiceProvider.GetRequiredService<MainWindow>();
            mainWindow.Show();
        }
                
    }
}
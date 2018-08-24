using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Storage.Database;
using Storage.Mempool;
using Swashbuckle.AspNetCore.Swagger;

namespace StorageRest
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
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
            
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Info { Title = "My API", Version = "v1" });
            });
            
            var conectionString = Configuration.GetConnectionString("DefaultConnection");
            services.AddSingleton(
                new AppointmentToTheDoctorRepository(conectionString));
            services.AddSingleton(
                new BlockRepository(conectionString));
            services.AddSingleton(
                new CoinTransactionRepository(conectionString));
            services.AddSingleton(
                new MempoolRepository(conectionString));

            services.AddSingleton(provider =>
                new Mempool(
                    provider.GetService<MempoolRepository>(),
                    provider.GetService<BlockRepository>(),
                    Helper.UTXOs,
                    provider.GetService<AppointmentToTheDoctorRepository>(),
                    provider.GetService<CoinTransactionRepository>()     
                ));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
            });
            app.UseMvc();
        }
    }
}
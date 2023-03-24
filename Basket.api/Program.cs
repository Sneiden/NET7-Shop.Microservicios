
using Basket.api.Repositories;
using Inventory.grpc.Protos;

namespace Basket.api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            builder.Services.AddStackExchangeRedisCache(options=>
            {
                options.Configuration = builder.Configuration.GetValue<string>("CacheSettings:ConnectionString");
            });

            builder.Services.AddScoped<IBasketRepository, BasketRepository>();

            builder.Services.AddGrpcClient<ExistanceService.ExistanceServiceClient>
                (o => o.Address = new Uri(builder.Configuration["GrpcSettings:HostAddress"]!));

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
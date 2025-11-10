using NotificationService.Infrastructure;
using NotificationService.Services;

namespace Notification_Service
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Configure Kafka settings
            builder.Services.Configure<KafkaSettings>(builder.Configuration.GetSection("Kafka"));

            // Add services to the container.
            builder.Services.AddScoped<INotificationService, EmailNotificationService>();
            builder.Services.AddHostedService<KafkaConsumerService>();
            
            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

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

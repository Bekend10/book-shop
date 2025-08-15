using Microsoft.AspNetCore.SignalR;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using realtime_service.Hubs;
using realtime_service.Models;
using realtime_service.Services.Interfaces;
using System.Text;
using System.Text.Json;

namespace realtime_service.Consumers
{
    public class NotificationConsumer : BackgroundService
    {
        private readonly IHubContext<NotificationHub> _hubContext;
        private readonly ConnectionFactory _factory;
        private IConnection? _connection;
        private IModel? _channel;
        private readonly string _queueName = "BookShop_SendOrderNotification";
        private readonly IServiceProvider _serviceProvider;

        public NotificationConsumer(IConfiguration config,
                                     IHubContext<NotificationHub> hubContext,
                                     IServiceProvider serviceProvider)
        {
            _hubContext = hubContext;
            _factory = new ConnectionFactory()
            {
                HostName = config["RabbitMQ:HostName"],
                Port = int.Parse(config["RabbitMQ:Port"]),
                UserName = config["RabbitMQ:UserName"],
                Password = config["RabbitMQ:Password"]
            };
            _serviceProvider = serviceProvider;
        }

        public override Task StartAsync(CancellationToken cancellationToken)
        {
            _connection = _factory.CreateConnection();
            _channel = _connection.CreateModel();
            _channel.QueueDeclare(queue: _queueName, durable: true, exclusive: false, autoDelete: false);
            return base.StartAsync(cancellationToken);
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var consumer = new EventingBasicConsumer(_channel);
            consumer.Received += async (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var msg = Encoding.UTF8.GetString(body);
                var data = JsonSerializer.Deserialize<AdminNotificationModelCommand>(msg);

                if (data != null)
                {
                    // Mở scope để lấy service scoped
                    using (var scope = _serviceProvider.CreateScope())
                    {
                        var notificationService = scope.ServiceProvider.GetRequiredService<INotificationService>();
                        await notificationService.AddNotificationAsync(data.receiver_user_id, data.message_content);
                    }

                    // Gửi SignalR
                    await _hubContext.Clients.User(data.receiver_user_id.ToString())
                        .SendAsync("ReceiveOrderNotification", data.message_content);
                }
            };

            _channel.BasicConsume(queue: _queueName, autoAck: true, consumer: consumer);

            return Task.CompletedTask;
        }
    }
}

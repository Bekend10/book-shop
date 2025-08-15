using hangfire_service.Model;
using hangfire_service.Repositories.Interfaces;
using RabbitMQ.Client;
using System.Text;
using System.Text.Json;

namespace hangfire_service.Repositories.Implements
{
    public class NotificationRepository : INotificationRepository
    {
        private readonly ConnectionFactory _factory;
        private readonly string _queueName = "BookShop_SendOrderNotification";

        public NotificationRepository(IConfiguration config)
        {
            _factory = new ConnectionFactory()
            {
                HostName = config["RabbitMQ:HostName"],
                Port = int.Parse(config["RabbitMQ:Port"]),
                UserName = config["RabbitMQ:UserName"],
                Password = config["RabbitMQ:Password"]
            };
        }

        public void QueueNotification(int orderId, string message, int receiverUserId)
        {
            using var connection = _factory.CreateConnection();
            using var channel = connection.CreateModel();

            channel.QueueDeclare(queue: _queueName,
                                 durable: true,
                                 exclusive: false,
                                 autoDelete: false,
                                 arguments: null);

            var payload = new AdminNotificationModelCommand
            {
                order_id = orderId,
                message_content = message,
                receiver_user_id = receiverUserId
            };

            var body = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(payload));

            channel.BasicPublish(exchange: "",
                                 routingKey: _queueName,
                                 basicProperties: null,
                                 body: body);
        }
    }
}

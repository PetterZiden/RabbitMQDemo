using RabbitMQ.Client;
using System;
using System.Text;
using RabbitMQ.Client.Events;
using Newtonsoft.Json;
using RabbitMqProducerV2.Models;

namespace RabbitMqConsumerAlbum
{
    public class RabbitMqConsumerAlbum
    {
        public static void Main()
        {
            var factory = new ConnectionFactory();
            using var connection = factory.CreateConnection();
            using var channel = connection.CreateModel();
            {
                channel.ExchangeDeclare("ex", ExchangeType.Direct);

                var queueName = channel.QueueDeclare().QueueName;

                channel.QueueBind(queueName, "ex", "album");

                Console.WriteLine($"Waiting for messages.");

                var consumer = new EventingBasicConsumer(channel);
                consumer.Received += (model, ea) =>
                {
                    var message = ByteToAlbumConverter(ea.Body);
                    var routingKey = ea.RoutingKey;

                    Console.WriteLine($"Received album: {message.Name} with routing key: {routingKey} ");
                };

                channel.BasicConsume(queueName, true, consumer);

                Console.WriteLine(" Press [enter] to exit.");
                Console.ReadLine();
            }
        }

        private static Album ByteToAlbumConverter(byte[] body)
        {
            var tempString = Encoding.UTF8.GetString(body);
            var artist = JsonConvert.DeserializeObject<Album>(tempString);
            return artist;
        }
    }
}

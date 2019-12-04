using RabbitMQ.Client;

using System;
using System.Runtime.InteropServices;
using System.Text;

using Newtonsoft.Json;

using RabbitMQ.Client.Events;
using RabbitMqProducerV2.Models;

namespace RabbitMqConsumerArtist
{
    public class RabbitMqConsumerArtist
    {
        public static void Main()
        {
            var factory = new ConnectionFactory();
            using var connection = factory.CreateConnection();
            using var channel = connection.CreateModel();
            {
                channel.ExchangeDeclare("ex", ExchangeType.Direct);

                var queueName = channel.QueueDeclare().QueueName;

                channel.QueueBind(queueName, "ex", "artist");

                Console.WriteLine($"Waiting for messages.");

                var consumer = new EventingBasicConsumer(channel);
                consumer.Received += (model, ea) =>
                {
                    var message = ByteToArtistConverter(ea.Body);
                    var routingKey = ea.RoutingKey;

                    Console.WriteLine($"Received artist: {message.Name} with routing key: {routingKey} ");
                };

                channel.BasicConsume(queueName, true, consumer);

                Console.WriteLine(" Press [enter] to exit.");
                Console.ReadLine();
            }
        }

        private static Artist ByteToArtistConverter(byte[] body)
        {
            var tempString = Encoding.UTF8.GetString(body);
            var artist = JsonConvert.DeserializeObject<Artist>(tempString);
            return artist;
        }
    }
}

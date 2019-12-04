using System;
using System.Text;

using Newtonsoft.Json;

using RabbitMQ.Client;
using RabbitMQ.Client.Events;

using RabbitMqProducer.Models;

namespace RabbitMqConsumer
{
    public class RabbitMqConsumer
    {
        public static void Main(string[] args)
        {
            var factory = new ConnectionFactory();
            using var connection = factory.CreateConnection();
            using var channel = connection.CreateModel();
            channel.ExchangeDeclare("artist", ExchangeType.Fanout);

            var queueName = channel.QueueDeclare().QueueName;
            channel.QueueBind(queueName, "artist", "");

            Console.WriteLine($"Waiting for artist");

            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += (model, ea) =>
            {
                var artist = ByteToArtistConverter(ea.Body);

                Console.WriteLine($"Received // Name: {artist.Name}   Date of birth: {artist.DateOfBirth}");
            };

            channel.BasicConsume(queueName, true, consumer);

            Console.WriteLine(" Press [enter] to exit.");
            Console.ReadLine();
        }

        private static Artist ByteToArtistConverter(byte[] body)
        {
            var tempString = Encoding.UTF8.GetString(body);
            var artist = JsonConvert.DeserializeObject<Artist>(tempString);
            return artist;
        }
    }
}

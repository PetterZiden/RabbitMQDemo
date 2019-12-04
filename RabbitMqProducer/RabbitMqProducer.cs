using RabbitMQ.Client;

using System;
using System.Collections.Generic;
using System.Text;

using RabbitMqProducer.Models;

using Newtonsoft.Json;

namespace RabbitMqProducer
{
    public class RabbitMqProducer
    {
        public static void Main(string[] args)
        {
            var factory = new ConnectionFactory();
            using var connection = factory.CreateConnection();
            using var channel = connection.CreateModel();
            {
                channel.ExchangeDeclare("artist", ExchangeType.Fanout);

                var artist = GetArtist();
                var body = ArtistToByteConverter(artist);
                channel.BasicPublish("artist", "", null, body);

                Console.WriteLine($"Sent // Name: {artist.Name}   Date of birth: {artist.DateOfBirth}");
            }

            Console.WriteLine(" Press [enter] to exit.");
            Console.ReadLine();
        }

        private static byte[] ArtistToByteConverter(Artist artist)
        {
            var tempString = JsonConvert.SerializeObject(artist);
            var response = Encoding.UTF8.GetBytes(tempString);
            return response;
        }

        private static Artist GetArtist()
        {
            var random = new Random().Next(1, 3);

            var artists = new List<Artist>
            {
                new Artist
                {
                    Name = "Cage",
                    DateOfBirth = "1978-03-08"
                },
                new Artist
                {
                    Name = "Copywrite",
                    DateOfBirth = "1983-10-15"
                },
                new Artist
                {
                    Name = "Nas",
                    DateOfBirth = "1962-08-08"
                }
            };

            return artists[random];
        }
    }
}

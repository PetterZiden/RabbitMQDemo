using RabbitMQ.Client;

using System;
using System.Collections.Generic;
using System.Text;

using RabbitMqProducerV2.Models;

using Newtonsoft.Json;

namespace RabbitMqProducerV2
{
    public class RabbitMqProducerV2
    {
        public static void Main()
        {
            var factory = new ConnectionFactory();
            using var connection = factory.CreateConnection();
            using var channel = connection.CreateModel();
            {
                channel.ExchangeDeclare("ex", ExchangeType.Direct);

                var artist = GetArtist();
                var album = GetAlbum();

                var artistBody = ArtistToByteConverter(artist);
                var albumBody = AlbumToByteConverter(album);

                channel.BasicPublish("ex", "artist", null, artistBody);
                channel.BasicPublish("ex", "album", null, albumBody);

                Console.WriteLine($"Sent artist {artist.Name} to artist queue ");
                Console.WriteLine($"Sent album {album.Name} to album queue ");

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

        private static byte[] AlbumToByteConverter(Album album)
        {
            var tempString = JsonConvert.SerializeObject(album);
            var response = Encoding.UTF8.GetBytes(tempString);
            return response;
        }

        private static Artist GetArtist()
        {
            var random = new Random().Next(1, 4);

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

        private static Album GetAlbum()
        {
            var random = new Random().Next(1, 4);

            var albums = new List<Album>
            {
                new Album
                {
                    Name = "Hells winter",
                    ReleaseDate = "2005-10-16",
                    Genre = "Hiphop"
                },
                new Album
                {
                    Name = "Kill the architect",
                    ReleaseDate = "2013-02-22",
                    Genre = "Hiphop"
                },
                new Album
                {
                    Name = "King is back",
                    ReleaseDate = "2010-09-15",
                    Genre = "Pop"
                }
            };

            return albums[random];
        }
    }
}

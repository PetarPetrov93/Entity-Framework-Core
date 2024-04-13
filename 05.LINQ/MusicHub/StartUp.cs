namespace MusicHub
{
    using System;
    using System.Linq;
    using System.Text;
    using Data;
    using Initializer;

    public class StartUp
    {
        public static void Main()
        {
            MusicHubDbContext context =
                new MusicHubDbContext();

            DbInitializer.ResetDatabase(context);

            //string albumsInfo = ExportAlbumsInfo(context, 4);
            //Console.WriteLine(albumsInfo);

            string songsAboveDuration = ExportSongsAboveDuration(context, 4);
            Console.WriteLine(songsAboveDuration);
        }

        public static string ExportAlbumsInfo(MusicHubDbContext context, int producerId)
        {
            var albums = context.Producers
                        .FirstOrDefault(p => p.Id == producerId)
                        .Albums
                        .OrderByDescending(a => a.Price)
                        .Select(a => new
                        {
                            AlbumName = a.Name,
                            ReleaseDate = a.ReleaseDate.ToString("MM/dd/yyyy"),
                            ProducerName = a.Producer.Name,
                            AlbumSongs = a.Songs
                                .Select(s => new
                                {
                                    SongName = s.Name,
                                    Price = s.Price.ToString("f2"),
                                    WriterName = s.Writer.Name
                                })
                                .OrderByDescending(s => s.SongName)
                                .ThenBy(s => s.WriterName)
                                .ToArray(),
                            AlbumPrice = a.Price.ToString("f2")
                        })
                        .ToArray();

            StringBuilder sb = new StringBuilder();

            foreach (var a in albums)
            {
                int songCounter = 1;
                sb.AppendLine($"-AlbumName: {a.AlbumName}");
                sb.AppendLine($"-ReleaseDate: {a.ReleaseDate}");
                sb.AppendLine($"-ProducerName: {a.ProducerName}");
                sb.AppendLine($"-Songs:");

                foreach (var s in a.AlbumSongs)
                {
                    sb.AppendLine($"---#{songCounter}");
                    sb.AppendLine($"---SongName: {s.SongName}");
                    sb.AppendLine($"---Price: {s.Price}");
                    sb.AppendLine($"---Writer: {s.WriterName}");
                    songCounter++;
                }
                sb.AppendLine($"-AlbumPrice: {a.AlbumPrice}");
            }

            return sb.ToString().TrimEnd();
        }

        public static string ExportSongsAboveDuration(MusicHubDbContext context, int duration)
        {
            var songs = context.Songs
                        .ToArray() // This is because of the TotalSeconds because it cannot be performed in SQL. AsEnumarable() can be also used instead of ToArray()
                        .Where(s => s.Duration.TotalSeconds > duration)
                        .Select(s => new
                        {
                            s.Name,
                            SongPerformers = s.SongPerformers
                                            .Select(sp => $"{sp.Performer.FirstName} {sp.Performer.LastName}")
                                            .OrderBy(p => p)
                                            .ToArray(),
                            WriterName = s.Writer.Name,
                            AlbumProducer = s.Album!.Producer!.Name,
                            Duration = s.Duration.ToString("c")
                        })
                        .OrderBy(s => s.Name)
                        .ThenBy(s => s.WriterName)
                        .ToArray();

            StringBuilder sb = new StringBuilder();
            int counter = 1;

            foreach (var s in songs)
            {
                sb.AppendLine($"-Song #{counter}");
                sb.AppendLine($"---SongName: {s.Name}");
                sb.AppendLine($"---Writer: {s.WriterName}");

                foreach (var perfName in s.SongPerformers)
                {
                    sb.AppendLine($"---Performer: {perfName}");
                }
                sb.AppendLine($"---AlbumProducer: {s.AlbumProducer}");
                sb.AppendLine($"---Duration: {s.Duration}");
                counter++;
            }

            return sb.ToString().TrimEnd();
        }
    }
}

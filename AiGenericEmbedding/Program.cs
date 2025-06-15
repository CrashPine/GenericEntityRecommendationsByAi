using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AiGenericEmbedding;
using Microsoft.Extensions.VectorData;

namespace AiGenericEmbedding;

    class Program
    {
        static async Task Main(string[] args)
        {
            var rawMovies = new List<Movie>
            {
                new Movie { Id = 1, Title = "Inception",  Description = "A thief who steals corporate secrets through dream-sharing technology." },
                new Movie { Id = 2, Title = "The Matrix", Description = "A hacker learns the true nature of his reality." },
                new Movie { Id = 3, Title = "Interstellar",Description = "Explorers travel through a wormhole in space to save humanity." },
                new Movie { Id = 4, Title = "The Prestige",Description = "Two magicians battle to create the ultimate illusion." },
                new Movie { Id = 5, Title = "Memento",    Description = "A man with short-term memory loss hunts his wife's killer." }
            };

            var adapters = rawMovies
                .Select(m => new VectorAdapter<int, Movie>(m, mv => mv.Id, mv => mv.Title, mv => mv.Description))
                .ToList();

            var uri   = new Uri("http://192.168.130.86:44444");
            var model = "nomic-embed-text:v1.5";

            var itemRec = new AiRecommendations<int, VectorAdapter<int, Movie>>(adapters, uri, model);
            var target  = adapters.First();
            Console.WriteLine($"Target: {target.Entity.Title}\n");

            var similar = await itemRec.GetSimilarItemsAsync(target, 3);
            Console.WriteLine("Similar items:");
            similar.ForEach(a => Console.WriteLine($"- {((VectorAdapter<int, Movie>)a).Entity.Title}"));

            Console.WriteLine("\n---\n");

            var queryRec = new AiQueryRecommendations<int, VectorAdapter<int, Movie>>(adapters, uri, model);
            var query    = "dream sharing and corporate espionage";
            Console.WriteLine($"Query: \"{query}\"\n");

            var results = await queryRec.GetByQueryAsync(query, 3);
            Console.WriteLine("Results:");
            results.ForEach(a => Console.WriteLine($"- {((VectorAdapter<int, Movie>)a).Entity.Title}"));

            Console.WriteLine("\nPress any key to exit...");
            Console.ReadKey();
        }
    }
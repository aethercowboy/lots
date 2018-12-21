using lots.BusinessLogic.Interfaces;
using lots.BusinessLogic.Services;
using lots.Domain.Interfaces;
using lots.Domain.Models;
using lots.Domain.Services;
using McMaster.Extensions.CommandLineUtils;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace lots
{
    public class Program
    {
        [Option(ShortName = "p", Description = "Path to the resource.")]
        public string Path { get; }

        [Option(ShortName = "l", Description = "Limit the number of results")]
        public int Limit { get; }

        public static void Main(string[] args) => CommandLineApplication.Execute<Program>(args);

        private IConsoleService ConsoleService { get; set; }

        private void OnExecute()
        {
            var serviceProvider = InitializeServiceCollection();

            ConsoleService = serviceProvider.GetService<IConsoleService>();

            var ratingService = serviceProvider.GetService<IRatingService>();

            var items = JsonConvert.DeserializeObject<IEnumerable<MeasuredItem>>(File.ReadAllText(Path));

            var scores = ratingService.Rate(items, Limit, Prompt)
                .ToList();

            ConsoleService.WriteMessage("The Results:");
            foreach (var s in scores)
            {
                ConsoleService.WriteMessage(s.ToString());
            }
        }

        private int Prompt(params MeasuredItem[] measuredItems)
        {
            ConsoleService.WriteMessage("Select One:");
            foreach (var m in measuredItems)
            {
                ConsoleService.WriteMessage(m.ToString());
            }

            ConsoleService.Write("> ");
            var response = ConsoleService.ReadLine();

            if (int.TryParse(response, out var val))
            {
                if (measuredItems.Any(x => x.Id == val))
                    return val;
            }

            ConsoleService.WriteError($"I'm sorry, {response} is not an option.");

            return Prompt(measuredItems);
        }

        private IServiceProvider InitializeServiceCollection()
        {
            var services = new ServiceCollection();

            services.AddScoped<IRatingService, RatingService>();
            services.AddScoped<IConsoleService, ConsoleService>();

            return services.BuildServiceProvider();
        }
    }
}
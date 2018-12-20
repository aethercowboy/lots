using lots.BusinessLogic.Interfaces;
using lots.BusinessLogic.Services;
using lots.Domain.Interfaces;
using lots.Domain.Models;
using lots.Domain.Services;
using McMaster.Extensions.CommandLineUtils;
using Microsoft.Extensions.DependencyInjection;
using System;
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

            var scores = ratingService.Rate(Path, Limit, Prompt)
                .ToList();

            ConsoleService.WriteMessage("The Results:");
            foreach (var s in scores)
            {
                ConsoleService.WriteMessage(s.ToString());
            }
        }

        private int Prompt(MeasuredItem a, MeasuredItem b)
        {
            ConsoleService.WriteMessage("Select One:");
            ConsoleService.WriteMessage(a.ToString());
            ConsoleService.WriteMessage(b.ToString());
            ConsoleService.Write("> ");
            var response = ConsoleService.ReadLine();

            if (int.TryParse(response, out var val))
            {
                if (a.Id == val || b.Id == val)
                    return val;
            }

            ConsoleService.WriteError($"I'm sorry, {response} is not an option.");

            return Prompt(a, b);
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
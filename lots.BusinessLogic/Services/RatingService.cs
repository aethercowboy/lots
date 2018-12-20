using lots.BusinessLogic.Interfaces;
using lots.Domain.Interfaces;
using lots.Domain.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace lots.BusinessLogic.Services
{
    public class RatingService : IRatingService
    {
        private readonly IConsoleService _consoleService;

        public RatingService(IConsoleService consoleService)
        {
            _consoleService = consoleService;
        }

        public IEnumerable<MeasuredItem> Rate(string path, int limit, Func<MeasuredItem, MeasuredItem, int> selectFunc)
        {
            var items = JsonConvert.DeserializeObject<IEnumerable<MeasuredItem>>(File.ReadAllText(path))
                .OrderByDescending(x => x.Score)
                .ToList();

            var itemDict = items.ToDictionary(x => x.Id, x => x);

            var groupedItems = items.GroupBy(x => x.Score);

            var scores = new Dictionary<int, decimal>();

            foreach (var group in groupedItems)
            {
                if (scores.Count >= limit) break;

                var count = group.Count();

                for (var i = 0; i < count; ++i)
                {
                    var a = group.ElementAt(i);

                    for (var j = i + 1; j < count; ++j)
                    {
                        var b = group.ElementAt(j);

                        var toCheck = new List<int> { a.Id, b.Id };

                        var result = selectFunc(a, b);

                        if (!toCheck.Any(x => x == result))
                        {
                            _consoleService.WriteError($"I'm sorry, {result} is not an option.");

                            // redo
                            --j;
                            continue;
                        }

                        foreach (var c in toCheck)
                        {
                            var item = itemDict[c];

                            AddOrUpdateScore(scores, item, result == c ? 1 : -1);
                        }
                    }
                }
            }

            var taken = 0;

            foreach (var group in groupedItems) {
                if (taken >= limit) yield break;

                var ids = group.Select(x => x.Id);

                foreach (var score in scores.Where(x => ids.Contains(x.Key)).OrderByDescending(x => x.Value).Select(x => itemDict[x.Key]))
                {
                    if (taken >= limit) yield break;

                    yield return score;
                    ++taken;
                }
            }
        }

        private void AddOrUpdateScore(Dictionary<int, decimal> dict, MeasuredItem item, decimal score)
        {
            if (dict.ContainsKey(item.Id))
            {
                dict[item.Id] += score;
            }
            else
            {
                dict.Add(item.Id, score);
            }
        }
    }
}

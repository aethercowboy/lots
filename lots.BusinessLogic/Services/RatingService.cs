using lots.BusinessLogic.Extensions;
using lots.BusinessLogic.Interfaces;
using lots.Domain.Interfaces;
using lots.Domain.Models;
using System;
using System.Collections.Generic;
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

        public IEnumerable<MeasuredItem> Rate(IEnumerable<MeasuredItem> items, int limit, Func<MeasuredItem[], int> selectFunc)
        {
            var sortedItems = items.OrderByDescending(x => x.Score)
                .ToList()
                ;

            var itemDict = items.ToDictionary(x => x.Id, x => x);

            var groupedItems = items.GroupBy(x => x.Score);

            var scores = new Dictionary<int, decimal>();

            void ProcessBrackets(IEnumerable<MeasuredItem> source)
            {
                var brackets = source.Bracketify().ToList(); // todo - get size of array from selectFunc to determine bracket size

                var winners = new List<MeasuredItem>();

                foreach (var pair in brackets)
                {
                    var result =
                        pair.Count() == 1
                        ? pair.First().Id
                        : selectFunc(pair.ToArray())
                        ;

                    var toCheck = pair.Select(x => x.Id);

                    if (!toCheck.Any(x => x == result))
                    {
                        _consoleService.WriteError($"I'm sorry, {result} is not an option.");
                        winners.AddRange(pair);
                    }
                    else
                    {
                        foreach (var c in toCheck)
                        {
                            var item = itemDict[c];

                            var score = 0;
                            if (result == c)
                            {
                                winners.Add(item);
                                score = 1;
                            }
                            else
                            {
                                score = -1;
                            }

                            AddOrUpdateScore(scores, item, score);
                        }
                    }
                }

                if (winners.Count() > 1)
                {
                    ProcessBrackets(winners);
                }
            }

            foreach (var grouping in groupedItems)
            {
                ProcessBrackets(grouping);
            }

            var taken = 0;

            foreach (var group in groupedItems)
            {
                if (taken >= limit)
                {
                    yield break;
                }

                var ids = group.Select(x => x.Id);

                //var ranked = scores.Where(x => ids.Contains(x.Key))
                //    .OrderByDescending(x => x.Value)
                //    .Select(x => itemDict[x.Key])
                //    ;
                var ranked = items.OrderByDescending(x => x.Score)
                    .ThenByDescending(x => x.Rating)
                    ;

                foreach (var score in ranked)
                {
                    if (taken >= limit)
                    {
                        yield break;
                    }

                    yield return score;
                    ++taken;
                }
            }
        }

        private void AddOrUpdateScore(Dictionary<int, decimal> dict, MeasuredItem item, decimal score)
        {
            if (dict.ContainsKey(item.Id))
            {
                item.Rating += score;
                dict[item.Id] += score;
            }
            else
            {
                item.Rating += score;
                dict.Add(item.Id, score);
            }
        }
    }
}

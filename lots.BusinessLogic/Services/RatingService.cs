﻿using lots.BusinessLogic.Extensions;
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

        public IEnumerable<MeasuredItem> Rate(IEnumerable<MeasuredItem> items, int? limit, Func<MeasuredItem[], int> selectFunc)
        {
            var limit2 = limit.GetValueOrDefault(items.Count());

            var sortedItems = items.OrderByDescending(x => x.Score)
                .ToList()
                ;

            var itemDict = items.ToDictionary(x => x.Id, x => x);

            var groupedItems = items.GroupBy(x => x.Score);

            void ProcessBrackets(IEnumerable<MeasuredItem> source)
            {
                var brackets = source.Bracketify()
                    .ToList(); // todo - get size of array from selectFunc to determine bracket size

                var winners = new List<MeasuredItem>();

                foreach (var pair in brackets)
                {
                    var orderedPair = pair.OrderBy(x => x.Id)
                        .ToList();

                    var result =
                        orderedPair.Count() == 1
                        ? orderedPair.First().Id
                        : selectFunc(orderedPair.ToArray())
                        ;

                    var toCheck = orderedPair.Select(x => x.Id);

                    if (!toCheck.Any(x => x == result))
                    {
                        _consoleService.WriteError($"I'm sorry, {result} is not an option.");
                        winners.AddRange(orderedPair);
                    }
                    else
                    {
                        foreach (var c in toCheck)
                        {
                            var item = itemDict[c];

                            var score = toCheck.Count() - 1; 
                            if (result == c)
                            {
                                winners.Add(item);
                            }
                            else
                            {
                                score = 0;
                            }

                            AddOrUpdateScore(item, score);
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

            var haveTies = true;

            while (haveTies)
            {
                var check = items.OrderByDescending(x => x.Score)
                    .ThenBy(x => x.Rating)
                    .Take(limit2 + 1)
                    ;

                var ties = check.GroupBy(x => new { x.Score, x.Rating }).Where(x => x.Count() > 1)
                    .ToList();

                haveTies = ties.Any();

                foreach (var t in ties)
                {
                    ProcessBrackets(t);
                }
            }

            var taken = 0;

            foreach (var group in groupedItems)
            {
                if (taken >= limit)
                {
                    yield break;
                }

                var ids = group.Select(x => x.Id);

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

        private void AddOrUpdateScore(MeasuredItem item, decimal score)
        {
            item.Rating += score;
        }
    }
}
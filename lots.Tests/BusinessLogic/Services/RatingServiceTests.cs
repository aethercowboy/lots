using lots.BusinessLogic.Services;
using lots.Domain.Models;
using Moq.AutoMock;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace lots.Tests.BusinessLogic.Services
{
    public class RatingServiceTests
    {
        private readonly AutoMocker _mocker;

        public RatingServiceTests()
        {
            _mocker = new AutoMocker();
        }

        private int RateByLength(params MeasuredItem[] items)
        {
            return items.OrderByDescending(x => x.Name.Length)
                .First()
                .Id;
        }

        [Fact]
        public void Rate_ByLengthSimple()
        {
            var data = new List<MeasuredItem>()
            {
                new MeasuredItem()
                {
                    Id = 1,
                    Name = "A",
                    Description = "Shortest Name",
                    Score = 0
                },
                new MeasuredItem()
                {
                    Id = 2,
                    Name = "BO",
                    Description = "Medium Name",
                    Score = 0
                },
                new MeasuredItem()
                {
                    Id = 3,
                    Name = "CEE",
                    Description = "Longest Name",
                    Score = 0
                }
            };

            var rater = _mocker.CreateInstance<RatingService>();

            var ratings = rater.Rate(data, 3, RateByLength)
                .ToList();

            Assert.Equal("CEE", ratings[0].Name);
            Assert.Equal("BO", ratings[1].Name);
            Assert.Equal("A", ratings[2].Name);
        }

        [Fact]
        public void Rate_ByAlphabeticalOrder()
        {
            var data = new List<MeasuredItem>
            {
                new MeasuredItem{ Name = "F", Description = "F", Id = 6, Score = 0},
                new MeasuredItem{ Name = "E", Description = "E", Id = 5, Score = 0},
                new MeasuredItem{ Name = "D", Description = "D", Id = 4, Score = 0},
                new MeasuredItem{ Name = "C", Description = "C", Id = 3, Score = 0},
                new MeasuredItem{ Name = "B", Description = "B", Id = 2, Score = 0},
                new MeasuredItem{ Name = "A", Description = "A", Id = 1, Score = 0},
            };

            var rater = _mocker.CreateInstance<RatingService>();

            var ratings = rater.Rate(data, data.Count, (items) => items.OrderBy(x => x).First().Id)
                .ToList();

            Assert.Equal("A", ratings[0].Name);
            Assert.Equal("B", ratings[1].Name);
            Assert.Equal("C", ratings[2].Name);
            Assert.Equal("D", ratings[3].Name);
            Assert.Equal("E", ratings[4].Name);
            Assert.Equal("F", ratings[5].Name);
        }
    }
}

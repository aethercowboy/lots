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
    }
}

using System.Linq;
using AJP.MediatrEndpoints.Helpers;
using FluentAssertions;
using Xunit;


namespace AJP.MediatrEndpoints.Tests
{
    public class RouteParameterHelperTests
    {
        [Fact]
        public void GetRouteParamNamesFromPattern_should_return_two_params()
        {
            var sut = new RouteParameterHelper();

            var results = sut.GetRouteParamNamesFromPattern(@"\api\v1\test\{testOneId}\test2{testTwoId}");
            
            results.Count().Should().Be(2);
            results[0].Should().Be("testOneId");
            results[1].Should().Be("testTwoId");
        }
    }
}

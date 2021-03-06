﻿using FluentAssertions;
using Moq;
using NUnit.Framework;
using OutlookMatters.Core.Reply;

namespace Test.OutlookMatters.Core.Reply
{
    [TestFixture]
    public class PostIdFromPermalinkFilterTest
    {
        [Test]
        public void Get_ExtractsPostIdFromPermalink()
        {
            var baseProvider = new Mock<IStringProvider>();
            baseProvider.Setup(x => x.Get()).Returns("http://localhost/teamid/pl/abcdefghijklmnopqrstuvwxyz");
            var classUnderTest = new PostIdFromPermalinkFilter(baseProvider.Object);

            var result = classUnderTest.Get();

            result.Should().Be("abcdefghijklmnopqrstuvwxyz", "because the filter should extract the permalink id");
        }
    }
}
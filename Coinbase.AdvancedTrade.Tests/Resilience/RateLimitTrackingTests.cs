using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Coinbase.AdvancedTrade.Models;
using RestSharp;
using Xunit;

namespace Coinbase.AdvancedTrade.Tests.Resilience
{
    /// <summary>
    /// Unit tests for rate limit tracking and backpressure functionality.
    /// </summary>
    public class RateLimitTrackingTests
    {
        [Fact]
        public void RateLimitInfo_ShouldCalculateIsExhausted_Correctly()
        {
            // Arrange & Act
            var exhausted = new RateLimitInfo
            {
                Limit = 100,
                Remaining = 0,
                ResetAt = DateTimeOffset.UtcNow.AddMinutes(1)
            };

            var notExhausted = new RateLimitInfo
            {
                Limit = 100,
                Remaining = 50,
                ResetAt = DateTimeOffset.UtcNow.AddMinutes(1)
            };

            // Assert
            Assert.True(exhausted.IsExhausted);
            Assert.False(notExhausted.IsExhausted);
        }

        [Fact]
        public void RateLimitInfo_ShouldCalculateTimeUntilReset_Correctly()
        {
            // Arrange
            var futureReset = DateTimeOffset.UtcNow.AddMinutes(5);
            var rateLimitInfo = new RateLimitInfo
            {
                Limit = 100,
                Remaining = 10,
                ResetAt = futureReset
            };

            // Act
            var timeUntilReset = rateLimitInfo.TimeUntilReset;

            // Assert
            Assert.True(timeUntilReset.TotalSeconds > 290); // ~5 minutes
            Assert.True(timeUntilReset.TotalSeconds < 310);
        }

        [Fact]
        public void RateLimitInfo_ToString_ShouldFormatCorrectly()
        {
            // Arrange
            var resetAt = new DateTimeOffset(2026, 3, 1, 12, 0, 0, TimeSpan.Zero);
            var rateLimitInfo = new RateLimitInfo
            {
                Limit = 100,
                Remaining = 45,
                ResetAt = resetAt
            };

            // Act
            var result = rateLimitInfo.ToString();

            // Assert
            Assert.Contains("45/100", result);
            Assert.Contains("remaining", result);
        }

        [Fact]
        public void RateLimitInfo_ShouldHandleNegativeTimeUntilReset()
        {
            // Arrange
            var pastReset = DateTimeOffset.UtcNow.AddMinutes(-5);
            var rateLimitInfo = new RateLimitInfo
            {
                Limit = 100,
                Remaining = 0,
                ResetAt = pastReset
            };

            // Act
            var timeUntilReset = rateLimitInfo.TimeUntilReset;

            // Assert
            Assert.True(timeUntilReset.TotalSeconds < 0);
        }

        [Theory]
        [InlineData(100, 100, false)] // Full quota
        [InlineData(100, 50, false)]  // Half quota
        [InlineData(100, 1, false)]   // One remaining
        [InlineData(100, 0, true)]    // Exhausted
        public void RateLimitInfo_IsExhausted_ShouldMatchExpectation(int limit, int remaining, bool expectedExhausted)
        {
            // Arrange
            var rateLimitInfo = new RateLimitInfo
            {
                Limit = limit,
                Remaining = remaining,
                ResetAt = DateTimeOffset.UtcNow.AddMinutes(1)
            };

            // Act & Assert
            Assert.Equal(expectedExhausted, rateLimitInfo.IsExhausted);
        }

        [Fact]
        public void RateLimitInfo_ShouldStoreHeaderValues_Correctly()
        {
            // Arrange
            var resetAt = DateTimeOffset.UtcNow.AddMinutes(5);

            // Act
            var rateLimitInfo = new RateLimitInfo
            {
                Limit = 200,
                Remaining = 150,
                ResetAt = resetAt
            };

            // Assert
            Assert.Equal(200, rateLimitInfo.Limit);
            Assert.Equal(150, rateLimitInfo.Remaining);
            Assert.Equal(resetAt, rateLimitInfo.ResetAt);
        }
    }
}

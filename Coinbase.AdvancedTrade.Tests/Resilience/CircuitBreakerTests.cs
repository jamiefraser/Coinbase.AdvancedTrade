using System;
using System.Threading.Tasks;
using Coinbase.AdvancedTrade.Resilience;
using Microsoft.Extensions.Logging.Abstractions;
using Xunit;

namespace Coinbase.AdvancedTrade.Tests.Resilience
{
    /// <summary>
    /// Unit tests for CircuitBreaker implementation.
    /// </summary>
    public class CircuitBreakerTests
    {
        [Fact]
        public async Task CircuitBreaker_ShouldStartInClosedState()
        {
            // Arrange
            var config = new CircuitBreakerConfiguration
            {
                ServiceName = "TestService",
                FailureThreshold = 3
            };
            var circuitBreaker = new CircuitBreaker(config);

            // Act
            var state = circuitBreaker.State;

            // Assert
            Assert.Equal(CircuitState.Closed, state);
        }

        [Fact]
        public async Task CircuitBreaker_ShouldAllowSuccessfulCall_WhenClosed()
        {
            // Arrange
            var config = new CircuitBreakerConfiguration
            {
                ServiceName = "TestService",
                FailureThreshold = 3
            };
            var circuitBreaker = new CircuitBreaker(config);

            // Act
            var result = await circuitBreaker.ExecuteAsync(async () =>
            {
                await Task.Delay(10);
                return "Success";
            });

            // Assert
            Assert.Equal("Success", result);
            Assert.Equal(CircuitState.Closed, circuitBreaker.State);
        }

        [Fact]
        public async Task CircuitBreaker_ShouldOpenAfterThresholdFailures()
        {
            // Arrange
            var config = new CircuitBreakerConfiguration
            {
                ServiceName = "TestService",
                FailureThreshold = 3,
                FailureTimeoutSeconds = 60
            };
            var circuitBreaker = new CircuitBreaker(config, NullLogger.Instance);

            // Act - Trigger failures up to threshold
            for (int i = 0; i < 3; i++)
            {
                try
                {
                    await circuitBreaker.ExecuteAsync<string>(async () =>
                    {
                        await Task.Delay(10);
                        throw new Exception("Simulated failure");
                    });
                }
                catch (Exception)
                {
                    // Expected
                }
            }

            // Assert
            Assert.Equal(CircuitState.Open, circuitBreaker.State);
            Assert.Equal(3, circuitBreaker.FailureCount);
        }

        [Fact]
        public async Task CircuitBreaker_ShouldThrowCircuitBreakerException_WhenOpen()
        {
            // Arrange
            var config = new CircuitBreakerConfiguration
            {
                ServiceName = "TestService",
                FailureThreshold = 2,
                FailureTimeoutSeconds = 60
            };
            var circuitBreaker = new CircuitBreaker(config, NullLogger.Instance);

            // Trigger failures to open circuit
            for (int i = 0; i < 2; i++)
            {
                try
                {
                    await circuitBreaker.ExecuteAsync<string>(async () => throw new Exception("Fail"));
                }
                catch (Exception) { }
            }

            // Act & Assert
            var exception = await Assert.ThrowsAsync<CircuitBreakerException>(
                () => circuitBreaker.ExecuteAsync(async () => "This should not execute"));

            Assert.Equal("TestService", exception.ServiceName);
            Assert.Equal(CircuitState.Open, exception.State);
        }

        [Fact]
        public async Task CircuitBreaker_ShouldTransitionToHalfOpen_AfterRecoveryTimeout()
        {
            // Arrange
            var config = new CircuitBreakerConfiguration
            {
                ServiceName = "TestService",
                FailureThreshold = 2,
                FailureTimeoutSeconds = 60,
                RecoveryTimeoutSeconds = 0.5  // Short timeout for test
            };
            var circuitBreaker = new CircuitBreaker(config, NullLogger.Instance);

            // Trigger failures to open circuit
            for (int i = 0; i < 2; i++)
            {
                try
                {
                    await circuitBreaker.ExecuteAsync<string>(async () => throw new Exception("Fail"));
                }
                catch (Exception) { }
            }

            Assert.Equal(CircuitState.Open, circuitBreaker.State);

            // Act - Wait for recovery timeout
            await Task.Delay(TimeSpan.FromSeconds(0.6));

            // Assert - Circuit should transition to HalfOpen
            var state = circuitBreaker.State;
            Assert.Equal(CircuitState.HalfOpen, state);
        }

        [Fact]
        public async Task CircuitBreaker_ShouldCloseAfterSuccessfulCallInHalfOpen()
        {
            // Arrange
            var config = new CircuitBreakerConfiguration
            {
                ServiceName = "TestService",
                FailureThreshold = 2,
                FailureTimeoutSeconds = 60,
                RecoveryTimeoutSeconds = 0.5
            };
            var circuitBreaker = new CircuitBreaker(config, NullLogger.Instance);

            // Open circuit
            for (int i = 0; i < 2; i++)
            {
                try
                {
                    await circuitBreaker.ExecuteAsync<string>(async () => throw new Exception("Fail"));
                }
                catch (Exception) { }
            }

            // Wait for half-open
            await Task.Delay(TimeSpan.FromSeconds(0.6));
            Assert.Equal(CircuitState.HalfOpen, circuitBreaker.State);

            // Act - Execute successful call
            var result = await circuitBreaker.ExecuteAsync(async () =>
            {
                await Task.Delay(10);
                return "Success";
            });

            // Assert
            Assert.Equal("Success", result);
            Assert.Equal(CircuitState.Closed, circuitBreaker.State);
            Assert.Equal(0, circuitBreaker.FailureCount);
        }

        [Fact]
        public async Task CircuitBreaker_ShouldReopenAfterFailureInHalfOpen()
        {
            // Arrange
            var config = new CircuitBreakerConfiguration
            {
                ServiceName = "TestService",
                FailureThreshold = 2,
                FailureTimeoutSeconds = 60,
                RecoveryTimeoutSeconds = 0.5
            };
            var circuitBreaker = new CircuitBreaker(config, NullLogger.Instance);

            // Open circuit
            for (int i = 0; i < 2; i++)
            {
                try
                {
                    await circuitBreaker.ExecuteAsync<string>(async () => throw new Exception("Fail"));
                }
                catch (Exception) { }
            }

            // Wait for half-open
            await Task.Delay(TimeSpan.FromSeconds(0.6));
            Assert.Equal(CircuitState.HalfOpen, circuitBreaker.State);

            // Act - Execute failed call
            try
            {
                await circuitBreaker.ExecuteAsync<string>(async () => throw new Exception("Fail again"));
            }
            catch (Exception) { }

            // Assert - Circuit should reopen
            Assert.Equal(CircuitState.Open, circuitBreaker.State);
        }

        [Fact]
        public async Task CircuitBreaker_ShouldResetFailureCount_AfterTimeWindow()
        {
            // Arrange
            var config = new CircuitBreakerConfiguration
            {
                ServiceName = "TestService",
                FailureThreshold = 3,
                FailureTimeoutSeconds = 0.5  // Short window for test
            };
            var circuitBreaker = new CircuitBreaker(config, NullLogger.Instance);

            // Act - Trigger one failure
            try
            {
                await circuitBreaker.ExecuteAsync<string>(async () => throw new Exception("Fail"));
            }
            catch (Exception) { }

            Assert.Equal(1, circuitBreaker.FailureCount);

            // Wait for time window to expire
            await Task.Delay(TimeSpan.FromSeconds(0.6));

            // Trigger another failure (should reset count)
            try
            {
                await circuitBreaker.ExecuteAsync<string>(async () => throw new Exception("Fail"));
            }
            catch (Exception) { }

            // Assert - Failure count should be reset to 1 (not 2)
            Assert.Equal(1, circuitBreaker.FailureCount);
            Assert.Equal(CircuitState.Closed, circuitBreaker.State);
        }

        [Fact]
        public async Task CircuitBreaker_ShouldPropagateException_WhenFunctionFails()
        {
            // Arrange
            var config = new CircuitBreakerConfiguration
            {
                ServiceName = "TestService",
                FailureThreshold = 5
            };
            var circuitBreaker = new CircuitBreaker(config);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<InvalidOperationException>(
                () => circuitBreaker.ExecuteAsync<string>(async () =>
                {
                    await Task.Delay(10);
                    throw new InvalidOperationException("Custom error");
                }));

            Assert.Equal("Custom error", exception.Message);
            Assert.Equal(1, circuitBreaker.FailureCount);
        }
    }
}

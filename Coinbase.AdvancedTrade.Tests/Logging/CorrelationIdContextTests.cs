using System;
using System.Threading.Tasks;
using Coinbase.AdvancedTrade.Logging;
using Xunit;

namespace Coinbase.AdvancedTrade.Tests.Logging
{
    /// <summary>
    /// Tests for correlation ID context propagation and scope management.
    /// </summary>
    public class CorrelationIdContextTests
    {
        [Fact]
        public void GetOrCreate_WhenNoIdSetReturnsNewId()
        {
            // Clear any existing context
            CorrelationIdContext.Set(null);

            // Act
            var id1 = CorrelationIdContext.GetOrCreate();
            var id2 = CorrelationIdContext.GetOrCreate();

            // Assert
            Assert.NotNull(id1);
            Assert.NotEmpty(id1);
            Assert.Equal(id1, id2); // Should return same ID on subsequent calls
        }

        [Fact]
        public void Set_StoresCorrelationId()
        {
            // Arrange
            var testId = "test-correlation-id-12345";

            // Act
            CorrelationIdContext.Set(testId);
            var retrievedId = CorrelationIdContext.GetIfExists();

            // Assert
            Assert.Equal(testId, retrievedId);
        }

        [Fact]
        public void BeginScope_TemporarilySetsCorrelationId()
        {
            // Arrange
            var originalId = CorrelationIdContext.GetOrCreate();
            var newId = "new-scope-id-67890";

            // Act
            var idBeforeScope = CorrelationIdContext.GetIfExists();
            using (CorrelationIdContext.BeginScope(newId))
            {
                var idInScope = CorrelationIdContext.GetIfExists();
                Assert.Equal(newId, idInScope);
            }
            var idAfterScope = CorrelationIdContext.GetIfExists();

            // Assert
            Assert.Equal(originalId, idBeforeScope);
            Assert.Equal(originalId, idAfterScope);
        }

        [Fact]
        public async Task CorrelationIdIsolatedPerAsyncContext()
        {
            // Arrange
            CorrelationIdContext.Set(null);
            var mainId = CorrelationIdContext.GetOrCreate();

            // Act
            var taskId = await Task.Run(() =>
            {
                // This should have its own context
                return CorrelationIdContext.GetOrCreate();
            });

            // Assert
            Assert.NotEqual(mainId, taskId); // Different async contexts get different IDs
        }

        [Fact]
        public async Task CorrelationIdPropagatesWithinNestedAsyncCalls()
        {
            // Arrange
            var testId = "nested-async-test";
            CorrelationIdContext.Set(testId);

            // Act
            var retrievedId = await GetCorrelationIdFromNestedCall();

            // Assert
            Assert.Equal(testId, retrievedId);
        }

        private async Task<string> GetCorrelationIdFromNestedCall()
        {
            await Task.Delay(10); // Simulate async work
            return CorrelationIdContext.GetIfExists() ?? "null";
        }

        [Fact]
        public void NestedScopes_RestorePreviousIdOnDispose()
        {
            // Arrange
            var originalId = "original-id";
            var scope1Id = "scope-1";
            var scope2Id = "scope-2";
            CorrelationIdContext.Set(originalId);

            // Act
            using (CorrelationIdContext.BeginScope(scope1Id))
            {
                Assert.Equal(scope1Id, CorrelationIdContext.GetIfExists());

                using (CorrelationIdContext.BeginScope(scope2Id))
                {
                    Assert.Equal(scope2Id, CorrelationIdContext.GetIfExists());
                }

                Assert.Equal(scope1Id, CorrelationIdContext.GetIfExists());
            }

            // Assert
            Assert.Equal(originalId, CorrelationIdContext.GetIfExists());
        }

        [Fact]
        public void CorrelationIdScopeClass_CreatesAndManagesScope()
        {
            // Arrange
            CorrelationIdContext.Set(null);
            var testId = "scope-class-test";

            // Act
            using (var scope = new CorrelationIdScope(testId))
            {
                var idInScope = CorrelationIdContext.GetIfExists();
                Assert.Equal(testId, idInScope);
            }

            // Assert
            // Should restore to whatever was there before
            var idAfterScope = CorrelationIdContext.GetIfExists();
            Assert.NotNull(idAfterScope);
        }
    }
}

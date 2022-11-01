namespace Kent.Logging.Tests
{
    using Kent.Logging.MongoDb;
    using Kent.Logging.MongoDb.Models;
    using Kent.MongoDb.Abstractions;
    using Microsoft.Extensions.Logging;
    using MongoDB.Driver;
    using Moq;
    using System.Threading;
    using Xunit;

    public class MongoDbLoggerTests
    {
        private readonly Mock<IMongoCollection<LogMessageEntry>> _mockCollection;
        private readonly Mock<IMongoContext> _mockContext;
        private readonly Mock<IMongoContextFactory> _mockContextFactory;
        private readonly ILogger _logger;

        public MongoDbLoggerTests()
        {
            _mockCollection = new Mock<IMongoCollection<LogMessageEntry>>();

            _mockContext = new Mock<IMongoContext>();
            _mockContext.Setup(m => m.GetCollection<LogMessageEntry>()).Returns(_mockCollection.Object);

            _mockContextFactory = new Mock<IMongoContextFactory>();
            _mockContextFactory.Setup(m => m.CreateContext(It.IsAny<string>())).Returns(_mockContext.Object);

            _logger = new MongoDbLogger(nameof(MongoDbLoggerTests), _mockContextFactory.Object, () => null);
        }

        [Fact]
        public void MongoDbLogger_Logs_Messages()
        {
            // Prepare

            // Act
            _logger.LogError($"Log error");
            _logger.LogInformation($"Log information");
            _logger.LogWarning($"Log warning");

            // Assert
            _mockCollection.Verify(m => m.InsertOne(It.IsAny<LogMessageEntry>(), It.IsAny<InsertOneOptions>(), It.IsAny<CancellationToken>()), Times.Exactly(3));
        }
    }
}
namespace Kent.Logging.Tests
{
    using Kent.Logging.SqlServer;
    using Kent.Logging.SqlServer.Models;
    using Kent.SqlServer.Abstractions;
    using Microsoft.Extensions.Logging;
    using Moq;
    using Xunit;

    public class SqlServerLoggerTests
    {
        private readonly Mock<IRepository<LogMessageEntry>> _mockRepository;
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly Mock<IUnitOfWorkFactory> _mockUnitOfWorkFactory;
        private readonly ILogger _logger;

        public SqlServerLoggerTests()
        {
            _mockRepository = new Mock<IRepository<LogMessageEntry>>();

            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _mockUnitOfWork.Setup(m => m.GetEntityRepository<LogMessageEntry>()).Returns(_mockRepository.Object);

            _mockUnitOfWorkFactory = new Mock<IUnitOfWorkFactory>();
            _mockUnitOfWorkFactory.Setup(m => m.CreateUnitOfWork()).Returns(_mockUnitOfWork.Object);

            _logger = new SqlServerLogger(nameof(SqlServerLoggerTests), _mockUnitOfWorkFactory.Object, () => new SqlServerLoggerConfiguration ());
        }

        [Fact]
        public void SqlServerLogger_Logs_Messages()
        {
            // Prepare

            // Act
            _logger.LogError($"Log error");
            _logger.LogInformation($"Log information");
            _logger.LogWarning($"Log warning");

            // Assert
            _mockRepository.Verify(m => m.InsertOne(It.IsAny<LogMessageEntry>()), Times.Exactly(3));
            _mockUnitOfWork.Verify(m => m.Save(), Times.Exactly(3));
        }
    }
}
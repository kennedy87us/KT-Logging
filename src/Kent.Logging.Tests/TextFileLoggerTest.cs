namespace Kent.Logging.Tests
{
    using Kent.Logging.Tests.Constants;
    using Kent.Logging.TextFile;
    using Microsoft.Extensions.Logging;
    using System;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using Xunit;

    public class TextFileLoggerTest : IDisposable
    {
        private readonly TextFileLoggerConfiguration _configuration;
        private readonly ILogger _logger;
        private bool _disposed = false;

        public TextFileLoggerTest()
        {
            _configuration = new TextFileLoggerConfiguration
            {
                FilePath = TestConstants.FILE_PATH_TEST,
                TimestampFormat = TestConstants.TIMESTAMP_FORMAT_TEST
            };

            _logger = new TextFileLogger(nameof(TextFileLoggerTest), () => _configuration);

            CleanFiles();
        }

        [Fact]
        public void TextFileLogger_WithTimestamp_DefaultFormat_NoCategory_FullPathName_Then_Log_File()
        {
            // Prepare
            _configuration.FileNameFormat = "test1.txt";
            var fullFilePath = Path.Join(_configuration.FilePath, _configuration.FileNameFormat);

            // Act
            _logger.LogInformation("Log information");
            _logger.LogError("Log error");
            _logger.LogWarning("Log warning");

            // Assert
            string[] lines = File.ReadAllLines(fullFilePath);
            var timestampstring = string.Empty;

            Assert.Collection(lines,
                line =>     //Information
                {
                    timestampstring = line[..TestConstants.TIMESTAMP_FORMAT_TEST.Length];
                    DateTime.TryParseExact(timestampstring, TestConstants.TIMESTAMP_FORMAT_TEST, null, DateTimeStyles.None, out DateTime actualDate);
                    Assert.NotEqual(DateTime.MinValue, actualDate);
                    Assert.Contains($"[{LogLevel.Information.ToString().ToUpper()}]", line);
                },
                line =>     //Error
                {
                    timestampstring = line[..TestConstants.TIMESTAMP_FORMAT_TEST.Length];
                    DateTime.TryParseExact(timestampstring, TestConstants.TIMESTAMP_FORMAT_TEST, null, DateTimeStyles.None, out DateTime actualDate);
                    Assert.NotEqual(DateTime.MinValue, actualDate);
                    Assert.Contains($"[{LogLevel.Error.ToString().ToUpper()}]", line);
                },
                line =>     //Warning
                {
                    timestampstring = line[..TestConstants.TIMESTAMP_FORMAT_TEST.Length];
                    DateTime.TryParseExact(timestampstring, TestConstants.TIMESTAMP_FORMAT_TEST, null, DateTimeStyles.None, out DateTime actualDate);
                    Assert.NotEqual(DateTime.MinValue, actualDate);
                    Assert.Contains($"[{LogLevel.Warning.ToString().ToUpper()}]", line);
                }
            );
        }

        [Fact]
        public void TextFileLogger_WithTimestamp_DefaultFormat_WithCategory_FullPathName_Then_Log_File()
        {
            // Prepare
            _configuration.FileNameFormat = "test2.txt";
            _configuration.IncludeCategory = true;
            var fullFilePath = Path.Join(_configuration.FilePath, _configuration.FileNameFormat);

            // Act
            _logger.LogInformation("Log information");
            _logger.LogError("Log error");
            _logger.LogWarning("Log warning");

            // Assert
            string[] lines = File.ReadAllLines(fullFilePath);
            var timestampstring = string.Empty;

            Assert.Collection(lines,
                line =>     //Information
                {
                    timestampstring = line[..TestConstants.TIMESTAMP_FORMAT_TEST.Length];
                    DateTime.TryParseExact(timestampstring, TestConstants.TIMESTAMP_FORMAT_TEST, null, DateTimeStyles.None, out DateTime actualDate);
                    Assert.NotEqual(DateTime.MinValue, actualDate);
                    Assert.Contains($"[{LogLevel.Information.ToString().ToUpper()} - {nameof(TextFileLoggerTest)}]", line);
                },
                line =>     //Error
                {
                    timestampstring = line[..TestConstants.TIMESTAMP_FORMAT_TEST.Length];
                    DateTime.TryParseExact(timestampstring, TestConstants.TIMESTAMP_FORMAT_TEST, null, DateTimeStyles.None, out DateTime actualDate);
                    Assert.NotEqual(DateTime.MinValue, actualDate);
                    Assert.Contains($"[{LogLevel.Error.ToString().ToUpper()} - {nameof(TextFileLoggerTest)}]", line);
                },
                line =>     //Warning
                {
                    timestampstring = line[..TestConstants.TIMESTAMP_FORMAT_TEST.Length];
                    DateTime.TryParseExact(timestampstring, TestConstants.TIMESTAMP_FORMAT_TEST, null, DateTimeStyles.None, out DateTime actualDate);
                    Assert.NotEqual(DateTime.MinValue, actualDate);
                    Assert.Contains($"[{LogLevel.Warning.ToString().ToUpper()} - {nameof(TextFileLoggerTest)}]", line);
                }
            );
        }

        [Fact]
        public void TextFileLogger_WithTimestamp_MultiLinesFormat_NoCategory_FullPathName_Then_Log_File()
        {
            // Prepare
            _configuration.FileNameFormat = "test3.txt";
            _configuration.Format = TextFileLoggerFormat.MultiLines;
            var fullFilePath = Path.Join(_configuration.FilePath, _configuration.FileNameFormat);

            // Act
            _logger.LogInformation("Log information");
            _logger.LogError("Log error");
            _logger.LogWarning("Log warning");

            // Assert
            string[] lines = File.ReadAllLines(fullFilePath);
            var timestampstring = string.Empty;

            Assert.Collection(lines.ToList().Where((l, i) => i % 2 == 0),
                line =>     //Information
                {
                    timestampstring = line[..TestConstants.TIMESTAMP_FORMAT_TEST.Length];
                    DateTime.TryParseExact(timestampstring, TestConstants.TIMESTAMP_FORMAT_TEST, null, DateTimeStyles.None, out DateTime actualDate);
                    Assert.NotEqual(DateTime.MinValue, actualDate);
                    Assert.Contains($"[{LogLevel.Information.ToString().ToUpper()}]", line);
                },
                line =>     //Error
                {
                    timestampstring = line[..TestConstants.TIMESTAMP_FORMAT_TEST.Length];
                    DateTime.TryParseExact(timestampstring, TestConstants.TIMESTAMP_FORMAT_TEST, null, DateTimeStyles.None, out DateTime actualDate);
                    Assert.NotEqual(DateTime.MinValue, actualDate);
                    Assert.Contains($"[{LogLevel.Error.ToString().ToUpper()}]", line);
                },
                line =>     //Warning
                {
                    timestampstring = line[..TestConstants.TIMESTAMP_FORMAT_TEST.Length];
                    DateTime.TryParseExact(timestampstring, TestConstants.TIMESTAMP_FORMAT_TEST, null, DateTimeStyles.None, out DateTime actualDate);
                    Assert.NotEqual(DateTime.MinValue, actualDate);
                    Assert.Contains($"[{LogLevel.Warning.ToString().ToUpper()}]", line);
                }
            );
        }

        [Fact]
        public void TextFileLogger_WithTimestamp_MultiLinesFormat_WithCategory_FullPathName_Then_Log_File()
        {
            // Prepare
            _configuration.FileNameFormat = "test4.txt";
            _configuration.Format = TextFileLoggerFormat.MultiLines;
            _configuration.IncludeCategory = true;
            var fullFilePath = Path.Join(_configuration.FilePath, _configuration.FileNameFormat);

            // Act
            _logger.LogInformation("Log information");
            _logger.LogError("Log error");
            _logger.LogWarning("Log warning");

            // Assert
            string[] lines = File.ReadAllLines(fullFilePath);
            var timestampstring = string.Empty;

            Assert.Collection(lines.ToList().Where((l, i) => i % 2 == 0),
                line =>     //Information
                {
                    timestampstring = line[..TestConstants.TIMESTAMP_FORMAT_TEST.Length];
                    DateTime.TryParseExact(timestampstring, TestConstants.TIMESTAMP_FORMAT_TEST, null, DateTimeStyles.None, out DateTime actualDate);
                    Assert.NotEqual(DateTime.MinValue, actualDate);
                    Assert.Contains($"[{LogLevel.Information.ToString().ToUpper()} - {nameof(TextFileLoggerTest)}]", line);
                },
                line =>     //Error
                {
                    timestampstring = line[..TestConstants.TIMESTAMP_FORMAT_TEST.Length];
                    DateTime.TryParseExact(timestampstring, TestConstants.TIMESTAMP_FORMAT_TEST, null, DateTimeStyles.None, out DateTime actualDate);
                    Assert.NotEqual(DateTime.MinValue, actualDate);
                    Assert.Contains($"[{LogLevel.Error.ToString().ToUpper()} - {nameof(TextFileLoggerTest)}]", line);
                },
                line =>     //Warning
                {
                    timestampstring = line[..TestConstants.TIMESTAMP_FORMAT_TEST.Length];
                    DateTime.TryParseExact(timestampstring, TestConstants.TIMESTAMP_FORMAT_TEST, null, DateTimeStyles.None, out DateTime actualDate);
                    Assert.NotEqual(DateTime.MinValue, actualDate);
                    Assert.Contains($"[{LogLevel.Warning.ToString().ToUpper()} - {nameof(TextFileLoggerTest)}]", line);
                }
            );
        }

        [Fact]
        public void TextFileLogger_NoTimestamp_DefaultFormat_WithCategory_FullPathName_Then_Log_File()
        {
            // Prepare
            _configuration.FileNameFormat = "test5.txt";
            _configuration.IncludeCategory = true;
            _configuration.TimestampFormat = string.Empty;
            
            var fullFilePath = Path.Join(_configuration.FilePath, _configuration.FileNameFormat);

            // Act
            _logger.LogInformation("Log information");
            _logger.LogError("Log error");
            _logger.LogWarning("Log warning");

            // Assert
            string[] lines = File.ReadAllLines(fullFilePath);
            var timestampstring = string.Empty;

            Assert.Collection(lines,
                line =>     //Information
                {
                    timestampstring = line[..TestConstants.TIMESTAMP_FORMAT_TEST.Length];
                    DateTime.TryParseExact(timestampstring, TestConstants.TIMESTAMP_FORMAT_TEST, null, DateTimeStyles.None, out DateTime actualDate);
                    Assert.Equal(DateTime.MinValue, actualDate);
                    Assert.Contains($"[{LogLevel.Information.ToString().ToUpper()} - {nameof(TextFileLoggerTest)}]", line);
                },
                line =>     //Error
                {
                    timestampstring = line[..TestConstants.TIMESTAMP_FORMAT_TEST.Length];
                    DateTime.TryParseExact(timestampstring, TestConstants.TIMESTAMP_FORMAT_TEST, null, DateTimeStyles.None, out DateTime actualDate);
                    Assert.Equal(DateTime.MinValue, actualDate);
                    Assert.Contains($"[{LogLevel.Error.ToString().ToUpper()} - {nameof(TextFileLoggerTest)}]", line);
                },
                line =>     //Warning
                {
                    timestampstring = line[..TestConstants.TIMESTAMP_FORMAT_TEST.Length];
                    DateTime.TryParseExact(timestampstring, TestConstants.TIMESTAMP_FORMAT_TEST, null, DateTimeStyles.None, out DateTime actualDate);
                    Assert.Equal(DateTime.MinValue, actualDate);
                    Assert.Contains($"[{LogLevel.Warning.ToString().ToUpper()} - {nameof(TextFileLoggerTest)}]", line);
                }
            );
        }

        [Fact]
        public void TextFileLogger_NoTimestamp_MultiLinesFormat_WithCategory_FullPathName_Then_Log_File()
        {
            // Prepare
            _configuration.FileNameFormat = "test6.txt";
            _configuration.IncludeCategory = true;
            _configuration.TimestampFormat = string.Empty;
            _configuration.Format = TextFileLoggerFormat.MultiLines;
            var fullFilePath = Path.Join(_configuration.FilePath, _configuration.FileNameFormat);

            // Act
            _logger.LogInformation("Log information");
            _logger.LogError("Log error");
            _logger.LogWarning("Log warning");

            // Assert
            string[] lines = File.ReadAllLines(fullFilePath);
            var timestampstring = string.Empty;

            Assert.Collection(lines.ToList().Where((l, i) => i % 2 == 0),
                line =>     //Information
                {
                    timestampstring = line[..TestConstants.TIMESTAMP_FORMAT_TEST.Length];
                    DateTime.TryParseExact(timestampstring, TestConstants.TIMESTAMP_FORMAT_TEST, null, DateTimeStyles.None, out DateTime actualDate);
                    Assert.Equal(DateTime.MinValue, actualDate);
                    Assert.Contains($"[{LogLevel.Information.ToString().ToUpper()} - {nameof(TextFileLoggerTest)}]", line);
                },
                line =>     //Error
                {
                    timestampstring = line[..TestConstants.TIMESTAMP_FORMAT_TEST.Length];
                    DateTime.TryParseExact(timestampstring, TestConstants.TIMESTAMP_FORMAT_TEST, null, DateTimeStyles.None, out DateTime actualDate);
                    Assert.Equal(DateTime.MinValue, actualDate);
                    Assert.Contains($"[{LogLevel.Error.ToString().ToUpper()} - {nameof(TextFileLoggerTest)}]", line);
                },
                line =>     //Warning
                {
                    timestampstring = line[..TestConstants.TIMESTAMP_FORMAT_TEST.Length];
                    DateTime.TryParseExact(timestampstring, TestConstants.TIMESTAMP_FORMAT_TEST, null, DateTimeStyles.None, out DateTime actualDate);
                    Assert.Equal(DateTime.MinValue, actualDate);
                    Assert.Contains($"[{LogLevel.Warning.ToString().ToUpper()} - {nameof(TextFileLoggerTest)}]", line);
                }
            );
        }

        private void CleanFiles()
        {
            if (Directory.Exists(_configuration.FilePath))
            {
                Directory.Delete(_configuration.FilePath, true);
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    CleanFiles();
                }
            }
            _disposed = true;
        }
    }
}
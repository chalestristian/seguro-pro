using Microsoft.Extensions.Logging;
using Moq;


public static class LoggerExtensions
{
    public static void VerifyLogError<T>(this Mock<ILogger<T>> loggerMock, Exception expectedException, string expectedMessage, Times times)
    {
        loggerMock.Verify(
            x => x.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((state, type) => state.ToString().Contains(expectedMessage)),
                expectedException,
                It.IsAny<Func<It.IsAnyType, Exception, string>>()),times);
    }
}
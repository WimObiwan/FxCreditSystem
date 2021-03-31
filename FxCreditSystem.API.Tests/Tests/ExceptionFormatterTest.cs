using System;
using Xunit;

namespace FxCreditSystem.API.Tests
{
    public class ExceptionFormatterTest
    {
        private Exception GetTestException()
        {
            try
            {
                try
                {
                    throw new Exception("Test inner exception");
                }
                catch (Exception InnerException)
                {
                    throw new Exception("Test outer exception", InnerException);
                }
            }
            catch (Exception outerException)
            {
                return outerException;
            }
        }

        [Fact]
        public void GetString_WithoutCallstack_ShouldSucceed()
        {
            Exception testException = GetTestException();

            var exceptionFormatter = new ExceptionFormatter();
            string text = exceptionFormatter.GetText(testException, false);

            Assert.Equal("Test outer exception --> Test inner exception", text);
        }

        [Fact]
        public void GetString_WithCallstack_ShouldSucceed()
        {
            Exception testException = GetTestException();

            var exceptionFormatter = new ExceptionFormatter();
            string text = exceptionFormatter.GetText(testException, true);

            Assert.Contains("Test outer exception --> Test inner exception", text);
            Assert.Contains("GetTestException", text);
        }
    }
}

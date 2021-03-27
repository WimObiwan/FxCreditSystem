using System;
using System.Text;
using System.Security.Claims;
using System.Collections.Generic;

namespace FxCreditSystem.API
{
    public interface IExceptionFormatter
    {
        string GetText(Exception exception, bool includeCallStack);
    }

    public class ExceptionFormatter : IExceptionFormatter
    {
        public string GetText(Exception exception, bool includeCallStack)
        {
            var list = new List<String>();
            for (var x = exception; x != null; x = x.InnerException)
                list.Add(x.Message);

            var text = string.Join(" --> ", list);

            if (includeCallStack)
                text = string.Concat(text, Environment.NewLine, exception.StackTrace);

            return text;
        }
    }
}
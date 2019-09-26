using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Formatting;
using System.Web;

namespace Contoso.Apps.PaymentGateway.Models
{
    public class FormatterLogger : IFormatterLogger
    {
        public void LogError(string errorPath, Exception exception)
        {
            throw new NotImplementedException();
        }

        public void LogError(string errorPath, string errorMessage)
        {
            throw new NotImplementedException();
        }
    }
}
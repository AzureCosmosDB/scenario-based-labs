using System;

namespace Contoso.Apps.Movies.Web.Models
{
    /// <summary>
    /// Model holds error information pertaining to the checkout process.
    /// </summary>
    public class CheckoutErrorModel
    {
        public string ErrorCode { get; set; }
        public string LongMessage { get; set; }
        public string ShortMessage { get; set; }

        /// <summary>
        /// Write error Model as a formatted string.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            string output = string.Format("Error Code: {0}{1}", this.ErrorCode, Environment.NewLine);
            if (!string.IsNullOrWhiteSpace(this.ShortMessage))
            {
                output += string.Format("Message: {0}{1}", this.ShortMessage, Environment.NewLine);
            }
            if (!string.IsNullOrWhiteSpace(this.LongMessage))
            {
                output += string.Format("Detailed Message: {0}", this.LongMessage);
            }
            return output;
        }
    }
}
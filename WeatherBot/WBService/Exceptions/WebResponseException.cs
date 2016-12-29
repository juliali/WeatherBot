using System;
using System.Net;
using System.Runtime.Serialization;

namespace WBService.Exceptions
{
    [Serializable]
    internal class WebResponseException : Exception
    {
        private HttpStatusCode internalServerError;
        private string v;

        public WebResponseException()
        {
        }

        public WebResponseException(string message) : base(message)
        {
        }

        public WebResponseException(string message, Exception innerException) : base(message, innerException)
        {
        }

        public WebResponseException(HttpStatusCode internalServerError, string v)
        {
            this.internalServerError = internalServerError;
            this.v = v;
        }

        protected WebResponseException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
using System;
using MNIX.Webservices;

namespace MNIX.HttpRequester
{
    /// <summary>
    /// Exceção disparada quando o método HTTP utilizado não suporta o rastreamento de progresso de upload
    /// </summary>
    public class InvalidHTTPMethodForProgressTrackingException : Exception
    {
        private const string MESSAGE_FORMAT = "Esse tipo de método HTTP não suporta rastreamento de progresso de upload! Método HTTP utilizado : {0}";

        private HttpMethod _mHttpMethod;

        public InvalidHTTPMethodForProgressTrackingException(HttpMethod httpMethod)
        {
            _mHttpMethod = httpMethod;
        }

        /// <summary>
        /// Método HTTP utilizado quando essa exceção foi disparada
        /// </summary>
        /// <owner>Mnix-Victor</owner>
        public HttpMethod UsedHttpMethod
        {
            get { return _mHttpMethod; }
        }

        /// <summary>
        /// Mensagem de erro da exceção
        /// </summary>
        /// <owner>Mnix-Victor</owner>
        public override string Message
        {
            get
            {
                return String.Format(MESSAGE_FORMAT, this.UsedHttpMethod);
            }
        }
    }
}

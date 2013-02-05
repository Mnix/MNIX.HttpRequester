using System;
using MNIX.Webservices;

namespace MNIX.HttpRequester
{
    /// <summary>
    /// Exceção disparada quando há uma tentativa de enviar um tipo de parâmetro não permitido no atual método HTTP
    /// </summary>
    /// <owner>Mnix-Victor</owner>
    public class InvalidParameterForHttpMethodException : Exception
    {
        private const string MESSAGE_FORMAT = "Há parâmetros não permitidos para o método HTTP {0}!";

        private HttpMethod mHttpMethod;

        /// <summary>
        /// Mensagem disparada na exceção
        /// </summary>
        /// <owner>Mnix-Victor</owner>
        public override string Message
        {
            get
            {
                return String.Format(MESSAGE_FORMAT, mHttpMethod);
            }
        }

        public InvalidParameterForHttpMethodException(HttpMethod httpMethod)
        {
            mHttpMethod = httpMethod;
        }
    }
}

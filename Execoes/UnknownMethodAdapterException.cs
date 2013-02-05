using System;
using MNIX.Webservices;

namespace MNIX.HttpRequester
{
    /// <summary>
    /// Exceção disparada quando foi utilizado um método HTTP desconhecido para fazer a requisição
    /// </summary>
    /// <owner>Mnix-Victor</owner>
    public class UnknownMethodAdapterException : Exception
    {
        private const string MESSAGE_FORMAT = "Método HTTP desconhecido sendo utilizado! O valor utilizado foi : {0}";

        private HttpMethod _mHttpMethod;

        /// <summary>
        /// Método HTTP utilizado quando essa exceção foi disparada
        /// </summary>
        /// <owner>Mnix-Victor</owner>
        public HttpMethod UsedHttpMethod
        {
            get { return _mHttpMethod; }
        }

        /// <summary>
        /// Mensagem disparada pela exceção
        /// </summary>
        /// <owner>Mnix-Victor</owner>
        public override string Message
        {
            get
            {
                return String.Format(MESSAGE_FORMAT, this.UsedHttpMethod);
            }
        }

        public UnknownMethodAdapterException(HttpMethod httpMethod)
        {
            this._mHttpMethod = httpMethod;
        }
    }
}

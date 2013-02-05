using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MNIX.HttpRequester
{
    /// <summary>
    /// Exceção disparada quando já há alguma requisição rodando com a instância do MxHttpRequest
    /// </summary>
    /// <owner>Mnix-Victor</owner>
    public class RequestAlreadyRunningException : Exception
    {
        private const string MESSAGE_FORMAT = "Já há uma requisição rodando com essa instância de MxHttpRequest";

        private HttpRequest _mMxHttpRequest;

        /// <summary>
        /// Objeto MxHttpRequest utilizado que disparou o erro
        /// </summary>
        /// <owner>Mnix-Victor</owner>
        public HttpRequest UsedMxHttpRequest
        {
            get { return _mMxHttpRequest; }
        }

        /// <summary>
        /// Mensagem de erro da exceção
        /// </summary>
        /// <owner>Mnix-Victor</owner>
        public override string Message
        {
            get
            {
                return MESSAGE_FORMAT;
            }
        }

        public RequestAlreadyRunningException(HttpRequest usedMxHttpRequest)
        {
            this._mMxHttpRequest = usedMxHttpRequest;
        }
    }
}

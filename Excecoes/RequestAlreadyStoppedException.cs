using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MNIX.HttpRequester
{
    /// <summary>
    /// Exceção disparada quando ocorre uma tentativa de abortar uma requisição que não está rodando
    /// </summary>
    /// <owner>Mnix-Victor</owner>
    public class RequestAlreadyStoppedException : Exception
    {
        private const string MESSAGE_FORMAT = "Ocorreu uma tentativa de abortar uma requisição que não está rodando!";

        private HttpRequest _mRequest;

        public RequestAlreadyStoppedException(HttpRequest request)
        {
            _mRequest = request;
        }

        /// <summary>
        /// Retorna a instância da requisição que disparou erro
        /// </summary>
        /// <owner>Mnix-Victor</owner>
        public HttpRequest Request
        {
            get { return _mRequest; }
        }

        /// <summary>
        /// Retorna a mensagem padrão da exceção
        /// </summary>
        /// <owner>Mnix-Victor</owner>
        public override string Message
        {
            get
            {
                return MESSAGE_FORMAT;
            }
        }
    }
}

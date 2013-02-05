using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MNIX.HttpRequester
{
    /// <summary>
    /// Exceção disparada quando a URL enviada para a requisição possui um formato incorreto
    /// </summary>
    /// <owner>Mnix-Victor</owner>
    public class InvalidTargetURLException : Exception
    {
        private const string MESSAGE_FORMAT = "A URL não pode possuir parâmetros diretamente nela, Ex: http://www.contoso.com.br/Default.aspx?parametro=valor, utilize somente http://www.contoso.com.br/Default.aspx";

        /// <summary>
        /// Mensagem da exceção
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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace MNIX.HttpRequester
{
    /// <summary>
    /// Representa um adaptador HTTP genérico, uma classe responsável por montar os cabeçalhos e a requisição a ser enviada
    /// </summary>
    /// <owner>Mnix-Victor</owner>
    internal abstract class HttpMethodAdapter
    {
        private const string EQUAL_SYMBOL = "=";
        private const string AND_SYMBOL = "&";

        private ParameterCollection _mParameters;
        private Encoding _mUsedEncoding;

        /// <summary>
        /// Parametros a serem enviados pela requisição
        /// </summary>
        /// <owner>Mnix-Victor</owner>
        protected ParameterCollection Parameters
        {
            get { return _mParameters; }
        }

        /// <summary>
        /// Encoding a ser utilizado pela requisição
        /// </summary>
        /// <owner>Mnix-Victor</owner>
        protected Encoding UsedEncoding
        {
            get { return _mUsedEncoding; }
        }

        internal HttpMethodAdapter(ParameterCollection parameters, Encoding usedEncoding)
        {
            this._mParameters = parameters;
            this._mUsedEncoding = usedEncoding;
        }

        /// <summary>
        /// Retorna a URI da requisição
        /// </summary>
        /// <owner>Mnix-Victor</owner>
        internal abstract Uri MountRequestUri(Uri uri);

        /// <summary>
        /// Retorna o Content-Type utilizado pelo cabeçalho da requisição
        /// </summary>
        /// <owner>Mnix-Victor</owner>
        internal abstract string MountContentType();

        /// <summary>
        /// Retorna o apelido do método HTTP em String
        /// </summary>
        /// <owner>Mnix-Victor</owner>
        internal abstract string GetStringRepresentation();

        /// <summary>
        /// Retorna parâmetro a parâmetro no formato parametro=valor, sendo o valor já encodado em formato HTTPEncode
        /// </summary>
        /// <owner>Mnix-Victor</owner>
        internal IEnumerable<string> CreateParametersStrings()
        {
            foreach (Parameter parameter in this.Parameters.Where((singleParameter) => (singleParameter.ParameterType == ParameterType.Value)))
            {
                // nome=valor&
                string formatParameter = string.Format("{0}{1}{2}", parameter.ParameterName, EQUAL_SYMBOL, Uri.EscapeDataString(parameter.ParameterValue));

                //Só adiciona & se não for o último parâmetro
                if (!this.Parameters.IsLastParameter(parameter))
                {
                    formatParameter += AND_SYMBOL;
                }

                yield return formatParameter;
			}
        }
    }
}

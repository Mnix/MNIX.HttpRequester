using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MNIX.HttpRequester
{
    /// <summary>
    /// Classe parcial HttpMethodAdapterFactory
    /// </summary>
    /// <owner>Mnix-Victor</owner>
    partial class HttpMethodAdapterFactory
    {
        /// <summary>
        /// Subclasse do HttpMethodAdapter, que monta as requisições do tipo GET
        /// </summary>
        /// <owner>Mnix-Victor</owner>
        private class GETHttpMethodAdapter : HttpMethodAdapter
        {
            private const string QUERY_STRING_SYMBOL = "?";
            private const string GET_CONTENT_TYPE_FORMAT = "encoding='{0}'";
            private const string GET_STRING_REPRESENTATION = "GET";

            public GETHttpMethodAdapter(ParameterCollection parameters, Encoding encoding)
                : base(parameters, encoding)
            {
            }

            /// <summary>
            /// Monta a URL de requisição concatenando os parâmetros no formato QueryString Ex: http://www.contoso.com.br/Default.aspx?parametro1=valor1&parametro2=valor2
            /// </summary>
            /// <owner>Mnix-Victor</owner>
            internal override Uri MountRequestUri(Uri uri)
            {
                string targetUrl = uri.ToString();

                StringBuilder parametersStringBuilder = new StringBuilder();

                foreach (string parameterString in CreateParametersStrings())
                {
                    parametersStringBuilder.Append(parameterString);
                }

                return new Uri(String.Format(targetUrl + "{0}{1}", QUERY_STRING_SYMBOL, parametersStringBuilder.ToString()));
            }

            /// <summary>
            /// Monta a definição Content-Type do cabeçalho de requisição
            /// </summary>
            /// <owner>Mnix-Victor</owner>
            internal override string MountContentType()
            {
                return String.Format(GET_CONTENT_TYPE_FORMAT, this.UsedEncoding.BodyName);
            }

            /// <summary>
            /// Retorna o apelido do método HTTP em formato String
            /// </summary>
            /// <owner>Mnix-Victor</owner>
            internal override string GetStringRepresentation()
            {
                return GET_STRING_REPRESENTATION;
            }
        }
    }
}

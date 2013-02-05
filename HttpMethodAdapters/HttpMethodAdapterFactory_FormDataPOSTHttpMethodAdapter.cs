using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace MNIX.HttpRequester
{
    /// <summary>
    /// Classe parcial do HttpMethodAdapterFactory
    /// </summary>
    /// <owner>Mnix-Victor</owner>
    partial class HttpMethodAdapterFactory
    {
        /// <summary>
        /// Define uma subclasse do HttpMethodAdapter para montar requests do tipo FormData, que só enviam parâmetros simples (texto) via POST
        /// </summary>
        /// <owner>Mnix-Victor</owner>
        private class FormDataPOSTHttpMethodAdapter : BasePOSTHttpMethodAdapter
        {
            private const string FORM_DATA_POST_CONTENT_TYPE_FORMAT = "application/x-www-form-urlencoded; encoding='{0}'";

            public FormDataPOSTHttpMethodAdapter(ParameterCollection parameters, Encoding encoding)
                : base(parameters, encoding)
            {
            }

            /// <summary>
            /// Transforma parâmetro a parâmetro a ser enviado em um array de bytes
            /// </summary>
            /// <owner>Mnix-Victor</owner>
            private IEnumerable<byte[]> TransformParametersToByteArray()
            {
                foreach (string parameter in CreateParametersStrings())
                {
                    byte[] parameterByteArray = new byte[parameter.Length];
                    parameterByteArray = this.UsedEncoding.GetBytes(parameter);

                    yield return parameterByteArray;
                }
            }

            /// <summary>
            /// Monta a string de Content-Type de cabeçalho da requisição
            /// </summary>
            /// <owner>Mnix-Victor</owner>
            internal override string MountContentType()
            {
                return String.Format(FORM_DATA_POST_CONTENT_TYPE_FORMAT, this.UsedEncoding.BodyName);
            }

            /// <summary>
            /// Escreve os dados no Stream de entrada da requisição
            /// </summary>
            /// <owner>Mnix-Victor</owner>
            internal override void WriteRequestStream(Stream requestStream)
            {
                long totalBytesSent = 0;

                UploadProgressChangedEventHandler(totalBytesSent, this.CachedContentSize);

                foreach (byte[] parameterByteArray in TransformParametersToByteArray())
                {
                    requestStream.Write(parameterByteArray, 0, parameterByteArray.Length);

                    totalBytesSent += parameterByteArray.Length;
                    UploadProgressChangedEventHandler(totalBytesSent, this.CachedContentSize);
                }
            }

            /// <summary>
            /// Retorna quantos bytes vão ser enviados por essa requisição
            /// </summary>
            /// <owner>Mnix-Victor</owner>
            protected override long CalculateContentSize()
            {
                long totalSize = 0;

                foreach (byte[] parameterByteArray in TransformParametersToByteArray())
                {
                    totalSize += parameterByteArray.Length;
                }

                return totalSize;
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace MNIX.HttpRequester
{
    /// <summary>
    /// Classe parcial HttpMethodAdapterFactory
    /// </summary>
    partial class HttpMethodAdapterFactory
    {
        /// <summary>
        /// Subclasse que define uma classe base de requisição utilizando o método HTTP Post
        /// </summary>
        /// <owner>Mnix-Victor</owner>
        internal abstract class BasePOSTHttpMethodAdapter : HttpMethodAdapter
        {
            private const string POST_CONTENT_TYPE_FORMAT = "application/x-www-form-urlencoded; encoding='{0}'";
            private const string POST_STRING_REPRESENTATION = "POST";

            private long _mCachedContentSize;

            /// <summary>
            /// Retorna o total em bytes do tamanho da requisição
            /// </summary>
            internal long CachedContentSize
            {
                get { return _mCachedContentSize; }
            }

            /// <summary>
            /// Delegate utilizado para disparar o método de mudança de progresso no upload
            /// </summary>
            /// <owner>Mnix-Victor</owner>
            internal delegate void InternalRequestUploadProgressChangedEventHandler(long sentBytes, long totalBytesToSend);

            /// <summary>
            /// Acesso ao método de mudança de progresso de upload
            /// </summary>
            /// <owner>Mnix-Victor</owner>
            internal InternalRequestUploadProgressChangedEventHandler UploadProgressChangedEventHandler
            {
                get;
                set;
            }

            protected BasePOSTHttpMethodAdapter(ParameterCollection parameters, Encoding encoding)
                : base(parameters, encoding)
            {
                _mCachedContentSize = CalculateContentSize();
            }

            /// <summary>
            /// Monta a URL de requisição
            /// </summary>
            /// <owner>Mnix-Victor</owner>
            internal override Uri MountRequestUri(Uri uri)
            {
                return uri;
            }

            /// <summary>
            /// Retorna o apelido do método HTTP em String
            /// </summary>
            /// <owner>Mnix-Victor</owner>
            internal override string GetStringRepresentation()
            {
                return POST_STRING_REPRESENTATION;
            }

            /// <summary>
            /// Escreve os dados da requisição no Stream de entrada
            /// </summary>
            /// <owner>Mnix-Victor</owner>
            internal abstract void WriteRequestStream(Stream requestStream);

            /// <summary>
            /// Calcula o tamanho total da requisição
            /// </summary>
            /// <owner>Mnix-Victor</owner>
            protected abstract long CalculateContentSize();
        }
    }
}

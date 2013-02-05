using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace MNIX.HttpRequester
{
    /// <summary>
    /// Classe parcial da HttpMethodAdapterFactory
    /// </summary>
    /// <owner>Mnix-Victor</owner>
    partial class HttpMethodAdapterFactory
    {
        /// <summary>
        /// Subclasse do HttpMethodAdapter, utilizado para montar requisições do tipo Multipart POST (comum em envio de arquivos)
        /// </summary>
        /// <owner>Mnix-Victor</owner>
        private class MultipartPOSTHttpMethodAdapter : BasePOSTHttpMethodAdapter
        {
            private const string MULTIPART_POST_CONTENT_TYPE_FORMAT = "multipart/form-data; encoding='{0}'; boundary={1}";
            private const string BOUNDARY_PREFIX = "----------------------------{0}";

            private readonly string FORM_DATA_PARAMETER_TEMPLATE = Environment.NewLine + "--{0}" + Environment.NewLine + "Content-Disposition: form-data; name=\"{1}\";" + Environment.NewLine + Environment.NewLine + "{2}";
            private readonly string FORM_DATA_FILE_TEMPLATE = Environment.NewLine + "--{0}" + Environment.NewLine + "Content-Disposition: form-data; name=\"{1}\"; filename=\"{2}\"" + Environment.NewLine + "Content-Type: {3}" + Environment.NewLine + Environment.NewLine;
            private readonly string MULTIPART_END = Environment.NewLine + "--{0}--" + Environment.NewLine;

            private string _mBoundary = String.Empty;

            /// <summary>
            /// Boundary (código) utilizado pela requisição multipart
            /// Se não existir um já criado, monta um
            /// </summary>
            /// <owner>Mnix-Victor</owner>
            private string Boundary
            {
                get
                {
                    //POG utilizada, para corrigir o problema de o tamanho do Content-Type ser diferente do que realmente vai ser enviado
                    if (this._mBoundary == String.Empty)
                    {
                        MountBoundaryIdentifier();
                    }

                    return this._mBoundary;
                }
            }

            public MultipartPOSTHttpMethodAdapter(ParameterCollection parameters, Encoding encoding)
                : base(parameters, encoding)
            {
            }

            /// <summary>
            /// Monta a string de boundary
            /// </summary>
            /// <owner>Mnix-Victor</owner>
            private void MountBoundaryIdentifier()
            {
                this._mBoundary = String.Format(BOUNDARY_PREFIX, DateTime.Now.Ticks.ToString("x"));
            }

            /// <summary>
            /// Transforma um a um os parâmetros simples (texto) em um array de bytes
            /// </summary>
            /// <owner>Mnix-Victor</owner>
            private IEnumerable<byte[]> TransformValueParametersToByteArray()
            {
                foreach (Parameter parameter in this.Parameters.Where((parameter) => (parameter.ParameterType == ParameterType.Value)))
                {
                    string formDataParameter = String.Format(FORM_DATA_PARAMETER_TEMPLATE, this.Boundary, parameter.ParameterName, parameter.ParameterValue);
                    byte[] formDataParameterByteArray = UsedEncoding.GetBytes(formDataParameter);

                    yield return formDataParameterByteArray;
                }
            }

            /// <summary>
            /// Transforma o marcador de final de requisição multipart em um array de bytes
            /// </summary>
            /// <owner>Mnix-Victor</owner>
            private byte[] TransformMultipartEndToByteArray()
            {
                string multipartRequestEnd = String.Format(MULTIPART_END, this.Boundary);
                byte[] multipartRequestEndByteArray = UsedEncoding.GetBytes(multipartRequestEnd);

                return multipartRequestEndByteArray;
            }

            /// <summary>
            /// Transforma os parâmetros de arquivo um a um em uma tupla que contém o cabecalho descritor do arquivo e a referência ao arquivo no disco
            /// </summary>
            /// <owner>Mnix-Victor</owner>
            private IEnumerable<FileParameter> TransformFileParametersDefinitionsToByteArray()
            {
                foreach (Parameter parameter in this.Parameters.Where((parameter) => (parameter.ParameterType == ParameterType.File)))
                {
                    FileInfo fileInfo = new FileInfo(parameter.ParameterValue);

                    string formDataFileParameter = String.Format(FORM_DATA_FILE_TEMPLATE, this.Boundary, parameter.ParameterName, fileInfo.Name, MimeAssistant.GetMIMEType(fileInfo.FullName));
                    byte[] formDataFileParameterByteArray = UsedEncoding.GetBytes(formDataFileParameter);

                    yield return new FileParameter(fileInfo, formDataFileParameterByteArray);
                }
            }

            /// <summary>
            /// Tupla que agrupa o cabeçalho de definição de arquivo e o acesso ao arquivo no disco
            /// </summary>
            /// <owner>Mnix-Victor</owner>
            private class FileParameter
            {
                private byte[] _mFileParameterDefinitionByteArray;
                private FileInfo _mFileInfoDescription;

                /// <summary>
                /// Byte array do cabeçalho descritor do arquivo
                /// </summary>
                public byte[] FileParameterDefitionByteArray
                {
                    get { return _mFileParameterDefinitionByteArray; }
                }

                /// <summary>
                /// Instância FileInfo do arquivo especificado pelo parâmetro
                /// </summary>
                public FileInfo FileInfoDescription
                {
                    get { return _mFileInfoDescription; }
                }

                public FileParameter(FileInfo fileInfoDescription, byte[] fileParameterDefitionByteArray)
                {
                    this._mFileInfoDescription = fileInfoDescription;
                    this._mFileParameterDefinitionByteArray = fileParameterDefitionByteArray;
                }
            }

            /// <summary>
            /// Monta o cabeçalho Content-Type da requisição
            /// </summary>
            /// <owner>Mnix-Victor</owner>
            internal override string MountContentType()
            {
                return String.Format(MULTIPART_POST_CONTENT_TYPE_FORMAT, UsedEncoding.BodyName, this.Boundary);
            }

            /// <summary>
            /// Escreve os dados da requisição no stream de entrada
            /// </summary>
            /// <owner>Mnix-Victor</owner>
            internal override void WriteRequestStream(Stream requestStream)
            {
                long totalBytesSent = 0;
                UploadProgressChangedEventHandler(totalBytesSent, this.CachedContentSize);

                foreach (byte[] parameterByteArray in TransformValueParametersToByteArray())
                {
                    requestStream.Write(parameterByteArray, 0, parameterByteArray.Length);

                    totalBytesSent += parameterByteArray.Length;
                    UploadProgressChangedEventHandler(totalBytesSent, this.CachedContentSize);
                }

                foreach (FileParameter fileParameter in TransformFileParametersDefinitionsToByteArray())
                {
                    requestStream.Write(fileParameter.FileParameterDefitionByteArray, 0, fileParameter.FileParameterDefitionByteArray.Length);

                    totalBytesSent += fileParameter.FileParameterDefitionByteArray.Length;
                    UploadProgressChangedEventHandler(totalBytesSent, this.CachedContentSize);

                    FileStream fileStream = fileParameter.FileInfoDescription.OpenRead();

                    int readBytesFromFile;
                    byte[] fileBuffer = new byte[HttpRequest.DEFAULT_BLOCK_SIZE];

                    while ((readBytesFromFile = fileStream.Read(fileBuffer, 0, fileBuffer.Length)) != 0)
                    {
                        requestStream.Write(fileBuffer, 0, readBytesFromFile);

                        totalBytesSent += readBytesFromFile;
                        UploadProgressChangedEventHandler(totalBytesSent, this.CachedContentSize);
                    }

                    fileStream.Close();
                }

                byte[] multipartEndByteArray = TransformMultipartEndToByteArray();
                requestStream.Write(multipartEndByteArray, 0, multipartEndByteArray.Length);

                totalBytesSent += multipartEndByteArray.Length;
                UploadProgressChangedEventHandler(totalBytesSent, this.CachedContentSize);
            }

            /// <summary>
            /// Calcula o total em bytes de toda a requisição
            /// </summary>
            /// <owner>Mnix-Victor</owner>
            protected override long CalculateContentSize()
            {
                long totalSize = 0;

                foreach (byte[] parameterByteArray in TransformValueParametersToByteArray())
                {
                    totalSize += parameterByteArray.Length;
                }

                foreach (FileParameter fileParameter in TransformFileParametersDefinitionsToByteArray())
                {
                    totalSize += fileParameter.FileInfoDescription.Length + fileParameter.FileParameterDefitionByteArray.Length;
                }

                totalSize += TransformMultipartEndToByteArray().Length;

                return totalSize;
            }
        }
    }
}

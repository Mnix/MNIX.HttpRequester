using System;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;
using System.Threading;
using MNIX.Webservices;

namespace MNIX.HttpRequester
{
    /// <summary>
    /// Classe que representa uma requisição HTTP que pode enviar arquivos, dados simples e o misto dos 2
    /// </summary>
    /// <owner>Mnix-Victor</owner>
    public class HttpRequest : IHttpRequest
    {
        public const int DEFAULT_BLOCK_SIZE = 4096;
        private const uint DEFAULT_TIMEOUT = 100000;
        private const long BEGIN_STREAM_POSITION = 0;

        private object @lock = new object();

        private static readonly Encoding DEFAULT_ENCODING = Encoding.UTF8;

        private uint _mTimeout = DEFAULT_TIMEOUT;
        private Encoding _mUsedEncoding = DEFAULT_ENCODING;
        private Uri _mTargetUri;
        private ParameterCollection mParameters = new ParameterCollection();
        private HttpWebRequest mRunningRequest;
        private UploadProgressChangedHandler mProgressChangedHandler;
        private bool mIsAborted = false;
        private bool _mIsRequestRunning = false;

        /// <summary>
        /// Quantidade de timeout que a requisição pode aguardar antes de disparar erro, em milisegundos
        /// </summary>
        /// <owner>Mnix-Victor</owner>
        public uint Timeout
        {
            get { return _mTimeout; }
            set { _mTimeout = value; }
        }

        /// <summary>
        /// Encoding utilizado para fazer as requisições e converter o resultado delas em string novamente
        /// </summary>
        /// <owner>Mnix-Victor</owner>
        public Encoding UsedEncoding
        {
            get { return _mUsedEncoding; }
            set { _mUsedEncoding = value;}
        }

        /// <summary>
        /// Uri utilizada para fazer esse request
        /// </summary>
        /// <owner>Mnix-Victor</owner>
        public Uri TargetUri
        {
            get { return _mTargetUri; }
        }

        /// <summary>
        /// Flag que diz se ja há uma requisição rodando com essa instância de HttpRequest
        /// </summary>
        /// <owner>Mnix-Victor</owner>
        public bool IsRequestRunning
        {
            get { return _mIsRequestRunning; }
        }

        public HttpRequest(string targetUrl): this(new Uri(targetUrl)) { }

        public HttpRequest(Uri targetUrl)
        {
            _mTargetUri = targetUrl;

            //Parâmetros não podem ser enviados diretamente pela URL (Método GET), podendo causar problemas de conflito no momento de montar eles via request
            if (_mTargetUri.Query != String.Empty)
            {
                throw new InvalidTargetURLException();
            }
        }

        /// <summary>
        /// Faz a requisição utilizando o método HTTP enviado via parâmetro e retornando o retorno em string
        /// </summary>
        /// <owner>Mnix-Victor</owner>
        private Stream MakeRequest(HttpMethod httpMethod, UploadProgressChangedHandler progressChangedHandler)
        {
            lock (@lock)
            {
                HttpMethodAdapter methodAdapter = HttpMethodAdapterFactory.GetAdapter(httpMethod, this.mParameters, this.UsedEncoding);

                mRunningRequest = (HttpWebRequest)HttpWebRequest.Create(methodAdapter.MountRequestUri(this._mTargetUri));

                if (mIsAborted)
                {
                    this.Abort();
                    this.mIsAborted = false;
                }

                mRunningRequest.Method = httpMethod.ToString();
                mRunningRequest.Timeout = (int)this.Timeout;
                mRunningRequest.ContentType = methodAdapter.MountContentType();

                if (methodAdapter is HttpMethodAdapterFactory.BasePOSTHttpMethodAdapter)
                {
                    this.mProgressChangedHandler = progressChangedHandler;

                    HttpMethodAdapterFactory.BasePOSTHttpMethodAdapter postHttpAdapter = (HttpMethodAdapterFactory.BasePOSTHttpMethodAdapter)methodAdapter;
                    postHttpAdapter.UploadProgressChangedEventHandler = SendProgressChangedNotification;

                    mRunningRequest.AllowWriteStreamBuffering = false;
                    mRunningRequest.ContentLength = postHttpAdapter.CachedContentSize;

                    //Pega o stream de saida para ser escrito na requsição
                    Stream writeableRequestStream = mRunningRequest.GetRequestStream();

                    postHttpAdapter.WriteRequestStream(writeableRequestStream);

                    writeableRequestStream.Close();
                }

                WebResponse response = mRunningRequest.GetResponse();

                Stream responseStream = response.GetResponseStream();

                return responseStream;
            }
        }

        /// <summary>
        /// Dispara o delegate de mudança de progresso no upload
        /// </summary>
        /// <owner>Mnix-Victor</owner>
        private void SendProgressChangedNotification(long bytesAlreadySent, long totalBytesToSend)
        {
            if (mProgressChangedHandler != null)
            {
                mProgressChangedHandler.Invoke(this, bytesAlreadySent, totalBytesToSend);
            }
        }

        /// <summary>
        /// Pega o stream de response (saída) e monta a string de retorno, retornando ela
        /// </summary>
        /// <owner>Mnix-Victor</owner>
        private string MountReturnString(Stream stream)
        {
            StringBuilder resultStringBuilder = new StringBuilder();

            int readBytes;
            byte[] totalBuffer = new byte[DEFAULT_BLOCK_SIZE];

            while ((readBytes = stream.Read(totalBuffer, 0, DEFAULT_BLOCK_SIZE)) != 0)
            {
                resultStringBuilder.Append(UsedEncoding.GetString(totalBuffer, 0, readBytes));
            }

            return resultStringBuilder.ToString();
        }

        /// <summary>
        /// Seta um valor para o parâmetro de nome enviado, a partir de um tipo
        /// Se o parâmetro já existe, o valor dele é substituído
        /// OBS: Um parâmetro de um tipo não pode ser mudado para outro tipo, disparando a exceção WrongParameterTypeException
        /// </summary>
        /// <owner>Mnix-Victor</owner>
        private void SetParameter(string parameterName, string parameterValue, ParameterType parameterType)
        {
            Parameter parameterWithName = mParameters.FirstOrDefault((parameter) => (parameter.ParameterName == parameterName));

            if (parameterWithName != null)
            {
                //Caso o parâmetro já exista só alterar o valor

                //Não é possivel alterar um parâmetro já existe de outro tipo
                if (parameterWithName.ParameterType != parameterType)
                {
                    throw new WrongParameterTypeException(parameterName, parameterWithName.ParameterType, parameterType);
                }

                parameterWithName.ParameterValue = parameterValue;
            }
            else
            {
                //Se o parâmetro ainda não existe, crio um novo
                Parameter newParameter = new Parameter(parameterName, parameterValue, parameterType);
                newParameter.ParameterValue = parameterValue;

                mParameters.AddParameter(newParameter);
            }
        }

        /// <summary>
        /// Trata os erros enviados pela exception e disparada o delegate de erro
        /// </summary>
        /// <owner>Mnix-Victor</owner>
        private void DiagnoseRequestErrors(WebException exception, ErrorResultHandler errorResultHandler)
        {
            string errorOutput = String.Empty;
            WebResponse response = exception.Response;

            if (response != null)
            {
                StreamReader errorStreamReader = new StreamReader(exception.Response.GetResponseStream());
                errorOutput = errorStreamReader.ReadToEnd();
            }

            errorResultHandler.Invoke(this, exception, errorOutput);
        }

        /// <summary>
        /// Seta um valor para um parâmetro de nome enviado
        /// OBS: Utilizar somente para parâmetros simples, para arquivos utilize o SetFile
        /// </summary>
        /// <owner>Mnix-Victor</owner>
        public void SetParameter(string parameterName, string parameterValue)
        {
            SetParameter(parameterName, parameterValue, ParameterType.Value);
        }

        /// <summary>
        /// Seta um arquivo para ser enviado com um nome de parâmetro
        /// OBS: Enviar o caminho completo do arquivo
        /// DÚVIDA: Pedir string ou FileInfo (forçando o programador a realmente enviar um arquivo)?
        /// </summary>
        /// <owner>Mnix-Victor</owner>
        public void SetFile(string parameterName, string fileAddress)
        {
            SetParameter(parameterName, fileAddress, ParameterType.File);
        }

        /// <summary>
        /// Executa a requisição de modo síncrono utilizando o método HTTP enviado e retorna a string de response
        /// OBS: Utilizar o bloco try/catch para chamar esse método sempre que possível
        /// </summary>
        /// <param name="method">Método HTTP a ser utilizado para fazer a requisição</param>
        /// <owner>Mnix-Victor</owner>
        public string Execute(HttpMethod method)
        {
            Stream responseStream = MakeRequest(method, null);

            string responseString = MountReturnString(responseStream);

            responseStream.Close();

            return responseString;
        }

        /// <summary>
        /// Executa a requisição de modo assíncrono utilizando o método HTTP enviado e retorna o resultado a partir dos delegates de callback. Esse overload tem suporte a track de porcentagem de upload
        /// Nesse overload também o resultado é retornado a partir de um Stream
        /// </summary>
        /// <param name="method">Método HTTP a ser utilizado para fazer a requisição</param>
        /// <param name="sucessResultWithStreamHandler">Handler chamado quando houver sucesso na requisição</param>
        /// <param name="progressChangedHandler">Handler chamado para notificar o progresso de upload da requisição</param>
        /// <param name="errorResultHandler">Handler chamado quando houver erro na requisição</param>
        /// <owner>Mnix-Victor</owner>
        public void ExecuteAsync(HttpMethod method, SucessResultWithStreamHandler sucessResultWithStreamHandler, UploadProgressChangedHandler progressChangedHandler, ErrorResultHandler errorResultHandler)
        {
            if (method == HttpMethod.GET && progressChangedHandler != null)
            {
                throw new InvalidHTTPMethodForProgressTrackingException(method);
            }

            this._mIsRequestRunning = true;

            Thread newThread = new Thread(delegate()
            {
                try
                {
                    Stream responseStream = MakeRequest(method, progressChangedHandler);
                    sucessResultWithStreamHandler.Invoke(this, responseStream);

                    responseStream.Close();
                }
                catch (WebException webException)
                {
                    this.DiagnoseRequestErrors(webException, errorResultHandler);
                }
                finally
                {
                    this._mIsRequestRunning = false;
                }
            });

            newThread.Start();
        }

        /// <summary>
        /// Executa a requisição de modo assíncrono utilizando o método HTTP enviado e retorna o resultado a partir dos delegates de callback. Esse overload tem suporte a track de porcentagem de upload
        /// </summary>
        /// <param name="method">Método HTTP a ser utilizado para fazer a requisição</param>
        /// <param name="sucessResultWithStreamHandler">Handler chamado quando houver sucesso na requisição</param>
        /// <param name="progressChangedHandler">Handler chamado para notificar o progresso de upload da requisição</param>
        /// <param name="errorResultHandler">Handler chamado quando houver erro na requisição</param>
        /// <owner>Mnix-Victor</owner>
        public void ExecuteAsync(HttpMethod method, SuccessResultHandler successResultHandler, UploadProgressChangedHandler progressChangedHandler, ErrorResultHandler errorResultHandler)
        {
            if (method == HttpMethod.GET && progressChangedHandler != null)
            {
                throw new InvalidHTTPMethodForProgressTrackingException(method);
            }

            this._mIsRequestRunning = true;

            Thread newThread = new Thread(delegate()
            {
                try
                {
                    Stream responseStream = MakeRequest(method, progressChangedHandler);

                    string resultString = MountReturnString(responseStream);
                    responseStream.Close();

                    successResultHandler.Invoke(this, resultString);
                }
                catch (WebException webException)
                {
                    this.DiagnoseRequestErrors(webException, errorResultHandler);
                }
                finally
                {
                    this._mIsRequestRunning = false;
                }
            });
            
            newThread.Start();
        }

        /// <summary>
        /// Cancela a requisição rodando
        /// </summary>
        /// <owner>Mnix-Victor</owner>
        public void Abort()
        {
            if (!this.IsRequestRunning)
            {
                throw new RequestAlreadyStoppedException(this);
            }

            if (mRunningRequest == null)
            {
                //Flag de correção utilizada quando o request não foi iniciado ainda (utilização em modo assíncrono)
                this.mIsAborted = true;
            }
            else
            {
                mRunningRequest.Abort();
                this._mIsRequestRunning = false;
            }
        }
    }
}

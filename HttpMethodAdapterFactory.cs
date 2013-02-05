using System.Text;
using MNIX.Webservices;

namespace MNIX.HttpRequester
{
    /// <summary>
    /// Classe factory, que cria um adapter de acordo com o método HTTP enviado
    /// </summary>
    /// <owner>Mnix-Victor</owner>
    internal static partial class HttpMethodAdapterFactory
    {
        /// <summary>
        /// Retorna o adaptador correto, dependendo do método pedido, e se tem ou não arquivos a serem enviados
        /// </summary>
        /// <owner>Mnix-Victor</owner>
        internal static HttpMethodAdapter GetAdapter(HttpMethod method, ParameterCollection parameters, Encoding encoding)
        {
            switch (method)
            {
                case HttpMethod.GET:

                    if (parameters.HasFileParameter())
                    {
                        throw new InvalidParameterForHttpMethodException(method);
                    }

                    return new GETHttpMethodAdapter(parameters, encoding);
                case HttpMethod.POST:
                    if (parameters.HasFileParameter())
                    {
                        return new MultipartPOSTHttpMethodAdapter(parameters, encoding);
                    }
                    else
                    {
                        return new FormDataPOSTHttpMethodAdapter(parameters, encoding);
                    }
                default:
                    throw new UnknownMethodAdapterException(method);
            }
        }
    }
}

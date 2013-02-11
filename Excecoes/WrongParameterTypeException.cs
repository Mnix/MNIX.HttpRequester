using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MNIX.HttpRequester
{
    /// <summary>
    /// Exceção disparada quando um tipo de parâmetro já existe é alterado para outro
    /// </summary>
    /// <owner>Mnix-Victor</owner>
    public class WrongParameterTypeException : Exception
    {
        private const string MESSAGE_FORMAT = "Ocorreu uma tentativa de alterar o tipo do parâmetro de nome {0}, sendo o tipo antigo = {1} e o novo tipo = {2}";

        private ParameterType mOldParameterType;
        private ParameterType mNewParameterType;
        private string mParameterName;

        /// <summary>
        /// Mensagem disparada pela exceção
        /// </summary>
        public override string Message
        {
            get
            {
                return String.Format(MESSAGE_FORMAT, this.mParameterName, this.mOldParameterType, this.mNewParameterType);
            }
        }

        internal WrongParameterTypeException(string parameterName, ParameterType oldParameterType, ParameterType newParameterType)
        {
            mParameterName = parameterName;

            mOldParameterType = oldParameterType;
            mNewParameterType = newParameterType;
        }
    }
}

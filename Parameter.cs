using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MNIX.HttpRequester
{
    /// <summary>
    /// Classe que representa um parâmetro que pode ser enviado via uma requisição HTTP
    /// Encapsula um parâmetro com valor simples e um parâmetro de arquivo a ser enviado
    /// </summary>
    /// <owner>Mnix-Victor</owner>
    internal class Parameter
    {
        private string _mParameterName;
        private string _mParameterValue;
        private ParameterType _mParameterType;

        /// <summary>
        /// Nome do parâmetro
        /// </summary>
        /// <owner>Mnix-Victor</owner>
        public string ParameterName
        {
            get { return _mParameterName; }
        }

        /// <summary>
        /// Valor do parâmetro
        /// </summary>
        /// <owner>Mnix-Victor</owner>
        public string ParameterValue
        {
            get { return _mParameterValue; }
            set { _mParameterValue = value; }
        }

        /// <summary>
        /// Tipo do parâmetro
        /// </summary>
        /// <owner>Mnix-Victor</owner>
        public ParameterType ParameterType
        {
            get { return _mParameterType; }
            set { _mParameterType = value; }
        }

        internal Parameter(string parameterName, string parameterValue, ParameterType parameterType)
        {
            if (parameterType == ParameterType.File)
            {
                Uri uri = new Uri(parameterValue);

                if (!uri.IsFile)
                {
                    throw new InvalidFilePathException(parameterName, uri);
                }
            }

            this._mParameterName = parameterName;
            this._mParameterValue = parameterValue;
            this._mParameterType = parameterType;
        }
    }
}

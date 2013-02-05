using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MNIX.HttpRequester
{
    /// <summary>
    /// Exceção disparada quando é enviado um caminho de arquivo inválido para um parâmetro de arquivo a ser enviado em uma requisição
    /// </summary>
    /// <owner>Mnix-Victor</owner>
    public class InvalidFilePathException : Exception
    {
        private const string MESSAGE_FORMAT = "Caminho inválido de arquivo para o parâmetro de nome : {0}, o caminho incorreto é : {1}";

        private string _mParameterName;
        private string _mFilePath;

        /// <summary>
        /// Mensagem disparada pela exceção
        /// </summary>
        /// <owner>Mnix-Victor</owner>
        public override string Message
        {
            get
            {
                return String.Format(MESSAGE_FORMAT, this.ParameterName, this.FilePath);
            }
        }

        /// <summary>
        /// Nome de parâmetro que possuia o caminho incorreto de arquivo
        /// </summary>
        /// <owner>Mnix-Victor</owner>
        public string ParameterName
        {
            get { return _mParameterName; }
        }

        /// <summary>
        /// Caminho inválido enviado pelo parâmetro que causou a exceção
        /// </summary>
        /// <owner>Mnix-Victor</owner>
        public string FilePath
        {
            get { return _mFilePath; }
        }

        public InvalidFilePathException(string parameterName, Uri filePath)
        {
            this._mParameterName = parameterName;
            this._mFilePath = filePath.ToString();
        }
    }
}

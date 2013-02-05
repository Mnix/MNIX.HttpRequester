using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MNIX.HttpRequester
{
    /// <summary>
    /// Classe que encapsula uma coleção de parâmetros a serem enviados por uma requisição feita pelo MxHttpRequest
    /// </summary>
    /// <owner>Mnix-Victor</owner>
    internal class ParameterCollection : IEnumerable<Parameter>
    {
        private List<Parameter> mParameters = new List<Parameter>();

        /// <summary>
        /// Operador de índice, retorna o parâmetro no índice passado
        /// </summary>
        /// <owner>Mnix-Victor</owner>
        public Parameter this[int idx]
        {
            get { return mParameters[idx]; }
        }

        #region IEnumerable<Parameter> implementation owner: Mnix-Victor
        public IEnumerator<Parameter> GetEnumerator()
        {
            return mParameters.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return mParameters.GetEnumerator();
        }
        #endregion

        /// <summary>
        /// Adiciona um parâmetro na coleção
        /// </summary>
        /// <owner>Mnix-Victor</owner>
        public void AddParameter(Parameter parameter)
        {
            mParameters.Add(parameter);
        }

        /// <summary>
        /// Testa se há algum parâmetro de arquivo na coleção
        /// </summary>
        /// <owner>Mnix-Victor</owner>
        public bool HasFileParameter()
        {
            return mParameters.Exists((parameter) => (parameter.ParameterType == ParameterType.File));
        }

        /// <summary>
        /// Testa se o parâmetro enviado é o último da coleção
        /// </summary>
        /// <owner>Mnix-Victor</owner>
        public bool IsLastParameter(Parameter parameter)
        {
            return mParameters[mParameters.Count - 1] == parameter;
        }
    }
}

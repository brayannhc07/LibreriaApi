namespace LibreriaApi.Models.Responses
{
    /// <summary>
    /// Modelo que sirve para estructurar las respuestas del servicio.
    /// Es una clase genérica para cualquier clase que se quiera devolver.
    /// </summary>
    /// <typeparam name="T">Modelo que se quiere devolver en la respuesta.</typeparam>
    public class Response<T>
    {
        #region Properties
        /// <summary>
        /// Obtiene o establece si la respuesta fue exitosa o no.
        /// </summary>
        public bool Success { get; set; } = false;
        /// <summary>
        /// Obtiene o establece el mensaje de la respuesta, puede contener información
        /// en caso de error o confirmación de éxito.
        /// </summary>
        public string Message { get; set; } = string.Empty;
        /// <summary>
        /// Obtiene o establece el contenido de la respuesta.
        /// </summary>
        public T? Data { get; set; }
        #endregion
        #region Methods
        /// <summary>
        /// Confirma la respuesta como exitosa.
        /// </summary>
        /// <returns>Devuelve la repuesta ya actualizada.</returns>
        public Response<T> Commit()
        {
            Success = true;
            return this;
        }
        /// <summary>
        /// Confirma la repuesta como exitosa y se le asigna un mensaje.
        /// </summary>
        /// <param name="message">Mensaje que se le asignará a la respuesta.</param>
        /// <returns>Devuelve la respuesta ya actualizada.</returns>
        public Response<T> Commit(string message)
        {
            Commit();
            Message = message;
            return this;
        }
        /// <summary>
        /// Confirma la repuesta como exitosa asignándole un mensaje y los datos.
        /// </summary>
        /// <param name="message">Mensaje que se le asignará a la respuesta.</param>
        /// <param name="data">Contenido que se le asigna a la respuesta.</param>
        /// <returns>Devuelve la repuesta ya actualizada.</returns>
        public Response<T> Commit(string message, T data)
        {
            Commit(message);
            Data = data;
            return this;
        }
        /// <summary>
        /// Define la repuesta como fallida.
        /// </summary>
        /// <returns>Devuelve la respuesta actualizada.</returns>
        public Response<T> Defeat()
        {
            Success = false;
            return this;
        }
        /// <summary>
        /// Define la repuesta como fallida y se le asigna un mensaje.
        /// </summary>
        /// <param name="message">Mensaje que se le asginará a la repuesta.</param>
        /// <returns>Devuelve la repuesta actualizada.</returns>
        public Response<T> Defeat(string message)
        {
            Defeat();
            Message = message;
            return this;
        }
        /// <summary>
        /// Define la respuesta como fallida asignándole un mensaje y un contenido.
        /// </summary>
        /// <param name="message">Mensaje que se le asignará a la respuesta.</param>
        /// <param name="data">Contenido que se le asigna a la respuesta.</param>
        /// <returns>Devuelve la repuesta ya actualizada.</returns>
        public Response<T> Defeat(string message, T data)
        {
            Defeat();
            Message = message;
            Data = data;
            return this;
        }
        #endregion
    }
}

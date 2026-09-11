namespace PuntoDeVentaEntities.Seguridad
{
    /// <summary>
    /// Envoltura uniforme de respuesta de la API. Toda la API responde HTTP 200;
    /// los errores viajan en IsSuccess=false + Message (nunca se lanzan al cliente).
    /// </summary>
    public class ModelResponse
    {
        public ModelResponse() { IsSuccess = false; }

        public bool IsSuccess { get; set; }
        public string Message { get; set; }
        public object Response { get; set; }
    }

    public class ModelResponse<T>
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; }
        public T Response { get; set; }
    }
}

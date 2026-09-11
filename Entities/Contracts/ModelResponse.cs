using System.Collections.Generic;

namespace PuntoDeVenta.Entities.Contracts
{
    public class ModelResponse<T>
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public T Data { get; set; }
        public List<string> Errors { get; set; } = new List<string>();
    }

    public class ModelResponse : ModelResponse<object>
    {
    }
}

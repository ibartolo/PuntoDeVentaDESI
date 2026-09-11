using System;
using System.Collections.Generic;
using PuntoDeVenta.Entities.Contracts;
using PuntoDeVenta.Entities.Models;
using PuntoDeVenta.WebApi.Dal;

namespace PuntoDeVenta.WebApi.Services
{
    internal class MarcaService
    {
        private readonly MarcaDal _marcaDal;

        public MarcaService()
        {
            _marcaDal = new MarcaDal();
        }

        public ModelResponse<List<Marca>> Listar(long empresaId)
        {
            try
            {
                var data = _marcaDal.Listar(empresaId);
                return new ModelResponse<List<Marca>>
                {
                    Success = true,
                    Message = "Marcas activas obtenidas correctamente.",
                    Data = data
                };
            }
            catch (Exception)
            {
                return new ModelResponse<List<Marca>>
                {
                    Success = false,
                    Message = "Ocurrió un error al consultar las Marcas.",
                    Errors = new List<string> { "Error de persistencia al consultar Marcas." }
                };
            }
        }

        public ModelResponse<Marca> Consultar(long empresaId, long marcaId)
        {
            if (marcaId <= 0)
            {
                return ErrorMarca("El identificador de Marca es inválido.");
            }

            try
            {
                var marca = _marcaDal.Consultar(empresaId, marcaId);
                if (marca == null)
                {
                    return ErrorMarca("Marca no encontrada para la empresa autenticada.");
                }

                if (!marca.Estatus)
                {
                    return ErrorMarca("La Marca está inactiva y no está disponible en el catálogo operativo.");
                }

                return new ModelResponse<Marca>
                {
                    Success = true,
                    Message = "Marca obtenida correctamente.",
                    Data = marca
                };
            }
            catch (Exception)
            {
                return new ModelResponse<Marca>
                {
                    Success = false,
                    Message = "Ocurrió un error al consultar la Marca.",
                    Errors = new List<string> { "Error de persistencia al consultar Marca." }
                };
            }
        }

        public ModelResponse<Marca> Crear(long empresaId, string actor, string nombre, string descripcion)
        {
            var normalizedNombre = NormalizarNombre(nombre);
            if (normalizedNombre == null)
            {
                return ErrorMarca("El Nombre es requerido y no puede exceder 100 caracteres.");
            }

            var normalizedActor = NormalizarActor(actor);
            if (normalizedActor == null)
            {
                return ErrorMarca("No se pudo resolver el actor autenticado para auditoría.");
            }

            try
            {
                var id = _marcaDal.Insertar(empresaId, normalizedNombre, descripcion, normalizedActor);
                var marca = _marcaDal.Consultar(empresaId, id);
                if (marca == null || !marca.Estatus)
                {
                    return ErrorMarca("No fue posible recuperar la Marca recién creada.");
                }

                return new ModelResponse<Marca>
                {
                    Success = true,
                    Message = "Marca creada correctamente.",
                    Data = marca
                };
            }
            catch (InvalidOperationException ex)
            {
                return ErrorMarca(ex.Message);
            }
            catch (Exception)
            {
                return new ModelResponse<Marca>
                {
                    Success = false,
                    Message = "Ocurrió un error al crear la Marca.",
                    Errors = new List<string> { "Error de persistencia al crear Marca." }
                };
            }
        }

        public ModelResponse<Marca> Actualizar(long empresaId, long marcaId, string actor, string nombre, string descripcion)
        {
            if (marcaId <= 0)
            {
                return ErrorMarca("El identificador de Marca es inválido.");
            }

            var normalizedNombre = NormalizarNombre(nombre);
            if (normalizedNombre == null)
            {
                return ErrorMarca("El Nombre es requerido y no puede exceder 100 caracteres.");
            }

            var normalizedActor = NormalizarActor(actor);
            if (normalizedActor == null)
            {
                return ErrorMarca("No se pudo resolver el actor autenticado para auditoría.");
            }

            try
            {
                var affectedRows = _marcaDal.Actualizar(empresaId, marcaId, normalizedNombre, descripcion, normalizedActor);
                if (affectedRows <= 0)
                {
                    return ErrorMarca("Marca no encontrada o inactiva para la empresa autenticada.");
                }

                var marca = _marcaDal.Consultar(empresaId, marcaId);
                if (marca == null || !marca.Estatus)
                {
                    return ErrorMarca("No fue posible recuperar la Marca actualizada.");
                }

                return new ModelResponse<Marca>
                {
                    Success = true,
                    Message = "Marca actualizada correctamente.",
                    Data = marca
                };
            }
            catch (InvalidOperationException ex)
            {
                return ErrorMarca(ex.Message);
            }
            catch (Exception)
            {
                return new ModelResponse<Marca>
                {
                    Success = false,
                    Message = "Ocurrió un error al actualizar la Marca.",
                    Errors = new List<string> { "Error de persistencia al actualizar Marca." }
                };
            }
        }

        public ModelResponse EliminarLogico(long empresaId, long marcaId, string actor)
        {
            if (marcaId <= 0)
            {
                return ErrorNoGenerico("El identificador de Marca es inválido.");
            }

            var normalizedActor = NormalizarActor(actor);
            if (normalizedActor == null)
            {
                return ErrorNoGenerico("No se pudo resolver el actor autenticado para auditoría.");
            }

            try
            {
                var affectedRows = _marcaDal.EliminarLogico(empresaId, marcaId, normalizedActor);
                if (affectedRows <= 0)
                {
                    return ErrorNoGenerico("Marca no encontrada o ya inactiva para la empresa autenticada.");
                }

                return new ModelResponse
                {
                    Success = true,
                    Message = "Marca desactivada correctamente."
                };
            }
            catch (Exception)
            {
                return new ModelResponse
                {
                    Success = false,
                    Message = "Ocurrió un error al desactivar la Marca.",
                    Errors = new List<string> { "Error de persistencia al desactivar Marca." }
                };
            }
        }

        private static string NormalizarNombre(string nombre)
        {
            var trimmed = nombre?.Trim();
            if (string.IsNullOrWhiteSpace(trimmed) || trimmed.Length > 100)
            {
                return null;
            }

            return trimmed;
        }

        private static string NormalizarActor(string actor)
        {
            var trimmed = actor?.Trim();
            if (string.IsNullOrWhiteSpace(trimmed))
            {
                return null;
            }

            if (trimmed.Length > 25)
            {
                return trimmed.Substring(0, 25);
            }

            return trimmed;
        }

        private static ModelResponse<Marca> ErrorMarca(string message)
        {
            return new ModelResponse<Marca>
            {
                Success = false,
                Message = message,
                Errors = new List<string> { message }
            };
        }

        private static ModelResponse ErrorNoGenerico(string message)
        {
            return new ModelResponse
            {
                Success = false,
                Message = message,
                Errors = new List<string> { message }
            };
        }
    }
}

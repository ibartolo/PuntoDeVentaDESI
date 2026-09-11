using Newtonsoft.Json;
using PuntoDeVentaEntities.Autenticacion;
using PuntoDeVentaEntities.Seguridad;
using PuntoDeVentaMVC.Filters;
using PuntoDeVentaMVC.Helpers;
using PuntoDeVentaMVC.Services;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace PuntoDeVentaMVC.Controllers
{
    public class UserController : BaseController
    {
        private readonly UsuarioService _usuarioService;
        private readonly RolService _rolService;

        public UserController()
        {
            _usuarioService = new UsuarioService(httpClientConnection);
            _rolService = new RolService(httpClientConnection);
        }

        /// <summary>Perfil del usuario autenticado (resuelve SYNC-2: /User/MyProfile).</summary>
        public async Task<ActionResult> MyProfile()
        {
            var sesion = SessionHelper.GetSessionUser();
            if (sesion == null || sesion.UserID == 0)
            {
                return RedirectToAction("Autentication", "Home");
            }

            var usuario = await _usuarioService.ObtenerUsuarioPorId(sesion.UserID);
            if (usuario == null)
            {
                usuario = new Usuario
                {
                    Id = sesion.UserID,
                    EmpresaId = sesion.EmpresaID,
                    NombreUsuario = sesion.UserName,
                    ImagenPerfil = sesion.ProfileImage
                };
                ViewBag.ErrorMessage = "No se pudo obtener el usuario.";
            }
            else if (sesion.ProfileImage != usuario.ImagenPerfil)
            {
                sesion.ProfileImage = usuario.ImagenPerfil;
                SessionHelper.CreateSession(JsonConvert.SerializeObject(sesion));
            }

            return View("~/Views/User/MyProfile.cshtml", usuario);
        }

        #region Catálogo de usuarios

        public async Task<ActionResult> Users(long id = 0)
        {
            var usuario = new Usuario();

            if (id > 0)
            {
                var encontrado = await _usuarioService.ObtenerUsuarioPorId(id);
                if (encontrado != null)
                {
                    usuario = encontrado;
                }
                else
                {
                    ViewBag.ErrorMessage = "No se pudo obtener el usuario.";
                }
            }

            var rolesResponse = await _rolService.ObtenerTodosLosRoles();
            ViewBag.Roles = rolesResponse.IsSuccess && rolesResponse.Response != null
                ? rolesResponse.Response
                : new List<Rol>();
            ViewBag.EmpresaId = tokenCookie != null ? tokenCookie.EmpresaID : 0;

            return View("~/Views/User/Users.cshtml", usuario);
        }

        public async Task<string> ConsultarTodosLosUsuarios()
        {
            var response = await _usuarioService.ObtenerUsuarios();
            return JsonConvert.SerializeObject(response);
        }

        public async Task<string> ConsultarUsuarioPorId(long id)
        {
            var response = await _usuarioService.ObtenerUsuarioPorId(id);
            return JsonConvert.SerializeObject(response);
        }

        [Permiso("Usuarios")]
        public async Task<string> GuardarOActualizarUsuarioAdmin(Usuario usuario)
        {
            if (tokenCookie != null)
            {
                usuario.EmpresaId = tokenCookie.EmpresaID;
            }

            var response = await _usuarioService.GuardarOActualizarUsuarioAdmin(usuario);

            if (response.IsSuccess && response.Response != null)
            {
                var rolId = Request.Form["RolId"];
                if (!string.IsNullOrEmpty(rolId))
                {
                    var usuarioRoles = await _rolService.ObtenerUsuarioRolesPorUsuario(response.Response.Id);
                    if (usuarioRoles.IsSuccess && usuarioRoles.Response != null)
                    {
                        foreach (var ur in usuarioRoles.Response)
                        {
                            await _rolService.EliminarRolUsuario(ur.Id);
                        }
                    }

                    await _rolService.AsignarRolUsuario(response.Response.Id, Convert.ToInt64(rolId));
                }
            }

            return JsonConvert.SerializeObject(response);
        }

        [Permiso("Usuarios", "Eliminar")]
        public async Task<string> EliminarUsuarioAdmin(Usuario usuario)
        {
            usuario.ModificadoPor = tokenCookie != null ? tokenCookie.UserName : "system";
            usuario.FechaModificacion = DateTime.Now;

            var response = await _usuarioService.EliminarUsuario(usuario);
            return JsonConvert.SerializeObject(response);
        }

        #endregion
    }
}

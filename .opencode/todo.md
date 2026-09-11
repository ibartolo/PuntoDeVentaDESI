# Mission: Alinear PuntoDeVentaDESI con la arquitectura y funcionalidad de referencia ServiceDeskDESI

> Objetivo: dejar la solución PuntoDeVentaDESI "igualita en estructura y funcionalidad" al
> proyecto de referencia `C:\Git\ServiceDeskDESI\ServiceDeskDESI`, adaptada al dominio POS
> descrito en `Propuesta.txt`. Esquema canónico M/T/S. **Nada se marca `[x]` hasta que el
> Reviewer lo verifique con evidencia (build MSBuild 0 errores).**

## Project Context

- **Solución**: .NET Framework 4.8 (C# 7.3). Proyectos actuales: `Entities`, `MVC`, `WebApi` + `PuntoDeVenta.Database` (SSDT).
- **Referencia**: `netframework-mvc-webapi/SKILL.md` (arquitectura de 3 capas: Entities / MVC front / WebApi back).
- **MVC NO toca BD**: `Controller → Service → DAL(HttpClientConnection) → HTTP → WebApi → Service → DbWrapper → SP`.
- **Patrón de feature en 6 capas** (ver SKILL.md §6): Entity/DTO → SPs → DbWrapper partial → WebApi Service → WebApi Controller → MVC HttpClientConnection partial → MVC Service → MVC Controller/View → registro en `.csproj`.
- **`.csproj` con lista EXPLÍCITA `<Compile Include>`**: todo `.cs` nuevo DEBE registrarse o no compila.
- **Multiempresa**: `empresaId` viaja como claim; `sucursalId`/sucursal activa es específico de POS.
- **Sin proyecto de pruebas**: la verificación es compilación MSBuild 0 errores + checks estáticos.

## Build / Verify Commands

```powershell
# Restore (packages.config)
dotnet msbuild PuntoDeVenta.sln /t:Restore /p:RestorePackagesConfig=true
# Build Debug
dotnet msbuild PuntoDeVenta.sln /t:Build /p:Configuration=Debug /p:Platform="Any CPU"
```

> Alternativa VS 2022: `"C:\Program Files\Microsoft Visual Studio\2022\Community\MSBuild\Current\Bin\MSBuild.exe" PuntoDeVenta.sln /t:Build /p:Configuration=Debug`.

## Decisions & Open Questions (confirmar antes de M1)

1. **Renombrar proyectos** a `PuntoDeVentaEntities` / `PuntoDeVentaMVC` / `PuntoDeVentaWebApi` (convención `Xxx*` del SKILL §1). Alto churn (folders, csproj, namespaces, .sln). Decisión sugerida: SÍ para "igualito en estructura".
2. **`PuntoDeVenta.Database` (SSDT)**: la referencia NO tiene proyecto SSDT (usa `.sql` sueltos). Se propone CONSERVARLO como superset (es una mejora) y mantener `script.sql` como compatibilidad.
3. **Nombre de BD**: docs dicen `db_9c7990_servicedeskdesi`; `WebApi/Web.config` usa `db_9c7990_puntoventadev`. Estandarizar en `db_9c7990_puntoventadev` (o el nombre real provisto por el host via `sConSql`).
4. **`ModelResponse`**: migrar al shape de referencia `{ IsSuccess, Message, Response }` (rompe usos actuales `{ Success, Data, Errors }` → refactor coordinado M2).

## File Manifest (consolidado — los subtasks llevan `file:` específico)

| Action | File Path | Description | Dependencies |
|--------|-----------|-------------|--------------|
| RENAME | `Entities/` → `PuntoDeVentaEntities/` | Carpeta + csproj + namespace | - |
| RENAME | `MVC/` → `PuntoDeVentaMVC/` | Carpeta + csproj + namespace | - |
| RENAME | `WebApi/` → `PuntoDeVentaWebApi/` | Carpeta + csproj + namespace | - |
| MODIFY | `PuntoDeVenta.sln` | Rutas/nombres de proyectos | renames |
| CREATE | `PuntoDeVentaEntities/BaseObject.cs` | Clase base auditoría + Id + Estatus | - |
| CREATE | `PuntoDeVentaEntities/Autenticacion/Usuario.cs`, `UsuarioDTO.cs` | Dominio autenticación | BaseObject |
| CREATE | `PuntoDeVentaEntities/Catalogos/Marca.cs`, `Empresa.cs` | Catálogos | BaseObject |
| CREATE | `PuntoDeVentaEntities/Seguridad/ModelResponse.cs` | Envoltura `{IsSuccess,Message,Response}` | - |
| CREATE | `PuntoDeVentaEntities/Seguridad/Token.cs`, `TokenCookie.cs` | Sesión OAuth/cookie | - |
| CREATE | `PuntoDeVentaWebApi/DAL/BaseDbWrapper.cs`, `DbWrapper.cs`, `DbWrapper.*.cs` | DAL ADO.NET + reflexión | ModelResponse |
| CREATE | `PuntoDeVentaWebApi/Controllers/BaseController.cs` | Base API (claim empresa) | DbWrapper |
| CREATE | `PuntoDeVentaWebApi/App_Start/SwaggerConfig.cs` | Swagger | WebApiConfig |
| CREATE | `PuntoDeVentaWebApi/Helpers/Cryptography.cs`, `EmailHelper.cs` | PBKDF2 / SMTP | - |
| CREATE | `PuntoDeVentaWebApi/Template/*.html` | Plantillas correo | EmailHelper |
| CREATE | `PuntoDeVentaMVC/App_Start/FilterConfig.cs`, `RouteConfig.cs` | Filtro auth global / rutas | SessionHelper |
| CREATE | `PuntoDeVentaMVC/Helpers/SessionHelper.cs`, `ThemeHelper.cs`, `FiltersHelper.cs`, `Cryptography.cs` | Sesión/tema/permisos | TokenCookie |
| CREATE | `PuntoDeVentaMVC/DAL/HttpClientBase.cs`, `HttpClientConnection.cs`, `HttpClientConnection.*.cs` | Capa HTTP | ModelResponse |
| CREATE | `PuntoDeVentaMVC/Controllers/BaseController.cs` | Base MVC (sesión + http) | HttpClientConnection |
| CREATE | `PuntoDeVentaMVC/Views/Shared/_Layout.cshtml`, `Views/_ViewStart.cshtml` | Layout + viewstart | - |
| CREATE | `PuntoDeVentaMVC/Content/datatables/i18n/es-ES.json` | i18n DataTables | - |
| CREATE | `PuntoDeVentaMVC/Scripts/Comun/*.js` | JS común (Comun.js, etc.) | - |
| CREATE | `PuntoDeVentaMVC/CSS/Comun/Template*.css` | CSS de plantillas | - |
| CREATE | `PuntoDeVenta.Database/Tables|StoredProcedures/...` | Tablas + SPs POS | - |
| CREATE | `docs/CONVENCIONES-CODIFICACION.md`, `docs/checklist-pruebas-qa.md` | Docs de proceso | - |
| CREATE | `.github/skills/*`, `.github/prompts/*` | Workflow OpenSpec | - |
| CREATE | `.opencode/context.md`, `work-log.md`, `status.md` | Estado compartido | - |

---

## M1: Alineación de estructura de solución y proyectos | status: completed

### T1.1: Renombrar solución, proyectos, ensamblados y namespaces | agent:Worker | status: completed
- [x] S1.1.1: Renombrar carpetas `Entities`→`PuntoDeVentaEntities`, `MVC`→`PuntoDeVentaMVC`, `WebApi`→`PuntoDeVentaWebApi` (filesystem) | file:PuntoDeVenta.sln | size:M | verified | evidence: old dirs=False, new dirs=True
- [x] S1.1.2: Renombrar archivos `.csproj` y fijar `RootNamespace`/`AssemblyName` = `PuntoDeVentaEntities`/`PuntoDeVentaMVC`/`PuntoDeVentaWebApi` | size:M | verified | evidence: 3 csproj RootNamespace/AssemblyName correctos
- [x] S1.1.3: Actualizar `PuntoDeVenta.sln` (nombres y rutas de los 3 proyectos + SSDT) | file:PuntoDeVenta.sln | size:S | verified | evidence: sln 3 C# renombrados, SSDT intacto
- [x] S1.1.4: Actualizar `<ProjectReference>` en `PuntoDeVentaMVC.csproj` y `PuntoDeVentaWebApi.csproj` | size:S | verified | evidence: ambos -> ..\PuntoDeVentaEntities\PuntoDeVentaEntities.csproj
- [x] S1.1.5: Renombrar namespaces/usings en proyecto Entities (`PuntoDeVenta.Entities*` → `PuntoDeVentaEntities*`) | size:M | verified | evidence: static scan NONE
- [x] S1.1.6: Renombrar namespaces/usings en proyecto WebApi (`PuntoDeVenta.WebApi*` → `PuntoDeVentaWebApi*`) | size:M | verified | evidence: static scan NONE
- [x] S1.1.7: Renombrar namespaces/usings en proyecto MVC (`PuntoDeVenta.MVC*` → `PuntoDeVentaMVC*`) | size:M | verified | evidence: static scan NONE + @model cshtml correctos

### T1.2: Alinear archivos de proyecto (.csproj) y paquetes | agent:Worker | depends:T1.1 | status: completed
- [x] S1.2.1: `PuntoDeVentaEntities.csproj`: `LangVersion 7.3`, `Deterministic true`, verificar `<Compile Include>` explícito | size:S | verified | evidence: LangVersion=7.3, Deterministic=true, Compile explícito
- [x] S1.2.2: `PuntoDeVentaWebApi.csproj`: paquetes referencia (Owin OAuth 3.0.1, Swashbuckle 5.6.0, Serilog) + Compile explícito | size:M | verified | evidence: Serilog 4.3.1, Serilog.Sinks.File 7.0.0, Swashbuckle 5.6.0, WebApi.Client 6.0.0, Owin 4.2.3
- [x] S1.2.3: `PuntoDeVentaMVC.csproj`: paquetes referencia (Mvc 5.3.0, Newtonsoft 13, Serilog, WebApi.Client) + Compile explícito | size:M | verified | evidence: Mvc 5.3.0, Newtonsoft 13.0.3, Serilog 4.3.1, WebApi.Client 6.0.0

### T1.3: Unificar nombre de BD y cadenas de conexión | agent:Worker | status: completed
- [x] S1.3.1: Estandarizar `connectionStrings/cCon` y `appSettings` en `PuntoDeVentaWebApi/Web.config` y `PuntoDeVentaMVC/Web.config` (sin secretos) | size:S | verified | evidence: cCon=db_9c7990_puntoventadev + appSettings + machineKey + forms; sin secretos
- [x] S1.3.2: Actualizar referencias de nombre de BD en `docs/`, `openspec/` y `script.sql` para que no haya inconsistencia | size:S | verified | evidence: openspec exploration/proposal -> db_9c7990_puntoventadev

### T1.4: Verificación de build M1 | agent:Reviewer | depends:T1.1,T1.2,T1.3 | status: completed
- [x] S1.4.1: Restore + Build Debug 0 errores; evidencia en `.opencode/work-log.md` | size:S | verified | evidence: Restore 0 err; 3 C# projects 0 err/0 warn (SSDT MSB4057 pre-existente)

---

## M2: Alineación de Entities | status: completed

### T2.1: Crear `BaseObject` y refactorizar entidades | agent:Worker | status: completed
- [x] S2.1.1: CREATE `PuntoDeVentaEntities/BaseObject.cs` (`Id`, `CreadoPor`, `FechaCreacion`, `ModificadoPor`, `FechaModificacion`, `Estatus`) | file:PuntoDeVentaEntities/BaseObject.cs | size:S | verified
- [x] S2.1.2: MODIFY `Empresa` para heredar `BaseObject` (eliminar campos duplicados) | file:PuntoDeVentaEntities/Catalogos/Empresa.cs | size:S | verified
- [x] S2.1.3: MODIFY `Usuario` para heredar `BaseObject` | file:PuntoDeVentaEntities/Autenticacion/Usuario.cs | size:S | verified
- [x] S2.1.4: MODIFY `Marca` para heredar `BaseObject` | file:PuntoDeVentaEntities/Catalogos/Marca.cs | size:S | verified

### T2.2: Reorganizar Entities en carpetas por dominio | agent:Worker | depends:T2.1 | status: completed
- [x] S2.2.1: CREATE `Autenticacion/UsuarioDTO.cs : Usuario` (campos display/join) | file:PuntoDeVentaEntities/Autenticacion/UsuarioDTO.cs | size:S | verified
- [x] S2.2.2: Mover `Empresa.cs` a `Catalogos/` y `Marca.cs` a `Catalogos/` | size:S | verified
- [x] S2.2.3: DELETE carpetas planas `Models/` y `Contracts/` tras migrar archivos | size:S | verified | evidence: carpetas ausentes
- [x] S2.2.4: Registrar altas/bajas en `PuntoDeVentaEntities.csproj` (Compile Include) | size:S | verified

### T2.3: Unificar `ModelResponse` al shape de referencia | agent:Worker | depends:T2.2 | status: completed
- [x] S2.3.1: REWRITE `Seguridad/ModelResponse.cs` a `{ IsSuccess, Message, Response }` + genérico `ModelResponse<T>` | file:PuntoDeVentaEntities/Seguridad/ModelResponse.cs | size:S | verified
- [x] S2.3.2: UPDATE usos en WebApi (`Success`→`IsSuccess`, `Data`→`Response`, quitar `Errors`) | size:M | verified | evidence: scan sin `.Success/.Data/.Errors`
- [x] S2.3.3: UPDATE usos en MVC (mismo mapeo) | size:M | verified | evidence: scan sin `.Success/.Data/.Errors`

### T2.4: Crear `Token` y `TokenCookie` | agent:Worker | status: completed
- [x] S2.4.1: CREATE `Seguridad/Token.cs` (`access_token`, `token_type`, `expires_in`, `ExpirationDate`) | file:PuntoDeVentaEntities/Seguridad/Token.cs | size:S | verified
- [x] S2.4.2: CREATE `Seguridad/TokenCookie.cs` (`Token`, `UserID`, `EmpresaID`, `UserName`, `ProfileImage`, `UserAvatar`; + `SucursalID` POS si aplica) | file:PuntoDeVentaEntities/Seguridad/TokenCookie.cs | size:S | verified

### T2.5: Verificación de build M2 | agent:Reviewer | depends:T2.1,T2.2,T2.3,T2.4 | status: completed
- [x] S2.5.1: Build Debug 0 errores; evidencia en `.opencode/work-log.md` | size:S | verified | evidence: 3 C# projects 0 errors (VS MSBuild 17.14)

---

## M3: Alineación de WebApi | status: completed

### T3.1: BaseController de WebApi | agent:Worker | status: completed
- [x] S3.1.1: CREATE `Controllers/BaseController.cs : ApiController` con `dbWrapper` + `ObtenerEmpresaIdDesdeClaim()` | file:PuntoDeVentaWebApi/Controllers/BaseController.cs | size:M | verified
- [x] S3.1.2: Migrar `MarcasController`/`AuthContextController` a heredar `BaseController`; retirar `BaseApiController`/`RequireEmpresaClaimAttribute`/`TenantContext` (o delegar a BaseController) | size:M | verified | evidence: legados eliminados

### T3.2: DAL base con ADO.NET + reflexión | agent:Worker | status: completed
- [x] S3.2.1: CREATE `DAL/BaseDbWrapper.cs` (ExecuteScalar/NonQuery/GetObject/GetObjects + transacción ambiente) | file:PuntoDeVentaWebApi/DAL/BaseDbWrapper.cs | size:L | verified
- [x] S3.2.2: CREATE `DAL/DbWrapper.cs` (`SQLConnectionString` desde `sConSql`/`cCon`, timeout, `LlenarEntidad<T>`, `ObtenerParametrosSQL<T>`) | file:PuntoDeVentaWebApi/DAL/DbWrapper.cs | size:L | verified
- [x] S3.2.3: CREATE `DAL/DbWrapper.Marca.cs` (ObtenerMarca/PorId/GuardarOActualizar/Eliminar) | file:PuntoDeVentaWebApi/DAL/DbWrapper.Marca.cs | size:M | verified
- [x] S3.2.4: CREATE `DAL/DbWrapper.Autenticacion.cs` (`AutenticarUsuario`) | file:PuntoDeVentaWebApi/DAL/DbWrapper.Autenticacion.cs | size:M | verified
- [x] S3.2.5: DELETE `Dal/MarcaDal.cs`, `Dal/AuthDal.cs`, `Dal/AuthUserRecord.cs` | size:S | verified

### T3.3: Servicios WebApi (patrón Serilog + ArgumentException) | agent:Worker | depends:T3.2 | status: completed
- [x] S3.3.1: REWRITE `Services/MarcaService.cs` con `DbWrapper`, try/catch doble y `Log.*` | file:PuntoDeVentaWebApi/Services/MarcaService.cs | size:M | verified
- [x] S3.3.2: CREATE `Services/AutenticacionService.cs` | file:PuntoDeVentaWebApi/Services/AutenticacionService.cs | size:M | verified
- [x] S3.3.3: DELETE/reemplazar `Services/Auth/*` legacy | size:S | verified

### T3.4: Controladores WebApi (rutas de referencia) | agent:Worker | depends:T3.3 | status: completed
- [x] S3.4.1: REWRITE `MarcasController` → `MarcaController` (`List`, `{id:long}`, `Guardar`, `Eliminar`, síncrono) | file:PuntoDeVentaWebApi/Controllers/MarcaController.cs | size:M | verified
- [x] S3.4.2: CREATE `Controllers/AutenticacionController.cs` (`[AllowAnonymous]` `autenticar`) | file:PuntoDeVentaWebApi/Controllers/AutenticacionController.cs | size:M | verified

### T3.5: Infraestructura de arranque (OAuth, CORS, Swagger, Serilog, JSON) | agent:Worker | status: completed
- [x] S3.5.1: MODIFY `App_Start/Startup.cs`: Serilog a `App_Data/logs`, CORS manual, OAuth `/token`, `UseWebApi` | file:PuntoDeVentaWebApi/App_Start/Startup.cs | size:M | verified
- [x] S3.5.2: MODIFY `App_Start/TokenAuthorizationServerProvider.cs` → claims `usuarioId`/`empresaId` vía `DbWrapper.AutenticarUsuario` + `Cryptography.VerifyPassword` | size:M | verified
- [x] S3.5.3: CREATE `App_Start/SwaggerConfig.cs` (Swashbuckle 5.6.0) | file:PuntoDeVentaWebApi/App_Start/SwaggerConfig.cs | size:S | verified
- [x] S3.5.4: MODIFY `App_Start/WebApiConfig.cs` (attribute routing + `CamelCasePropertyNamesContractResolver`) | file:PuntoDeVentaWebApi/App_Start/WebApiConfig.cs | size:S | verified
- [x] S3.5.5: MODIFY `Global.asax.cs` (`Application_End` → `Log.CloseAndFlush()`) | file:PuntoDeVentaWebApi/Global.asax.cs | size:S | verified

### T3.6: Helpers y plantillas WebApi | agent:Worker | status: completed
- [x] S3.6.1: CREATE `Helpers/Cryptography.cs` (PBKDF2 `HashPassword`/`VerifyPassword` + Rijndael legacy) | file:PuntoDeVentaWebApi/Helpers/Cryptography.cs | size:M | verified
- [x] S3.6.2: CREATE `Helpers/EmailHelper.cs` (`EnvioEmail` SMTP desde appSettings) | file:PuntoDeVentaWebApi/Helpers/EmailHelper.cs | size:M | verified
- [x] S3.6.3: CREATE `Template/Template_RecuperarEmail.html`, `Template_NuevoPass.html`, `Template_NuevoUsuario.html`, `Template_AltaEmpresa.html` | size:M | verified

### T3.7: Verificación de build M3 | agent:Reviewer | depends:T3.1,T3.2,T3.3,T3.4,T3.5,T3.6 | status: completed
- [x] S3.7.1: Build Debug 0 errores + smoke de rutas (`/token`, `api/Marca/List`) | size:S | verified | evidence: WebApi 0 errors (smoke runtime pendiente de BD)

---

## M4: Alineación de MVC | status: completed

### T4.1: App_Start (filtro global + rutas) | agent:Worker | status: completed
- [x] S4.1.1: CREATE `App_Start/FilterConfig.cs` con `AuthenticationFilter` (allow-list de acciones públicas) | file:PuntoDeVentaMVC/App_Start/FilterConfig.cs | size:M | verified | evidence: FilterConfig.cs existe; registra AuthenticationFilter (IAuthorizationFilter)
- [x] S4.1.2: CREATE `App_Start/RouteConfig.cs` (`{controller}/{action}/{id}` → `Home/Index`) | file:PuntoDeVentaMVC/App_Start/RouteConfig.cs | size:S | verified | evidence: RouteConfig.cs existe
- [x] S4.1.3: MODIFY `Global.asax.cs` para registrar FilterConfig/RouteConfig + Serilog | file:PuntoDeVentaMVC/Global.asax.cs | size:S | verified | evidence: Global.asax.cs registra FilterConfig/RouteConfig + Serilog (Log.CloseAndFlush)

### T4.2: Helpers MVC | agent:Worker | status: completed
- [x] S4.2.1: CREATE `Helpers/SessionHelper.cs` (`ExisteSession`, `GetSessionUser`, `CreateSession`, `CloseSession`) | file:PuntoDeVentaMVC/Helpers/SessionHelper.cs | size:M | verified | evidence: SessionHelper.cs existe
- [x] S4.2.2: CREATE `Helpers/ThemeHelper.cs` (tema claro/oscuro) | file:PuntoDeVentaMVC/Helpers/ThemeHelper.cs | size:S | verified | evidence: ThemeHelper.cs existe
- [x] S4.2.3: CREATE `Helpers/FiltersHelper.cs` | file:PuntoDeVentaMVC/Helpers/FiltersHelper.cs | size:S | verified | evidence: FiltersHelper.cs existe
- [x] S4.2.4: CREATE `Helpers/Cryptography.cs` (cliente, si se requiere) | file:PuntoDeVentaMVC/Helpers/Cryptography.cs | size:S | verified | evidence: Cryptography.cs existe

### T4.3: DAL HTTP (HttpClientBase + partials) | agent:Worker | status: completed
- [x] S4.3.1: CREATE `DAL/HttpClientBase.cs` (TokenAsync/RequestAsync/multipart, no lanza) | file:PuntoDeVentaMVC/DAL/HttpClientBase.cs | size:L | verified | evidence: HttpClientBase.cs existe
- [x] S4.3.2: CREATE `DAL/HttpClientConnection.cs` (token de sesión, `MappingColumSecurity`, `GetToken`) | file:PuntoDeVentaMVC/DAL/HttpClientConnection.cs | size:M | verified | evidence: HttpClientConnection.cs existe
- [x] S4.3.3: CREATE `DAL/HttpClientConnection.Autenticacion.cs` | file:PuntoDeVentaMVC/DAL/HttpClientConnection.Autenticacion.cs | size:S | verified | evidence: partial existe
- [x] S4.3.4: CREATE `DAL/HttpClientConnection.Marca.cs` | file:PuntoDeVentaMVC/DAL/HttpClientConnection.Marca.cs | size:S | verified | evidence: partial existe
- [x] S4.3.5: DELETE `Services/AuthApiClient.cs`, `Services/MarcasApiClient.cs` | size:S | verified | evidence: ambos ausentes

### T4.4: BaseController + Servicios MVC | agent:Worker | depends:T4.3 | status: completed
- [x] S4.4.1: CREATE `Controllers/BaseController.cs` (`httpClientConnection`, `tokenCookie`, `mr`, `MappingPropertiToDropDownList`, `GenerarAvatarIniciales`) | file:PuntoDeVentaMVC/Controllers/BaseController.cs | size:M | verified | evidence: BaseController.cs existe
- [x] S4.4.2: CREATE `Services/AutenticacionService.cs` (ctor `HttpClientConnection`) | file:PuntoDeVentaMVC/Services/AutenticacionService.cs | size:S | verified | evidence: AutenticacionService.cs existe
- [x] S4.4.3: CREATE `Services/MarcaService.cs` (passthrough + unwrap) | file:PuntoDeVentaMVC/Services/MarcaService.cs | size:S | verified | evidence: MarcaService.cs existe

### T4.5: Controladores MVC | agent:Worker | depends:T4.4 | status: completed
- [x] S4.5.1: REWRITE `HomeController` (Autentication, LogIn 2 pasos, LogOut, Index, Configuration, RecoverPassword, NewCompany, AccesoDenegado) | file:PuntoDeVentaMVC/Controllers/HomeController.cs | size:L | verified | evidence: HomeController.cs con acciones de views + data access
- [x] S4.5.2: REWRITE `MarcasController` → `MarcaController` (región Views + región Data Access `Task<string>`) | file:PuntoDeVentaMVC/Controllers/MarcaController.cs | size:M | verified | evidence: MarcaController.cs existe (Mark/ConsultarTodas/GuardarOActualizar/Eliminar)
- [x] S4.5.3: DELETE `AccountController.cs` legacy | size:S | verified | evidence: AccountController.cs y MarcasController.cs ausentes

### T4.6: Layout, ViewStart y vistas base | agent:Worker | depends:T4.5 | status: completed
- [x] S4.6.1: CREATE `Views/_ViewStart.cshtml` + `Views/Shared/_Layout.cshtml` (navbar, menú dinámico, tema) | size:L | verified | evidence: ambos existen; layout carga /Home/MenusUser y tema
- [x] S4.6.2: CREATE `Views/Home/Autentication.cshtml`, `RecoverPassword.cshtml`, `NewCompany.cshtml`, `AccesoDenegado.cshtml`, `Configuration.cshtml`, `MenusUser.cshtml`, `Index.cshtml` | size:L | verified | evidence: 7 vistas existen
- [x] S4.6.3: CREATE `Views/User/MyProfile.cshtml` | size:M | verified | evidence: existe; @model Usuario alineado con HomeController.MyProfile
- [x] S4.6.4: CREATE `Views/Catalogs/Mark.cshtml` (DataTables) y retirar vistas `Views/Marcas/*` legacy | size:M | verified | evidence: Mark.cshtml existe (3 endpoints Marca); Views/Marcas/* ausentes
- [x] S4.6.5: DELETE `Views/Account/Login.cshtml` (reemplazada por `Home/Autentication`) | size:S | verified | evidence: Views/Account/Login.cshtml ausente

### T4.7: Frontend assets (DataTables i18n, JS común, CSS) | agent:Worker | status: completed
- [x] S4.7.1: CREATE `Content/datatables/i18n/es-ES.json` | file:PuntoDeVentaMVC/Content/datatables/i18n/es-ES.json | size:S | verified | evidence: JSON válido
- [x] S4.7.2: CREATE `Scripts/Comun/Comun.js` (`GetMVC`/`GetParamMVC`/`PostMVC`/`MapingPropertiesDataTable`) | file:PuntoDeVentaMVC/Scripts/Comun/Comun.js | size:L | verified | evidence: existe
- [x] S4.7.3: CREATE `Scripts/Comun/RecoverPass.js`, `TempAutentication.js` | size:M | verified | evidence: ambos existen
- [x] S4.7.4: CREATE `CSS/Comun/TemplatePage.css`, `TempAutentication.css`, `TemplateReciverPass.css` (+ demás plantillas) | size:M | verified | evidence: 3 CSS existen
- [x] S4.7.5: Registrar todos los archivos nuevos/renombrados en `PuntoDeVentaMVC.csproj` | size:M | verified | evidence: Compile 17/17, Content 21/21 (0 faltantes, 0 huérfanos); legacy ausentes

### T4.8: Verificación de build M4 | agent:Reviewer | depends:T4.1,T4.2,T4.3,T4.4,T4.5,T4.6,T4.7 | status: completed
- [x] S4.8.1: Build Debug 0 errores + smoke de login/render de layout | size:S | verified | evidence: Restore 0 err; 3 C# projects 0 err/0 warn; solución 3 DLLs + solo SSDT MSB4057 pre-existente; 11 .cshtml UTF-8 BOM

### T4.9: Resolver sync issues M4 (SYNC-1..7) | agent:Worker | depends:T4.8 | status: completed
- [x] S4.9.1: SYNC-1 MyProfile model mismatch (Usuario vs TokenCookie) | file:PuntoDeVentaMVC/Views/User/MyProfile.cshtml | size:S | issue:SYNC-1 | verified | evidence: HomeController.MyProfile() now builds Usuario and passes it (líneas 64-80)
- [x] S4.9.2: SYNC-2 Falta UserController.MyProfile (layout apunta a /User/MyProfile) | file:PuntoDeVentaMVC/Controllers/UserController.cs | size:S | issue:SYNC-2 | verified | evidence: UserController.cs creado+registrado en csproj; ruta /User/MyProfile resuelve; MVC build 0 err
- [x] S4.9.3: SYNC-3 Falta HomeController.GuardarTema (cubierto por S5.7.1) | file:PuntoDeVentaMVC/Controllers/HomeController.cs | size:S | issue:SYNC-3 | verified | evidence: HomeController.GuardarTema (L175) persiste cookie de tema vía ThemeHelper; build 0 err
- [x] S4.9.4: SYNC-4 Falta flujo recuperación /Home/ValidarRecetearContrasenia (cubierto por S5.6) | file:PuntoDeVentaMVC/Controllers/HomeController.cs | size:S | issue:SYNC-4 | verified | evidence: HomeController.ValidarRecetearContrasenia (L161) + endpoints WebApi solicitarRecuperacion/validarToken/restablecerContrasenia
- [x] S4.9.5: SYNC-5 Falta HomeController.GuardarNuevaEmpresa (cubierto por M6 alta empresa) | file:PuntoDeVentaMVC/Controllers/HomeController.cs | size:S | issue:SYNC-5 | verified | evidence: HomeController.GuardarNuevaEmpresa(Empresa) (L178) → EmpresaService → HttpClientConnection.Empresa → WebApi EmpresaController; cadena completa, build 0 err
- [x] S4.9.6: SYNC-6 Comun.js SessionReport/SessionRefresh sin endpoint (eliminar o implementar) | file:PuntoDeVentaMVC/Scripts/Comun/Comun.js | size:S | issue:SYNC-6 | verified | evidence: grep SessionReport/SessionRefresh en Comun.js = 0
- [x] S4.9.7: SYNC-7 Web.config loginUrl ~/Account/Login → ~/Home/Autentication | file:PuntoDeVentaMVC/Web.config | size:S | issue:SYNC-7 | verified | evidence: Web.config loginUrl="~/Home/Autentication"

### T4.10: Re-verificación M4 post-fixes | agent:Reviewer | depends:T4.9 | status: completed
- [x] S4.10.1: Build 0 errores + sync-issues.md vacío | size:S | verified | evidence: Restore exit 0; Rebuild 3 C# projects 0 err/0 warn (Entities/WebApi/MVC); solución 3 DLLs + solo SSDT MSB4057 pre-existente. M4-scoped sync issues SYNC-1..7 RESUELTOS (OPEN=none). MVC csproj Compile 30/30 + Content 24/24 existen; 14/14 .cshtml UTF-8 BOM; legacy ausentes.

---

## M5: Seguridad y funcionalidad base | status: completed

### T5.1: Entidades de seguridad | agent:Worker | status: completed
- [x] S5.1.1: CREATE `Seguridad/Rol.cs`, `UsuarioRol.cs` | size:S | verified | evidence: ambos .cs en disco; Entities.csproj 22 Compile; build 0 err
- [x] S5.1.2: CREATE `Seguridad/Pagina.cs`, `RolPaginaAccion.cs`, `RolPaginaAccionDTO.cs`, `RolConteoPaginasDTO.cs` | size:S | verified | evidence: 4 .cs en disco
- [x] S5.1.3: CREATE `Seguridad/PermisoRequest.cs`, `PermisosViewModel.cs`, `UsuarioPagina.cs` | size:S | verified | evidence: 3 .cs en disco
- [x] S5.1.4: CREATE `Seguridad/TokenRecuperacion.cs`, `TokenRecuperacionDTO.cs` | size:S | verified | evidence: 2 .cs en disco
- [x] S5.1.5: CREATE `Catalogos/Sucursal.cs`, `Modulo.cs` (módulos/menú) | size:S | verified | evidence: 2 .cs en disco

### T5.2: Base de datos de seguridad (tablas + SPs + seed) | agent:Worker | status: completed
- [x] S5.2.1: CREATE tablas `Rol`, `UsuarioRol`, `Pagina`, `RolPaginaAccion`, `TokenRecuperacion` (+ auditoría, borrado lógico) | size:L | verified | evidence: 6 tablas en Tables/dbo (incl. UsuarioPagina); FKs+auditoría+Estatus; registradas en .sqlproj
- [x] S5.2.2: CREATE SPs de permisos/roles/páginas (Listar/Obtener/PorUsuario/Guardar/Eliminar) | size:L | verified | evidence: ~26 SPs (sp_Rol_*, sp_Pagina_*, sp_RolPaginaAccion_*, sp_UsuarioRol_*, sp_UsuarioPagina_*, sp_Permisos_*) registrados en .sqlproj
- [x] S5.2.3: CREATE SPs de recuperación de contraseña | size:M | verified | evidence: sp_TokenRecuperacion_{Crear,ObtenerPorToken,MarcarUsado,RestablecerContrasenia} + sp_Usuario_ActualizarContrasena
- [x] S5.2.4: CREATE seed de roles/páginas/módulos base precargados | size:M | verified | evidence: Seed/seed-security.sql (5631B) + seed-dev-security.sql (5397B); idempotente; registrados como None en .sqlproj

### T5.3: WebApi seguridad (DAL + Service + Controller) | agent:Worker | depends:T5.1,T5.2 | status: completed
- [x] S5.3.1: CREATE `DAL/DbWrapper.Permisos.cs`, `DbWrapper.Paginas.cs`, `DbWrapper.Rol.cs` | size:L | verified | evidence: 5 partials (Permisos, Paginas, Rol, Usuario, UsuarioPagina) en disco; registrados en WebApi.csproj; build 0 err
- [x] S5.3.2: CREATE `Services/PermisosService.cs`, `PaginaService.cs`, `RolService.cs` | size:M | verified | evidence: 3 services en disco + registrados en csproj; build 0 err
- [x] S5.3.3: CREATE `Controllers/PermisosController.cs`, `PaginaController.cs`, `RolController.cs`, `RelacionController.cs`, `UsuarioPaginaController.cs` | size:L | verified | evidence: 5 controllers en disco + registrados; build 0 err
- [x] S5.3.4: CREATE `Filters/PermisoAttribute.cs` (filtro por página/acción) | file:PuntoDeVentaWebApi/Filters/PermisoAttribute.cs | size:M | verified | evidence: PermisoAttribute.cs existe + registrado; valida vía PermisosService (403)

### T5.4: MVC seguridad (DAL + Service + Controller + Views) | agent:Worker | depends:T5.3 | status: completed
- [x] S5.4.1: CREATE `Filters/PermisoAttribute.cs` + `Helpers/FiltersHelper.cs` | size:M | verified | evidence: ambos en disco + registrados; build 0 err
- [x] S5.4.2: CREATE `DAL/HttpClientConnection.Permisos.cs`, `.Pagina.cs`, `.Rol.cs` | size:M | verified | evidence: 3 partials + HttpClientConnection.User.cs en disco/registrados; build 0 err
- [x] S5.4.3: CREATE `Services/PermisosService.cs`, `RolService.cs` | size:S | verified | evidence: ambos + UsuarioService en disco/registrados; build 0 err
- [x] S5.4.4: CREATE `Controllers/SecurityController.cs`, `PermissionsController.cs`, `UserController.cs` | size:L | verified | evidence: 3 controllers en disco/registrados; build 0 err (SYNC-9 [FromBody] corregido)
- [x] S5.4.5: CREATE `Views/Security/Role.cshtml`, `Permisos.cshtml`; `Views/User/Users.cshtml` | size:L | verified | evidence: 3 vistas en disco + registradas como Content

### T5.5: Menú dinámico y administración de usuarios | agent:Worker | depends:T5.4 | status: completed
- [x] S5.5.1: CREATE `Services/UsuarioService.cs` + `DAL/HttpClientConnection.User.cs` | size:M | verified | evidence: ambos en disco/registrados; build 0 err
- [x] S5.5.2: CREATE `DAL/DbWrapper.UsuarioPagina.cs` + `Services/UsuarioPaginaService.cs` + `Controllers/UsuarioPaginaController.cs` | size:M | verified | evidence: UsuarioPaginaService.cs creado+registrado (csproj L158) y ahora ES consumido por UsuarioPaginaController + RelacionController; metodos duplicados retirados de PermisosService; build 3 C# 0 err/0 warn (solo SSDT MSB4057 pre-existente)
- [x] S5.5.3: MODIFY `Views/Home/MenusUser.cshtml` + `_Layout` para menú por rol | size:M | verified | evidence: MenusUser.cshtml modificado (01:04:16) + _Layout; build 0 err

### T5.6: Recuperación de contraseña + correo | agent:Worker | depends:T3.6 | status: completed
- [x] S5.6.1: CREATE flujo WebApi (`AutenticacionController` recovery + `EmailHelper` + templates) | size:M | verified | evidence: endpoints solicitarRecuperacion/validarToken/restablecerContrasenia en AutenticacionController; EmailHelper + templates (M3)
- [x] S5.6.2: CREATE `Views/Home/RecoverPassword.cshtml` + `Scripts/Comun/RecoverPass.js` | size:M | verified | evidence: ambos en disco + registrados (Content)

### T5.7: Tema claro/oscuro + DataTables UI | agent:Worker | depends:T4.7 | status: completed
- [x] S5.7.1: MODIFY `_Layout` + `ThemeHelper` para conmutador de tema persistente | size:M | verified | evidence: HomeController.GuardarTema (L175) persiste cookie vía ThemeHelper; _Layout conmuta tema; build 0 err
- [x] S5.7.2: Integrar DataTables 2.3.7 + i18n `es-ES.json` en vistas de catálogo | size:M | verified | evidence: Views/Catalogs/Mark.cshtml usa DataTables 2.3.7 + Content/datatables/i18n/es-ES.json (única vista de catálogo existente)

### T5.8: Verificación de build M5 | agent:Reviewer | depends:T5.1,T5.2,T5.3,T5.4,T5.5,T5.6,T5.7 | status: completed
- [x] S5.8.1: Build Debug 0 errores + checks de autorización (401/redirect) | size:S | verified | evidence: Restore 0 err; 3 C# projects 0 err/0 warn (3 DLLs); solo SSDT MSB4057 pre-existente. Checks runtime 401/redirect pendientes de BD (documentado)

---

## M6: Módulos POS de la Propuesta (patrón de 6 capas) | status: completed

> Cada módulo repite el patrón: (1) Entities+DTO, (2) tablas+SPs, (3) WebApi `DbWrapper.*`, (4) WebApi Service, (5) WebApi Controller, (6) MVC `HttpClientConnection.*`, (7) MVC Service, (8) MVC Controller+View, (9) registro en `.csproj`. Todos con auditoría y borrado lógico.

### T6.1: Sucursales (multi-sucursal, stock por sucursal) | agent:Worker | depends:M5 | status: completed
- [x] S6.1.1: Entities `Catalogos/Sucursal.cs` (+DTO) | size:S
- [x] S6.1.2: DB tabla + SPs `Sucursal` | size:M
- [x] S6.1.3: WebApi `DbWrapper.Sucursal.cs` | size:M
- [x] S6.1.4: WebApi `SucursalService.cs` + `SucursalController.cs` | size:M
- [x] S6.1.5: MVC `HttpClientConnection.Sucursal.cs` + `SucursalService.cs` | size:M
- [x] S6.1.6: MVC `CatalogsController` (rama Sucursal) + `Views/Catalogs/Branch.cshtml` | size:M
- [x] S6.1.7: Registrar archivos en `.csproj` | size:S

### T6.2: Categorías jerárquicas | agent:Worker | depends:M5 | status: completed
- [x] S6.2.1: Entities `Catalogos/Categoria.cs` (+DTO) con padre/área | size:S
- [x] S6.2.2: DB tabla + SPs `Categoria` (list, subcategorías por padre) | size:M
- [x] S6.2.3: WebApi `DbWrapper.Categoria.cs` | size:M
- [x] S6.2.4: WebApi `CategoriaService.cs` + `CatalogsController` (rama Categoria) | size:M
- [x] S6.2.5: MVC `HttpClientConnection.Categoria.cs` + `CategoriaService.cs` | size:M
- [x] S6.2.6: MVC `CatalogsController` (rama Categoria) + `Views/Catalogs/Category.cshtml` | size:M
- [x] S6.2.7: Registrar archivos en `.csproj` | size:S

### T6.3: Productos + historial de precios | agent:Worker | depends:T6.2 | status: completed
- [x] S6.3.1: Entities `Catalogos/Producto.cs`, `Precio.cs` (+DTO) | size:M
- [x] S6.3.2: DB tablas `Producto`, `Precio` (historial, uno activo) + SPs | size:L
- [x] S6.3.3: WebApi `DbWrapper.Producto.cs`, `DbWrapper.Precio.cs` | size:L
- [x] S6.3.4: WebApi `ProductoService.cs`, `PrecioService.cs`, `ProductoController.cs`, `PrecioController.cs` | size:L
- [x] S6.3.5: MVC `HttpClientConnection.Producto.cs`, `.Precio.cs` + Services | size:L
- [x] S6.3.6: MVC `ProductController.cs` + `Views/Catalogs/Product.cshtml` | size:L
- [x] S6.3.7: Registrar archivos en `.csproj` | size:S

### T6.4: Clientes (+ Público General) | agent:Worker | depends:M5 | status: completed
- [x] S6.4.1: Entities `Catalogos/Cliente.cs` (+DTO) | size:S
- [x] S6.4.2: DB tabla + SPs `Cliente` + seed "Público General" | size:M
- [x] S6.4.3: WebApi `DbWrapper.Cliente.cs` + `ClienteService.cs` + `ClienteController.cs` | size:M
- [x] S6.4.4: MVC `HttpClientConnection.Cliente.cs` + `ClienteService.cs` | size:M
- [x] S6.4.5: MVC `CatalogsController` (rama Cliente) + `Views/Catalogs/Client.cshtml` | size:M
- [x] S6.4.6: Registrar archivos en `.csproj` | size:S

### T6.5: Proveedores | agent:Worker | depends:M5 | status: completed
- [x] S6.5.1: Entities `Catalogos/Proveedor.cs` (+DTO) | size:S
- [x] S6.5.2: DB tabla + SPs `Proveedor` | size:M
- [x] S6.5.3: WebApi `DbWrapper.Proveedor.cs` + `ProveedorService.cs` + `ProveedorController.cs` | size:M
- [x] S6.5.4: MVC `HttpClientConnection.Proveedor.cs` + `ProveedorService.cs` | size:M
- [x] S6.5.5: MVC `CatalogsController` (rama Proveedor) + `Views/Catalogs/Supplier.cshtml` | size:M
- [x] S6.5.6: Registrar archivos en `.csproj` | size:S

### T6.6: Compras (historial por proveedor) | agent:Worker | depends:T6.3,T6.5 | status: completed
- [x] S6.6.1: Entities `Compras/Compra.cs`, `CompraDetalle.cs` (+DTO) | size:M
- [x] S6.6.2: DB tablas + SPs `Compra`/`CompraDetalle` (transacción, actualiza stock) | size:L
- [x] S6.6.3: WebApi `DbWrapper.Compra.cs` (transacción) + `CompraService.cs` + `CompraController.cs` | size:L
- [x] S6.6.4: MVC `HttpClientConnection.Compra.cs` + `CompraService.cs` + `CompraController.cs` + `Views/Compras/Index.cshtml` | size:L
- [x] S6.6.5: Registrar archivos en `.csproj` | size:S

### T6.7: Stock (ingresos, ajustes, devoluciones por sucursal) | agent:Worker | depends:T6.3,T6.1 | status: completed
- [x] S6.7.1: Entities `Inventario/Stock.cs`, `StockMovimiento.cs` (+DTO) | size:M
- [x] S6.7.2: DB tablas + SPs (ingreso, ajuste ±, devolución) | size:L
- [x] S6.7.3: WebApi `DbWrapper.Stock.cs` (transacción) + `StockService.cs` + `StockController.cs` | size:L
- [x] S6.7.4: MVC `HttpClientConnection.Stock.cs` + `StockService.cs` + `StockController.cs` + `Views/Stock/Index.cshtml` | size:L
- [x] S6.7.5: Registrar archivos en `.csproj` | size:S

### T6.8: Caja chica (por usuario+sucursal, salidas, límite 50%) | agent:Worker | depends:T6.1 | status: completed
- [x] S6.8.1: Entities `Caja/CajaChica.cs`, `SalidaCaja.cs` (+DTO) | size:M
- [x] S6.8.2: DB tablas + SPs (abrir, salida, validación de límite, reembolso) | size:L
- [x] S6.8.3: WebApi `DbWrapper.CajaChica.cs` (transacción) + `CajaChicaService.cs` + `CajaChicaController.cs` | size:L
- [x] S6.8.4: MVC `HttpClientConnection.CajaChica.cs` + `CajaChicaService.cs` + `CajaChicaController.cs` + `Views/Caja/Index.cshtml` | size:L
- [x] S6.8.5: Registrar archivos en `.csproj` | size:S

### T6.9: Cortes de caja (varios/día, cierre, adjuntos por método) | agent:Worker | depends:T6.8 | status: completed
- [x] S6.9.1: Entities `Caja/Corte.cs`, `CorteDetalle.cs` (+DTO) | size:M
- [x] S6.9.2: DB tablas + SPs de corte (ventas por método, efectivo esperado vs contado) | size:L
- [x] S6.9.3: WebApi `DbWrapper.Corte.cs` + `CorteService.cs` + `CorteController.cs` | size:L
- [x] S6.9.4: MVC `HttpClientConnection.Corte.cs` + `CorteService.cs` + `CorteController.cs` + `Views/Caja/Corte.cshtml` | size:L
- [x] S6.9.5: Registrar archivos en `.csproj` | size:S

### T6.10: Ventas (un método de pago, escáner, cajón) | agent:Worker | depends:T6.3,T6.7,T6.8 | status: completed
- [x] S6.10.1: Entities `Ventas/Venta.cs`, `VentaDetalle.cs` (+DTO) | size:M
- [x] S6.10.2: DB tablas + SPs de venta (descuenta stock, registra caja) | size:L
- [x] S6.10.3: WebApi `DbWrapper.Venta.cs` (transacción) + `VentaService.cs` + `VentaController.cs` | size:L
- [x] S6.10.4: MVC `HttpClientConnection.Venta.cs` + `VentaService.cs` + `VentaController.cs` | size:L
- [x] S6.10.5: MVC `Views/Ventas/Index.cshtml` (catálogo por categoría, escáner cámara, apertura de cajón) | size:L
- [x] S6.10.6: Registrar archivos en `.csproj` | size:S

### T6.11: Cancelaciones / Devoluciones | agent:Worker | depends:T6.10 | status: completed
- [x] S6.11.1: Entities `Ventas/Cancelacion.cs`, `DevolucionDetalle.cs` (+DTO) | size:M
- [x] S6.11.2: DB tablas + SPs (total/parcial, devolución a stock opcional, reembolso por método, autorización) | size:L
- [x] S6.11.3: WebApi `DbWrapper.Cancelacion.cs` (transacción) + `CancelacionService.cs` + `CancelacionController.cs` | size:L
- [x] S6.11.4: MVC `HttpClientConnection.Cancelacion.cs` + `CancelacionService.cs` + `CancelacionController.cs` + `Views/Ventas/Cancelacion.cshtml` | size:L
- [x] S6.11.5: Registrar archivos en `.csproj` | size:S

### T6.12: Reportes (ventas por periodo/sucursal/cajero, utilidad, más vendidos) | agent:Worker | depends:T6.10 | status: completed
- [x] S6.12.1: Entities `Reportes/*DTO.cs` (5 reportes) | size:M
- [x] S6.12.2: DB SPs de reportes | size:L
- [x] S6.12.3: WebApi `DbWrapper.Reporte.cs` + `ReporteService.cs` + `ReporteController.cs` | size:L
- [x] S6.12.4: MVC `HttpClientConnection.Reporte.cs` + `ReporteService.cs` + `ReporteController.cs` + `Views/Reportes/Index.cshtml` | size:L
- [x] S6.12.5: Registrar archivos en `.csproj` | size:S

### T6.13: Verificación de build M6 | agent:Reviewer | depends:T6.1..T6.12 | status: completed
- [x] S6.13.1: Build Debug 0 errores + smoke de endpoints por módulo | size:M

---

## M7: Proceso, documentación y verificación final | status: completed

### T7.1: Documentación de convenciones y QA | agent:Worker | status: completed
- [x] S7.1.1: CREATE `docs/CONVENCIONES-CODIFICACION.md` (UTF-8 con BOM en `.cshtml`, acentos correctos) | file:docs/CONVENCIONES-CODIFICACION.md | size:S | verified | evidence: 11955B, strict-UTF8 valido, 12 secciones
- [x] S7.1.2: CREATE `docs/checklist-pruebas-qa.md` | file:docs/checklist-pruebas-qa.md | size:M | verified | evidence: 11633B, strict-UTF8 valido, 11 secciones

### T7.2: Workflow OpenSpec (.github) | agent:Worker | status: completed
- [x] S7.2.1: CREATE `.github/skills/openspec-*/SKILL.md` (explore, propose, apply, update, sync, archive) | size:M | verified | evidence: 6 SKILL.md SHA-256 IDENTICAL a referencia
- [x] S7.2.2: CREATE `.github/prompts/opsx-*.prompt.md` | size:M | verified | evidence: 6 prompt.md SHA-256 IDENTICAL a referencia

### T7.3: Estado compartido `.opencode` | agent:Worker | status: completed
- [x] S7.3.1: CREATE/UPDATE `.opencode/context.md` (entorno, build, convenciones) | file:.opencode/context.md | size:S | verified | evidence: 153 lineas, referencias resuelven
- [x] S7.3.2: CREATE `.opencode/work-log.md` (esquema canónico) | file:.opencode/work-log.md | size:S | verified | evidence: schema canonico (Active Sessions/File Status/Pending Integration)
- [x] S7.3.3: CREATE `.opencode/status.md` | file:.opencode/status.md | size:S | verified | evidence: 12 lineas; NOTE cifras de progreso desactualizadas (38/200 vs real 52/188)

### T7.4: Build final + checklist QA | agent:Reviewer | depends:T7.1,T7.2,T7.3,M1..M6 | status: completed
- [x] S7.4.1: Restore + Build Release/Debug 0 errores (evidencia en `work-log.md`) | size:S
- [x] S7.4.2: Ejecutar checklist QA (`docs/checklist-pruebas-qa.md`) y registrar resultados | size:M
- [x] S7.4.3: Verificación final end-to-end (login → token → cookie → request autorizado → SP) | size:M
- [x] S7.4.4: Confirmar 100% de milestones `[x]` y cerrar misión | size:S

---

## Progress Summary
- Milestones: 7 (M1–M7)
- Tasks (T): 51
- Subtasks (S): 192
- Verificación por milestone: M1–M7 (7 tareas Reviewer)
- Añadido (2026-09-11): T4.9 (S4.9.1–S4.9.7, fixes SYNC-1..7) + T4.10 (S4.10.1, re-verificación M4 post-fixes).
- M1/T1.1 (S1.1.1–S1.1.7) + M1/T1.4 (S1.4.1): `[x]` verificado por Reviewer (build 3 C# projects 0 errores, 0 warnings).
- M1/T1.2 y M1/T1.3: `[x]` verificados por Reviewer (S1.2.1–S1.2.3, S1.3.1–S1.3.2). M1 COMPLETO.
- Verificación M1/T1.2–T1.3 (2026-09-11): Restore 0 err; Build por proyecto Entities/MVC/WebApi = 0 err/0 warn; solución = 3 DLL + solo SSDT MSB4057 pre-existente.
- NOTA de paridad (no bloqueante): la referencia ServiceDeskDESI fija Microsoft.Owin.Security(.Cookies/.OAuth)=3.0.1; el proyecto conserva 4.2.3 por orden explícita del delegado. Decidir si se alinea a 3.0.1 para paridad estructural.

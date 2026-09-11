---
name: netframework-mvc-webapi
description: >-
  Scaffold and develop new ASP.NET MVC 5 + ASP.NET Web API solutions on .NET
  Framework 4.8 following the ServiceDeskDESI reference architecture (3 projects:
  Entities / MVC front / WebApi back). Covers login with OAuth2 bearer token stored
  in a FormsAuthentication cookie, MVC service+DAL layer that calls the API over
  HttpClient, WebApi controllers/services/DAL over ADO.NET stored procedures, and the
  ModelResponse envelope. Use when starting a new .NET Framework project with the same
  structure, replicating this solution, adding a new entity/feature/catálogo, or asked
  for "la estructura del ServiceDesk", "crear proyecto MVC + WebApi", "plantilla
  netframework", "scaffold MVC WebApi". Do NOT use for modern .NET Core/5+ or for
  permissions/roles (RolPaginaAccion / PermisoPaginaAccion) logic.
---

# .NET Framework MVC + WebApi Reference Architecture

Plantilla de arquitectura extraída de **ServiceDeskDESI** (.NET Framework 4.8, C# 7.3).
Úsala para crear OTRO proyecto con la misma estructura o para agregar entidades/módulos
a un proyecto que ya siga este patrón.

> **Alcance**: esta skill NO cubre permisos/roles (`Rol`, `Pagina`, `RolPaginaAccion`,
> `PermisoPaginaAccion`, `PermisoAttribute`, menús por permiso). Solo autenticación
> (¿está logueado?) y el flujo del token.

---

## 1. Topología de la solución (3 proyectos)

```
<nombre>.sln
├── <Nombre>Entities/          ← Class Library (.NET 4.8) — entidades + DTOs + envoltura compartida
├── <Nombre>MVC/               ← ASP.NET MVC 5 — frontend. NO toca BD; consume la API por HTTP
└── <Nombre>WebApi/            ← ASP.NET Web API 5 (OWIN) — backend. Sí toca BD (ADO.NET, SPs)
```

- **MVC** referencia a **Entities** (proyecto). **WebApi** referencia a **Entities** (y en el
  original también a MVC, pero lo limpio es solo Entities).
- Los tres proyectos usan `<TargetFrameworkVersion>v4.8</TargetFrameworkVersion>`.
- **IMPORTANTE (legado)**: los `.csproj` usan lista EXPLÍCITA de archivos
  (`<Compile Include="...">`). Cada `.cs` nuevo DEBE registrarse en el `.csproj` o no compila.
  Si el proyecto nuevo lo permite, preferir `<Compile>` por glob (`**\*.cs`).

Convención de nombres: `XxxEntities`, `XxxMVC`, `XxxWebApi` (prefijo del sistema, ej. `ServiceDeskDESI`).

---

## 2. Proyecto Entities (clases compartidas)

Layout por dominio = subcarpeta por namespace:

```
Entities/
├── BaseObject.cs                      (raíz)
├── Autenticacion/  Usuario.cs, UsuarioDTO.cs
├── Catalogos/      Marca.cs, Persona.cs, Categoria.cs, CategoriaDTO.cs, ...
├── Seguridad/      ModelResponse.cs, Token.cs, TokenCookie.cs, TokenRecuperacion.cs, ...
└── Tickets/        Ticket.cs, TicketDTO.cs, ...
```

### 2.1 Clase base común (`BaseObject`)

```csharp
namespace <Nombre>Entities
{
    public class BaseObject
    {
        public long Id { get; set; }
        public string CreadoPor { get; set; }
        public DateTime FechaCreacion { get; set; }
        public string ModificadoPor { get; set; }
        public DateTime? FechaModificacion { get; set; }
        public bool Estatus { get; set; }   // "Estatus", no "Activo"
    }
}
```

Toda entidad de dominio hereda `BaseObject`. Nombres de auditoría en español.

### 2.2 Entidad y DTO

- Entidad POCO, propiedades auto, **sin** `[DataMember]`/data annotations. FK por propiedad escalar
  (`long PuestoId`, `long? EmpresaId`), **sin** objetos de navegación.
- **DTO = herencia** `XDTO : X` (no composición), añade columnas de join/display
  (ej. `PersonaDTO : Persona` agrega `PuestoNombre`, `long? UsuarioId`). Los DTO viven en la misma
  carpeta que la entidad. NO todas las entidades tienen DTO (solo cuando se necesita join/display).

```csharp
// Catalogos/Marca.cs
public class Marca : BaseObject { public string Nombre { get; set; } public string Descripcion { get; set; } }

// Catalogos/PersonaDTO.cs
public class PersonaDTO : Persona { public string PuestoNombre { get; set; } }
```

### 2.3 Envoltura de respuesta (`ModelResponse`)

```csharp
namespace <Nombre>Entities.Seguridad
{
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
```

TODA la API responde HTTP 200 con esta envoltura; los errores van en `IsSuccess=false` + `Message`
en español. Nunca se lanzan excepciones al cliente (salvo el endpoint OAuth `/token`, que sigue
semántica OAuth: HTTP 400 + `invalid_grant`/`invalid_client`).

### 2.4 Token y TokenCookie

```csharp
// Seguridad/Token.cs — espejo del JSON OAuth (minúsculas) + expiración local
public class Token
{
    public string access_token { get; set; }
    public string token_type { get; set; }
    public int expires_in { get; set; }          // segundos (21600 = 6h)
    public DateTime ExpirationDate { get; set; } // computado por el cliente: Now.AddSeconds(expires_in)
}

// Seguridad/TokenCookie.cs — DTO que viaja DENTRO de la cookie FormsAuth del front
public class TokenCookie
{
    public Token Token { get; set; }
    public long UserID { get; set; }
    public long EmpresaID { get; set; }
    public string UserName { get; set; }
    public string ProfileImage { get; set; }
    public string UserAvatar { get; set; }
}
```

---

## 3. Proyecto WebApi (backend)

### 3.1 Arranque OWIN + OAuth (token)

El token NO es JWT: es un **blob opaco OAuth 2.0 cifrado con el `machineKey`** (paquete
`Microsoft.Owin.Security.OAuth 3.0.1`). `Startup.cs` con `[assembly: OwinStartup(...)]`:

```csharp
// App_Start/Startup.cs
public void Configuration(IAppBuilder app)
{
    // 1) Serilog a App_Data/logs/log-.txt (rolling diario)
    // 2) Middleware CORS manual: ACAO *, Allow-Headers Authorization,Content-Type,
    //    Allow-Methods GET,POST,PUT,DELETE,OPTIONS, corto OPTIONS con 200
    ConfigureOAuth(app);

    var config = new HttpConfiguration();
    config.MapHttpAttributeRoutes();
    // ... Swagger ...
    app.UseWebApi(config);
}

private void ConfigureOAuth(IAppBuilder app)
{
    var OAuthServerOptions = new OAuthAuthorizationServerOptions
    {
        AllowInsecureHttp = bool.Parse(ConfigurationManager.AppSettings["AllowInsecureHttp"]),
        TokenEndpointPath = new PathString("/token"),
        AccessTokenExpireTimeSpan = TimeSpan.FromHours(6),
        Provider = new SimpleAuthorizationServerProvider()
    };
    app.UseOAuthAuthorizationServer(OAuthServerOptions);
    app.UseOAuthBearerAuthentication(new OAuthBearerAuthenticationOptions());
}
```

**Provider de credenciales** (misma clase, en `Startup.cs`). Aquí se construyen los claims:

```csharp
public override async Task GrantResourceOwnerCredentials(OAuthGrantResourceOwnerCredentialsContext context)
{
    var user = new DAL.DbWrapper().AutenticarUsuario(context.UserName, context.Password);
    if (!(user != null && (user.IsSuccess && user.Response != null)))
    {
        context.SetError("invalid_grant", "The user name or password is incorrect.");
        return;
    }
    var usuario = (Usuario)user.Response;

    var identity = new ClaimsIdentity(context.Options.AuthenticationType);
    identity.AddClaim(new Claim(ClaimTypes.Name, context.UserName));
    identity.AddClaim(new Claim("usuarioId", usuario.Id.ToString()));
    if (usuario.EmpresaId != null && usuario.EmpresaId.Value > 0)
        identity.AddClaim(new Claim("empresaId", usuario.EmpresaId.Value.ToString()));
    // roles: foreach (rol) identity.AddClaim(new Claim(ClaimTypes.Role, rol.Nombre));  ← permisos, fuera de alcance

    context.Validated(identity);
}
```

**Claims del token**: `ClaimTypes.Name` (nombreUsuario), `"usuarioId"`, `"empresaId"`,
`ClaimTypes.Role` (roles). Validación de credenciales = `DbWrapper.AutenticarUsuario` (SP
`AutenticarUsuario` con `@NombreUsuario`; la contraseña se compara en C# con
`Cryptography.VerifyPassword`, PBKDF2 con fallback Rijndael legacy).

**Validación por request**: la hace el middleware `UseOAuthBearerAuthentication` (no hay filtro
manual). Los controladores marcan `[Authorize]` a nivel de clase. No existe endpoint de logout
ni de validar-sesión en la API (stateless hasta expirar a las 6h).

### 3.2 Config clave en `Web.config` (WebApi)

```xml
<connectionStrings>
  <add name="cCon" connectionString="Data Source=...;Initial Catalog=...;User Id=...;Password=...;Encrypt=True;TrustServerCertificate=True;" />
</connectionStrings>
<appSettings>
  <add key="owin:AppStartup" value="<Nombre>WebApi.App_Start.Startup, <Nombre>WebApi" />
  <add key="owin:AutomaticAppStartup" value="true" />
  <add key="AllowInsecureHttp" value="true" />
  <add key="client_id" value="<Nombre>MVC" />
  <add key="client_secret" value="<secreto>" />
  <add key="smtpClient" value="smtp.gmail.com" /> <add key="port" value="587" />
  <add key="userEmail" value="..." /> <add key="passEmail" value="..." />
  <add key="EvidenciasMaxArchivos" value="3" />  <!-- ejemplo de configuración de dominio -->
</appSettings>
<system.web>
  <httpRuntime targetFramework="4.8" maxRequestLength="15360" />
  <machineKey validationKey="..." decryptionKey="..." validation="SHA1" decryption="AES" />
</system.web>
```

- **`machineKey` FIJA obligatoria**: sin ella, cada reciclaje del AppPool invalida los tokens OAuth
  emitidos (→ 401) y las cookies FormsAuth del front. Generar una y compartirla entre MVC y WebApi
  si viven en servidores distintos.
- Cadena de conexión: `DbWrapper` lee primero la variable de entorno **`sConSql`**; si no, el
  `connectionStrings["cCon"]`. (En producción el `.Release.config` vacía `cCon` y el host inyecta `sConSql`.)
- `client_id`/`client_secret` del OAuth deben coincidir entre MVC y WebApi.

### 3.3 Controladores

`Controllers/BaseController.cs`:

```csharp
public class BaseController : ApiController
{
    public DbWrapper dbWrapper;
    public BaseController() { dbWrapper = new DbWrapper(); }

    public long ObtenerEmpresaIdDesdeClaim()
    {
        var identity = User.Identity as ClaimsIdentity;
        var claim = identity?.FindFirst("empresaId");
        long empresaId;
        if (claim != null && long.TryParse(claim.Value, out empresaId)) return empresaId;
        return 0;
    }
}
```

Controlador CRUD representativo (`MarcaController`):

```csharp
[Authorize]
[RoutePrefix("api/Marca")]
public class MarcaController : BaseController
{
    private readonly MarcaService _marcaService;
    public MarcaController() { _marcaService = new MarcaService(); }

    [HttpGet, Route("List")]
    public ModelResponse<List<Marca>> ObtenerTodasLasMarcas()
        => _marcaService.ObtenerMarcas(User.Identity.Name);

    [HttpGet, Route("{id:long}")]
    public ModelResponse<Marca> ObtenerMarcaPorId(long id)
        => _marcaService.ObtenerMarcaPorId(id, User.Identity.Name);

    [HttpPost, Route("Guardar")]
    public ModelResponse<Marca> GuardarOActualizarMarca(Marca marca)
        => _marcaService.GuardarOActualizarMarca(marca, User.Identity.Name);

    [HttpDelete, Route("Eliminar")]
    public ModelResponse EliminarMarca(Marca marca)
    {
        marca.FechaModificacion = DateTime.Now;
        return _marcaService.EliminarMarca(marca.Id, marca.ModificadoPor, marca.FechaModificacion.Value, User.Identity.Name);
    }
}
```

**Convenciones de controlador**:
- `[Authorize]` + `[RoutePrefix("api/<X>")]` en la clase. Constructor `new`-ea el servicio.
- Acciones **síncronas** (sin `async`), devuelven `ModelResponse<...>` tal cual (sin try/catch).
- List: `[HttpGet, Route("List")]`. GetById: `[HttpGet, Route("{id:long}")]` (restricción `:long`).
- Guardar = **upsert único** `[HttpPost, Route("Guardar")]` (crea si `Id==0`, actualiza si `Id>0`).
- Eliminar = **lógico** `[HttpDelete, Route("Eliminar")]`, recibe la entidad en el body (con `Id` + `ModificadoPor`).
- El usuario autenticado se lee como `User.Identity.Name` (nombreUsuario) y se pasa como string a
  servicio/DAL (scoping por tenant en los SPs). `ObtenerEmpresaIdDesdeClaim()` para el scope de empresa.
- Grupos de sub-recursos: `CatalogsController` usa `[RoutePrefix("api/Catalogs")]` con rutas
  `"Categoria/List"`, `"Categoria/Lista/{areaId}"`, `"Categoria/Subcategorias/{padreId}"`.
- Endpoints públicos (login/recovery): `[AllowAnonymous]` en la acción (controlador `AutenticationController`,
  `[RoutePrefix("api/Autentication")]`). Nota: en el original el archivo/clase está mal escrito
  `Autentication` (sin `i` tras la `c`); en proyectos nuevos escribe `Autenticacion` correctamente.
- `[Permiso("Marcas")]` / `[Permiso("Marcas","Eliminar")]` = filtro de permisos por página/acción
  (**fuera de alcance** de esta skill; ver exclusión al inicio).

### 3.4 Servicios

```csharp
public class MarcaService
{
    private readonly DbWrapper _dbWrapper;
    public MarcaService() { _dbWrapper = new DbWrapper(); }

    public ModelResponse<List<Marca>> ObtenerMarcas(string usuario)
    {
        try
        {
            Log.Information("MarcaService.ObtenerMarcas para {Usuario}", usuario);
            return _dbWrapper.ObtenerMarcas(usuario);
        }
        catch (ArgumentException ex)
        {
            Log.Warning(ex, "Error de validación en ObtenerMarcas");
            return new ModelResponse<List<Marca>> { IsSuccess = false, Message = ex.Message };
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error en ObtenerMarcas");
            return new ModelResponse<List<Marca>> { IsSuccess = false, Message = "Ocurrió un error al obtener las marcas." };
        }
    }
    // ... ObtenerMarcaPorId(id, usuario), GuardarOActualizarMarca(marca, usuario), EliminarMarca(id, modificadoPor, fecha, usuario)
}
```

**Convenciones de servicio**:
- Clase plana, sin interfaz/DI/static. Constructor `new DbWrapper()`, campo `private readonly DbWrapper _dbWrapper`.
- Firma: `Get(list)` = `(string usuario)`; `GetById` = `(long id, string usuario)`;
  `upsert` = `(Entity entity, string usuario)`; `delete` = `(long id, string modificadoPor, DateTime fecha, string usuario)`.
- Validación de negocio en el servicio: `throw new ArgumentException("...")` para input inválido.
- Dos `catch`: `ArgumentException` → mensaje verbatim; `Exception` → `Log.Error` + mensaje genérico.
- `Serilog` (`Log.Information/Warning/Error`) envuelve cada llamada.
- Trabajo multi-paso → `_dbWrapper.BeginTransaction()` / `CommitTransaction()` / `RollbackTransaction()`.
- Siempre devuelve `ModelResponse<T>` (nunca `null`); éxito = `IsSuccess=true` + `Response` + `Message` en español.

### 3.5 DAL (DbWrapper)

`DAL/BaseDbWrapper.cs` — base abstracta:

```csharp
public abstract class BaseDbWrapper
{
    protected abstract string SQLConnectionString { get; }
    protected abstract TimeSpan SQLCommandTimeOut { get; }

    // Helpers (protected), todos contra SQL Server / System.Data.SqlClient / SPs (CommandType.StoredProcedure por defecto):
    protected object ExecuteScalar(string cmdText, CommandType cmdType = CommandType.StoredProcedure, IEnumerable<SqlParameter> pars = null);
    protected int ExecuteNonQuery(string cmdText, CommandType cmdType = CommandType.StoredProcedure, IEnumerable<SqlParameter> pars = null);
    protected T GetObject<T>(string cmdText, Func<IDataReader, T> mapper, CommandType cmdType = CommandType.StoredProcedure, IEnumerable<SqlParameter> pars = null);
    protected IEnumerable<T> GetObjects<T>(string cmdText, Func<IDataReader, T> mapper, CommandType cmdType = CommandType.StoredProcedure, IEnumerable<SqlParameter> pars = null);
    // + variantes *Async (que NO participan de la transacción ambiente)
    public void BeginTransaction(); public void CommitTransaction(); public void RollbackTransaction();
    // Transacción "ambiente": una sola SqlConnection; los helpers la reutilizan si está activa (evita MSDTC).
}
```

`DAL/DbWrapper.cs` — wrapper concreto (clase `partial`):

```csharp
public partial class DbWrapper : BaseDbWrapper
{
    protected override string SQLConnectionString { get; }
    protected override TimeSpan SQLCommandTimeOut { get; }

    public DbWrapper()
    {
        var env = Environment.GetEnvironmentVariable("sConSql");
        var cfg = ConfigurationManager.ConnectionStrings["cCon"]?.ConnectionString;
        SQLConnectionString = !string.IsNullOrWhiteSpace(env) ? env : cfg;
        if (string.IsNullOrWhiteSpace(SQLConnectionString))
            throw new InvalidOperationException("Cadena de conexión no configurada (sConSql o cCon).");
        SQLCommandTimeOut = TimeSpan.FromSeconds(15);
    }

    // Mapeador por reflexión: por cada columna del reader, match case-insensitive por nombre de propiedad.
    private T LlenarEntidad<T>(IDataReader reader) where T : class, new() { /* DBNull->null, enum por nombre, Convert.ChangeType */ }

    // Refleja TODAS las propiedades públicas de T a SqlParameter("@Prop", value) { IsNullable = true }.
    public List<SqlParameter> ObtenerParametrosSQL<T>(T o) { /* ... */ }
}
```

`DAL/DbWrapper.Marca.cs` — **un partial por feature** (`public partial class DbWrapper`):

```csharp
public partial class DbWrapper
{
    public ModelResponse<List<Marca>> ObtenerMarcas(string usuario)
    {
        var mr = new ModelResponse<List<Marca>>();
        try
        {
            var marcas = GetObjects("ObtenerMarca", CommandType.StoredProcedure,
                new[] { new SqlParameter("@Usuario", usuario) },
                new Func<IDataReader, Marca>((r) => LlenarEntidad<Marca>(r)));
            mr.IsSuccess = true;
            mr.Response = marcas.ToList();
            mr.Message = "Marcas obtenidas correctamente";
        }
        catch (Exception ex) { Log.Error(ex, "Error al obtener marcas para {Usuario}", usuario);
            mr.IsSuccess = false; mr.Message = "Ocurrió un error al obtener las marcas."; }
        return mr;
    }

    public ModelResponse<Marca> GuardarOActualizarMarca(Marca m)
    {
        var mr = new ModelResponse<Marca>();
        try
        {
            var pars = ObtenerParametrosSQL(new { m.Id, m.Nombre, m.Descripcion, m.CreadoPor,
                m.FechaCreacion, m.ModificadoPor, m.FechaModificacion, m.Estatus, Usuario = m.CreadoPor }).ToArray();
            var id = ExecuteScalar("GuardarOActualizarMarca", CommandType.StoredProcedure, pars);
            if (Convert.ToInt64(id) == 0) { mr.IsSuccess = false; mr.Message = "No tiene permisos para realizar esta operación."; return mr; }
            m.Id = Convert.ToInt64(id);   // el SP devuelve el id nuevo/actualizado por scalar
            mr.IsSuccess = true; mr.Response = m; mr.Message = "Marca guardada correctamente";
        }
        catch (Exception ex) { Log.Error(ex, "Error al guardar marca"); mr.IsSuccess = false; mr.Message = "Ocurrió un error al guardar la marca."; }
        return mr;
    }
    // Eliminar: ExecuteScalar/NonQuery de SP "EliminarX" con @Id,@ModificadoPor,@FechaModificacion,@Usuario
}
```

**Convenciones DAL**:
- **Solo SPs** (nada de SQL inline). Mapeo manual por reflexión (`LlenarEntidad<T>`), sin AutoMapper.
- Nombres de SP: list `ObtenerX`/`ObtenerXs`, by-id `ObtenerXPorId`, upsert `GuardarOActualizarX`, delete `EliminarX`.
- Upsert → `ExecuteScalar` del SP que devuelve el id; `0` = sin permisos/error; **códigos negativos** =
  errores de negocio que el DAL mapea a mensaje en español (ej. `-1` duplicado).
- Un archivo `DbWrapper.<Feature>.cs` por feature; todos `public partial class DbWrapper`.

### 3.6 Helpers (WebApi)

- `Helpers/Cryptography.cs` (static):
  - `HashPassword(string)` → PBKDF2 (`Rfc2898DeriveBytes`, 10000 iter, salt 16B, hash 32B),
    formato `"PBKDF2$<iter>$<saltB64>$<hashB64>"`.
  - `VerifyPassword(string password, string stored)` → si `stored` empieza con `PBKDF2$` re-deriva y
    compara XOR en tiempo constante; si no, fallback legacy `string.Equals(Encrypt(password), stored)`.
  - `Encrypt(string)`/`Decrypt(string)` → Rijndael/AES CBC legacy (claves hardcodeadas, solo para
    passwords viejas).
- `Helpers/EmailHelper.cs`: `public static void EnvioEmaiil(IEnumerable<string> para, string asunto,
  string mensaje, bool ssl = false, string attachment = "")` — SMTP desde appSettings (`smtpClient`,
  `port`, `userEmail`, `passEmail`), body HTML, attachment opcional. (En el original el método está
  mal escrito `EnvioEmaiil`; escribir `EnvioEmail` en proyectos nuevos.)
- Plantillas de correo en `Template/Template_*.html`.

---

## 4. Proyecto MVC (frontend)

El front **NO accede a BD**. Flujo por feature: `Controller → Service → DAL(HttpClientConnection) → HTTP → WebApi`.
Patrón de respuesta en toda la cadena: `ModelResponse<T>`.

### 4.1 Sesión / cookie de login (FormsAuthentication)

La "sesión" es una **cookie FormsAuth** cuyo `UserData` contiene el JSON del `TokenCookie`.
Cookie name = `autentication` (definido en `<forms name="autentication">`).

`Helpers/SessionHelper.cs`:

```csharp
public class SessionHelper
{
    public static bool EixstSession()   // original mal escrito; usar "ExisteSession"
    {
        var t = GetSessionUser();
        return t != null && t.Token != null && t.Token.ExpirationDate >= DateTime.Now;
    }

    public static void CloseSession() { FormsAuthentication.SignOut(); }

    public static TokenCookie GetSessionUser()
    {
        if (HttpContext.Current?.User?.Identity is FormsIdentity fi && fi.Ticket != null)
            return JsonConvert.DeserializeObject<TokenCookie>(fi.Ticket.UserData);
        return null;
    }

    public static void CreateSession(string id)   // id = JsonConvert.SerializeObject(tokenCookie)
    {
        var tokenCookie = JsonConvert.DeserializeObject<TokenCookie>(id);
        var cookie = FormsAuthentication.GetAuthCookie("token", persist: true);
        cookie.Name = FormsAuthentication.FormsCookieName;                 // "autentication"
        cookie.Expires = tokenCookie.Token.ExpirationDate;

        var ticket = FormsAuthentication.Decrypt(cookie.Value);
        var newTicket = new FormsAuthenticationTicket(ticket.Version, ticket.Name, ticket.IssueDate,
            cookie.Expires, ticket.IsPersistent, id);
        cookie.Value = FormsAuthentication.Encrypt(newTicket);
        HttpContext.Current.Response.Cookies.Add(cookie);
    }
}
```

Puntos clave:
- Cookie HttpOnly (default de FormsAuth), expiración = expiración del token OAuth, persistente.
- `Web.config` del MVC: `<authentication mode="Forms"><forms name="autentication"
  cookieless="UseCookies" protection="All" /></authentication>` + **`<machineKey>` fija** (la misma
  que la WebApi si comparten servidor) para que la cookie sobreviva reciclajes del AppPool.
- Cada request: el módulo FormsAuthentication autentica la cookie → `HttpContext.Current.User` =
  `GenericPrincipal` con `FormsIdentity` cuyo `Ticket.UserData` = JSON. `GetSessionUser()` lo deserializa.

### 4.2 Filtro global de autenticación

`App_Start/FilterConfig.cs` registra UN filtro global `IAuthorizationFilter` con allow-list:

```csharp
public static void RegisterGlobalFilters(GlobalFilterCollection filters)
{
    filters.Add(new AuthenticationFilter());
}

public class AuthenticationFilter : IAuthorizationFilter
{
    private static readonly HashSet<string> PublicActions = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
    {
        "Home.Autentication", "Home.LogIn", "Home.RecoverPassword", "Home.ValidarToken",
        "Home.RestablecerContrasenia", "Home.NewCompany", "Home.AccesoDenegado" /* ... */
    };

    public void OnAuthorization(AuthorizationContext ctx)
    {
        var key = ctx.ActionDescriptor.ControllerDescriptor.ControllerName + "." + ctx.ActionDescriptor.ActionName;
        if (PublicActions.Contains(key)) return;
        if (!SessionHelper.ExisteSession())
            ctx.Result = new RedirectToRouteResult(new RouteValueDictionary(
                new { controller = "Home", action = "Autentication" }));
    }
}
```

- No hay `[Authorize]` en el MVC: la autenticación la impone este filtro global; los permisos de
  escritura los impone `[Permiso]` (fuera de alcance).

### 4.3 Flujo de login (2 pasos)

`Controllers/HomeController.cs`:

```csharp
[HttpPost]
public async Task<string> LogIn(string user, string pass)
{
    var mr = new ModelResponse();
    try
    {
        // PASO 1: autenticar para obtener mensaje específico + datos del usuario (SIN token)
        var response = await _autenticacionService.AutenticarUsuario(new Usuario { NombreUsuario = user, Contrasena = pass });
        if (!response.IsSuccess || response.Response == null)
            return JsonConvert.SerializeObject(new ModelResponse { IsSuccess = false, Message = response?.Message ?? "Usuario o contraseña incorrectos" });

        // PASO 2: obtener token OAuth (password grant)
        Token token = await httpClientConnection.GetToken(user, pass);
        if (token == null) return JsonConvert.SerializeObject(new ModelResponse { IsSuccess = false, Message = "Error de usuario o contraseña" });

        token.ExpirationDate = DateTime.Now.AddSeconds(token.expires_in);
        var usuario = response.Response;
        var tokenCookie = new TokenCookie
        {
            Token = token,
            UserID = usuario.Id,
            EmpresaID = usuario.EmpresaId ?? 0,
            UserName = user,
            ProfileImage = usuario.ImagenPerfil,
            UserAvatar = GenerarAvatarIniciales(usuario.NombreUsuario)
        };
        SessionHelper.CreateSession(JsonConvert.SerializeObject(tokenCookie));

        mr.IsSuccess = true; mr.Message = "Ok";
    }
    catch (Exception ex) { Log.Error(ex, "LogIn error"); mr.IsSuccess = false; mr.Message = ex.Message; }
    return JsonConvert.SerializeObject(mr);
}
```

- `AutenticarUsuario` → `POST api/Autentication/autenticar` (sin header Authorization), devuelve
  `ModelResponse<UsuarioDTO>` con mensaje rico + perfil.
- `GetToken(user, pass)` → `POST /token` (OAuth, `application/x-www-form-urlencoded`,
  `grant_type=password&UserName=...&Password=...&client_id=...&client_secret=...`).
- La acción devuelve un **JSON string** (doble encode); el JS hace `JSON.parse(response)`.
- Logout: `SessionHelper.CloseSession()` + `RedirectToAction("Autentication")`.

### 4.4 BaseController del MVC

```csharp
public class BaseController : Controller
{
    public HttpClientConnection httpClientConnection;
    public TokenCookie tokenCookie;
    public ModelResponse mr { get; set; }

    public BaseController()
    {
        httpClientConnection = new HttpClientConnection();
        mr = new ModelResponse();
        tokenCookie = SessionHelper.GetSessionUser();
    }

    public List<SelectListItem> MappingPropertiToDropDownList<T>(IEnumerable<T> items, string value, string title, string prefix = "") { /* reflexión */ }
    public string GenerarAvatarIniciales(string nombreUsuario) { /* 2 iniciales */ }
}
```

Todos los controladores heredan `BaseController` y en su constructor crean los servicios:

```csharp
public class CatalogsController : BaseController
{
    private readonly CategoriaService _categoriaService;
    private readonly PersonaService _personaService;

    public CatalogsController() : base()
    {
        _categoriaService = new CategoriaService(httpClientConnection);
        _personaService = new PersonaService(httpClientConnection);
    }
}
```

### 4.5 DAL del MVC (capa HTTP)

`DAL/HttpClientBase.cs` — motor HTTP genérico:

```csharp
public class HttpClientBase
{
    private HttpClient httpClient;   // instancia por request (NO static, NO using por llamada)

    public HttpClientBase(string baseUrl)
    {
        BaseUri = string.IsNullOrEmpty(baseUrl)
            ? ConfigurationManager.AppSettings["BaseUriWebApi"] : baseUrl;
        httpClient = new HttpClient { BaseAddress = new Uri(BaseUri), Timeout = TimeSpan.FromMinutes(5) };
    }

    private void SetParametersHttpCliente(string contentType, string token)
    {
        httpClient.DefaultRequestHeaders.Clear();
        httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(contentType));
        if (!string.IsNullOrEmpty(token))
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
    }

    // 1) OAuth token (form-urlencoded): TokenAsync<T>(endpoint, IEnumerable<KeyValuePair<string,string>>, contentType)
    // 2) Raw con mapper: RequestAsync<T>(endpoint, method, content, Func<string,T> func, token, contentType)
    // 3) Tipado estándar: RequestAsync<TResponse>(endpoint, method, content, token, contentType) -> ModelResponse<TResponse>
    // 4) RequestAsyncByteArray(...) | 5) SendMultipartAsync<T>(...) | 6) RequestFileAsync(...)
}
```

Serie cuerpo compartida: `StringContent(JsonConvert.SerializeObject(content), Encoding.UTF8, contentType)`;
éxito → deserializa `ModelResponse<T>`; error HTTP → devuelve `ModelResponse<T>{ IsSuccess=false,
Message="Error {code} ({reason}) al consumir {endpoint}." }` (nunca lanza). `JsonConvert` (Newtonsoft) en todo.

`DAL/HttpClientConnection.cs` — núcleo `partial`:

```csharp
public partial class HttpClientConnection : HttpClientBase
{
    private TokenCookie token;

    public HttpClientConnection(string baseUrl = "") : base(baseUrl)
    {
        token = SessionHelper.GetSessionUser();   // NULL antes del login
    }

    public BaseObject MappingColumSecurity(BaseObject o)   // auditoría cliente
    {
        if (o.Id == 0 || o.Id == -1) { o.CreadoPor = SessionHelper.GetSessionUser().UserName;
            o.FechaCreacion = SessionHelper.GetDateCenterMexico(); }
        else { o.ModificadoPor = SessionHelper.GetSessionUser().UserName;
            o.FechaModificacion = SessionHelper.GetDateCenterMexico(); }
        return o;
    }

    public async Task<Token> GetToken(string user, string pass)
        => await TokenAsync<Token>("token", new[]
        {
            new KeyValuePair<string, string>("grant_type", "password"),
            new KeyValuePair<string, string>("UserName", user),
            new KeyValuePair<string, string>("Password", pass),
            new KeyValuePair<string, string>("client_id", ConfigurationManager.AppSettings["client_id"]),
            new KeyValuePair<string, string>("client_secret", ConfigurationManager.AppSettings["client_secret"])
        }, "application/x-www-form-urlencoded");

    public async Task<ModelResponse<T>> PostMultipartAsync<T>(string endpoint, MultipartFormDataContent content)
        => await SendMultipartAsync<T>(endpoint, content, token.Token.access_token);
}
```

`DAL/HttpClientConnection.Categoria.cs` — **un partial por feature** (ejemplo representativo):

```csharp
public partial class HttpClientConnection
{
    public async Task<ModelResponse<List<CategoriaDTO>>> ObtenerCategorias()
        => await RequestAsync<List<CategoriaDTO>>("api/Catalogs/Categoria/List", HttpMethod.Get, null, token.Token.access_token);

    public async Task<ModelResponse<CategoriaDTO>> ObtenerCategoriaPorId(long id)
        => await RequestAsync<CategoriaDTO>($"api/Catalogs/Categoria/{id}", HttpMethod.Get, null, token.Token.access_token);

    public async Task<ModelResponse<Categoria>> GuardarOActualizarCategoria(Categoria categoria)
    {
        MappingColumSecurity(categoria);
        return await RequestAsync<Categoria>("api/Catalogs/Categoria", HttpMethod.Post, categoria, token.Token.access_token);
    }

    public async Task<ModelResponse> EliminarCategoria(Categoria categoria)
    {
        MappingColumSecurity(categoria);
        var result = await RequestAsync<object>("api/Catalogs/Categoria", HttpMethod.Delete, categoria,
            new Func<string, string>(s => s), token.Token.access_token);
        return JsonConvert.DeserializeObject<ModelResponse>(result.ToString());
    }
}
```

**Convenciones HttpClientConnection**:
- Un método `async Task<ModelResponse<...>>` por endpoint; ruta interpolada
  `$"api/<Area>/<Accion>[/{id}]"` relativa a `BaseAddress`.
- GET list → `api/X/List`; GET by id → `api/X/{id}`; Create/Update → **ambos** `HttpMethod.Post` a la raíz
  (o `.../Guardar`), con `MappingColumSecurity(entidad)` antes; Delete → `HttpMethod.Delete` con entidad
  como body, audit-stamped.
- Para endpoints que devuelven `ModelResponse` no genérico: patrón legacy
  `RequestAsync<object>(..., new Func<string,string>(s => s), token)` + `DeserializeObject<ModelResponse>`.
- Login/password-recovery: sin token.

### 4.6 Servicios del MVC

```csharp
public class CategoriaService
{
    private readonly HttpClientConnection _httpClient;
    public CategoriaService(HttpClientConnection httpClient) { _httpClient = httpClient; }

    public async Task<CategoriaDTO> ObtenerCategoriaPorId(long id)
    {
        var r = await _httpClient.ObtenerCategoriaPorId(id);
        return (r.IsSuccess && r.Response != null) ? r.Response : null;   // "unwrap" en get-by-id
    }

    public async Task<ModelResponse<Categoria>> GuardarOActualizarCategoria(Categoria c)
        => await _httpClient.GuardarOActualizarCategoria(c);              // passthrough

    public async Task<ModelResponse> EliminarCategoria(Categoria c)
        => await _httpClient.EliminarCategoria(c);

    public async Task<ModelResponse<List<CategoriaDTO>>> ConsultarTodasCategorias()
        => await _httpClient.ObtenerCategorias();
}
```

- Clase plana, ctor recibe `HttpClientConnection` (inyección manual desde el controller).
- **Passthrough** para list/save/delete; **unwrap** para get-by-id (devuelve DTO o `null`).

### 4.7 Controladores del MVC (2 regiones por feature)

- `#region Views` → acciones `ActionResult` que devuelven `View(model)`:
  permiso de lectura (si aplica) → `new Entity()` (crear) o `ObtenerXPorId` si `id>0` (editar, misma
  acción `Category(long id = 0)`) → llenar `ViewBag` dropdowns con `MappingPropertiToDropDownList(...)`
  → `return View(model)`.
- `#region Data Access` → endpoints AJAX que devuelven **`Task<string>`** =
  `JsonConvert.SerializeObject(response)` (doble encode; NO `Json()`). Verbos: `ConsultarTodasX()`,
  `ConsultarXPorId(id)`, `GuardarOActualizarX(x)` (POST), `EliminarX(x)` (POST/lógico).

```csharp
#region Data Access
public async Task<string> GuardarOActualizarCategoria(Categoria categoria)
{
    var tc = SessionHelper.GetSessionUser();
    if (categoria.Id == 0) { categoria.CreadoPor = tc?.UserName ?? "system"; categoria.FechaCreacion = DateTime.Now; }
    else { categoria.ModificadoPor = tc?.UserName ?? "system"; categoria.FechaModificacion = DateTime.Now; }
    categoria.Estatus = true;
    var response = await _categoriaService.GuardarOActualizarCategoria(categoria);
    return JsonConvert.SerializeObject(response);
}
#endregion
```

- Routing MVC: ruta por defecto `{controller}/{action}/{id}` → `Home/Index`; sin attribute routing.
- El JS (Comun.js) provee `GetMVC(url, cb)`, `GetParamMVC(url, params, cb)`, `PostMVC(url, params, cb)`
  (POST para crear/editar/eliminar lógico), `PostViewMVC(...)`, `PostFileMVC(...)`. Las vistas hacen
  `var result = typeof response === 'string' ? JSON.parse(response) : response;` y alimentan DataTables
  con `MapingPropertiesDataTable("tblX", result.Response)`.

### 4.8 Config clave en `Web.config` (MVC)

```xml
<appSettings>
  <add key="BaseUriWebApi" value="http://localhost:44357/" />   <!-- base URL de la WebApi -->
  <add key="client_id" value="<Nombre>MVC" />                  <!-- deben coincidir con la WebApi -->
  <add key="client_secret" value="<secreto>" />
  <add key="owin:AutomaticAppStartup" value="false" />         <!-- MVC NO es OWIN -->
</appSettings>
<system.web>
  <compilation debug="true" targetFramework="4.8" />
  <httpRuntime targetFramework="4.8" maxRequestLength="15360" />
  <authentication mode="Forms">
    <forms name="autentication" cookieless="UseCookies" protection="All" />
  </authentication>
  <machineKey validationKey="..." decryptionKey="..." validation="SHA1" decryption="AES" />
</system.web>
```

---

## 5. Flujo end-to-end de referencia

**Login:**
1. GET `/Home/Autentication` (allow-list) → vista standalone (`Layout = null`).
2. `$.ajax POST /Home/LogIn {user, pass}`.
3. `HomeController.LogIn`: (a) `AutenticacionService.AutenticarUsuario` → `POST api/Autentication/autenticar`
   (sin token) → `ModelResponse<UsuarioDTO>`; (b) `httpClientConnection.GetToken` → `POST /token`
   (password grant) → `Token`.
4. Construye `TokenCookie` y `SessionHelper.CreateSession(json)` → cookie FormsAuth `autentication`
   con `UserData` = JSON, expiración = token OAuth.
5. `window.location.href = '/Home/Index'`.

**Request autenticado:**
1. Filtro global `AuthenticationFilter` → `SessionHelper.ExisteSession()` (deserializa cookie, valida expiración).
2. `BaseController` crea `HttpClientConnection` (lee sesión → `token.access_token`).
3. Controlador → servicio → `HttpClientConnection.<Feature>` → `HttpClientBase.RequestAsync` con
   header `Authorization: Bearer <access_token>`.
4. WebApi: middleware `UseOAuthBearerAuthentication` valida el token (machineKey) → `[Authorize]` →
   controlador lee `User.Identity.Name`/`ObtenerEmpresaIdDesdeClaim()` → servicio → `DbWrapper` → SP.
5. Respuesta `ModelResponse<T>` envuelta, HTTP 200 siempre (errores en `IsSuccess=false`).

---

## 6. Checklist para AGREGAR una entidad/módulo nuevo (ej. "Proveedor")

Sigue el patrón en 6 capas. Para una entidad nueva `Proveedor` con DTO:

**Entities:**
1. `Entities/Catalogos/Proveedor.cs : BaseObject` (+ `ProveedorDTO.cs : Proveedor` si necesita joins).

**WebApi:**
2. `DAL/DbWrapper.Proveedor.cs` (partial): `ObtenerProveedores(usuario)`, `ObtenerProveedorPorId(id, usuario)`,
   `GuardarOActualizarProveedor(p, usuario)` (ExecuteScalar devuelve id; 0/negativos = error),
   `EliminarProveedor(id, modificadoPor, fecha, usuario)`.
3. `Services/ProveedorService.cs`: `new DbWrapper()`; validación con `ArgumentException`; try/catch doble; Serilog.
4. `Controllers/ProveedorController.cs : BaseController` `[Authorize] [RoutePrefix("api/Proveedor")]`:
   `List` / `{id:long}` / `Guardar` (upsert) / `Eliminar` (lógico). Acciones síncronas.
5. Registrar SPs en BD (`ObtenerProveedor`, `ObtenerProveedorPorId`, `GuardarOActualizarProveedor`, `EliminarProveedor`).

**MVC:**
6. `DAL/HttpClientConnection.Proveedor.cs` (partial): métodos async → `RequestAsync` con `token.Token.access_token`.
7. `Services/ProveedorService.cs`: ctor `(HttpClientConnection)`, passthrough + unwrap.
8. `Controllers/<Feature>Controller.cs : BaseController`: región Views (`Proveedor(long id=0)`) + región
   Data Access (`ConsultarTodosProveedores`, `GuardarOActualizarProveedor`, `EliminarProveedor` → `Task<string>`).
9. `Views/<Feature>/Proveedor.cshtml` + `_Layout` (si aplica) con DataTables 2.3.7 (CDN) e
   i18n `/Content/datatables/i18n/es-ES.json`.
10. **Registrar los `.cs` nuevos en el `.csproj`** (si usa lista explícita).

---

## 7. Referencias rápidas

- **Paquetes clave**: MVC → `Microsoft.AspNet.Mvc 5.3.0`, `Newtonsoft.Json 13`, `Serilog`+`Serilog.Sinks.File`,
  `Microsoft.AspNet.WebApi.Client`, `Microsoft.Owin 4.2.3`. WebApi → `Microsoft.AspNet.WebApi 5.3.0`,
  `Microsoft.Owin.Host.SystemWeb`, `Microsoft.Owin.Security.OAuth 3.0.1`, `Swashbuckle 5.6.0`, `Serilog`.
  **No hay JWT** (token opaco OAuth).
- **Serilog**: `Log.Logger = new LoggerConfiguration().WriteTo.File(path: App_Data/logs/log-.txt,
  rollingInterval: Day, retainedFileCountLimit: 31)`. `Global.asax.cs` `Application_End` → `Log.CloseAndFlush()`.
- **JSON**: MVC serializa con `JsonConvert.SerializeObject` (string); WebApi aplica
  `CamelCasePropertyNamesContractResolver` en `WebApiConfig.Register`.
- **Errores**: nunca se lanzan al cliente; `ModelResponse{ IsSuccess=false, Message=... }`.
- **Tipos erróneos del original (NO replicar)**: `AutenticationController` → usar `AutenticacionController`;
  `SessionHelper.EixstSession` → `ExisteSession`; `EmailHelper.EnvioEmaiil` → `EnvioEmail`;
  `DbWrapper.MapearPorpiedades` → `MapearPropiedades`. Conservar solo si se reutiliza el mismo código.

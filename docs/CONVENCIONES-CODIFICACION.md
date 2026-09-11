# Convenciones de codificación — PuntoDeVentaDESI

> Documento normativo del proyecto. Aplica a **todos** los archivos de la solución
> (`PuntoDeVentaEntities`, `PuntoDeVentaMVC`, `PuntoDeVentaWebApi`, `PuntoDeVenta.Database`).
> Se deriva de la arquitectura de referencia (`netframework-mvc-webapi/SKILL.md`) y del
> proyecto de referencia `ServiceDeskDESI`. Ante duda, la referencia manda.

---

## 1. Plataforma y lenguaje

| Elemento | Valor |
|---|---|
| Framework | .NET Framework **4.8** |
| Lenguaje | C# **7.3** (`<LangVersion>7.3</LangVersion>`) |
| Build | Visual Studio 2022 / `dotnet msbuild` |
| Web | ASP.NET MVC 5 + ASP.NET Web API 5 (OWIN) |
| Datos | SQL Server + ADO.NET + **stored procedures** (sin ORM) |
| Estilo de proyecto | `<RestoreProjectStyle>PackageReference</RestoreProjectStyle>` |

- No se introducen características posteriores a C# 7.3 (sin `record`, sin *target-typed
  new*, sin `using` implícitos, sin *nullable reference types*).
- Los `.csproj` usan **lista explícita de archivos** (ver §4).

---

## 2. Codificación de archivos (encoding)

- **Todo archivo de texto se guarda en UTF-8.** Los archivos con acentos, `ñ`, `¿`, `¡`
  o cualquier carácter no ASCII **DEBEN** guardarse en **UTF-8 con BOM**.
- **`.cshtml` (Razor): UTF-8 con BOM obligatorio.** Sin BOM, IIS/ASP.NET puede
  interpretar el archivo con la *code page* del sistema y romper los acentos
  (`contraseña` → `contraseÃ±a`).
- `.cs`, `.js`, `.css`, `.json`, `.sql`, `.config`, `.md`: UTF-8 (se prefiere con BOM en
  `.cs` para mantener consistencia con el tooling de VS).
- Prohibido guardar archivos en ANSI / Windows-1252 / Latin-1.
- Verificación rápida (PowerShell):

```powershell
# ¿Tiene BOM UTF-8?
$b = [System.IO.File]::ReadAllBytes("ruta\archivo.cshtml")
($b[0] -eq 0xEF -and $b[1] -eq 0xBB -and $b[2] -eq 0xBF)
```

- Textos de interfaz y mensajes (`Message` de `ModelResponse`) van **en español**, con
  acentos correctos.

---

## 3. Namespaces y estructura de carpetas

### 3.1 Raíces de namespace

| Proyecto | Root namespace |
|---|---|
| `PuntoDeVentaEntities` | `PuntoDeVentaEntities` |
| `PuntoDeVentaMVC` | `PuntoDeVentaMVC` |
| `PuntoDeVentaWebApi` | `PuntoDeVentaWebApi` |

- El namespace **coincide con la ruta de carpetas** (ej.
  `PuntoDeVentaEntities/Catalogos/Marca.cs` → `namespace PuntoDeVentaEntities.Catalogos`).
- No usar el prefijo antiguo `PuntoDeVenta.Entities|MVC|WebApi` (eliminado en M1).

### 3.2 Organización por dominio (feature-oriented)

```
PuntoDeVentaEntities/
├── BaseObject.cs                 (raíz: Id + auditoría + Estatus)
├── Autenticacion/                Usuario.cs, UsuarioDTO.cs
├── Catalogos/                    Marca.cs, Empresa.cs, ...
└── Seguridad/                    ModelResponse.cs, Token.cs, TokenCookie.cs

PuntoDeVentaWebApi/
├── App_Start/                    Startup.cs, WebApiConfig.cs, SwaggerConfig.cs, TokenAuthorizationServerProvider.cs
├── Controllers/                  BaseController.cs, MarcaController.cs, AutenticacionController.cs
├── DAL/                          BaseDbWrapper.cs, DbWrapper.cs, DbWrapper.<Feature>.cs
├── Helpers/                      Cryptography.cs, EmailHelper.cs
├── Services/                     <Feature>Service.cs
└── Template/                     Template_*.html

PuntoDeVentaMVC/
├── App_Start/                    FilterConfig.cs, RouteConfig.cs
├── Controllers/                  BaseController.cs, HomeController.cs, <Feature>Controller.cs
├── DAL/                          HttpClientBase.cs, HttpClientConnection.cs, HttpClientConnection.<Feature>.cs
├── Helpers/                      SessionHelper.cs, ThemeHelper.cs, FiltersHelper.cs, Cryptography.cs
├── Services/                     <Feature>Service.cs
└── Views/                        _ViewStart.cshtml, Shared/_Layout.cshtml, <Feature>/*.cshtml
```

- **Un archivo `DbWrapper.<Feature>.cs`** y **un archivo `HttpClientConnection.<Feature>.cs`
  por feature** (todos `partial`). No concentrar múltiples features en un archivo.
- Carpetas `Shared/`, `Helpers/`, `Scripts/Comun/`, `CSS/Comun/` solo para utilidades
  realmente transversales.

---

## 4. Archivos de proyecto (`.csproj`) — `<Compile Include>` explícito

Los `.csproj` **NO** usan globs; listan cada archivo explícitamente.

- **Todo `.cs` nuevo DEBE registrarse** con `<Compile Include="ruta\Archivo.cs" />` o no
  compila.
- Las vistas `.cshtml` se registran como `<Content Include="...\*.cshtml" />` (con
  `DependentUpon` cuando aplique).
- Los assets (`Scripts/*.js`, `CSS/*.css`, `Content/*.json`, `Template/*.html`) también se
  registran.
- No se eliminan entradas de `bin`/`obj` ni `ProjectTypeGuids`.
- Al renombrar/mover un archivo hay que actualizar **la vez** la entrada del `.csproj`.
- Los `<ProjectReference>` apuntan a `..\PuntoDeVentaEntities\PuntoDeVentaEntities.csproj`.

---

## 5. Envoltura de respuesta (`ModelResponse`) — obligatoria

Toda la API responde **HTTP 200** con la envoltura `ModelResponse`; los errores viajan
en `IsSuccess=false` + `Message`. Nunca se lanzan excepciones al cliente (única
excepción: el endpoint OAuth `/token`, que usa la semántica OAuth estándar:
HTTP 400 + `invalid_grant`/`invalid_client`).

```csharp
namespace PuntoDeVentaEntities.Seguridad
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

- Éxito: `IsSuccess = true`, `Response` poblado, `Message` en español
  ("Marca guardada correctamente").
- Error: `IsSuccess = false`, `Message` en español comprensible para el usuario.
- No existe el shape antiguo `{ Success, Data, Errors }`.
- Los servicios y el DAL **siempre** devuelven `ModelResponse<T>` (nunca `null`).

---

## 6. Capas y flujo (MVC **no** toca base de datos)

```
Controller → Service → DAL(HttpClientConnection) → HTTP → WebApi → Service → DbWrapper → SP
```

### 6.1 Entities

- Entidades POCO que heredan `BaseObject` (`Id`, `CreadoPor`, `FechaCreacion`,
  `ModificadoPor`, `FechaModificacion`, `Estatus`).
- **Sin** `[DataMember]`, **sin** data annotations, **sin** navegación ORM. Las FK son
  propiedades escalares (`long EmpresaId`, `long? SucursalId`).
- **DTO = herencia** (`MarcaDTO : Marca`) que agrega columnas de join/display. Viven en
  la misma carpeta que la entidad.
- Nombres de auditoría en español y **exactamente** los prescritos.

### 6.2 WebApi (backend)

- **Solo stored procedures** (`CommandType.StoredProcedure`). Prohibido SQL inline.
- `DbWrapper` es `partial`; un archivo por feature.
- Servicios: clase plana, `new DbWrapper()`, validación de negocio con
  `ArgumentException`, **doble `try/catch`** (`ArgumentException` → mensaje verbatim;
  `Exception` → `Log.Error` + mensaje genérico) y `Serilog`.
- Controladores: `[Authorize]` + `[RoutePrefix("api/<X>")]`, acciones **síncronas**,
  devuelven `ModelResponse<...>` tal cual (sin try/catch). `List` / `{id:long}` /
  `Guardar` (upsert) / `Eliminar` (lógico).
- El tenant (`empresaId`) se deriva **siempre** del claim autenticado
  (`ObtenerEmpresaIdDesdeClaim()`), nunca de un parámetro del cliente.
- El usuario autenticado se lee con `User.Identity.Name`.

### 6.3 MVC (frontend)

- **No** usa `SqlConnection` ni conoce la base de datos.
- Servicios: clase plana, ctor recibe `HttpClientConnection`; *passthrough* para
  list/save/delete, *unwrap* para get-by-id.
- Controladores heredan `BaseController` y tienen **dos regiones** por feature:
  `#region Views` (acciones `ActionResult`) y `#region Data Access` (endpoints AJAX que
  devuelven `Task<string>` = `JsonConvert.SerializeObject(response)`).
- Sesión = cookie `FormsAuthentication` (`name="autentication"`) cuyo `UserData` lleva
  el JSON del `TokenCookie`. El bearer **nunca** se expone al navegador.
- `Web.config`: `<authentication mode="Forms">` + `<machineKey>` fija compartida con la
  WebApi.

---

## 7. Seguridad y configuración (sin secretos)

- **Prohibido versionar secretos**: cadenas de conexión reales, `client_secret`,
  `machineKey` productiva, credenciales SMTP, tokens.
- `Web.config` y `*.config.example` llevan **placeholders**:
  `Server=__SQL_HOST__;Database=db_9c7990_puntoventadev;User Id=__SQL_USER__;Password=__SQL_PASSWORD__;...`
- En producción la cadena se inyecta por variable de entorno **`sConSql`**; `DbWrapper`
  lee primero `sConSql` y luego `connectionStrings["cCon"]`.
- `client_id` / `client_secret` deben coincidir entre MVC y WebApi.
- `machineKey` **fija** obligatoria (si no, cada reciclaje del AppPool invalida tokens y
  cookies).
- Nombre de base de datos canónico: **`db_9c7990_puntoventadev`**.
- Contraseñas: PBKDF2-SHA256, salt único, **≥ 100000 iteraciones**, comparación en tiempo
  constante. Nunca en texto plano ni en logs.

---

## 8. Estilo de código C#

- `PascalCase` para tipos, métodos, propiedades y constantes públicas; `camelCase` para
  parámetros y variables locales; `_camelCase` para campos privados.
- Llaves de Allman (llave en línea nueva). Indentación de 4 espacios.
- `using` ordenados (System primero) y sin usings sin usar.
- Preferir `var` cuando el tipo es evidente.
- Una clase pública por archivo; el nombre del archivo coincide con la clase.
- Mensajes y comentarios en **español**; identificadores según la convención existente
  (el dominio usa español: `Marca`, `Empresa`, `ObtenerMarcaPorId`).
- No dejar código muerto, `TODO` sin dueño ni *logging* de depuración (`Console.WriteLine`,
  `Debug.WriteLine`, `Log.Debug` de trazas temporales).

---

## 9. JSON y serialización

- WebApi: `CamelCasePropertyNamesContractResolver` en `WebApiConfig.Register`.
- MVC: `JsonConvert.SerializeObject` (Newtonsoft) para los endpoints AJAX (doble encode
  intencional; el JS hace `JSON.parse`).
- DataTables consume `result.Response` mediante `MapingPropertiesDataTable(...)`.

---

## 10. Base de datos (SSDT + SPs)

- Esquema versionado en `PuntoDeVenta.Database` (`Tables/`, `StoredProcedures/`,
  `Functions/`, `Views/`, `Security/`, `Seed/`).
- **Auditoría obligatoria** en toda tabla: `CreadoPor nvarchar(25) NOT NULL`,
  `FechaCreacion datetime NOT NULL`, `ModificadoPor nvarchar(25) NULL`,
  `FechaModificacion datetime NULL`.
- **Borrado lógico** en todas las entidades (`Estatus = 0`); prohibido `DELETE` físico.
- Los SPs reciben `@EmpresaId` y `@Actor` **solo** desde WebApi; filtran por tenant.
- Un archivo `.sql` por objeto. Sin credenciales en scripts.

---

## 11. Documentación y cambios (OpenSpec)

- Todo cambio no trivial se documenta con el workflow OpenSpec en `openspec/`:
  `exploration.md` → `proposal.md` → `specs/<capability>/spec.md` → `design.md` →
  `tasks.md`. Ver `.github/skills/openspec-*` y `.github/prompts/opsx-*`.
- Las specs usan **Given/When/Then** y palabras RFC 2119 (MUST/SHALL/SHOULD/MAY).
- Pruebas manuales y QA en `docs/` (ver `docs/checklist-pruebas-qa.md`).
- El estado compartido de ejecución vive en `.opencode/` (`todo.md`, `work-log.md`,
  `context.md`, `status.md`).

---

## 12. Checklist antes de dar por terminada una tarea

- [ ] Compila con **0 errores / 0 warnings** (build de los 3 proyectos C#).
- [ ] Todo archivo nuevo está registrado en el `.csproj` correspondiente.
- [ ] `.cshtml` en UTF-8 con BOM y acentos correctos.
- [ ] Respuestas API envueltas en `ModelResponse`.
- [ ] Sin secretos versionados.
- [ ] Tenant derivado del claim; sin `EmpresaId` aceptado del cliente.
- [ ] Sin borrado físico; auditoría poblada por los SPs.
- [ ] Sin `Console.WriteLine` / logging de depuración.

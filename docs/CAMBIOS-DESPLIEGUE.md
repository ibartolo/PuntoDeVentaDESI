# Bitácora de cambios — Configuración y despliegue

> Fecha: 2026-09-11
> Proyecto: PuntoDeVentaDESI (.NET Framework 4.8 — Entities / MVC / WebApi + SQL Server)

Este documento resume los cambios realizados para dejar la aplicación configurada,
desplegable y ejecutando (login, layout y Swagger). Se actualizará conforme avance la
puesta en marcha.

---

## 1. Configuración de `Web.config`

### 1.1 `PuntoDeVentaWebApi\Web.config`

Cadena de conexión real:

```xml
<connectionStrings>
  <add name="cCon"
       connectionString="Data Source=sql5080.site4now.net;Initial Catalog=db_9c7990_puntoventadev;User Id=db_9c7990_puntoventadev_admin;Password=Ifbc121290.01;Encrypt=True;TrustServerCertificate=True;"
       providerName="System.Data.SqlClient" />
</connectionStrings>
```

OAuth (debe coincidir con el MVC):

```xml
<add key="client_id" value="PuntoDeVentaMVC" />
<add key="client_secret" value="PdV_7e865c59d7c4e527f55cc823209fc2b40648fbda417d578a" />
```

`machineKey` compartida (misma en ambos proyectos):

```xml
<machineKey
  validationKey="0b033dd6685eaa8675ecb18ca0be82b236fb9502a507c2766b023976ccdaad7c4b36086fe70bcd0a55b29e817db1bcb9a394d88e656cd96a71bd2f72b456d640"
  decryptionKey="d6d1870ccfec356079e65c617280fab54b5a7dfb40047240739cf0c883698a74"
  validation="SHA1" decryption="AES" />
```

### 1.2 `PuntoDeVentaMVC\Web.config`

```xml
<add key="BaseUriWebApi" value="http://localhost:5102/" />
<add key="client_id" value="PuntoDeVentaMVC" />
<add key="client_secret" value="PdV_7e865c59d7c4e527f55cc823209fc2b40648fbda417d578a" />
<machineKey validationKey="0b033dd6...b456d640" decryptionKey="d6d1870c...c883698a74" validation="SHA1" decryption="AES" />
```

> `client_secret` y `machineKey` deben ser **idénticos** en ambos proyectos.

### 1.3 Valores pendientes (faltan datos del hosting)
- `BaseUriWebApi` (MVC) → URL pública de la WebApi desplegada.
- `AllowedCorsOrigins` (WebApi) → URL pública del MVC.
- `smtpClient`, `userEmail`, `passEmail` (WebApi) → envío del correo de recuperación.

---

## 2. Script SQL completo

Archivo generado:

```
C:\Git\PuntoDeVentaDESI\deploy\PuntoDeVenta-FullScript.sql
```

Contenido (orden de dependencias FK):
- `USE [db_9c7990_puntoventadev]`
- **29 tablas**
- **112 stored procedures**
- Seeds base + credencial inicial conocida

**Uso:** abrir en SSMS conectado a `db_9c7990_puntoventadev`, seleccionar todo y ejecutar (F5). Pensado para base vacía.

**Credencial inicial:**
- Usuario: `dev-admin`
- Contraseña: `Admin123!`
- Rol: Administrador (asignado al final del script)

---

## 3. Correcciones de runtime aplicadas

### 3.1 DLLs obsoletos del renombrado (controlador `Home` duplicado)

Síntoma: `Multiple types were found that match the controller named 'Home'`
(`PuntoDeVentaMVC.Controllers.HomeController` y `PuntoDeVenta.MVC.Controllers.HomeController`).

Causa: al renombrar los ensamblados (`MVC` → `PuntoDeVentaMVC`, `WebApi` → `PuntoDeVentaWebApi`),
`MSBuild Clean` no elimina los DLLs con nombre viejo que quedaron en `bin`.

Eliminados:
- `PuntoDeVentaMVC\bin\MVC.dll`
- `PuntoDeVentaMVC\bin\Entities.dll`
- `PuntoDeVentaWebApi\bin\WebApi.dll`
- `PuntoDeVentaWebApi\bin\Entities.dll`

### 3.2 Binding redirects (errores `System.IO.FileLoadException`)

Causa general: paquetes compilados contra versiones antiguas de ensamblados, mientras en
`bin` está una versión mayor. Se resolvió con redirects.

**`PuntoDeVentaWebApi\Web.config` (12 redirects):**

| Ensamblado | oldVersion | newVersion |
|---|---|---|
| Serilog | 0.0.0.0-4.3.0.0 | 4.3.0.0 |
| Serilog.Sinks.File | 0.0.0.0-7.0.0.0 | 7.0.0.0 |
| System.Web.Http | 0.0.0.0-5.3.0.0 | 5.3.0.0 |
| System.Web.Http.WebHost | 0.0.0.0-5.3.0.0 | 5.3.0.0 |
| System.Net.Http.Formatting | 0.0.0.0-6.0.0.0 | 6.0.0.0 |
| Microsoft.Owin | 0.0.0.0-4.2.3.0 | 4.2.3.0 |
| Microsoft.Owin.Host.SystemWeb | 0.0.0.0-4.2.3.0 | 4.2.3.0 |
| Microsoft.Owin.Security | 0.0.0.0-4.2.3.0 | 4.2.3.0 |
| Microsoft.Owin.Security.OAuth | 0.0.0.0-4.2.3.0 | 4.2.3.0 |
| Microsoft.Owin.Security.Cookies | 0.0.0.0-4.2.3.0 | 4.2.3.0 |
| Owin | 0.0.0.0-1.0.0.0 | 1.0.0.0 |
| Newtonsoft.Json | 0.0.0.0-13.0.0.0 | 13.0.0.0 |

**`PuntoDeVentaMVC\Web.config` (9 redirects):**
System.Web.Mvc, Serilog, Serilog.Sinks.File, Newtonsoft.Json, System.Net.Http.Formatting,
Microsoft.Owin, Microsoft.Owin.Security, Microsoft.Owin.Security.Cookies, Microsoft.Owin.Security.OAuth.

Casos concretos detectados:
- `Serilog.Sinks.File 7.0.0` pide `Serilog 4.2.0.0` → en `bin` 4.3.0.0.
- `System.Web.Http.Owin 5.3.0` pide `Microsoft.Owin 4.2.2.0` → en `bin` 4.2.3.0.
- `Swashbuckle.Core 5.6.0` pide `Newtonsoft.Json 7.0.0.0` → en `bin` 13.0.0.0 (bloqueaba Swagger).

### 3.3 Compilación de vistas Razor (C# 6 no soportado)

Síntoma: `CS1525: Invalid expression term '.'` en `_Layout.cshtml`.

Causa: el MVC no tiene el proveedor Roslyn (`Microsoft.CodeDom.Providers.DotNetCompilerPlatform`),
por lo que Razor compila con **C# 5**, que no admite `?.` (null-condicional) ni `$"..."` (interpolación).

Corregido (sintaxis compatible con C# 5):
- `Views\Shared\_Layout.cshtml` (3 casos): reemplazados `usuarioSesion?.X` por
  `usuarioSesion == null ? ... : usuarioSesion.X`.
- `Views\Home\Configuration.cshtml` (2 casos): interpolación `$"TemaUsuario_{id}"` →
  `"TemaUsuario_" + id`; `Request.Cookies[name]?.Value` → variable intermedia + ternario.

> Nota: queda un `?.` dentro de un comentario Razor `@* ... *@` en `Configuration.cshtml`
> (no se compila, es inofensivo).

### 3.4 Opcional recomendado (pendiente de decisión)
Agregar el paquete `Microsoft.CodeDom.Providers.DotNetCompilerPlatform` 4.1.0 + sección
`<system.codedom>` en `PuntoDeVentaMVC\Web.config` para habilitar C# 6+ en las vistas
(como en el proyecto de referencia ServiceDeskDESI). Así no hay que evitar `?.`/`$"..."`.

---

## 4. Estado actual

- Build de la solución: 3 proyectos C# **0 errores** (el proyecto SSDT `PuntoDeVenta.Database`
  reporta `MSB4057` por tooling; es pre-existente y no bloquea).
- **Login funciona** y el MVC redirige al layout sin error.
- **Swagger** (`http://localhost:5102/swagger`) cargable tras los redirects.

## 5. Pendientes

- [ ] Probar el resto de módulos y corregir errores reportados (el usuario los enviará).
- [ ] Definir `BaseUriWebApi`, `AllowedCorsOrigins` y SMTP para el hosting.
- [ ] (Opcional) Habilitar Roslyn en el MVC.
- [ ] (Opcional) Cambiar la contraseña inicial `Admin123!`.

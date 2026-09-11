# Apply Progress — scaffold-login-catalogo

## Mode

- **Resolved mode**: Standard (Strict TDD deshabilitado; sin test runner detectado)

## Completed in this batch

- [x] 1.1 Crear `PuntoDeVenta.sln` y proyectos `Entities`, `MVC`, `WebApi` targeting .NET Framework 4.8 con referencias cruzadas mínimas.
- [x] 1.2 Crear en `Entities/Models/` los contratos `Empresa`, `Usuario` y `Marca`; declarar `Usuario.EmpresaId bigint NOT NULL` y `Marca.EmpresaId bigint NOT NULL` como FKs a `Empresa.Id`, sin entidad de relación, y validar las 4 columnas de auditoría exactas.
- [x] 1.3 Crear `Entities/Contracts/ModelResponse.cs` y alinear la estructura (`Success`, `Message`, `Data`, `Errors`) para uso uniforme en API.
- [x] 1.4 Preparar `MVC/Web.config`, `WebApi/Web.config` y `*.config.example` con placeholders de conexión/keys, sin secretos reales ni `machineKey` productiva.
- [x] 2.1 Crear `Database/scripts/001-usuario-marca.sql` con tablas `Usuario` y `Marca`, sus FKs directas `EmpresaId bigint NOT NULL` a `Empresa.Id`, auditoría exacta y restricciones de estatus; no crear tabla de relación Usuario–Empresa.
- [x] 2.2 Definir en el mismo script índice único activo de Marca por empresa (`EmpresaId`,`Nombre`) y validaciones de longitud/obligatoriedad para `Nombre`.
- [x] 2.3 Crear SP `sp_Usuario_ObtenerParaLogin` para resolver Usuario y su única Empresa por `Usuario.EmpresaId`, activa/vigente (sin roles/sucursales).
- [x] 2.4 Crear SPs `sp_Marca_Insertar`, `sp_Marca_Consultar`, `sp_Marca_Listar`, `sp_Marca_Actualizar`, `sp_Marca_EliminarLogico` con filtro obligatorio por `@EmpresaId` y auditoría por `@Actor`.
- [x] 2.5 Crear `Database/scripts/seed-dev-user.sql` (solo desarrollo) con Empresa semilla + Usuario semilla cuyo `EmpresaId` apunte a ella, usando PBKDF2-SHA256 (salt único, >=100000 iteraciones) sin credenciales productivas.

## Remaining tasks

- [ ] 3.1 a 3.5 (autenticación y contexto empresarial seguro)
- [ ] 4.1 a 4.6 (CRUD MVC/API de Marcas)
- [ ] 5.1 a 5.4 (verificación manual y brecha de testing)

## Notes

- No se implementaron OAuth, FormsAuthentication operativo, endpoints WebApi ni vistas MVC de Marcas en este lote.

## Completed in this batch (infra fix: carga/compilación)

- [x] Corregir `PuntoDeVenta.sln` para que `MVC` y `WebApi` usen el `ProjectTypeGuid` de Web Application (`{349C5851-65DF-11DA-9384-00065B846F21}`), eliminando incompatibilidad de carga en Visual Studio.
- [x] Ajustar `MVC/MVC.csproj` y `WebApi/WebApi.csproj` para resolver dependencias ASP.NET clásicas con herramientas instaladas (`dotnet msbuild`):
  - `PackageReference` a `Microsoft.AspNet.Mvc 5.3.0`
  - `PackageReference` a `Microsoft.AspNet.WebApi.Core 5.3.0`
  - `PackageReference` a `Microsoft.AspNet.WebApi.WebHost 5.3.0`
  - referencias explícitas con `HintPath` a `System.Web.Mvc`, `System.Web.Http`, `System.Web.Http.WebHost`.
- [x] Validar restauración y compilación real en entorno local:
  - `dotnet msbuild PuntoDeVenta.sln /t:Restore /p:RestorePackagesConfig=true` ✅
  - `dotnet msbuild PuntoDeVenta.sln /t:Build /p:Configuration=Debug` ✅
  - `dotnet msbuild Entities/Entities.csproj /t:Build /p:Configuration=Debug` ✅
  - `dotnet msbuild MVC/MVC.csproj /t:Build /p:Configuration=Debug` ✅
  - `dotnet msbuild WebApi/WebApi.csproj /t:Build /p:Configuration=Debug` ✅

## Validation evidence

- Antes del fix: errores `CS0234` y advertencias `MSB3245` por referencias no resueltas (`System.Web.Mvc`, `System.Web.Http`) y no carga correcta de proyectos web.
- Después del fix: compilan solución y los 3 proyectos en Debug con los artefactos:
  - `Entities/bin/Debug/Entities.dll`
  - `MVC/bin/MVC.dll`
  - `WebApi/bin/WebApi.dll`

## Completed in this batch (investigación Visual Studio: solo se muestra Entities)

- [x] Verificar estructura real de `PuntoDeVenta.sln` (contenido exacto): incluye los 3 `Project(...)` (`Entities`, `MVC`, `WebApi`) y sus entradas en `ProjectConfigurationPlatforms` para `Debug|Any CPU` y `Release|Any CPU`.
- [x] Verificar archivos de proyecto y estructura de carpetas: existen `Entities/Entities.csproj`, `MVC/MVC.csproj`, `WebApi/WebApi.csproj` y sus directorios físicos en disco.
- [x] Validar funcionalidad por CLI (restore/build solicitado por usuario):
  - `dotnet sln PuntoDeVenta.sln list` ✅ (3 proyectos)
  - `dotnet msbuild PuntoDeVenta.sln /t:Restore /p:RestorePackagesConfig=true` ✅
  - `dotnet msbuild PuntoDeVenta.sln /t:Build /p:Configuration=Debug /p:Platform="Any CPU"` ✅
- [x] Identificar causa persistida en estado local de Visual Studio: en `.vs/PuntoDeVenta/v17/.suo` se detectan claves de estado de solución (`UnloadedProjects`, `UnloadedProjectsEx`, `UnloadedProjectsOne`) junto con rutas de `MVC\MVC.csproj` y `WebApi\WebApi.csproj`, consistente con proyectos descargados/ocultos por estado de sesión y no por estructura inválida del `.sln`.
- [x] Corrección aplicada: eliminar `.vs/PuntoDeVenta/v17/.suo` para forzar reconstrucción limpia del estado de carga de proyectos al reabrir la solución.

## Notes (investigación VS)

- No se encontró `.slnf` (solution filter) ni otros `.sln` alternos en el workspace.
- La incidencia observada es de estado local de Visual Studio (cache de solución), no de compilación ni de referencias de proyecto en el `.sln` actual.

## Completed in this batch (compatibilidad real csproj/sln para Web Application Project)

- [x] Diagnosticar por qué Visual Studio seguía ocultando `MVC` y `WebApi` aun con restore/build correctos por CLI:
  - Se validó que los 3 proyectos existen físicamente y en solución.
  - Se reprodujo la carga fallida con `devenv.com PuntoDeVenta.sln /Build "Debug|Any CPU"` mostrando `The application for the project is not installed` para ambos proyectos web.
  - Se confirmó que el entorno sí tiene targets web clásicos (`...\MSBuild\Microsoft\VisualStudio\v18.0\WebApplications\Microsoft.WebApplication.targets` existe).
- [x] Corregir `PuntoDeVenta.sln` para compatibilidad de carga en Visual Studio con proyectos web clásicos:
  - En `.sln`, `MVC` y `WebApi` se ajustaron para usar **ProjectType del proyecto C#** (`{FAE04EC0-301F-11D3-BF4B-00C04F79EFBC}`) en lugar del GUID de flavor web.
  - Se mantuvo en cada `.csproj` el flavor de Web Application mediante `ProjectTypeGuids` que incluye `{349C5851-65DF-11DA-9384-00065B846F21}`.
- [x] Endurecer compatibilidad de `.csproj` web clásicos para toolsets VS modernos:
  - Se agregó `VisualStudioVersion` por defecto (`17.0`) y `VSToolsPath` derivado para resolver imports de WebApplication targets.
  - Se reemplazó import frágil por imports condicionales con `Exists(...)` para:
    - `$(VSToolsPath)\WebApplications\Microsoft.WebApplication.targets`
    - fallback a `v16.0` y `v15.0` cuando aplique.
- [x] Resolver requisito de restore NuGet en compilación desde `devenv`:
  - Se detectó error de NuGet en VS (`RuntimeIdentifier 'win'` no listado) al compilar desde IDE/`devenv`.
  - Se agregó `RuntimeIdentifiers` con `win` en `MVC.csproj` y `WebApi.csproj`.
  - Se ejecutó restore explícito con MSBuild de Visual Studio (`MSBuild.exe` .NET Framework) antes de build en `devenv`.

## Validation evidence (compatibilidad VS real)

- Tooling/SDK detectado:
  - `dotnet --info` => SDK 10.0.300, MSBuild 18.6.3
  - Visual Studio principal => `Visual Studio Professional 2026 (18.6.2)`
  - Workload/componente ASP.NET detectado por `vswhere -requires Microsoft.VisualStudio.Workload.NetWeb` y `-requires Microsoft.VisualStudio.Component.AspNet`
- Confirmación de targets web clásicos instalados:
  - `C:\Program Files\Microsoft Visual Studio\18\Professional\MSBuild\Microsoft\VisualStudio\v18.0\WebApplications\Microsoft.WebApplication.targets` => `True`
- Validación de restore/build/parsing:
  - `dotnet msbuild PuntoDeVenta.sln /t:Restore /p:RestorePackagesConfig=true /p:VisualStudioVersion=17.0` ✅
  - `dotnet msbuild PuntoDeVenta.sln /t:Build /p:Configuration=Debug /p:Platform="Any CPU" /p:VisualStudioVersion=17.0` ✅
  - `"...\MSBuild\Current\Bin\MSBuild.exe" PuntoDeVenta.sln /t:Restore` ✅
  - `"...\Common7\IDE\devenv.com" PuntoDeVenta.sln /Build "Debug|Any CPU"` ✅ (cargan y compilan `MVC` y `WebApi`; 2 succeeded, 0 failed)

## Notes (causa raíz confirmada)

- En este entorno, usar en `.sln` el GUID web `{349C...}` como tipo de proyecto para `MVC`/`WebApi` provoca que Visual Studio intente resolverlos como tipo de proyecto no instalado y los descargue/oculte.
- La forma compatible es:
  - `.sln`: tipo de proyecto C# (`{FAE04...}`)
  - `.csproj`: `ProjectTypeGuids` con flavor web + import válido de `Microsoft.WebApplication.targets`.

## Completed in this batch (Phase 3: autenticación y contexto empresarial seguro)

- [x] 3.1 Implementar en `WebApi` el endpoint `POST /oauth/token` y servicio de autenticación ADO.NET que use `sp_Usuario_ObtenerParaLogin`.
  - Se agregó OWIN OAuth2 en `WebApi/App_Start/Startup.cs` con `TokenEndpointPath = "/oauth/token"`.
  - `TokenAuthorizationServerProvider` autentica contra `AuthService`.
  - `AuthDal` consume `sp_Usuario_ObtenerParaLogin` vía `SqlConnection` + `SqlCommand`.
- [x] 3.2 Implementar verificación PBKDF2-SHA256 en `WebApi/Services/Auth/*` con comparación en tiempo constante y rechazo de credenciales inválidas.
  - `Pbkdf2PasswordVerifier` valida salt/hash Base64, exige `>= 100000` iteraciones y compara bytes en tiempo constante.
  - Credenciales inválidas retornan `invalid_grant` (sin sesión/token).
- [x] 3.3 Emitir bearer con claims mínimos (`sub`, `empresaId`) derivados de `Usuario.EmpresaId` por el servidor; prohibir `EmpresaId` desde formulario, querystring o body y restringir el bearer a MVC-servidor → WebApi.
  - Claims emitidos: `sub` y `empresaId` desde resultado del SP (servidor).
  - No se recibe ni se utiliza `EmpresaId` desde request de login.
  - En MVC, el bearer se guarda únicamente en `Session["ServerBearerToken"]`.
- [x] 3.4 Implementar en `MVC/Controllers/AccountController` el flujo de login/logout con FormsAuthentication usando identidad ya validada por OAuth2, sin exponer bearer al navegador.
  - Login MVC pide token OAuth (`/oauth/token`), luego consulta `api/auth-context` con bearer servidor→API para resolver `sub` y `empresaId`.
  - FormsAuth cookie guarda solo `sub` y `empresaId` (no bearer).
  - Logout limpia sesión de servidor y hace `FormsAuthentication.SignOut()`.
- [x] 3.5 Implementar helper/filtro en `WebApi` para extraer `empresaId` desde claim autenticado y reutilizarlo como única fuente de tenant.
  - `TenantContext` centraliza lectura de `empresaId`/`sub` desde claims.
  - `RequireEmpresaClaimAttribute` exige claims mínimos en endpoints autenticados.
  - `BaseApiController` expone `EmpresaIdAutenticada` y `SubjectAutenticado` como fuente única.

## Validation evidence (Phase 3)

- Restore/build ejecutados exitosamente tras implementación:
  - `dotnet msbuild PuntoDeVenta.sln /t:Restore /p:RestorePackagesConfig=true` ✅
  - `dotnet msbuild PuntoDeVenta.sln /t:Build /p:Configuration=Debug /p:Platform="Any CPU"` ✅
- Smoke de seguridad del bearer en MVC:
  - Revisión de código confirma que el bearer solo se guarda/limpia en sesión de servidor (`Session["ServerBearerToken"]`) y se usa en header Authorization del cliente interno MVC→WebApi.
  - No se inyecta bearer en vistas, formularios ni cookies FormsAuth.

## Remaining tasks

- [ ] 4.1 a 4.6 (CRUD MVC/API de Marcas)
- [ ] 5.1 a 5.4 (verificación manual y brecha de testing)

## Completed in this batch (diagnóstico/corrección ERR_INVALID_HTTP_RESPONSE en localhost)

- [x] Diagnosticar causa real del error `localhost sent an invalid response / ERR_INVALID_HTTP_RESPONSE` sin asumir BD/autenticación.
  - Se verificó que el puerto configurado para MVC (`http://localhost:5001/`) estaba ocupado por un proceso ajeno (`java`, PID 8924), evidenciado con `netstat -ano | findstr :5001`.
  - Se confirmó que IIS Express no podía iniciar MVC en ese puerto: `Failed to register URL "http://localhost:5001/" ... (0x80070020)`.
  - Se descartó causa de protocolo/HTTPS en app al validar que el binding vigente de IIS Express para el sitio era HTTP (no TLS) en `applicationhost.config`.
- [x] Corregir configuración de puertos IIS Express/proyecto para eliminar colisión.
  - `MVC/MVC.csproj`: `IISUrl` de `http://localhost:5001/` a `http://localhost:5101/`.
  - `WebApi/WebApi.csproj`: `IISUrl` de `http://localhost:5002/` a `http://localhost:5102/`.
  - `.vs/PuntoDeVenta/config/applicationhost.config`: bindings HTTP `5001/5002` actualizados a `5101/5102` para sitios `MVC`/`WebApi`.
  - `MVC/Web.config`, `MVC/Web.Debug.config`, `MVC/Web.Local.config`: `BaseUriWebApi` actualizado a `http://localhost:5102/` para mantener consistencia MVC→WebApi.
- [x] Validación local por petición HTTP tras la corrección (sin conexión a SQL remoto).
  - MVC levantado en `5101` y respuesta HTTP válida (`302` por redirección a login), descartando respuesta inválida a nivel protocolo.
  - WebApi levantado en `5102` y `GET /api/auth-context` respondió `401` (esperado sin bearer), confirmando que el servidor devuelve respuesta HTTP válida.

## Validation evidence (ERR_INVALID_HTTP_RESPONSE)

- Antes del fix:
  - `netstat -ano | findstr :5001` => puerto en `LISTENING` por proceso `java` (PID 8924).
  - `iisexpress /config:.vs/.../applicationhost.config /site:MVC` => `Failed to register URL "http://localhost:5001/" ... (0x80070020)`.
- Después del fix:
  - `Invoke-WebRequest http://localhost:5101/` => `302` (HTTP correcto, redirección esperada por `[Authorize]`).
  - `Invoke-WebRequest http://localhost:5102/api/auth-context` => `401` (HTTP correcto, endpoint protegido).

## Notes (diagnóstico)

- La causa raíz fue **colisión de puerto IIS Express** en `5001` (ocupado por otro proceso), no base de datos ni autenticación.
- No se ejecutó ninguna conexión/publicación/operación contra SQL Server remoto.

## Update (decisión explícita del usuario)

- Se documenta decisión explícita de diferir/omitir la validación contra SQL Server remoto en este change.
- Las tareas **5.2** (checklist manual end-to-end) y **5.3** (evidencia SQL de auditoría/no borrado físico) permanecen **pendientes** por decisión del usuario.
- No se conectó a SQL Server remoto, no se ejecutó SQL y no se realizó publicación ni despliegue.
- Se conserva el historial previo de `apply-progress.md`; esta actualización solo agrega trazabilidad de la decisión.

## Completed in this batch (Phase 5 safe batch: 5.1 + 5.4)

- [x] 5.1 Preparar `docs/manual-tests/scaffold-login-catalogo.md` con cobertura manual Given/When/Then para escenarios de las 3 delta specs (`scaffold-solucion`, `autenticacion-login`, `catalogo-marcas`).
  - Se documentaron casos MT-SCF-01..04, MT-AUT-01..04 y MT-MAR-01..06.
  - Se incluyeron precondiciones, evidencia esperada y restricciones operativas del lote (sin conexión/publicación/despliegue SQL).
- [x] 5.4 Registrar formalmente la brecha técnica de test runner inexistente y dejar tarea explícita para cambio posterior de habilitación de pruebas automatizadas.
  - Se dejó sección dedicada “Registro formal de brecha de test runner (Task 5.4)”.
  - Se definió checklist mínimo para cambio futuro sugerido `enable-automated-tests-net48`.

## Validation evidence (Phase 5 safe batch)

- `openspec/changes/scaffold-login-catalogo/tasks.md` actualizado con `5.1` y `5.4` en estado `[x]`.
- `docs/manual-tests/scaffold-login-catalogo.md` creado con pruebas manuales Given/When/Then, evidencia esperada y guía de verificación SQL diferida para 5.3.
- Se respetó la restricción del lote: no se ejecutaron 5.2/5.3 y no hubo conexión/publicación/despliegue/creación de base de datos.

## Remaining tasks

- [ ] 5.2 Ejecutar checklist manual end-to-end (pendiente de autorización y entorno SQL Server disponible).
- [ ] 5.3 Verificar evidencias SQL de auditoría y ausencia de borrado físico (pendiente de autorización y entorno SQL Server disponible).

## Notes (Phase 5 safe batch)

- No se implementó funcionalidad adicional de negocio.
- No se modificaron endpoints, SPs ni flujo de autenticación/CRUD; solo artefactos de documentación y seguimiento OpenSpec.

## Completed in this batch (Phase 4: CRUD MVC/API de Marcas con borrado lógico)

- [x] 4.1 Crear `WebApi/Controllers/MarcasController.cs` con GET lista, GET detalle, POST, PUT y DELETE lógico, respondiendo siempre `ModelResponse`.
  - Se agregó `WebApi/Controllers/MarcasController.cs` con rutas `api/marcas`:
    - `GET /api/marcas` (lista activas),
    - `GET /api/marcas/{id}` (detalle),
    - `POST /api/marcas` (alta),
    - `PUT /api/marcas/{id}` (edición),
    - `DELETE /api/marcas/{id}` (desactivación lógica).
  - Todas las acciones devuelven `ModelResponse`/`ModelResponse<T>` en éxito y error.
- [x] 4.2 Crear `WebApi/Services/MarcaService.cs` y `WebApi/Dal/MarcaDal.cs` en ADO.NET + SPs, sin aceptar `EmpresaId` del cliente.
  - `MarcaDal` consume exclusivamente `sp_Marca_Listar`, `sp_Marca_Consultar`, `sp_Marca_Insertar`, `sp_Marca_Actualizar`, `sp_Marca_EliminarLogico` por `SqlConnection`/`SqlCommand`.
  - `EmpresaId` llega solo desde claims autenticados (`BaseApiController`), nunca por body/query.
  - Actor/auditoría (`@Actor`) se deriva de claim `sub`, no del cliente.
- [x] 4.3 Implementar validaciones de negocio (Nombre requerido, <=100, duplicado activo por empresa) y errores controlados.
  - Validación de `Nombre` requerido y máximo 100 en `MarcaService`.
  - Duplicado activo por Empresa se controla por índice único filtrado y captura de `SqlException` (2601/2627), devolviendo error de negocio controlado.
  - Errores de marca no encontrada/inactiva/tenant ajeno se devuelven sin filtrar datos de otras empresas.
- [x] 4.4 Crear `MVC/Controllers/MarcasController.cs` y cliente HTTP interno para consumir API con bearer de sesión servidor.
  - Se agregó `MVC/Services/MarcasApiClient.cs` con consumo HTTP interno MVC→WebApi usando bearer desde `Session["ServerBearerToken"]`.
  - Se agregó `MVC/Controllers/MarcasController.cs` protegido con `[Authorize]` para flujo Index/Create/Edit/Details/Delete.
  - Si el bearer de sesión está ausente/expirado, redirige a `Account/Login`.
- [x] 4.5 Crear vistas `MVC/Views/Marcas/{Index,Create,Edit,Details,Delete}.cshtml` aclarando que “Eliminar” desactiva (`Estatus=Inactivo`).
  - Se crearon las cinco vistas solicitadas.
  - `Delete.cshtml` explicita que la operación desactiva (`Estatus = Inactivo`) y no elimina físicamente.
- [x] 4.6 Confirmar que listados operativos muestran solo Marcas activas de la empresa autenticada y ocultan inactivas por defecto.
  - `sp_Marca_Listar` ya filtra `Estatus=1` por `@EmpresaId`.
  - En MVC `Index` también aplica filtro defensivo `Estatus == true` antes de renderizar.

## Validation evidence (Phase 4)

- Build técnico ejecutado posterior al cambio:
  - `dotnet msbuild Entities/Entities.csproj /t:Build /p:Configuration=Debug` ✅
  - `dotnet msbuild MVC/MVC.csproj /t:Build /p:Configuration=Debug` ✅
  - `dotnet msbuild WebApi/WebApi.csproj /t:Build /p:Configuration=Debug` ✅
- Restore/Build de solución completa con `dotnet msbuild PuntoDeVenta.sln`:
  - Restore ✅ (con warning esperado de `PuntoDeVenta.Database.sqlproj` en restore por dotnet)
  - Build parcial: `MVC`, `WebApi`, `Entities` compilan ✅; `PuntoDeVenta.Database.sqlproj` reporta `MSB4057` en target `Build` al usar `dotnet msbuild` (comportamiento de tooling SSDT fuera de este alcance funcional).

## Remaining tasks

- [ ] 5.1 a 5.4 (verificación manual y brecha de testing)

## Notes (Phase 3)

- Se agregó endpoint técnico `GET /api/auth-context` para que MVC valide contexto autenticado (`sub`, `empresaId`) sin exponer bearer al navegador; no pertenece al CRUD de Marcas.
- No se implementaron endpoints ni vistas CRUD de Marcas en este lote.

## Completed in this batch (Phase 2 extension: SQL Server Database Project)

- [x] 2.6 Agregar proyecto SSDT `PuntoDeVenta.Database` e incluirlo en `PuntoDeVenta.sln` para versionar esquema SQL en source control, sin ejecutar creación/publicación de base ni conexión a SQL Server.
- [x] Estructurar el proyecto SQL versionable con carpetas requeridas:
  - `Tables/`, `StoredProcedures/`, `Functions/`, `Views/`, `Security/`, `Seed/`
  - scripts de soporte en `Scripts/PreDeploy`, `Scripts/PostDeploy`, `Scripts/Migrations`, `Scripts/Deploy`
- [x] Migrar/refactorizar definiciones SQL existentes a archivos por objeto dentro de `PuntoDeVenta.Database`:
  - tablas `Empresa`, `Usuario`, `Marca`
  - SPs `sp_Usuario_ObtenerParaLogin`, `sp_Marca_Insertar`, `sp_Marca_Consultar`, `sp_Marca_Listar`, `sp_Marca_Actualizar`, `sp_Marca_EliminarLogico`
- [x] Separar explícitamente semilla DEV (`Seed/seed-dev-user.sql`) fuera de publicación productiva por defecto (`Script.PostDeployment.sql` no la incluye).
- [x] Convertir scripts heredados `script.sql`, `Database/scripts/001-usuario-marca.sql` y `Database/scripts/seed-dev-user.sql` en wrappers SQLCMD `:r` hacia archivos del proyecto SSDT para evitar duplicación de objetos SQL.
- [x] Agregar plantilla de publicación sin credenciales reales: `Scripts/Deploy/Publish.Template.publish.xml`.

## Validation evidence (Phase 2 extension)

- `PuntoDeVenta.sln` incluye `PuntoDeVenta.Database\PuntoDeVenta.Database.sqlproj` con configuraciones `Debug|Any CPU` y `Release|Any CPU`.
- `PuntoDeVenta.Database.sqlproj` incluye `Build` de objetos SQL (tablas/SPs), `PreDeploy`, `PostDeploy` y `None` para `Seed`/plantillas.
- `Scripts/PostDeploy/Script.PostDeployment.sql` documenta explícitamente que no incluye `seed-dev` en publicación de producción por defecto.
- Los scripts heredados ahora apuntan por `:r` al árbol SSDT, dejando una sola fuente de verdad versionada.

## Remaining tasks

- [ ] 4.1 a 4.6 (CRUD MVC/API de Marcas)
- [ ] 5.1 a 5.4 (verificación manual y brecha de testing)

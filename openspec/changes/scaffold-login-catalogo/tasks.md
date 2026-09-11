# Tasks: Scaffold de solución, Login y Catálogo de Marcas

## Phase 1: Fundación de solución y contratos

- [x] 1.1 Crear `PuntoDeVenta.sln` y proyectos `Entities`, `MVC`, `WebApi` targeting .NET Framework 4.8 con referencias cruzadas mínimas.
- [x] 1.2 Crear en `Entities/Models/` los contratos `Empresa`, `Usuario` y `Marca`; declarar `Usuario.EmpresaId bigint NOT NULL` y `Marca.EmpresaId bigint NOT NULL` como FKs a `Empresa.Id`, sin entidad de relación, y validar las 4 columnas de auditoría exactas.
- [x] 1.3 Crear `Entities/Contracts/ModelResponse.cs` y alinear la estructura (`Success`, `Message`, `Data`, `Errors`) para uso uniforme en API.
- [x] 1.4 Preparar `MVC/Web.config`, `WebApi/Web.config` y `*.config.example` con placeholders de conexión/keys, sin secretos reales ni `machineKey` productiva.

## Phase 2: Base de datos y semilla de desarrollo

- [x] 2.1 Crear `Database/scripts/001-usuario-marca.sql` con tablas `Usuario` y `Marca`, sus FKs directas `EmpresaId bigint NOT NULL` a `Empresa.Id`, auditoría exacta y restricciones de estatus; no crear tabla de relación Usuario–Empresa.
- [x] 2.2 Definir en el mismo script índice único activo de Marca por empresa (`EmpresaId`,`Nombre`) y validaciones de longitud/obligatoriedad para `Nombre`.
- [x] 2.3 Crear SP `sp_Usuario_ObtenerParaLogin` para resolver Usuario y su única Empresa por `Usuario.EmpresaId`, activa/vigente (sin roles/sucursales).
- [x] 2.4 Crear SPs `sp_Marca_Insertar`, `sp_Marca_Consultar`, `sp_Marca_Listar`, `sp_Marca_Actualizar`, `sp_Marca_EliminarLogico` con filtro obligatorio por `@EmpresaId` y auditoría por `@Actor`.
- [x] 2.5 Crear `Database/scripts/seed-dev-user.sql` (solo desarrollo) con Empresa semilla + Usuario semilla cuyo `EmpresaId` apunte a ella, usando PBKDF2-SHA256 (salt único, >=100000 iteraciones) sin credenciales productivas.
- [x] 2.6 Agregar `PuntoDeVenta.Database.sqlproj` (SSDT) a la solución y versionar esquema en `Tables/`, `StoredProcedures/`, `Functions/`, `Views/`, `Security/`, `Seed/` con plantillas de migración/publicación sin credenciales y sin seed-dev en publicación productiva por defecto.

## Phase 3: Autenticación y contexto empresarial seguro

- [x] 3.1 Implementar en `WebApi` el endpoint `POST /oauth/token` y servicio de autenticación ADO.NET que use `sp_Usuario_ObtenerParaLogin`.
- [x] 3.2 Implementar verificación PBKDF2-SHA256 en `WebApi/Services/Auth/*` con comparación en tiempo constante y rechazo de credenciales inválidas.
- [x] 3.3 Emitir bearer con claims mínimos (`sub`, `empresaId`) derivados de `Usuario.EmpresaId` por el servidor; prohibir `EmpresaId` desde formulario, querystring o body y restringir el bearer a MVC-servidor → WebApi.
- [x] 3.4 Implementar en `MVC/Controllers/AccountController` el flujo de login/logout con FormsAuthentication usando identidad ya validada por OAuth2, sin exponer bearer al navegador.
- [x] 3.5 Implementar helper/filtro en `WebApi` para extraer `empresaId` desde claim autenticado y reutilizarlo como única fuente de tenant.

## Phase 4: CRUD MVC/API de Marcas con borrado lógico

- [x] 4.1 Crear `WebApi/Controllers/MarcasController.cs` con GET lista, GET detalle, POST, PUT y DELETE lógico, respondiendo siempre `ModelResponse`.
- [x] 4.2 Crear `WebApi/Services/MarcaService.cs` y `WebApi/Dal/MarcaDal.cs` en ADO.NET + SPs, sin aceptar `EmpresaId` del cliente.
- [x] 4.3 Implementar validaciones de negocio (Nombre requerido, <=100, duplicado activo por empresa) y errores controlados.
- [x] 4.4 Crear `MVC/Controllers/MarcasController.cs` y cliente HTTP interno para consumir API con bearer de sesión servidor.
- [x] 4.5 Crear vistas `MVC/Views/Marcas/{Index,Create,Edit,Details,Delete}.cshtml` aclarando que “Eliminar” desactiva (`Estatus=Inactivo`).
- [x] 4.6 Confirmar que listados operativos muestran solo Marcas activas de la empresa autenticada y ocultan inactivas por defecto.

## Phase 5: Verificación honesta sin runner automático

- [x] 5.1 Preparar `docs/manual-tests/scaffold-login-catalogo.md` con casos manuales Given/When/Then para todos los escenarios de las 3 delta specs.
- [ ] 5.2 Ejecutar checklist manual: login exitoso, credenciales inválidas, empresa inactiva/vencida, CRUD Marca, duplicados, borrado lógico, aislamiento A/B.
- [ ] 5.3 Verificar en SQL evidencias de auditoría (`CreadoPor/FechaCreacion`, `ModificadoPor/FechaModificacion`) y ausencia de borrado físico de Marca.
- [x] 5.4 Registrar brecha técnica: no existe test runner (xUnit/NUnit/MSTest) y dejar tarea explícita para habilitar pruebas automatizadas en cambio posterior.

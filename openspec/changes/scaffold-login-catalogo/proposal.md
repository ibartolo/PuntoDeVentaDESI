# Proposal: Scaffold de solución, Login y Catálogo de Marcas

## Intent

El repositorio es greenfield para código; ya existe `script.sql` con la tabla `Empresa` en la base compartida `db_9c7990_servicedeskdesi`. El proyecto queda confirmado como **multiempresa**: una app MVC, una Web API y una única base de datos SQL Server compartida. Se necesita el primer vertical slice ejecutable —scaffold, autenticación consciente de empresa y un catálogo maestro aislado por empresa— para validar la arquitectura prescrita (MVC 5 + Web API 5, .NET Framework 4.8, ADO.NET + SPs, OAuth2 + FormsAuthentication).

**Impacto de tenancy**: el aislamiento multiempresa se introduce como propiedad transversal, no como módulo nuevo. Sin CRUD de Empresa ni sucursales; `Empresa` se consume tal como está en `script.sql`, agregando solo `Usuario.EmpresaId bigint NOT NULL` como FK a `Empresa.Id`. Cada Usuario pertenece a exactamente una Empresa y una Empresa puede tener varios Usuarios; no existe tabla intermedia.

## Scope

### In Scope
- Scaffold `.sln` con proyectos `Entities`, `MVC`, `WebApi` (patrón ServiceDeskDESI, skill `netframework-mvc-webapi`).
- Tabla `Empresa` conforme a `script.sql`, consumida como contexto técnico obligatorio, no como módulo administrable.
- Relación `Usuario → Empresa`: cada Usuario pertenece exactamente a una sola Empresa mediante `Usuario.EmpresaId bigint NOT NULL` (FK a `Empresa.Id`); una Empresa puede tener varios Usuarios y no existe tabla intermedia. Resuelve de forma segura la empresa activa en login.
- Login: OAuth2 + FormsAuthentication, usuario inicial sembrado vía script SQL de desarrollo (hash, sin credenciales productivas), asociado a Empresa activa/vigente. Token/cookie porta `EmpresaId` derivado del servidor a partir de la identidad autenticada, nunca de un valor del navegador.
- Catálogo **Marcas**: CRUD completo (alta, consulta, actualización, eliminación lógica) vía MVC + Web API con `ModelResponse`, aislado por `EmpresaId`.
- Campos de Marca: `EmpresaId` (FK a `Empresa.Id`), `Nombre` (único por empresa), `Descripción` (opcional). Foto queda fuera.
- `Estatus` en Marca: **Eliminar** solo hace borrado lógico (`Estatus = Inactivo`); nunca borrado físico. Consultas filtran por `EmpresaId` del claim y por `Estatus`.
- Unicidad recomendada: índice único `(EmpresaId, Nombre)` en Marca.
- Auditoría obligatoria: toda tabla nueva (`Marca`, `Usuario`) incluye exactamente `CreadoPor nvarchar(25) NOT NULL`, `FechaCreacion datetime NOT NULL`, `ModificadoPor nvarchar(25) NULL`, `FechaModificacion datetime NULL`, igual que `Empresa`.
- ADO.NET + SPs para Marca; los SPs reciben/derivan el `EmpresaId` autenticado, nunca un parámetro libre del cliente.

### Out of Scope
- CRUD de Empresa y de sucursales.
- Productos, códigos de barras/QR, categorías, proveedores, stock, precios.
- Roles/permisos, selección de sucursal, multi-sucursal y usuarios con varias empresas.
- Clientes, ventas, caja chica, cortes, cancelaciones/devoluciones, reportes.
- Fotos y lectura por cámara.
- Reglas de negocio sobre vigencia/periodo de prueba de Empresa: se usan tal como existen en `script.sql`.
- Hosting SQL Server 2019 real (instancia, TLS, firewall, credenciales productivas): pendiente de despliegue, sin versionarse.

## Capabilities

### New Capabilities
- `scaffold-solucion`: estructura de solución .NET (Entities/MVC/WebApi) lista para compilar y ejecutar, incluyendo el modelo `Empresa` (según `script.sql`) y `Usuario.EmpresaId NOT NULL` como FK directa a `Empresa` (sin tabla intermedia).
- `autenticacion-login`: login vía OAuth2 + FormsAuthentication con usuario sembrado por script SQL de desarrollo, asociado a una Empresa activa/vigente; el token/cookie porta `EmpresaId` derivado del servidor.
- `catalogo-marcas`: CRUD completo de Marca (EmpresaId, Nombre, Descripción, Estatus) con eliminación lógica y aislamiento por `EmpresaId`.

### Modified Capabilities
- None

## Approach

Skill `netframework-mvc-webapi`: `Entities` para `Empresa` (según `script.sql`), `Usuario` (con `EmpresaId NOT NULL` FK directa) y `Marca`, además de `ModelResponse`; `WebApi` expone endpoints consumidos por `MVC`. ADO.NET + SPs (`sp_Marca_Insertar/_Consultar/_Actualizar/_EliminarLogico`) parametrizados por `EmpresaId` derivado del servidor. Login: OAuth2 bearer + cookie FormsAuthentication con `EmpresaId` resuelto desde `Usuario.EmpresaId` en autenticación, nunca desde el formulario o URL. Script `seed-dev-user.sql` crea Empresa semilla y usuario dev con contraseña hasheada y `EmpresaId` asignado, sin promocionarse a hosting.

## Affected Areas

| Area | Impact | Description |
|------|--------|--------------|
| `Entities/` | New | `Empresa` (script.sql), `Usuario` con `EmpresaId NOT NULL` (FK directa a Empresa), `Marca` con `EmpresaId`, `ModelResponse` |
| `MVC/` | New | Vistas de login y CRUD de Marcas (sin selector de empresa en UI) |
| `WebApi/` | New | Controladores de autenticación y Marcas; resolución de `EmpresaId` desde claims |
| `Database/scripts/` | New | Tablas `Usuario`/`Marca` (FKs y auditoría), SPs filtrados por `EmpresaId`, script de usuario+empresa dev |

## Risks

| Risk | Likelihood | Mitigation |
|------|------------|------------|
| Sin runner de pruebas ni solución existente | High | Scaffold define estructura; verificación manual/build en esta fase |
| Confusión sobre "Eliminar" (esperan borrado físico) | Med | UI indica "Desactivar"; listados filtran inactivos por defecto |
| Fuga de datos entre empresas si `EmpresaId` viene del cliente | High | `EmpresaId` se deriva solo del claim/sesión; SPs y controladores nunca aceptan parámetro libre |
| Script de usuario dev promovido a hosting | Med | Documentar que es solo-dev; credenciales productivas nunca se versionan |
| Falta de parámetros de hosting SQL Server 2019 | Med | Pendientes de despliegue; solo plantillas sin valores reales |

## Rollback Plan

Change aditivo (no toca código existente; `Empresa` de `script.sql` no se modifica). Revertir = eliminar proyectos `.sln`/`.csproj` y scripts SQL añadidos (`Usuario`, `Marca`, SPs); sin datos productivos ni migraciones irreversibles.

## Dependencies

- Skill `netframework-mvc-webapi` (estructura, ModelResponse, OAuth2+FormsAuthentication).
- SQL Server 2019 con base `db_9c7990_servicedeskdesi` y tabla `Empresa` ya creada por `script.sql`.

## Success Criteria

- [ ] `.sln` compila con los 3 proyectos.
- [ ] Usuario dev, asociado a Empresa activa/vigente, inicia sesión y su token porta `EmpresaId`.
- [ ] CRUD completo de Marca extremo a extremo, acotado a la Empresa del usuario autenticado.
- [ ] Marca eliminada queda `Estatus = Inactivo`, sin borrado físico.
- [ ] No es posible ver/crear Marcas de otra Empresa manipulando parámetros de request.
- [ ] Tablas nuevas (`Usuario`, `Marca`) contienen exactamente las 4 columnas de auditoría prescritas.
- [ ] Sin cadenas de conexión, credenciales SQL, `client_secret` ni `machineKey` reales versionados.

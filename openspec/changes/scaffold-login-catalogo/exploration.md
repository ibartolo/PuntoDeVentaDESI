## Exploration: scaffold-login-catalogo

### Current State
El repositorio sigue greenfield: no existen `.sln`, `.csproj`, autenticación ni pruebas. `script.sql` es la fuente de verdad preliminar para `dbo.Empresa` en la base compartida `db_9c7990_servicedeskdesi`; define `Id bigint identity`, estatus, vigencias y las cuatro columnas de auditoría exactas.

El primer vertical slice está limitado a scaffold, login y CRUD MVC/API de Marcas. La arquitectura usa una MVC, una Web API y una base SQL Server compartida, con .NET Framework 4.8, ADO.NET, SPs, OAuth2, FormsAuthentication y `ModelResponse`.

### Decisión vinculante consolidada
Cada `Usuario` pertenece a exactamente una `Empresa` mediante `Usuario.EmpresaId bigint NOT NULL` FK a `Empresa.Id`; una Empresa puede tener varios Usuarios. La identidad autenticada resuelve ese `EmpresaId` en servidor. No existe selección de Empresa en UI ni una relación de pertenencia adicional.

`Marca` es propiedad de Empresa mediante `Marca.EmpresaId bigint NOT NULL` FK a `Empresa.Id`. Los SPs y WebApi filtran todas las operaciones por el tenant autenticado. MVC no acepta ni publica `EmpresaId` para Marca y usa bearer sólo en la comunicación servidor MVC a WebApi; el navegador usa FormsAuthentication.

### Constraints Confirmed
- `Empresa` no recibe CRUD ni cambios en este change.
- `Usuario` y `Marca` deben incluir exactamente `CreadoPor nvarchar(25) NOT NULL`, `FechaCreacion datetime NOT NULL`, `ModificadoPor nvarchar(25) NULL` y `FechaModificacion datetime NULL`.
- Login rechaza usuario inválido o Empresa inactiva/vencida; contraseña con PBKDF2-SHA256, salt único y al menos 100000 iteraciones.
- Marcas aplica unicidad activa por `(EmpresaId, Nombre)` y borrado lógico por `Estatus`; no hay borrado físico.
- Permanecen fuera roles/permisos, sucursales, Productos, fotos, hosting real y secretos versionados.

### Risks
- `script.sql` no define Usuario, Marca ni SPs; el script aditivo debe respetar tipos, auditoría y defaults existentes sin inferir otros.
- Los actores de auditoría excediendo `nvarchar(25)` requieren validación explícita para evitar truncamiento.
- Falta definir el proveedor de firma/validación OAuth2 y la custodia externa de la clave FormsAuthentication.
- No existe runner de pruebas; inicialmente se requiere verificación manual e integración SQL controlada.

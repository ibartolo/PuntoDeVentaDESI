# Design: Scaffold de solución, Login y Catálogo de Marcas

## Technical Approach

Se crea un vertical slice .NET Framework 4.8 con `Entities`, `MVC` y `WebApi`. `WebApi` concentra autenticación, contexto empresarial y ADO.NET sobre SPs; `MVC` entrega login y las vistas de Marcas. La base compartida conserva `Empresa` como está en `script.sql` y agrega `Usuario` y `Marca`, ambas con FK directa a Empresa. El diseño satisface las tres delta specs sin incorporar módulos fuera de alcance.

## Architecture Decisions

| Decisión | Alternativas / tradeoff | Decisión y rationale |
|---|---|---|
| Pertenencia Usuario–Empresa | Tabla intermedia permitiría varias empresas por usuario, pero contradice la decisión vinculante y complica login. | `Usuario.EmpresaId bigint NOT NULL` FK a `Empresa.Id`. Cada Usuario tiene una Empresa; una Empresa puede tener varios Usuarios. No existe tabla intermedia. |
| Fuente del tenant | Aceptar tenant de UI simplifica requests pero abre fuga entre empresas. | WebApi deriva `EmpresaId` de la identidad autenticada; MVC no lo publica ni lo recibe en requests de Marca. |
| Autenticación | Bearer en navegador incrementa exposición; cookie sola no cubre API. | OAuth2 produce bearer para MVC→WebApi exclusivamente. MVC crea y valida FormsAuthentication para navegador, con identidad y `EmpresaId` resueltos por servidor. |
| Persistencia | ORM reduciría código inicial pero contradice el estándar. | Servicios/DAL WebApi usan ADO.NET (`SqlConnection`/`SqlCommand`) y SPs parametrizados. |
| Eliminación | `DELETE` destruye historial. | `Marca.Estatus` pasa a inactivo; consultas operativas filtran tenant y activos. |

## Data Flow

```text
Browser -> MVC Login -> WebApi /oauth/token -> sp_Usuario_ObtenerParaLogin
                         <- Usuario.EmpresaId + Empresa habilitada/vigente
WebApi verifica PBKDF2; emite bearer {sub, empresaId}
MVC guarda FormsAuthentication; bearer queda sólo en sesión/servidor MVC
Browser -> MVC /Marcas -> WebApi Authorization: Bearer -> empresaId autenticado
MVC <- ModelResponse <- WebApi DAL -> sp_Marca_*(@EmpresaId, @Actor, ...)
```

El SP de login une únicamente `Usuario` con `Empresa` por `Usuario.EmpresaId = Empresa.Id`, exige estatus activo y vigencia que cubra la hora del servidor. WebApi verifica PBKDF2-SHA256 con salt único, mínimo 100000 iteraciones y comparación de tiempo constante. Nunca se persiste contraseña plana.

## Data Model and Stored Procedures

`Empresa` permanece exactamente como `script.sql`.

| Tabla | Campos y restricciones |
|---|---|
| `Usuario` | `Id bigint identity`, `EmpresaId bigint NOT NULL` FK a `Empresa.Id`, `NombreUsuario nvarchar(25)` único, hash/salt/iteraciones PBKDF2 y `Estatus bit`; cuatro auditorías exactas. |
| `Marca` | `Id bigint identity`, `EmpresaId bigint NOT NULL` FK a `Empresa.Id`, `Nombre nvarchar(100) NOT NULL`, `Descripcion nvarchar(max) NULL`, `Estatus bit NOT NULL`; índice único filtrado activo `(EmpresaId, Nombre) WHERE Estatus=1`; cuatro auditorías exactas. |

SPs: `sp_Usuario_ObtenerParaLogin` y `sp_Marca_Insertar`, `Consultar`, `Listar`, `Actualizar`, `EliminarLogico`. Los SPs de Marca reciben `@EmpresaId` y `@Actor` sólo desde WebApi, validan Empresa habilitada, filtran por tenant y escriben auditoría usando hora SQL. Errores de duplicado, inexistencia o tenant ajeno son controlados sin revelar datos externos.

## Interfaces / Contracts

`Entities` define `Empresa`, `Usuario`, `Marca`, DTOs de Marca, credenciales y `ModelResponse<T>` (`Success`, `Message`, `Data`, `Errors`). WebApi expone `POST /oauth/token` y `/api/marcas` (GET lista/detalle, POST, PUT, DELETE lógico); siempre devuelve `ModelResponse`. MVC implementa `AccountController`, `MarcasController` y cliente HTTP servidor. Requests de Marca no llevan `EmpresaId`, actor, auditoría ni Estatus.

## File Changes

| File | Action | Description |
|---|---|---|
| `PuntoDeVenta.sln`, `Entities/`, `MVC/`, `WebApi/` | Create | Solución 4.8, contratos, UI y API. |
| `WebApi/Services/*`, `WebApi/Dal/*` | Create | Auth, Marcas y ADO.NET/SPs. |
| `Database/scripts/001-usuario-marca.sql`, `seed-dev-user.sql` | Create | Tablas, FKs directas, SPs y semilla sólo-dev. |
| `MVC/Web.config`, `WebApi/Web.config`, `*.config.example` | Create | Placeholders sin secretos. |

## Testing Strategy

| Layer | What to Test | Approach |
|---|---|---|
| Unit | PBKDF2 y extracción de claims | Incorporar runner posteriormente. |
| Integration | Login por FK directa, SPs, auditoría y aislamiento A/B | Base SQL aislada con semillas. |
| E2E | Login y CRUD/desactivación MVC | Checklist manual inicial. |

## Migration / Rollout

Aplicar `script.sql`, después esquema/SPs aditivos y finalmente semilla sólo-dev. Versionar placeholders de configuración, nunca secretos. No hay migración productiva.

## Open Questions

- [ ] Definir proveedor para firma/validación del bearer OAuth2 y custodia de la clave FormsAuthentication fuera del repositorio.

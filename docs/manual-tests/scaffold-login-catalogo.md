# Pruebas manuales — `scaffold-login-catalogo`

## Estado

- **Checklist documentado**: ✅
- **Ejecución manual (5.2)**: ⛔ Pendiente (no autorizada)
- **Verificación SQL (5.3)**: ⛔ Pendiente (no autorizada)
- **Runner automatizado**: ⛔ No existe (brecha formal registrada en este documento)

## Restricciones operativas del lote

- No se permite conexión/publicación/despliegue/creación de base de datos.
- No se ejecutan scripts contra SQL Server en este lote.
- Este documento deja preparada la ejecución manual para cuando exista autorización explícita.

---

## Precondiciones para ejecución futura (cuando se autorice)

1. Solución `PuntoDeVenta.sln` restaurada y compilada.
2. `MVC` y `WebApi` ejecutándose localmente con configuración válida (sin secretos versionados).
3. Base SQL Server disponible con objetos SSDT ya publicados por canal autorizado.
4. Datos de prueba multiempresa:
   - Empresa A activa/vigente con usuario válido.
   - Empresa B activa/vigente con usuario válido.
   - Al menos una empresa inactiva o vencida para caso negativo.

---

## Casos manuales Given/When/Then

### A) Delta spec `scaffold-solucion`

#### MT-SCF-01 — Estructura y límites disponibles
**Given** un clon limpio del repositorio  
**When** se inspecciona la solución y proyectos  
**Then** existen `Entities`, `MVC` y `WebApi` dirigidos a .NET Framework 4.8  
**And** no existe módulo administrable de Empresa/sucursal/roles/permisos.

**Evidencia esperada**
- `PuntoDeVenta.sln` incluye los 3 proyectos.
- No hay controladores/vistas de administración de Empresa, sucursal o roles.

#### MT-SCF-02 — Respuesta de API uniforme (`ModelResponse`)
**Given** una solicitud a endpoints de WebApi del change  
**When** la API responde éxito o error controlado  
**Then** la respuesta usa `ModelResponse`.

**Evidencia esperada**
- JSON con `Success`, `Message`, y según aplique `Data`/`Errors`.

#### MT-SCF-03 — Contrato de datos verificable
**Given** el esquema SQL de `Usuario` y `Marca`  
**When** se inspeccionan columnas y relaciones  
**Then** ambas tablas contienen exactamente `CreadoPor`, `FechaCreacion`, `ModificadoPor`, `FechaModificacion`  
**And** `Usuario.EmpresaId` y `Marca.EmpresaId` son FKs `bigint NOT NULL` a `Empresa.Id`  
**And** no existe tabla intermedia Usuario–Empresa.

**Evidencia esperada**
- Definiciones en `PuntoDeVenta.Database/Tables/dbo/{Usuario,Marca}.sql`.

#### MT-SCF-04 — Configuración sin secretos
**Given** archivos de configuración versionados  
**When** se revisan `Web.config` y `*.config.example`  
**Then** hay placeholders y no secretos reales (cadena SQL, `client_secret`, `machineKey` productiva).

**Evidencia esperada**
- Valores no sensibles en repositorio.

---

### B) Delta spec `autenticacion-login`

#### MT-AUT-01 — Inicio de sesión empresarial exitoso
**Given** usuario válido asociado a Empresa activa y vigente  
**When** envía credenciales correctas en login  
**Then** se crea sesión autenticada con `empresaId` resuelto por servidor  
**And** el bearer se usa solo en MVC→WebApi (no expuesto al navegador).

**Evidencia esperada**
- Redirección a home/catálogo autenticado.
- Cookie FormsAuth sin bearer.
- Sesión servidor con token interno.

#### MT-AUT-02 — Credenciales inválidas
**Given** una persona no autenticada  
**When** envía usuario o contraseña inválidos  
**Then** el acceso es rechazado  
**And** no se crea sesión autenticada.

**Evidencia esperada**
- Mensaje de error controlado en login.
- Sin cookie de autenticación válida.

#### MT-AUT-03 — Empresa inactiva o vencida
**Given** usuario válido cuyo `Usuario.EmpresaId` apunta a Empresa inactiva o vencida  
**When** envía credenciales correctas  
**Then** el acceso es rechazado y no se emite token/cookie autenticados.

**Evidencia esperada**
- Error de autenticación (`invalid_grant` o equivalente controlado).
- Sin sesión autenticada.

#### MT-AUT-04 — Sembrado de desarrollo seguro
**Given** una base de desarrollo preparada  
**When** se ejecuta el script de seed dev por canal autorizado  
**Then** se crea usuario asociado a Empresa semilla  
**And** contraseña almacenada con PBKDF2-SHA256, salt único e iteraciones >= 100000.

**Evidencia esperada**
- `PuntoDeVenta.Database/Seed/seed-dev-user.sql` usa hash/salt/iteraciones.
- No hay credenciales productivas versionadas.

---

### C) Delta spec `catalogo-marcas`

#### MT-MAR-01 — Alta en empresa activa
**Given** sesión autenticada de Empresa A y nombre único válido (<=100)  
**When** registra Marca con nombre y descripción opcional  
**Then** la Marca queda activa con `EmpresaId` de Empresa A  
**And** aparece solo en listado operativo de Empresa A.

#### MT-MAR-02 — Nombre inválido o duplicado activo
**Given** sesión autenticada  
**When** intenta guardar nombre vacío, >100, o duplicado activo en su empresa  
**Then** el sistema rechaza con error controlado  
**And** no crea ni altera marca.

#### MT-MAR-03 — Aislamiento ante identificador ajeno
**Given** sesión de Empresa A y una marca de Empresa B  
**When** solicita detalle/edición/eliminación de marca ajena, incluso manipulando request  
**Then** el sistema no revela ni modifica datos de Empresa B.

#### MT-MAR-04 — Desactivar marca (eliminación lógica)
**Given** marca activa existente  
**When** confirma eliminar en UI  
**Then** la marca permanece en BD con `Estatus = Inactivo`  
**And** deja de aparecer en listado operativo por defecto.

#### MT-MAR-05 — Auditoría de cambios
**Given** marca activa de empresa autenticada  
**When** se crea, edita o desactiva  
**Then** se registran correctamente actor y fechas de auditoría (`CreadoPor/FechaCreacion`, `ModificadoPor/FechaModificacion`).

#### MT-MAR-06 — Consulta operativa aislada
**Given** marcas activas/inactivas de Empresa A y activas de Empresa B  
**When** se abre listado operativo sin filtros adicionales  
**Then** solo se muestran marcas activas de la empresa autenticada.

---

## Evidencias SQL esperadas (para 5.3 cuando se autorice)

1. **Auditoría en alta**: `CreadoPor` y `FechaCreacion` poblados; `ModificadoPor/FechaModificacion` nulos.
2. **Auditoría en edición/desactivación**: `ModificadoPor` y `FechaModificacion` actualizados.
3. **Sin borrado físico**: tras eliminar desde UI/API, el registro de Marca permanece con `Estatus=0`.
4. **Aislamiento**: consultas por tenant no devuelven marcas de otra empresa.

Scripts/objetos de referencia en SSDT:
- `StoredProcedures/dbo/sp_Marca_Insertar.sql`
- `StoredProcedures/dbo/sp_Marca_Actualizar.sql`
- `StoredProcedures/dbo/sp_Marca_EliminarLogico.sql`
- `StoredProcedures/dbo/sp_Marca_Listar.sql`
- `StoredProcedures/dbo/sp_Usuario_ObtenerParaLogin.sql`

---

## Registro formal de brecha de test runner (Task 5.4)

### Brecha
El proyecto actualmente **no tiene runner de pruebas automatizadas** detectado (xUnit/NUnit/MSTest), por lo que no existe ejecución automática de pruebas unitarias/integración/e2e.

### Impacto
- La validación depende de ejecución manual.
- Mayor riesgo de regresiones en autenticación, aislamiento multiempresa y borrado lógico.

### Tarea explícita para cambio posterior
**Propuesta de change futuro**: `enable-automated-tests-net48` (nombre sugerido)

Checklist mínimo:
1. Crear proyecto de pruebas .NET Framework 4.8 (MSTest o NUnit) dentro de la solución.
2. Añadir pruebas unitarias para:
   - `Pbkdf2PasswordVerifier` (iteraciones mínimas, comparación constante, entradas inválidas).
   - extracción de claims (`empresaId`, `sub`) y rechazo de claims faltantes.
3. Definir pruebas de integración contra BD de prueba aislada para SPs críticos de Marca.
4. Publicar comando de ejecución local/CI (`test_command`) y actualizar `openspec/config.yaml` cuando exista runner.

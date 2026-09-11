# Catálogo Marcas Specification

## Purpose

Administrar Marcas activas por empresa mediante MVC, sin fotografías ni dependencias operativas.

## Requirements

### Requirement: Administración aislada de Marcas

El sistema MUST permitir a una sesión autenticada crear, consultar, editar, detallar y listar Marcas desde MVC, consumiendo WebApi. Marca MUST tener `EmpresaId bigint NOT NULL` FK a `Empresa.Id`, `Nombre` obligatorio de máximo 100 caracteres y `Descripción` opcional. `Nombre` MUST ser único por `(EmpresaId, Nombre)` entre registros activos. MVC MUST usar bearer exclusivamente en su comunicación servidor a WebApi. El sistema MUST obtener EmpresaId exclusivamente de la identidad autenticada resuelta por el servidor y MUST NOT confiar en un EmpresaId del navegador o request. El catálogo MUST NOT incluir foto, productos, precios, inventario, sucursal, roles o permisos.

#### Scenario: Alta en empresa activa
- GIVEN una sesión autenticada en Empresa A y un Nombre de hasta 100 caracteres único en Empresa A
- WHEN registra Nombre y, opcionalmente, Descripción
- THEN la Marca queda creada activa con EmpresaId de Empresa A
- AND aparece únicamente en el listado operativo de Empresa A

#### Scenario: Nombre inválido o duplicado activo
- GIVEN una sesión autenticada
- WHEN intenta guardar un Nombre vacío, mayor a 100 caracteres o duplicado activo en su Empresa
- THEN el sistema rechaza la operación con un error controlado
- AND no crea ni altera una Marca

#### Scenario: Aislamiento ante identificador ajeno
- GIVEN una sesión de Empresa A y una Marca perteneciente a Empresa B
- WHEN solicita su detalle, edición o eliminación, incluso enviando EmpresaId B
- THEN el sistema no revela ni modifica la Marca de Empresa B

### Requirement: Auditoría, eliminación lógica y consulta operativa

Al crear Marca, el sistema MUST registrar `CreadoPor` y `FechaCreacion`; al editar o desactivar, MUST registrar `ModificadoPor` y `FechaModificacion` con el actor autenticado. Eliminar MUST establecer `Estatus = Inactivo` y MUST NOT efectuar borrado físico. El listado operativo MUST filtrar por EmpresaId de la sesión/claim y excluir inactivos por defecto; la interfaz SHOULD comunicar que Eliminar desactiva el registro.

#### Scenario: Desactivar Marca
- GIVEN una Marca activa existente
- WHEN la sesión autenticada confirma Eliminar
- THEN la Marca conserva su registro y queda con Estatus inactivo
- AND deja de aparecer en el listado operativo predeterminado

#### Scenario: Auditoría de cambios
- GIVEN una Marca activa de la Empresa autenticada
- WHEN la sesión la crea, edita o desactiva
- THEN se conservan los valores de auditoría obligatorios y el actor correspondiente

#### Scenario: Consulta operativa aislada
- GIVEN Marcas activas e inactivas de Empresa A y Marcas activas de Empresa B
- WHEN se abre el listado operativo sin filtros adicionales
- THEN solo se muestran las Marcas activas de la Empresa de la sesión

# Scaffold de solución Specification

## Purpose

Establecer el corte inicial multiempresa para login y Marcas.

## Requirements

### Requirement: Topología multiempresa y límites

La solución MUST dirigirse a .NET Framework 4.8 y contener `Entities`, `MVC` y `WebApi` conforme a ServiceDeskDESI. MVC MUST alojar la experiencia web, WebApi MUST exponer servicios y Entities MUST contener contratos compartidos; WebApi MUST responder con `ModelResponse`. Una MVC, una WebApi y una única base SQL Server compartida MUST atender a todas las empresas. Este change MUST NOT incorporar CRUD de Empresa, sucursales, roles, permisos, productos, precios, inventario ni hosting real.

#### Scenario: Estructura y límites disponibles
- GIVEN un clon limpio del repositorio
- WHEN se inspecciona la solución creada
- THEN existen Entities, MVC y WebApi dirigidos a .NET Framework 4.8
- AND no existe un módulo administrable de Empresa, sucursal, roles o permisos

#### Scenario: Respuesta de API uniforme
- GIVEN una solicitud a un servicio WebApi de este change
- WHEN el servicio devuelve éxito o error controlado
- THEN la respuesta usa el contrato `ModelResponse`

### Requirement: Contexto empresarial, auditoría y configuración

La solución MUST consumir `Empresa` conforme a `script.sql` como contexto técnico. `Usuario` MUST declarar `EmpresaId bigint NOT NULL` como FK a `Empresa.Id`; cada Usuario MUST pertenecer a exactamente una Empresa y una Empresa MAY tener varios Usuarios. No MUST existir una tabla de relación Usuario–Empresa. `Marca` MUST declarar su propia FK `EmpresaId bigint NOT NULL` a `Empresa.Id`. Cada tabla nueva de este change (`Usuario`, `Marca`) MUST incluir exactamente `CreadoPor nvarchar(25) NOT NULL`, `FechaCreacion datetime NOT NULL`, `ModificadoPor nvarchar(25) NULL` y `FechaModificacion datetime NULL`. La conexión SQL Server MUST ser genérica y configurable; los artefactos versionados MUST NOT contener cadenas, credenciales, secretos OAuth2 ni `machineKey` reales.

#### Scenario: Contrato de datos verificable
- GIVEN el esquema de Usuario y Marca del change
- WHEN se inspeccionan sus columnas y relaciones
- THEN ambos contienen exactamente las cuatro columnas de auditoría prescritas
- AND `Usuario.EmpresaId` y `Marca.EmpresaId` son FKs `bigint NOT NULL` a `Empresa.Id`
- AND no existe una tabla de relación Usuario–Empresa

#### Scenario: Configuración sin secretos
- GIVEN los archivos versionados del change
- WHEN se revisa la configuración
- THEN la conexión es configurable sin valores reales ni secretos versionados

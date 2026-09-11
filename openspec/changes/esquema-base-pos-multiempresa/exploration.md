## Exploration: esquema-base-pos-multiempresa

### Current State
`PuntoDeVenta.Database` ya es la fuente versionada del esquema SSDT (SQL 150) y contiene únicamente `Empresa`, `Usuario`, `Marca` y seis SPs de login/Marcas. `script.sql` solo incluye por SQLCMD la definición de `Empresa`; no hay base SQL real creada ni evidencia de publicación/conexión que deba ejecutarse para este change.

El dominio conocido en `Propuesta.txt` es un POS web multi-sucursal: catálogos, inventario, compras, venta, caja/cortes, cancelaciones-devoluciones y reportes. La decisión vinculante posterior prevalece para tenancy: `Usuario.EmpresaId` es obligatorio y cada Usuario pertenece a exactamente una Empresa. El esquema existente ya materializa esa relación, pero la propuesta aún describe relaciones Usuario-Rol y Usuario-Sucursal que no están diseñadas.

### Affected Areas
- `PuntoDeVenta.Database/PuntoDeVenta.Database.sqlproj` — proyecto SSDT que deberá registrar futuros objetos por objeto; no se modifica en esta exploración.
- `PuntoDeVenta.Database/Tables/dbo/{Empresa,Usuario,Marca}.sql` — baseline existente que define tipos, `Estatus`, FKs a Empresa y el patrón parcial de auditoría/borrado lógico.
- `PuntoDeVenta.Database/StoredProcedures/dbo/*.sql` — patrón existente de ADO.NET/SPs y scoping explícito por `@EmpresaId` para Marca.
- `Propuesta.txt` — única fuente del alcance funcional POS completo, con ambigüedades que impiden DDL/SPs definitivos.
- `openspec/changes/scaffold-login-catalogo/` — decisiones ya implementadas: Usuario→Empresa directa, Marca por empresa y token que deriva empresa del servidor.
- `.atl/skill-registry.md` y `netframework-mvc-webapi/SKILL.md` — confirman ADO.NET + SPs, pero excluyen explícitamente roles/permisos; no resuelven ese modelo.

### Domain Classification

| Área | Tablas candidatas y relaciones conocidas | Estado |
|---|---|---|
| Tenancy e identidad | `Empresa` → `Usuario` (1:N, `Usuario.EmpresaId NOT NULL`); `Usuario` ya existe. | Base existente; su unicidad de nombre es global hoy y la regla futura por empresa no está confirmada. |
| Catálogos de producto | `Marca`, `Categoria` jerárquica (autorreferencia), `Producto` → Marca/Categoría; código único de barras **o** QR; precios globales con historial. | Marca existe; el resto es candidato, sujeto a decisiones de identidad/código/precio. |
| Clientes y proveedores | `Cliente` (incluye Público General), `Proveedor`, relación N:M `ProductoProveedor`; compras/historial de compra relacionan proveedor, producto, costo y cantidad. | Candidatos; faltan claves comerciales, vigencias y alcance de compra. |
| Sucursales e inventario | `Sucursal`; asignación Usuario-Sucursal; stock independiente por sucursal; movimientos de stock por ingreso, ajuste, devolución y venta. | Bloqueado por modelo de asignación/selección de sucursal y libro de movimientos. |
| Venta | `Venta` → Sucursal, Usuario/cajero, Cliente, Caja chica; `VentaDetalle` → Producto; un método de pago por venta. | Candidato, pero requiere numeración, captura de precio, impuestos/redondeos y ciclo de cancelación. |
| Caja y corte | `CajaChica` por usuario+sucursal, `SalidaCaja`, `CorteCaja`, adjuntos/evidencias por pago. | Bloqueado por definición del periodo, saldo calculado y concurrencia/autorizar. |
| Cancelación/devolución | Cabecera y detalle que referencien venta/detalle original, actor que cancela, actor que autoriza, reembolso y retorno opcional a stock. | Bloqueado por semántica de total/parcial y pagos/reintegros. |
| Seguridad funcional | Rol, Módulo fijo, permisos por acción y asociaciones. | Postergar: hay requerimiento funcional, pero ningún diseño autorizado y la skill local lo excluye. |
| Reportes | Ventas por periodo/sucursal/cajero, utilidad y más vendidos. | Postergar como vistas/SPs de reporte; deben derivarse de transacciones ya cerradas. |

### Safe Base Schema Now
Los siguientes contratos estructurales se pueden especificar después sin inventar reglas comerciales: PK `bigint identity`; `Estatus bit NOT NULL` para borrado lógico; y, en **cada** tabla nueva, exactamente `CreadoPor nvarchar(25) NOT NULL`, `FechaCreacion datetime NOT NULL`, `ModificadoPor nvarchar(25) NULL`, `FechaModificacion datetime NULL`. Se debe conservar el patrón existente `EmpresaId bigint NOT NULL` FK a `Empresa` para toda entidad propiedad de una empresa. Las entidades transaccionales también deberán portar una Sucursal determinada por contexto servidor, no por una entrada libre del cliente.

Base de relaciones segura a nivel conceptual: Empresa→Usuario; Empresa→Sucursal; Empresa→catálogos propios (`Marca`, categorías, productos, clientes, proveedores); Producto→Marca/Categoría; Producto↔Proveedor; Sucursal↔Producto mediante existencia/movimientos; Venta→VentaDetalle; Venta/Caja/Corte/Solicitud de cancelación deben conservar referencias históricas, no depender de filas físicamente borradas. Precio se declara global por producto, no por sucursal; su historial necesita una sola fila vigente, pero la regla exacta de vigencia aún debe confirmarse.

Para aislamiento: índices, FKs y todos los SPs operativos deben incluir/validar el tenant y los accesos por Id deben acotarse por `EmpresaId`. WebApi deriva EmpresaId y actor de claims; MVC no los acepta del navegador. Las operaciones que afectan stock, efectivo o cancelaciones deberán ejecutarse transaccionalmente; no se debe confiar solo en validación de aplicación.

Estados mínimos conocidos: `Estatus` activo/inactivo (borrado lógico) para todo registro; Empresa activa y dentro de vigencia para login; Marca activa para listados actuales; Caja abierta/cerrada y Corte asociado; Venta vigente/cancelada; solicitud de cancelación/devolución pendiente/autorizada/rechazada. Los últimos tres conjuntos requieren nombres, transiciones y autoridad confirmados antes de convertirse en CHECKs, catálogos o SPs.

### Ambiguous Decisions Blocking Definitive DDL/SPs
1. **Sucursal y sesión:** ¿cada Usuario puede operar en una o varias Sucursales, cómo se persiste la asignación y cómo se selecciona/valida la sucursal activa? Esto determina la relación Usuario-Sucursal, claims y el scope obligatorio de inventario/caja/venta.
2. **Propiedad por empresa:** ¿Sucursal, Cliente, Categoría, Producto y Proveedor son siempre propios de Empresa? ¿`NombreUsuario` debe seguir siendo globalmente único como hoy o único por Empresa?
3. **Producto/códigos:** ¿un Producto admite exactamente un identificador de tipo barras o QR, o puede no tenerlo? ¿La unicidad del código es por Empresa o global? ¿"marca genérica" es una Marca especial por empresa o un valor nulo/controlado?
4. **Categoría y producto:** ¿un producto tiene una sola categoría? ¿cuál es la profundidad máxima, y se permite desactivar una categoría con hijos/productos activos?
5. **Precio/historial:** ¿qué fecha/hora determina vigencia, se permiten precios futuros/solapados, se conserva el precio vendido en `VentaDetalle`, y qué tratamiento monetario/moneda/redondeo aplica? Sin esto no puede garantizarse "uno activo" ni calcular utilidad reproducible.
6. **Compras e inventario:** ¿la compra es solo una captura histórica que ingresa stock inmediatamente? ¿las devoluciones mencionadas son de cliente, a proveedor o ambas? ¿se requiere un kardex inmutable con saldo derivado, y cuáles son los motivos/autoridades para ajuste negativo?
7. **Caja/corte:** ¿"Caja chica" es una sesión de caja por usuario+sucursal o un fondo independiente? ¿qué ventas integran sus ingresos, cómo se define el periodo de corte y qué ocurre con transferencia/tarjeta para el límite de 50% de salidas? ¿la regla usa ventas cobradas, efectivo disponible o ingresos totales de qué periodo?
8. **Pagos y reembolsos:** "un solo método por venta" limita el pago original, pero ¿un reembolso total/parcial puede dividirse entre efectivo/transferencia? ¿cómo se modela y valida el monto remanente reembolsable? ¿transferencia necesita folio además del comprobante?
9. **Cancelación/autorización:** ¿cancelación y devolución son una misma transacción? ¿quién puede autorizar y puede ser la misma persona que cancela? No se puede diseñar estados ni auditoría de autorización sin esta separación de funciones.
10. **Evidencias:** ¿dónde se almacenan fotos/archivos (BD, filesystem, blob), qué tipos/tamaño/retención se permiten y cuál es la cardinalidad por venta, pago, salida y reembolso?
11. **Roles/permisos:** ¿se desea diseñar el modelo de roles, módulos y permisos en un change dedicado? No se debe inferir sus tablas, permisos precargados ni reglas de autorización.

### Objects to Postpone
- Tablas y SPs de `Rol`, `Modulo`, permisos y asociaciones hasta una especificación de autorización independiente.
- Vistas/SPs de reportes y cálculo de utilidad hasta cerrar costo vigente/histórico, devoluciones, cancelaciones y precio efectivamente vendido.
- Estructuras de lealtad/frecuencia de clientes: la propuesta las declara futuras.
- Integración de cámara, apertura de cajón y almacenamiento físico de evidencias: requieren contratos externos y no son DDL base.
- Seeds operativos, datos de catálogo y publicación SSDT. El seed actual es solo desarrollo y el postdeploy excluye seeds; este change no crea, publica, despliega ni conecta a SQL Server.

### Sequencing Dependencies
1. Mantener y, si procede, normalizar de forma aditiva el baseline `Empresa` → `Usuario` → `Marca`; no modificarlo dentro de esta exploración.
2. Resolver tenancy de Sucursal y propiedad de catálogos antes de crear entidades empresariales nuevas.
3. Definir Categoría/Marca/Proveedor/Producto y precio histórico antes de compras, stock y ventas.
4. Definir kardex/movimientos y reglas de costo antes de inventario, utilidad y devoluciones a stock.
5. Definir sesión de caja, pagos y cortes antes de venta operativa y salidas/reembolsos.
6. Definir cancelación/devolución y autorización antes de reportes definitivos.
7. Diseñar roles/permisos de forma separada antes de imponer autorización fina en SPs o UI.

### Approaches
1. **Esquema integral inmediato** — Crear todas las tablas y SPs a partir de la propuesta actual.
   - Pros: cobertura rápida aparente.
   - Cons: cristaliza reglas no decididas, especialmente caja, stock, devoluciones y permisos; alto riesgo de migraciones destructivas.
   - Effort: High.

2. **Baseline transversal seguido de verticales dependientes** — Acordar las preguntas bloqueantes, documentar contratos base multiempresa/auditoría/borrado lógico y entregar módulos en el orden de dependencias.
   - Pros: preserva integridad histórica, evita DDL especulativo y mantiene el aislamiento de Empresa comprobable.
   - Cons: requiere decisiones de negocio antes de que exista un esquema "completo".
   - Effort: Medium por vertical, High para el programa completo.

### Recommendation
Adoptar el enfoque 2. La petición es viable como objetivo de varios changes, no como DDL definitivo responsable con la información actual. Primero se deben responder las once decisiones bloqueantes, en especial Sucursal, producto/precio, kardex, caja/pagos y cancelación; entonces una propuesta puede delimitar un baseline SSDT aditivo y verticales transaccionales. No se debe modificar el esquema actual ni ejecutar SQL hasta esa definición.

### Risks
- El modelo actual tiene `UQ_Usuario_NombreUsuario` global; cambiarlo posteriormente a unicidad por Empresa podría afectar login y migraciones.
- Aplicar `EmpresaId` sin resolver Sucursal dejaría fugas funcionales entre sucursales y una trazabilidad de caja/stock incorrecta.
- Calcular stock o utilidad desde saldos mutables, en vez de transacciones inmutables, impediría auditoría de ventas, mermas y reembolsos.
- La regla de salidas hasta 50% de ingresos y los reembolsos de tarjeta son sensibles a concurrencia y deben validarse dentro de una transacción SQL cuando se implementen.
- Definir permisos desde supuestos contradice el registro de skills: `netframework-mvc-webapi` no cubre roles/permisos.

### Ready for Proposal
No. Antes de propuesta/DDLs/SPs definitivos, el usuario debe responder las preguntas de Sucursal, scope por Empresa, producto/códigos/categoría, precio/costo, kardex/compras, caja/pagos/cortes, cancelación/reembolso/evidencias y decidir si autorización funcional se aborda en un change separado.

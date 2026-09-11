# Mission Status

## Progress
- .opencode/todo.md: **250/250 checkboxes `[x]`** (0 remaining). Canonical file.
- Milestones: **M1–M7 completed**.
- **Build**: `dotnet msbuild PuntoDeVenta.sln /t:Build /p:Configuration=Debug /p:Platform="Any CPU"`
  → 3 DLLs (Entities/MVC/WebApi), **0 C# errors / 0 warnings**; único error = SSDT `MSB4057` (pre-existente/ambiental).
  Per-project: Entities 0/0, WebApi 0/0, MVC 0/0.
- **Sync issues**: `.opencode/sync-issues.md` = **empty (0 bytes)** → **0 unresolved**.
  (Harness counts lines in `sync-issues.md`; it must stay empty when there are no open issues.
  Resolved history is kept in `.opencode/work-log.md`.)
- **Verification Strategy**: build 0 errores (3 proyectos C#) + checks estáticos de registro/contratos + checklist manual;
  no hay runner de pruebas ni BD en el entorno.

## Integration fixes applied (2026-09-11)
- **SYNC-11 (MEDIUM) — aislamiento por tenant**: `sp_Venta_Guardar.sql` / `sp_Cancelacion_Guardar.sql`
  ahora validan que `@SucursalId` (y `@CajaChicaId`) pertenezcan a `@EmpresaId` y filtran por
  `EmpresaId` los UPDATE de `Stock`/`CajaChica` y los JOIN de `StockMovimiento`.
- **SYNC-12 (MEDIUM) — validación de cancelación**: `sp_Cancelacion_Guardar.sql` valida que `@VentaId`
  pertenezca a `@EmpresaId`, que cada `VentaDetalleId` pertenezca a la venta, cantidades > 0, sin
  duplicados y sin exceder lo vendido (anti doble cancelación / anti inflado de stock y reembolso).
- **M6/T6.3 (MVC)**: catálogo Producto+Precio alineado al patrón canónico → rama `Product` en
  `CatalogsController` + `Views/Catalogs/Product.cshtml`; se eliminó `ProductController.cs`.

## Notes
- No hay runner de pruebas ni BD en este entorno: la verificación es build 0 errores + checks estáticos + checklist manual
  (`docs/checklist-pruebas-qa.md`). Los SPs/tablas SSDT se validaron estáticamente (SSDT no compila aquí: MSB4057/MSB3644).
- `lsp_diagnostics` no disponible (Rust tool process); N/A para .NET Framework.
- Conocido no bloqueante: SSDT `MSB4057`; paridad Owin 4.2.3 vs 3.0.1 de la referencia.

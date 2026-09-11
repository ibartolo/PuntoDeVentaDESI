# Checklist de pruebas QA — PuntoDeVentaDESI

> Checklist de verificación manual para la solución PuntoDeVentaDESI (POS multiempresa /
> multi-sucursal). Complementa `docs/manual-tests/scaffold-login-catalogo.md` y sirve como
> guion de regresión end-to-end. No sustituye a un runner automatizado (ver §9 Brecha).
>
> Convención de resultado: **PASS / FAIL / BLOQUEADO** + evidencia (captura, log, fila SQL).

---

## 0. Precondiciones de entorno

| # | Precondición | Verificación |
|---|---|---|
| 0.1 | Solución restaurada y compilada (0 errores) | `dotnet msbuild PuntoDeVenta.sln /t:Restore /p:RestorePackagesConfig=true` y `/t:Build` |
| 0.2 | WebApi en ejecución (p. ej. `http://localhost:5102/`) | `GET /token` responde (405/400 controlado) |
| 0.3 | MVC en ejecución (p. ej. `http://localhost:44300/`) | `GET /Home/Autentication` responde 200 |
| 0.4 | Base `db_9c7990_puntoventadev` con objetos SSDT publicados | `SELECT name FROM sys.tables` incluye `Empresa`, `Usuario`, `Marca` |
| 0.5 | Sin secretos reales en `Web.config` versionados | placeholders (`__SQL_*__`) |
| 0.6 | `machineKey` fija e idéntica en MVC y WebApi | comparar `Web.config` |
| 0.7 | Datos de prueba multiempresa: Empresa A activa, Empresa B activa, Empresa C inactiva/vencida | filas en `Empresa` |

---

## 1. Autenticación — Login en 2 pasos

| ID | Caso | Pasos | Resultado esperado |
|---|---|---|---|
| QA-AUT-01 | Login exitoso (2 pasos) | Enviar credenciales válidas de Empresa A en `/Home/LogIn` | Paso 1 (`POST api/Autenticacion/autenticar`, sin token) devuelve `ModelResponse<UsuarioDTO>` con `IsSuccess=true`; Paso 2 (`POST /token`, password grant) devuelve `access_token`. Redirige a `/Home/Index`. |
| QA-AUT-02 | Credenciales inválidas | Usuario o contraseña incorrectos | `IsSuccess=false`, mensaje controlado; no se crea cookie de sesión; no hay `access_token`. |
| QA-AUT-03 | Empresa inactiva/vencida | Credenciales válidas de Empresa C | Rechazo (`invalid_grant` o equivalente controlado); sin sesión autenticada. |
| QA-AUT-04 | Usuario inexistente | Usuario no registrado | Mismo mensaje genérico que QA-AUT-02 (no revelar si el usuario existe). |
| QA-AUT-05 | Campos vacíos | Enviar `user`/`pass` vacíos | Validación previa; sin llamada al backend; mensaje controlado. |
| QA-AUT-06 | Bloqueo por intentos | N intentos fallidos consecutivos | (Si aplica política) bloqueo/retardo; si no aplica, documentar como N/A. |
| QA-AUT-07 | Logout | Invocar `LogOut` | `FormsAuthentication.SignOut()`; cookie eliminada; acceso a páginas protegidas redirige a login. |
| QA-AUT-08 | Acceso a página protegida sin sesión | Navegar a `/Home/Index` sin cookie | Redirección a `/Home/Autentication` (filtro global). |
| QA-AUT-09 | Acceso denegado | Usuario sin permiso a un módulo | Redirección a `/Home/AccesoDenegado` (o 403 controlado). |

---

## 2. Token, cookie y sesión

| ID | Caso | Pasos | Resultado esperado |
|---|---|---|---|
| QA-TOK-01 | Cookie sin bearer | Tras login, inspeccionar cookie `autentication` | `UserData` = JSON de `TokenCookie`; **no** contiene el `access_token` en claro para el navegador. |
| QA-TOK-02 | Expiración de cookie | Esperar/forzar expiración del token | La cookie expira a la vez que `ExpirationDate`; siguiente request redirige a login. |
| QA-TOK-03 | Bearer en MVC→WebApi | Capturar tráfico servidor | Header `Authorization: Bearer <access_token>` presente solo en llamadas del servidor MVC. |
| QA-TOK-04 | Token manipulado | Alterar el valor de la cookie | Firma inválida → sesión rechazada (redirección a login). |
| QA-TOK-05 | `empresaId` desde servidor | Login Empresa A | Claim `empresaId` = `Usuario.EmpresaId`; no proviene del formulario/URL. |
| QA-TOK-06 | Reciclaje del AppPool | Reciclar pool y reintentar request | La cookie sigue válida gracias a `machineKey` fija. |
| QA-TOK-07 | Sin token en API | `GET api/Marca/List` sin `Authorization` | HTTP 401. |

---

## 3. Catálogos — CRUD de Marca (aislado por empresa)

| ID | Caso | Pasos | Resultado esperado |
|---|---|---|---|
| QA-MAR-01 | Listar (empresa propia) | Abrir catálogo de Marcas con sesión Empresa A | DataTables muestra solo Marcas activas de Empresa A. |
| QA-MAR-02 | Alta válida | Guardar Marca con nombre único (≤100) y descripción opcional | `IsSuccess=true`; `EmpresaId` = Empresa A; auditoría `CreadoPor/FechaCreacion` poblada. |
| QA-MAR-03 | Nombre requerido | Guardar con nombre vacío | Rechazo controlado; no se crea registro. |
| QA-MAR-04 | Nombre > 100 | Guardar nombre de 101 caracteres | Rechazo controlado; no se crea registro. |
| QA-MAR-05 | Duplicado activo | Guardar nombre ya existente activo en la misma empresa | Rechazo controlado (unicidad `(EmpresaId, Nombre)`); no se crea/edita. |
| QA-MAR-06 | Edición | Editar Marca existente | `IsSuccess=true`; `ModificadoPor/FechaModificacion` actualizados. |
| QA-MAR-07 | Eliminación lógica | "Eliminar" una Marca | `Estatus=0` en BD; desaparece del listado operativo; **no** hay `DELETE` físico. |
| QA-MAR-08 | Aislamiento (detalle ajeno) | Solicitar `api/Marca/{id}` de Empresa B con sesión Empresa A | No revela datos; error controlado. |
| QA-MAR-09 | Aislamiento (edición ajena) | Enviar `Guardar` con `Id` de Empresa B | No modifica datos ajenos; error controlado. |
| QA-MAR-10 | Inyección de `EmpresaId` | Agregar `EmpresaId=B` al request | El backend ignora el valor del cliente y usa el claim de Empresa A. |
| QA-MAR-11 | XSS en descripción | Guardar descripción con `<script>` | El valor se escapa al renderizar (sin ejecución). |
| QA-MAR-12 | Otros catálogos | Repetir patrón en Categorías/Clientes/Proveedores cuando existan | Mismo comportamiento (auditoría + borrado lógico + tenant). |

---

## 4. Módulos POS — Productos y precios

| ID | Caso | Resultado esperado |
|---|---|---|
| QA-PRD-01 | Alta de producto (nombre, marca, categoría, tipo) | Registro activo con auditoría; marca real o genérica. |
| QA-PRD-02 | Categorías jerárquicas | Árbol tipo supermercado (bebés, ropa, carnes, ...) navegable. |
| QA-PRD-03 | Código de barras **o** QR (uno solo) | No se permite registrar ambos a la vez. |
| QA-PRD-04 | Stock por sucursal | El stock es independiente por sucursal; el global no se mezcla. |
| QA-PRD-05 | Stock mínimo | Alerta/indicador cuando `stock disponible <= stock mínimo`. |
| QA-PRD-06 | Historial de precios | Solo un precio activo por producto; cambios quedan en historial. |
| QA-PRD-07 | Precio global | El mismo precio aplica en todas las sucursales. |
| QA-PRD-08 | Múltiples proveedores | Un producto puede asociarse a varios proveedores con historial de compras. |

---

## 5. Módulos POS — Ventas

| ID | Caso | Resultado esperado |
|---|---|---|
| QA-VEN-01 | Venta con un solo método de pago | Se registra con efectivo, transferencia **o** tarjeta (no combinados). |
| QA-VEN-02 | Búsqueda por catálogo/categorías | Se agregan productos correctamente al carrito. |
| QA-VEN-03 | Escaneo con cámara (barras/QR) | El código agrega el producto correcto (dispositivo compatible). |
| QA-VEN-04 | Cálculo de totales | Subtotal, impuestos (si aplican) y total correctos. |
| QA-VEN-05 | Apertura de cajón | Al concretar la venta se dispara la apertura del cajón. |
| QA-VEN-06 | Registro en sucursal activa | La venta queda en la sucursal activa del usuario. |
| QA-VEN-07 | Ticket/nota | Se genera la nota de venta; ticket del sistema disponible. |
| QA-VEN-08 | Venta con stock insuficiente | Rechazo/ajuste controlado según regla. |

---

## 6. Módulos POS — Caja chica y cortes

| ID | Caso | Resultado esperado |
|---|---|---|
| QA-CAJ-01 | Apertura de caja chica | Monto inicial registrado; caja asociada a usuario + sucursal. |
| QA-CAJ-02 | Bloqueo sin corte previo | No se puede abrir una nueva caja chica sin corte de la anterior. |
| QA-CAJ-03 | Salidas de dinero | N salidas por día con comentario, fecha/hora y monto. |
| QA-CAJ-04 | Límite de salidas | No superan el **50%** de los ingresos totales. |
| QA-CAJ-05 | Reembolso con tarjeta | Se registra como salida justificada, con evidencia fotográfica opcional. |
| QA-CAJ-06 | Monto inicial intacto | El fondo de cambio permanece intacto al cierre. |
| QA-COR-01 | Corte de caja | Cierra la caja chica; se debe crear una nueva para seguir vendiendo. |
| QA-COR-02 | Varios cortes al día | Permitido; cada corte asocia su caja chica. |
| QA-COR-03 | Contenido del corte | Ventas por método de pago, monto inicial, salidas, efectivo esperado vs contado, diferencias. |
| QA-COR-04 | Adjuntos por método | Tarjeta: folio + ticket cobro + ticket sistema; Transferencia: comprobante; Efectivo: ticket sistema. |

---

## 7. Módulos POS — Cancelaciones / Devoluciones

| ID | Caso | Resultado esperado |
|---|---|---|
| QA-CAN-01 | Cancelación total | Se revierte la venta; se registra quién cancela y quién autoriza. |
| QA-CAN-02 | Cancelación parcial | Solo los productos seleccionados. |
| QA-CAN-03 | Devolución a stock opcional | Por producto; puede no regresar (merma/descompuesto). |
| QA-CAN-04 | Reembolso efectivo/transferencia | El cliente elige forma (total o parcial). |
| QA-CAN-05 | Reembolso venta con tarjeta | Forzado en efectivo, registrado como salida justificada con evidencia. |
| QA-CAN-06 | Permiso de autorizar | Solo usuarios con permiso `autorizar` ejecutan la autorización. |
| QA-CAN-07 | Borrado lógico | La venta cancelada no se elimina físicamente. |

---

## 8. Módulos POS — Reportes

| ID | Caso | Resultado esperado |
|---|---|---|
| QA-REP-01 | Ventas por periodo | Filtro de fechas correcto; totales coinciden con las ventas registradas. |
| QA-REP-02 | Ventas por sucursal | Agrupa por sucursal; respeta el aislamiento por empresa. |
| QA-REP-03 | Ventas por cajero | Agrupa por usuario/cajero. |
| QA-REP-04 | Utilidad | Utilidad = ventas − costo, coherente con precios históricos. |
| QA-REP-05 | Productos más vendidos | Ranking correcto por cantidad/importe. |
| QA-REP-06 | Exportación | (Si aplica) exportar a Excel/PDF conserva acentos. |

---

## 9. Transversales (seguridad, tenancy, calidad)

| ID | Caso | Resultado esperado |
|---|---|---|
| QA-X-01 | Aislamiento multiempresa | Ninguna consulta devuelve datos de otra empresa. |
| QA-X-02 | Aislamiento multi-sucursal | Operaciones quedan en la sucursal activa. |
| QA-X-03 | Borrado lógico global | Ninguna entidad se borra físicamente. |
| QA-X-04 | Auditoría global | `CreadoPor/FechaCreacion` y `ModificadoPor/FechaModificacion` correctos. |
| QA-X-05 | Errores sin fuga | Los mensajes no exponen stack traces, SQL ni rutas. |
| QA-X-06 | Encoding | Acentos correctos en UI, reportes y correos. |
| QA-X-07 | Sin secretos en repo | `Web.config` con placeholders; sin credenciales reales. |
| QA-X-08 | Permisos por acción | Guardar/actualizar/imprimir/autorizar respetan el rol. |
| QA-X-09 | Menú por rol | El menú se carga según el rol del usuario. |

---

## 10. Brecha conocida — sin runner automatizado

- El proyecto **no cuenta con runner de pruebas** (xUnit/NUnit/MSTest) a la fecha; la
  verificación es manual + build MSBuild 0 errores.
- Propuesta de cambio futuro: `enable-automated-tests-net48` (ver
  `docs/manual-tests/scaffold-login-catalogo.md` §"Registro formal de brecha de test runner").
- Cuando exista runner, migrar estos casos a pruebas unitarias/integración/e2e y publicar
  `test_command` en `openspec/config.yaml`.

---

## 11. Registro de ejecución

| Fecha | Entorno | Responsable | PASS | FAIL | BLOQUEADO | Notas |
|---|---|---|---|---|---|---|
| | | | | | | |

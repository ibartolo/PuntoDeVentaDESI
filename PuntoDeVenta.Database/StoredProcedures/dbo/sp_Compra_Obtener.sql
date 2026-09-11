CREATE PROCEDURE [dbo].[sp_Compra_Obtener]
    @EmpresaId BIGINT,
    @CompraId BIGINT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        C.[Id], C.[EmpresaId], C.[SucursalId], S.[Nombre] AS [SucursalNombre],
        C.[ProveedorId], P.[Nombre] AS [ProveedorNombre], C.[Folio], C.[FechaCompra],
        C.[Subtotal], C.[Impuesto], C.[Total], C.[Observaciones],
        C.[Estatus], C.[CreadoPor], C.[FechaCreacion], C.[ModificadoPor], C.[FechaModificacion]
    FROM [dbo].[Compra] C
    INNER JOIN [dbo].[Sucursal] S ON S.[Id] = C.[SucursalId]
    INNER JOIN [dbo].[Proveedor] P ON P.[Id] = C.[ProveedorId]
    WHERE C.[EmpresaId] = @EmpresaId
      AND C.[Id] = @CompraId;

    SELECT
        D.[Id], D.[CompraId], D.[ProductoId], PR.[Nombre] AS [ProductoNombre],
        D.[Cantidad], D.[CostoUnitario], D.[Importe],
        D.[Estatus], D.[CreadoPor], D.[FechaCreacion], D.[ModificadoPor], D.[FechaModificacion]
    FROM [dbo].[CompraDetalle] D
    INNER JOIN [dbo].[Producto] PR ON PR.[Id] = D.[ProductoId]
    WHERE D.[CompraId] = @CompraId
      AND D.[Estatus] = 1;
END;

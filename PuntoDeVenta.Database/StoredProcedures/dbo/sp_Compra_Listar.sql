CREATE PROCEDURE [dbo].[sp_Compra_Listar]
    @EmpresaId BIGINT,
    @SucursalId BIGINT = 0
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
      AND C.[Estatus] = 1
      AND (@SucursalId = 0 OR C.[SucursalId] = @SucursalId)
    ORDER BY C.[FechaCompra] DESC;
END;

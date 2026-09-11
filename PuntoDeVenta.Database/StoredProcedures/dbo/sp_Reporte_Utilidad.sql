CREATE PROCEDURE [dbo].[sp_Reporte_Utilidad]
    @EmpresaId BIGINT,
    @FechaInicio DATETIME,
    @FechaFin DATETIME
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        P.[Id] AS [ProductoId],
        P.[Nombre] AS [ProductoNombre],
        ISNULL(SUM(D.[Cantidad]), 0) AS [CantidadVendida],
        ISNULL(SUM(D.[Importe]), 0) AS [TotalVenta],
        ISNULL(SUM(D.[Cantidad] * ISNULL(PR.[PrecioCompra], 0)), 0) AS [TotalCosto],
        ISNULL(SUM(D.[Importe]), 0) - ISNULL(SUM(D.[Cantidad] * ISNULL(PR.[PrecioCompra], 0)), 0) AS [Utilidad]
    FROM [dbo].[VentaDetalle] D
    INNER JOIN [dbo].[Venta] V ON V.[Id] = D.[VentaId]
    INNER JOIN [dbo].[Producto] P ON P.[Id] = D.[ProductoId]
    OUTER APPLY
    (
        SELECT TOP (1) [PrecioCompra]
        FROM [dbo].[Precio]
        WHERE [ProductoId] = P.[Id] AND [Activo] = 1 AND [Estatus] = 1
        ORDER BY [FechaInicio] DESC
    ) PR
    WHERE V.[EmpresaId] = @EmpresaId
      AND V.[Estatus] = 1
      AND D.[Estatus] = 1
      AND V.[FechaVenta] >= @FechaInicio
      AND V.[FechaVenta] <= @FechaFin
    GROUP BY P.[Id], P.[Nombre]
    ORDER BY [Utilidad] DESC;
END;

CREATE PROCEDURE [dbo].[sp_Reporte_MasVendidos]
    @EmpresaId BIGINT,
    @FechaInicio DATETIME,
    @FechaFin DATETIME,
    @Top INT = 10
AS
BEGIN
    SET NOCOUNT ON;

    IF @Top IS NULL OR @Top <= 0 SET @Top = 10;

    SELECT TOP (@Top)
        P.[Id] AS [ProductoId],
        P.[Nombre] AS [ProductoNombre],
        ISNULL(SUM(D.[Cantidad]), 0) AS [CantidadVendida],
        ISNULL(SUM(D.[Importe]), 0) AS [TotalVenta]
    FROM [dbo].[VentaDetalle] D
    INNER JOIN [dbo].[Venta] V ON V.[Id] = D.[VentaId]
    INNER JOIN [dbo].[Producto] P ON P.[Id] = D.[ProductoId]
    WHERE V.[EmpresaId] = @EmpresaId
      AND V.[Estatus] = 1
      AND D.[Estatus] = 1
      AND V.[FechaVenta] >= @FechaInicio
      AND V.[FechaVenta] <= @FechaFin
    GROUP BY P.[Id], P.[Nombre]
    ORDER BY [CantidadVendida] DESC;
END;

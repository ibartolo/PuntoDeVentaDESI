CREATE PROCEDURE [dbo].[sp_Reporte_VentasPorSucursal]
    @EmpresaId BIGINT,
    @FechaInicio DATETIME,
    @FechaFin DATETIME
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        S.[Id] AS [SucursalId],
        S.[Nombre] AS [SucursalNombre],
        COUNT(V.[Id]) AS [NumeroVentas],
        ISNULL(SUM(V.[Total]), 0) AS [Total]
    FROM [dbo].[Sucursal] S
    LEFT JOIN [dbo].[Venta] V ON V.[SucursalId] = S.[Id]
        AND V.[Estatus] = 1
        AND V.[FechaVenta] >= @FechaInicio
        AND V.[FechaVenta] <= @FechaFin
    WHERE S.[EmpresaId] = @EmpresaId
      AND S.[Estatus] = 1
    GROUP BY S.[Id], S.[Nombre]
    ORDER BY [Total] DESC;
END;

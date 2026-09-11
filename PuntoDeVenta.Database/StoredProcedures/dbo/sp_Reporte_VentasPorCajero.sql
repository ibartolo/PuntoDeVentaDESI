CREATE PROCEDURE [dbo].[sp_Reporte_VentasPorCajero]
    @EmpresaId BIGINT,
    @FechaInicio DATETIME,
    @FechaFin DATETIME
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        U.[Id] AS [UsuarioId],
        U.[NombreUsuario] AS [CajeroNombre],
        COUNT(V.[Id]) AS [NumeroVentas],
        ISNULL(SUM(V.[Total]), 0) AS [Total]
    FROM [dbo].[Usuario] U
    INNER JOIN [dbo].[Venta] V ON V.[UsuarioId] = U.[Id]
        AND V.[Estatus] = 1
        AND V.[FechaVenta] >= @FechaInicio
        AND V.[FechaVenta] <= @FechaFin
    WHERE U.[EmpresaId] = @EmpresaId
    GROUP BY U.[Id], U.[NombreUsuario]
    ORDER BY [Total] DESC;
END;

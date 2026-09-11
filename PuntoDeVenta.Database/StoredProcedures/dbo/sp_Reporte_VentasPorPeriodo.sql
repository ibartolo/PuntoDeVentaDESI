CREATE PROCEDURE [dbo].[sp_Reporte_VentasPorPeriodo]
    @EmpresaId BIGINT,
    @FechaInicio DATETIME,
    @FechaFin DATETIME,
    @SucursalId BIGINT = 0
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        V.[FechaVenta] AS [Fecha],
        V.[Folio],
        S.[Nombre] AS [SucursalNombre],
        U.[NombreUsuario] AS [CajeroNombre],
        V.[MetodoPago],
        V.[Subtotal],
        V.[Impuesto],
        V.[Total]
    FROM [dbo].[Venta] V
    INNER JOIN [dbo].[Sucursal] S ON S.[Id] = V.[SucursalId]
    INNER JOIN [dbo].[Usuario] U ON U.[Id] = V.[UsuarioId]
    WHERE V.[EmpresaId] = @EmpresaId
      AND V.[Estatus] = 1
      AND V.[FechaVenta] >= @FechaInicio
      AND V.[FechaVenta] <= @FechaFin
      AND (@SucursalId = 0 OR V.[SucursalId] = @SucursalId)
    ORDER BY V.[FechaVenta] DESC;
END;

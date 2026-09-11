CREATE PROCEDURE [dbo].[sp_StockMovimiento_Listar]
    @EmpresaId BIGINT,
    @SucursalId BIGINT = 0,
    @ProductoId BIGINT = 0
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        M.[Id], M.[EmpresaId], M.[SucursalId], SU.[Nombre] AS [SucursalNombre],
        M.[ProductoId], P.[Nombre] AS [ProductoNombre], M.[TipoMovimiento], M.[Cantidad],
        M.[ExistenciaAnterior], M.[ExistenciaNueva], M.[Motivo], M.[ReferenciaId],
        M.[Estatus], M.[CreadoPor], M.[FechaCreacion], M.[ModificadoPor], M.[FechaModificacion]
    FROM [dbo].[StockMovimiento] M
    INNER JOIN [dbo].[Sucursal] SU ON SU.[Id] = M.[SucursalId]
    INNER JOIN [dbo].[Producto] P ON P.[Id] = M.[ProductoId]
    WHERE M.[EmpresaId] = @EmpresaId
      AND (@SucursalId = 0 OR M.[SucursalId] = @SucursalId)
      AND (@ProductoId = 0 OR M.[ProductoId] = @ProductoId)
      AND M.[Estatus] = 1
    ORDER BY M.[FechaCreacion] DESC;
END;

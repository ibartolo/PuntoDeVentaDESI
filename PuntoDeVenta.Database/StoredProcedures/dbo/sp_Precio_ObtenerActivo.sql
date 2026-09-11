CREATE PROCEDURE [dbo].[sp_Precio_ObtenerActivo]
    @EmpresaId BIGINT,
    @ProductoId BIGINT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT TOP (1)
        PR.[Id], PR.[EmpresaId], PR.[ProductoId], P.[Nombre] AS [ProductoNombre],
        PR.[PrecioCompra], PR.[PrecioVentaSugerido], PR.[PrecioVenta], PR.[Activo],
        PR.[FechaInicio], PR.[FechaFin],
        PR.[Estatus], PR.[CreadoPor], PR.[FechaCreacion], PR.[ModificadoPor], PR.[FechaModificacion]
    FROM [dbo].[Precio] PR
    INNER JOIN [dbo].[Producto] P ON P.[Id] = PR.[ProductoId]
    WHERE PR.[EmpresaId] = @EmpresaId
      AND PR.[ProductoId] = @ProductoId
      AND PR.[Activo] = 1
      AND PR.[Estatus] = 1
    ORDER BY PR.[FechaInicio] DESC;
END;

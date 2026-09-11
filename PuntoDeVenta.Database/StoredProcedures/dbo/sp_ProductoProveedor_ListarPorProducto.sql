CREATE PROCEDURE [dbo].[sp_ProductoProveedor_ListarPorProducto]
    @EmpresaId BIGINT,
    @ProductoId BIGINT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        PP.[Id], PP.[EmpresaId], PP.[ProductoId], PP.[ProveedorId], PR.[Nombre] AS [ProveedorNombre],
        PP.[Costo],
        PP.[Estatus], PP.[CreadoPor], PP.[FechaCreacion], PP.[ModificadoPor], PP.[FechaModificacion]
    FROM [dbo].[ProductoProveedor] PP
    INNER JOIN [dbo].[Proveedor] PR ON PR.[Id] = PP.[ProveedorId]
    WHERE PP.[EmpresaId] = @EmpresaId
      AND PP.[ProductoId] = @ProductoId
      AND PP.[Estatus] = 1
    ORDER BY PR.[Nombre] ASC;
END;

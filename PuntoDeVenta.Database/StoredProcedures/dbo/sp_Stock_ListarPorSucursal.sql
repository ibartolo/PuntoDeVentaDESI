CREATE PROCEDURE [dbo].[sp_Stock_ListarPorSucursal]
    @EmpresaId BIGINT,
    @SucursalId BIGINT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        S.[Id], S.[EmpresaId], S.[SucursalId], SU.[Nombre] AS [SucursalNombre],
        S.[ProductoId], P.[Nombre] AS [ProductoNombre], S.[Cantidad], S.[StockMinimo],
        CASE WHEN S.[Cantidad] <= S.[StockMinimo] THEN 1 ELSE 0 END AS [BajoMinimo],
        S.[Estatus], S.[CreadoPor], S.[FechaCreacion], S.[ModificadoPor], S.[FechaModificacion]
    FROM [dbo].[Stock] S
    INNER JOIN [dbo].[Sucursal] SU ON SU.[Id] = S.[SucursalId]
    INNER JOIN [dbo].[Producto] P ON P.[Id] = S.[ProductoId]
    WHERE S.[EmpresaId] = @EmpresaId
      AND S.[SucursalId] = @SucursalId
      AND S.[Estatus] = 1
    ORDER BY P.[Nombre] ASC;
END;

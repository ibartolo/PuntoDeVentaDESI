CREATE PROCEDURE [dbo].[sp_Producto_Listar]
    @EmpresaId BIGINT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        PR.[Id], PR.[EmpresaId], PR.[CategoriaId], C.[Nombre] AS [CategoriaNombre],
        PR.[MarcaId], M.[Nombre] AS [MarcaNombre], PR.[MarcaTexto], PR.[Nombre], PR.[Descripcion],
        PR.[FotoUrl], PR.[Codigo], PR.[TipoProducto], PR.[UnidadMedida], PR.[StockMinimo],
        ISNULL(PV.[PrecioVenta], 0) AS [PrecioVenta], CAST(0 AS DECIMAL(18,4)) AS [StockDisponible],
        PR.[Estatus], PR.[CreadoPor], PR.[FechaCreacion], PR.[ModificadoPor], PR.[FechaModificacion]
    FROM [dbo].[Producto] PR
    LEFT JOIN [dbo].[Categoria] C ON C.[Id] = PR.[CategoriaId]
    LEFT JOIN [dbo].[Marca] M ON M.[Id] = PR.[MarcaId]
    LEFT JOIN [dbo].[Precio] PV ON PV.[ProductoId] = PR.[Id] AND PV.[Activo] = 1 AND PV.[Estatus] = 1
    WHERE PR.[EmpresaId] = @EmpresaId
      AND PR.[Estatus] = 1
    ORDER BY PR.[Nombre] ASC;
END;

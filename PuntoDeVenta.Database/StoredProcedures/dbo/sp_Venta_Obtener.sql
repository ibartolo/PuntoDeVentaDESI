CREATE PROCEDURE [dbo].[sp_Venta_Obtener]
    @EmpresaId BIGINT,
    @VentaId BIGINT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        V.[Id], V.[EmpresaId], V.[SucursalId], S.[Nombre] AS [SucursalNombre],
        V.[UsuarioId], U.[NombreUsuario] AS [UsuarioNombre], V.[ClienteId], C.[Nombre] AS [ClienteNombre],
        V.[CajaChicaId], V.[Folio], V.[FechaVenta], V.[MetodoPago], V.[Subtotal], V.[Impuesto], V.[Total],
        V.[Estatus], V.[CreadoPor], V.[FechaCreacion], V.[ModificadoPor], V.[FechaModificacion]
    FROM [dbo].[Venta] V
    INNER JOIN [dbo].[Sucursal] S ON S.[Id] = V.[SucursalId]
    INNER JOIN [dbo].[Usuario] U ON U.[Id] = V.[UsuarioId]
    LEFT JOIN [dbo].[Cliente] C ON C.[Id] = V.[ClienteId]
    WHERE V.[EmpresaId] = @EmpresaId
      AND V.[Id] = @VentaId;

    SELECT
        D.[Id], D.[VentaId], D.[ProductoId], P.[Nombre] AS [ProductoNombre],
        D.[Cantidad], D.[PrecioUnitario], D.[Importe],
        D.[Estatus], D.[CreadoPor], D.[FechaCreacion], D.[ModificadoPor], D.[FechaModificacion]
    FROM [dbo].[VentaDetalle] D
    INNER JOIN [dbo].[Producto] P ON P.[Id] = D.[ProductoId]
    WHERE D.[VentaId] = @VentaId
      AND D.[Estatus] = 1;
END;

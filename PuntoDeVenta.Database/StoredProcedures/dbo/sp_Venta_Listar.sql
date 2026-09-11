CREATE PROCEDURE [dbo].[sp_Venta_Listar]
    @EmpresaId BIGINT,
    @SucursalId BIGINT = 0
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
      AND V.[Estatus] = 1
      AND (@SucursalId = 0 OR V.[SucursalId] = @SucursalId)
    ORDER BY V.[FechaVenta] DESC;
END;

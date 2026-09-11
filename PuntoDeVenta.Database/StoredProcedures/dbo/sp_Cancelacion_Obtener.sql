CREATE PROCEDURE [dbo].[sp_Cancelacion_Obtener]
    @EmpresaId BIGINT,
    @CancelacionId BIGINT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        C.[Id], C.[EmpresaId], C.[SucursalId], C.[VentaId], V.[Folio] AS [VentaFolio],
        C.[Tipo], C.[Motivo], C.[UsuarioCancela], C.[UsuarioAutoriza], C.[FechaCancelacion],
        C.[TotalReembolsado], C.[MetodoReembolso],
        C.[Estatus], C.[CreadoPor], C.[FechaCreacion], C.[ModificadoPor], C.[FechaModificacion]
    FROM [dbo].[Cancelacion] C
    INNER JOIN [dbo].[Venta] V ON V.[Id] = C.[VentaId]
    WHERE C.[EmpresaId] = @EmpresaId
      AND C.[Id] = @CancelacionId;

    SELECT
        D.[Id], D.[CancelacionId], D.[VentaDetalleId], D.[ProductoId], P.[Nombre] AS [ProductoNombre],
        D.[Cantidad], D.[Importe], D.[RegresaAStock],
        D.[Estatus], D.[CreadoPor], D.[FechaCreacion], D.[ModificadoPor], D.[FechaModificacion]
    FROM [dbo].[DevolucionDetalle] D
    INNER JOIN [dbo].[Producto] P ON P.[Id] = D.[ProductoId]
    WHERE D.[CancelacionId] = @CancelacionId
      AND D.[Estatus] = 1;
END;

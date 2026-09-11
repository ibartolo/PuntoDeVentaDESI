CREATE PROCEDURE [dbo].[sp_Corte_Obtener]
    @EmpresaId BIGINT,
    @CorteId BIGINT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        C.[Id], C.[EmpresaId], C.[SucursalId], S.[Nombre] AS [SucursalNombre],
        C.[UsuarioId], U.[NombreUsuario] AS [UsuarioNombre], C.[CajaChicaId], C.[FechaCorte],
        C.[MontoInicial], C.[TotalVentas], C.[TotalEfectivo], C.[TotalTarjeta], C.[TotalTransferencia],
        C.[Salidas], C.[EfectivoEsperado], C.[EfectivoContado], C.[Diferencia], C.[Observaciones],
        C.[Estatus], C.[CreadoPor], C.[FechaCreacion], C.[ModificadoPor], C.[FechaModificacion]
    FROM [dbo].[Corte] C
    INNER JOIN [dbo].[Sucursal] S ON S.[Id] = C.[SucursalId]
    INNER JOIN [dbo].[Usuario] U ON U.[Id] = C.[UsuarioId]
    WHERE C.[EmpresaId] = @EmpresaId
      AND C.[Id] = @CorteId;

    SELECT
        D.[Id], D.[CorteId], D.[MetodoPago], D.[Monto], D.[FolioCobro],
        D.[TicketCobroUrl], D.[TicketSistemaUrl], D.[ComprobanteUrl],
        D.[Estatus], D.[CreadoPor], D.[FechaCreacion], D.[ModificadoPor], D.[FechaModificacion]
    FROM [dbo].[CorteDetalle] D
    WHERE D.[CorteId] = @CorteId
      AND D.[Estatus] = 1;
END;

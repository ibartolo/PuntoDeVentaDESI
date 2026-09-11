CREATE PROCEDURE [dbo].[sp_Corte_Listar]
    @EmpresaId BIGINT,
    @SucursalId BIGINT = 0
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
      AND C.[Estatus] = 1
      AND (@SucursalId = 0 OR C.[SucursalId] = @SucursalId)
    ORDER BY C.[FechaCorte] DESC;
END;

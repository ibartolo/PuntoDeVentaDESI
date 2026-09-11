CREATE PROCEDURE [dbo].[sp_Cancelacion_Listar]
    @EmpresaId BIGINT,
    @SucursalId BIGINT = 0
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
      AND C.[Estatus] = 1
      AND (@SucursalId = 0 OR C.[SucursalId] = @SucursalId)
    ORDER BY C.[FechaCancelacion] DESC;
END;

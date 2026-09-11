CREATE PROCEDURE [dbo].[sp_SalidaCaja_Listar]
    @EmpresaId BIGINT,
    @CajaChicaId BIGINT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        [Id], [EmpresaId], [CajaChicaId], [Monto], [Comentario], [FechaHora], [Justificada], [EvidenciaUrl], [TipoSalida],
        [Estatus], [CreadoPor], [FechaCreacion], [ModificadoPor], [FechaModificacion]
    FROM [dbo].[SalidaCaja]
    WHERE [EmpresaId] = @EmpresaId
      AND [CajaChicaId] = @CajaChicaId
      AND [Estatus] = 1
    ORDER BY [FechaHora] DESC;
END;

CREATE PROCEDURE [dbo].[sp_CajaChica_Cerrar]
    @EmpresaId BIGINT,
    @CajaChicaId BIGINT,
    @Actor NVARCHAR(25)
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE [dbo].[CajaChica]
    SET [Estado] = N'Cerrada',
        [FechaCierre] = GETDATE(),
        [ModificadoPor] = @Actor,
        [FechaModificacion] = GETDATE()
    WHERE [EmpresaId] = @EmpresaId
      AND [Id] = @CajaChicaId
      AND [Estado] = N'Abierta'
      AND [Estatus] = 1;

    SELECT @@ROWCOUNT AS [AffectedRows];
END;

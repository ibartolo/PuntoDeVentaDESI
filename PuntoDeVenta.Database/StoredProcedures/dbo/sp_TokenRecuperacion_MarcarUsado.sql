CREATE PROCEDURE [dbo].[sp_TokenRecuperacion_MarcarUsado]
    @TokenRecuperacionId BIGINT,
    @Actor NVARCHAR(25)
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE [dbo].[TokenRecuperacion]
    SET
        [Usado] = 1,
        [ModificadoPor] = @Actor,
        [FechaModificacion] = GETDATE()
    WHERE [Id] = @TokenRecuperacionId
      AND [Estatus] = 1;

    SELECT @@ROWCOUNT AS [AffectedRows];
END;

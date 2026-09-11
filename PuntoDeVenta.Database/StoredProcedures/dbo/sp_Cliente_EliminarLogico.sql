CREATE PROCEDURE [dbo].[sp_Cliente_EliminarLogico]
    @EmpresaId BIGINT,
    @ClienteId BIGINT,
    @Actor NVARCHAR(25)
AS
BEGIN
    SET NOCOUNT ON;

    IF EXISTS
    (
        SELECT 1
        FROM [dbo].[Cliente]
        WHERE [EmpresaId] = @EmpresaId
          AND [Id] = @ClienteId
          AND [EsPublicoGeneral] = 1
          AND [Estatus] = 1
    )
    BEGIN
        RAISERROR('El cliente "Público General" no puede eliminarse.', 16, 1);
        RETURN;
    END;

    UPDATE [dbo].[Cliente]
    SET
        [Estatus] = 0,
        [ModificadoPor] = @Actor,
        [FechaModificacion] = GETDATE()
    WHERE [EmpresaId] = @EmpresaId
      AND [Id] = @ClienteId
      AND [Estatus] = 1;

    SELECT @@ROWCOUNT AS [AffectedRows];
END;

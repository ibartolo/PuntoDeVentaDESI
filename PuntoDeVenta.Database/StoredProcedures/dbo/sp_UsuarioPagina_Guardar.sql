CREATE PROCEDURE [dbo].[sp_UsuarioPagina_Guardar]
    @Id BIGINT,
    @UsuarioId BIGINT,
    @PaginaId BIGINT,
    @Actor NVARCHAR(25)
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @ExistenteId BIGINT;

    SELECT TOP (1) @ExistenteId = [Id]
    FROM [dbo].[UsuarioPagina]
    WHERE [UsuarioId] = @UsuarioId
      AND [PaginaId] = @PaginaId;

    IF @ExistenteId IS NULL
    BEGIN
        INSERT INTO [dbo].[UsuarioPagina]
        (
            [UsuarioId], [PaginaId], [Estatus], [CreadoPor], [FechaCreacion], [ModificadoPor], [FechaModificacion]
        )
        VALUES
        (
            @UsuarioId, @PaginaId, 1, @Actor, GETDATE(), NULL, NULL
        );

        SELECT CAST(SCOPE_IDENTITY() AS BIGINT);
    END
    ELSE
    BEGIN
        UPDATE [dbo].[UsuarioPagina]
        SET
            [Estatus] = 1,
            [ModificadoPor] = @Actor,
            [FechaModificacion] = GETDATE()
        WHERE [Id] = @ExistenteId;

        SELECT CAST(@ExistenteId AS BIGINT);
    END;
END;

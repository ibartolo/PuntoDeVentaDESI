CREATE PROCEDURE [dbo].[sp_UsuarioPagina_Obtener]
    @Id BIGINT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT TOP (1)
        [Id],
        [UsuarioId],
        [PaginaId],
        [Estatus],
        [CreadoPor],
        [FechaCreacion],
        [ModificadoPor],
        [FechaModificacion]
    FROM [dbo].[UsuarioPagina]
    WHERE [Id] = @Id
      AND [Estatus] = 1;
END;

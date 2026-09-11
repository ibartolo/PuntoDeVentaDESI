CREATE PROCEDURE [dbo].[sp_UsuarioPagina_ListarTodos]
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        [Id],
        [UsuarioId],
        [PaginaId],
        [Estatus],
        [CreadoPor],
        [FechaCreacion],
        [ModificadoPor],
        [FechaModificacion]
    FROM [dbo].[UsuarioPagina]
    WHERE [Estatus] = 1;
END;

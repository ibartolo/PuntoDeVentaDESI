CREATE PROCEDURE [dbo].[sp_UsuarioPagina_ListarPorUsuario]
    @EmpresaId BIGINT,
    @UsuarioId BIGINT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        UP.[Id],
        UP.[UsuarioId],
        UP.[PaginaId],
        UP.[Estatus],
        UP.[CreadoPor],
        UP.[FechaCreacion],
        UP.[ModificadoPor],
        UP.[FechaModificacion]
    FROM [dbo].[UsuarioPagina] UP
    INNER JOIN [dbo].[Usuario] U
        ON UP.[UsuarioId] = U.[Id]
       AND U.[EmpresaId] = @EmpresaId
    WHERE UP.[UsuarioId] = @UsuarioId
      AND UP.[Estatus] = 1
    ORDER BY UP.[Id] ASC;
END;

CREATE PROCEDURE [dbo].[sp_UsuarioRol_ListarPorUsuario]
    @EmpresaId BIGINT,
    @UsuarioId BIGINT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        UR.[Id],
        UR.[UsuarioId],
        UR.[RolId],
        UR.[Estatus],
        UR.[CreadoPor],
        UR.[FechaCreacion],
        UR.[ModificadoPor],
        UR.[FechaModificacion]
    FROM [dbo].[UsuarioRol] UR
    INNER JOIN [dbo].[Rol] R
        ON UR.[RolId] = R.[Id]
       AND R.[Estatus] = 1
       AND R.[EmpresaId] = @EmpresaId
    WHERE UR.[UsuarioId] = @UsuarioId
      AND UR.[Estatus] = 1
    ORDER BY UR.[Id] ASC;
END;

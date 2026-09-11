CREATE PROCEDURE [dbo].[sp_Rol_PorUsuario]
    @EmpresaId BIGINT,
    @UsuarioId BIGINT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        R.[Id],
        R.[EmpresaId],
        R.[Nombre],
        R.[Descripcion],
        R.[PuedeAutorizar],
        R.[Estatus],
        R.[CreadoPor],
        R.[FechaCreacion],
        R.[ModificadoPor],
        R.[FechaModificacion]
    FROM [dbo].[Rol] R
    INNER JOIN [dbo].[UsuarioRol] UR
        ON R.[Id] = UR.[RolId]
    WHERE UR.[UsuarioId] = @UsuarioId
      AND UR.[Estatus] = 1
      AND R.[Estatus] = 1
      AND R.[EmpresaId] = @EmpresaId
    ORDER BY R.[Nombre] ASC;
END;

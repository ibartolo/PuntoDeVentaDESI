CREATE PROCEDURE [dbo].[sp_Pagina_PorUsuario]
    @EmpresaId BIGINT,
    @UsuarioId BIGINT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        P.[Id],
        P.[Nombre],
        P.[NombreVisible],
        P.[Descripcion],
        P.[Tipo],
        P.[Direccion],
        P.[PermisosPadreId],
        P.[Logo],
        P.[OrdenB],
        P.[Estatus],
        P.[CreadoPor],
        P.[FechaCreacion],
        P.[ModificadoPor],
        P.[FechaModificacion]
    FROM [dbo].[Pagina] P
    INNER JOIN [dbo].[RolPaginaAccion] RPA
        ON P.[Id] = RPA.[PaginaId]
       AND RPA.[Estatus] = 1
       AND RPA.[PuedeLeer] = 1
    INNER JOIN [dbo].[Rol] R
        ON RPA.[RolId] = R.[Id]
       AND R.[Estatus] = 1
       AND R.[EmpresaId] = @EmpresaId
    INNER JOIN [dbo].[UsuarioRol] UR
        ON R.[Id] = UR.[RolId]
       AND UR.[Estatus] = 1
       AND UR.[UsuarioId] = @UsuarioId
    WHERE P.[Estatus] = 1

    UNION

    SELECT
        P.[Id],
        P.[Nombre],
        P.[NombreVisible],
        P.[Descripcion],
        P.[Tipo],
        P.[Direccion],
        P.[PermisosPadreId],
        P.[Logo],
        P.[OrdenB],
        P.[Estatus],
        P.[CreadoPor],
        P.[FechaCreacion],
        P.[ModificadoPor],
        P.[FechaModificacion]
    FROM [dbo].[Pagina] P
    INNER JOIN [dbo].[UsuarioPagina] UP
        ON P.[Id] = UP.[PaginaId]
       AND UP.[Estatus] = 1
       AND UP.[UsuarioId] = @UsuarioId
    WHERE P.[Estatus] = 1

    ORDER BY [OrdenB] ASC, [Nombre] ASC;
END;

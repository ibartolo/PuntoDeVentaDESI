CREATE PROCEDURE [dbo].[sp_RolPaginaAccion_ListarPorRol]
    @EmpresaId BIGINT,
    @RolId BIGINT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        RPA.[Id],
        RPA.[RolId],
        RPA.[PaginaId],
        RPA.[PuedeLeer],
        RPA.[PuedeCrear],
        RPA.[PuedeEditar],
        RPA.[PuedeEliminar],
        RPA.[PuedeExportar],
        RPA.[Estatus],
        RPA.[CreadoPor],
        RPA.[FechaCreacion],
        RPA.[ModificadoPor],
        RPA.[FechaModificacion],
        P.[Nombre] AS [PaginaNombre],
        P.[Direccion] AS [Direccion]
    FROM [dbo].[RolPaginaAccion] RPA
    INNER JOIN [dbo].[Rol] R
        ON RPA.[RolId] = R.[Id]
       AND R.[Estatus] = 1
       AND R.[EmpresaId] = @EmpresaId
    INNER JOIN [dbo].[Pagina] P
        ON RPA.[PaginaId] = P.[Id]
    WHERE RPA.[RolId] = @RolId
      AND RPA.[Estatus] = 1
    ORDER BY P.[OrdenB] ASC, P.[Nombre] ASC;
END;

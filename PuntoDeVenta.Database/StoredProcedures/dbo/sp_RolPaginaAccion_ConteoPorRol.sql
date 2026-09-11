CREATE PROCEDURE [dbo].[sp_RolPaginaAccion_ConteoPorRol]
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        R.[Id] AS [RolId],
        CAST(COUNT(RPA.[Id]) AS INT) AS [TotalPaginas]
    FROM [dbo].[Rol] R
    LEFT JOIN [dbo].[RolPaginaAccion] RPA
        ON RPA.[RolId] = R.[Id]
       AND RPA.[Estatus] = 1
    WHERE R.[Estatus] = 1
    GROUP BY R.[Id];
END;

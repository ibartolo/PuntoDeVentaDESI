CREATE PROCEDURE [dbo].[sp_Categoria_Listar]
    @EmpresaId BIGINT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        C.[Id],
        C.[EmpresaId],
        C.[CategoriaPadreId],
        P.[Nombre] AS [CategoriaPadreNombre],
        C.[Nombre],
        C.[Descripcion],
        C.[Area],
        C.[Orden],
        C.[Estatus],
        C.[CreadoPor],
        C.[FechaCreacion],
        C.[ModificadoPor],
        C.[FechaModificacion]
    FROM [dbo].[Categoria] C
    LEFT JOIN [dbo].[Categoria] P
        ON P.[Id] = C.[CategoriaPadreId]
    WHERE C.[EmpresaId] = @EmpresaId
      AND C.[Estatus] = 1
    ORDER BY C.[Orden] ASC, C.[Nombre] ASC;
END;

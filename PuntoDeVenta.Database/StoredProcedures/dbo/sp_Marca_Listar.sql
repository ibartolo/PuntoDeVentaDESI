CREATE PROCEDURE [dbo].[sp_Marca_Listar]
    @EmpresaId BIGINT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        [Id],
        [EmpresaId],
        [Nombre],
        [Descripcion],
        [Estatus],
        [CreadoPor],
        [FechaCreacion],
        [ModificadoPor],
        [FechaModificacion]
    FROM [dbo].[Marca]
    WHERE [EmpresaId] = @EmpresaId
      AND [Estatus] = 1
    ORDER BY [Nombre] ASC;
END;

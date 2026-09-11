CREATE PROCEDURE [dbo].[sp_Sucursal_Listar]
    @EmpresaId BIGINT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        [Id],
        [EmpresaId],
        [Nombre],
        [Descripcion],
        [Calle],
        [Ciudad],
        [Colonia],
        [CodigoPostal],
        [Estatus],
        [CreadoPor],
        [FechaCreacion],
        [ModificadoPor],
        [FechaModificacion]
    FROM [dbo].[Sucursal]
    WHERE [EmpresaId] = @EmpresaId
      AND [Estatus] = 1
    ORDER BY [Nombre] ASC;
END;

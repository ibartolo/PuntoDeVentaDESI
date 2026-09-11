CREATE PROCEDURE [dbo].[sp_Sucursal_Consultar]
    @EmpresaId BIGINT,
    @SucursalId BIGINT
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
      AND [Id] = @SucursalId;
END;

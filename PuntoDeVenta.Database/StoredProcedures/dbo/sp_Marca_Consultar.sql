CREATE PROCEDURE [dbo].[sp_Marca_Consultar]
    @EmpresaId BIGINT,
    @MarcaId BIGINT
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
      AND [Id] = @MarcaId;
END;

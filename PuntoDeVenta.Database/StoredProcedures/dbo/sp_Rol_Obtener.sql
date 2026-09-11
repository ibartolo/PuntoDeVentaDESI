CREATE PROCEDURE [dbo].[sp_Rol_Obtener]
    @EmpresaId BIGINT,
    @RolId BIGINT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        [Id],
        [EmpresaId],
        [Nombre],
        [Descripcion],
        [PuedeAutorizar],
        [Estatus],
        [CreadoPor],
        [FechaCreacion],
        [ModificadoPor],
        [FechaModificacion]
    FROM [dbo].[Rol]
    WHERE [EmpresaId] = @EmpresaId
      AND [Id] = @RolId;
END;

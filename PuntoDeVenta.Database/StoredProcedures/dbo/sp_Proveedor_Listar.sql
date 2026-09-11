CREATE PROCEDURE [dbo].[sp_Proveedor_Listar]
    @EmpresaId BIGINT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        [Id],
        [EmpresaId],
        [Nombre],
        [Contacto],
        [Correo],
        [Telefono],
        [Direccion],
        [Estatus],
        [CreadoPor],
        [FechaCreacion],
        [ModificadoPor],
        [FechaModificacion]
    FROM [dbo].[Proveedor]
    WHERE [EmpresaId] = @EmpresaId
      AND [Estatus] = 1
    ORDER BY [Nombre] ASC;
END;

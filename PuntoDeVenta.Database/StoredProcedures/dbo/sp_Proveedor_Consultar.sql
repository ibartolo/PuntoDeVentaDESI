CREATE PROCEDURE [dbo].[sp_Proveedor_Consultar]
    @EmpresaId BIGINT,
    @ProveedorId BIGINT
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
      AND [Id] = @ProveedorId;
END;

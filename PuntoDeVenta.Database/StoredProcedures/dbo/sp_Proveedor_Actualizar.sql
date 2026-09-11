CREATE PROCEDURE [dbo].[sp_Proveedor_Actualizar]
    @EmpresaId BIGINT,
    @ProveedorId BIGINT,
    @Nombre NVARCHAR(150),
    @Contacto NVARCHAR(150) = NULL,
    @Correo NVARCHAR(250) = NULL,
    @Telefono NVARCHAR(50) = NULL,
    @Direccion NVARCHAR(500) = NULL,
    @Actor NVARCHAR(25)
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE [dbo].[Proveedor]
    SET
        [Nombre] = LTRIM(RTRIM(@Nombre)),
        [Contacto] = @Contacto,
        [Correo] = @Correo,
        [Telefono] = @Telefono,
        [Direccion] = @Direccion,
        [ModificadoPor] = @Actor,
        [FechaModificacion] = GETDATE()
    WHERE [EmpresaId] = @EmpresaId
      AND [Id] = @ProveedorId
      AND [Estatus] = 1;

    SELECT @@ROWCOUNT AS [AffectedRows];
END;

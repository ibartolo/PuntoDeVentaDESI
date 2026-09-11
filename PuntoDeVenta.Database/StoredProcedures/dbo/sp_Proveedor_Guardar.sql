CREATE PROCEDURE [dbo].[sp_Proveedor_Guardar]
    @EmpresaId BIGINT,
    @ProveedorId BIGINT = 0,
    @Nombre NVARCHAR(150),
    @Contacto NVARCHAR(150) = NULL,
    @Correo NVARCHAR(150) = NULL,
    @Telefono NVARCHAR(30) = NULL,
    @Direccion NVARCHAR(300) = NULL,
    @Actor NVARCHAR(25)
AS
BEGIN
    SET NOCOUNT ON;

    IF NOT EXISTS (SELECT 1 FROM [dbo].[Empresa] WHERE [Id] = @EmpresaId AND [Estatus] = 1)
    BEGIN
        RAISERROR('Empresa no valida para operacion de Proveedor.', 16, 1);
        RETURN;
    END;

    IF @ProveedorId IS NULL OR @ProveedorId = 0
    BEGIN
        INSERT INTO [dbo].[Proveedor]
        ([EmpresaId], [Nombre], [Contacto], [Correo], [Telefono], [Direccion],
         [Estatus], [CreadoPor], [FechaCreacion], [ModificadoPor], [FechaModificacion])
        VALUES
        (@EmpresaId, LTRIM(RTRIM(@Nombre)), @Contacto, @Correo, @Telefono, @Direccion,
         1, @Actor, GETDATE(), NULL, NULL);

        SELECT CAST(SCOPE_IDENTITY() AS BIGINT) AS [Id];
    END
    ELSE
    BEGIN
        UPDATE [dbo].[Proveedor]
        SET [Nombre] = LTRIM(RTRIM(@Nombre)),
            [Contacto] = @Contacto,
            [Correo] = @Correo,
            [Telefono] = @Telefono,
            [Direccion] = @Direccion,
            [ModificadoPor] = @Actor,
            [FechaModificacion] = GETDATE()
        WHERE [EmpresaId] = @EmpresaId
          AND [Id] = @ProveedorId
          AND [Estatus] = 1;

        SELECT @ProveedorId AS [Id];
    END
END;

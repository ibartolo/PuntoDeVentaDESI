CREATE PROCEDURE [dbo].[sp_Proveedor_Insertar]
    @EmpresaId BIGINT,
    @Nombre NVARCHAR(150),
    @Contacto NVARCHAR(150) = NULL,
    @Correo NVARCHAR(250) = NULL,
    @Telefono NVARCHAR(50) = NULL,
    @Direccion NVARCHAR(500) = NULL,
    @Actor NVARCHAR(25)
AS
BEGIN
    SET NOCOUNT ON;

    IF NOT EXISTS
    (
        SELECT 1
        FROM [dbo].[Empresa]
        WHERE [Id] = @EmpresaId
          AND [Estatus] = 1
    )
    BEGIN
        RAISERROR('Empresa no válida para operación de Proveedor.', 16, 1);
        RETURN;
    END;

    INSERT INTO [dbo].[Proveedor]
    (
        [EmpresaId], [Nombre], [Contacto], [Correo], [Telefono], [Direccion], [Estatus],
        [CreadoPor], [FechaCreacion], [ModificadoPor], [FechaModificacion]
    )
    VALUES
    (
        @EmpresaId, LTRIM(RTRIM(@Nombre)), @Contacto, @Correo, @Telefono, @Direccion, 1,
        @Actor, GETDATE(), NULL, NULL
    );

    SELECT CAST(SCOPE_IDENTITY() AS BIGINT) AS [Id];
END;

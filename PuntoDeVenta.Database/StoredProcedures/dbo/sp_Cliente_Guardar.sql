CREATE PROCEDURE [dbo].[sp_Cliente_Guardar]
    @EmpresaId BIGINT,
    @ClienteId BIGINT = 0,
    @Nombre NVARCHAR(150),
    @Correo NVARCHAR(150) = NULL,
    @Telefono NVARCHAR(30) = NULL,
    @Direccion NVARCHAR(300) = NULL,
    @EsPublicoGeneral BIT = 0,
    @Actor NVARCHAR(25)
AS
BEGIN
    SET NOCOUNT ON;

    IF NOT EXISTS (SELECT 1 FROM [dbo].[Empresa] WHERE [Id] = @EmpresaId AND [Estatus] = 1)
    BEGIN
        RAISERROR('Empresa no valida para operacion de Cliente.', 16, 1);
        RETURN;
    END;

    IF @ClienteId IS NULL OR @ClienteId = 0
    BEGIN
        INSERT INTO [dbo].[Cliente]
        ([EmpresaId], [Nombre], [Correo], [Telefono], [Direccion], [EsPublicoGeneral],
         [Estatus], [CreadoPor], [FechaCreacion], [ModificadoPor], [FechaModificacion])
        VALUES
        (@EmpresaId, LTRIM(RTRIM(@Nombre)), @Correo, @Telefono, @Direccion, @EsPublicoGeneral,
         1, @Actor, GETDATE(), NULL, NULL);

        SELECT CAST(SCOPE_IDENTITY() AS BIGINT) AS [Id];
    END
    ELSE
    BEGIN
        UPDATE [dbo].[Cliente]
        SET [Nombre] = LTRIM(RTRIM(@Nombre)),
            [Correo] = @Correo,
            [Telefono] = @Telefono,
            [Direccion] = @Direccion,
            [EsPublicoGeneral] = @EsPublicoGeneral,
            [ModificadoPor] = @Actor,
            [FechaModificacion] = GETDATE()
        WHERE [EmpresaId] = @EmpresaId
          AND [Id] = @ClienteId
          AND [Estatus] = 1;

        SELECT @ClienteId AS [Id];
    END
END;

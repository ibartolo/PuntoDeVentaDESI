CREATE PROCEDURE [dbo].[sp_Cliente_Insertar]
    @EmpresaId BIGINT,
    @Nombre NVARCHAR(150),
    @Correo NVARCHAR(250) = NULL,
    @Telefono NVARCHAR(50) = NULL,
    @Direccion NVARCHAR(500) = NULL,
    @EsPublicoGeneral BIT = 0,
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
        RAISERROR('Empresa no válida para operación de Cliente.', 16, 1);
        RETURN;
    END;

    INSERT INTO [dbo].[Cliente]
    (
        [EmpresaId], [Nombre], [Correo], [Telefono], [Direccion], [EsPublicoGeneral], [Estatus],
        [CreadoPor], [FechaCreacion], [ModificadoPor], [FechaModificacion]
    )
    VALUES
    (
        @EmpresaId, LTRIM(RTRIM(@Nombre)), @Correo, @Telefono, @Direccion, @EsPublicoGeneral, 1,
        @Actor, GETDATE(), NULL, NULL
    );

    SELECT CAST(SCOPE_IDENTITY() AS BIGINT) AS [Id];
END;

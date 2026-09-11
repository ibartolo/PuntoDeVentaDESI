CREATE PROCEDURE [dbo].[sp_Sucursal_Insertar]
    @EmpresaId BIGINT,
    @Nombre NVARCHAR(100),
    @Descripcion NVARCHAR(MAX) = NULL,
    @Calle NVARCHAR(200) = NULL,
    @Ciudad NVARCHAR(100) = NULL,
    @Colonia NVARCHAR(100) = NULL,
    @CodigoPostal NVARCHAR(10) = NULL,
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
        RAISERROR('Empresa no válida para operación de Sucursal.', 16, 1);
        RETURN;
    END;

    INSERT INTO [dbo].[Sucursal]
    (
        [EmpresaId], [Nombre], [Descripcion], [Calle], [Ciudad], [Colonia], [CodigoPostal], [Estatus],
        [CreadoPor], [FechaCreacion], [ModificadoPor], [FechaModificacion]
    )
    VALUES
    (
        @EmpresaId, LTRIM(RTRIM(@Nombre)), @Descripcion, @Calle, @Ciudad, @Colonia, @CodigoPostal, 1,
        @Actor, GETDATE(), NULL, NULL
    );

    SELECT CAST(SCOPE_IDENTITY() AS BIGINT) AS [Id];
END;

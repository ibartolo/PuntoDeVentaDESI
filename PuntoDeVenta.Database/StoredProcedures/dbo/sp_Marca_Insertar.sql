CREATE PROCEDURE [dbo].[sp_Marca_Insertar]
    @EmpresaId BIGINT,
    @Nombre NVARCHAR(100),
    @Descripcion NVARCHAR(MAX) = NULL,
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
        RAISERROR('Empresa no válida para operación de Marca.', 16, 1);
        RETURN;
    END;

    INSERT INTO [dbo].[Marca]
    (
        [EmpresaId], [Nombre], [Descripcion], [Estatus],
        [CreadoPor], [FechaCreacion], [ModificadoPor], [FechaModificacion]
    )
    VALUES
    (
        @EmpresaId, LTRIM(RTRIM(@Nombre)), @Descripcion, 1,
        @Actor, GETDATE(), NULL, NULL
    );

    SELECT CAST(SCOPE_IDENTITY() AS BIGINT) AS [Id];
END;

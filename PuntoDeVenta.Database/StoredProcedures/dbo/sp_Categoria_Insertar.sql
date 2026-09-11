CREATE PROCEDURE [dbo].[sp_Categoria_Insertar]
    @EmpresaId BIGINT,
    @CategoriaPadreId BIGINT = NULL,
    @Nombre NVARCHAR(100),
    @Descripcion NVARCHAR(MAX) = NULL,
    @Area NVARCHAR(100) = NULL,
    @Orden INT = 0,
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
        RAISERROR('Empresa no válida para operación de Categoría.', 16, 1);
        RETURN;
    END;

    IF @CategoriaPadreId IS NOT NULL
       AND NOT EXISTS
       (
           SELECT 1
           FROM [dbo].[Categoria]
           WHERE [Id] = @CategoriaPadreId
             AND [EmpresaId] = @EmpresaId
             AND [Estatus] = 1
       )
    BEGIN
        RAISERROR('Categoría padre no válida para la empresa.', 16, 1);
        RETURN;
    END;

    INSERT INTO [dbo].[Categoria]
    (
        [EmpresaId], [CategoriaPadreId], [Nombre], [Descripcion], [Area], [Orden], [Estatus],
        [CreadoPor], [FechaCreacion], [ModificadoPor], [FechaModificacion]
    )
    VALUES
    (
        @EmpresaId, @CategoriaPadreId, LTRIM(RTRIM(@Nombre)), @Descripcion, @Area, @Orden, 1,
        @Actor, GETDATE(), NULL, NULL
    );

    SELECT CAST(SCOPE_IDENTITY() AS BIGINT) AS [Id];
END;

CREATE PROCEDURE [dbo].[sp_Categoria_Guardar]
    @EmpresaId BIGINT,
    @CategoriaId BIGINT = 0,
    @CategoriaPadreId BIGINT = NULL,
    @Nombre NVARCHAR(100),
    @Descripcion NVARCHAR(MAX) = NULL,
    @Area NVARCHAR(100) = NULL,
    @Orden INT = 0,
    @Actor NVARCHAR(25)
AS
BEGIN
    SET NOCOUNT ON;

    IF NOT EXISTS (SELECT 1 FROM [dbo].[Empresa] WHERE [Id] = @EmpresaId AND [Estatus] = 1)
    BEGIN
        RAISERROR('Empresa no valida para operacion de Categoria.', 16, 1);
        RETURN;
    END;

    IF @CategoriaId IS NULL OR @CategoriaId = 0
    BEGIN
        INSERT INTO [dbo].[Categoria]
        ([EmpresaId], [CategoriaPadreId], [Nombre], [Descripcion], [Area], [Orden],
         [Estatus], [CreadoPor], [FechaCreacion], [ModificadoPor], [FechaModificacion])
        VALUES
        (@EmpresaId, @CategoriaPadreId, LTRIM(RTRIM(@Nombre)), @Descripcion, @Area, @Orden,
         1, @Actor, GETDATE(), NULL, NULL);

        SELECT CAST(SCOPE_IDENTITY() AS BIGINT) AS [Id];
    END
    ELSE
    BEGIN
        UPDATE [dbo].[Categoria]
        SET [CategoriaPadreId] = @CategoriaPadreId,
            [Nombre] = LTRIM(RTRIM(@Nombre)),
            [Descripcion] = @Descripcion,
            [Area] = @Area,
            [Orden] = @Orden,
            [ModificadoPor] = @Actor,
            [FechaModificacion] = GETDATE()
        WHERE [EmpresaId] = @EmpresaId
          AND [Id] = @CategoriaId
          AND [Estatus] = 1;

        SELECT @CategoriaId AS [Id];
    END
END;

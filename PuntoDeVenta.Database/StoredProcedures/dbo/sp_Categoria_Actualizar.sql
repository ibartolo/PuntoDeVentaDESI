CREATE PROCEDURE [dbo].[sp_Categoria_Actualizar]
    @EmpresaId BIGINT,
    @CategoriaId BIGINT,
    @CategoriaPadreId BIGINT = NULL,
    @Nombre NVARCHAR(100),
    @Descripcion NVARCHAR(MAX) = NULL,
    @Area NVARCHAR(100) = NULL,
    @Orden INT = 0,
    @Actor NVARCHAR(25)
AS
BEGIN
    SET NOCOUNT ON;

    IF @CategoriaPadreId IS NOT NULL
       AND @CategoriaPadreId = @CategoriaId
    BEGIN
        RAISERROR('Una categoría no puede ser su propia categoría padre.', 16, 1);
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

    UPDATE [dbo].[Categoria]
    SET
        [CategoriaPadreId] = @CategoriaPadreId,
        [Nombre] = LTRIM(RTRIM(@Nombre)),
        [Descripcion] = @Descripcion,
        [Area] = @Area,
        [Orden] = @Orden,
        [ModificadoPor] = @Actor,
        [FechaModificacion] = GETDATE()
    WHERE [EmpresaId] = @EmpresaId
      AND [Id] = @CategoriaId
      AND [Estatus] = 1;

    SELECT @@ROWCOUNT AS [AffectedRows];
END;

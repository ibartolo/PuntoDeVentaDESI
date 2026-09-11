CREATE PROCEDURE [dbo].[sp_Categoria_EliminarLogico]
    @EmpresaId BIGINT,
    @CategoriaId BIGINT,
    @Actor NVARCHAR(25)
AS
BEGIN
    SET NOCOUNT ON;

    IF EXISTS
    (
        SELECT 1
        FROM [dbo].[Categoria]
        WHERE [EmpresaId] = @EmpresaId
          AND [CategoriaPadreId] = @CategoriaId
          AND [Estatus] = 1
    )
    BEGIN
        RAISERROR('No se puede eliminar una categoría con subcategorías activas.', 16, 1);
        RETURN;
    END;

    UPDATE [dbo].[Categoria]
    SET
        [Estatus] = 0,
        [ModificadoPor] = @Actor,
        [FechaModificacion] = GETDATE()
    WHERE [EmpresaId] = @EmpresaId
      AND [Id] = @CategoriaId
      AND [Estatus] = 1;

    SELECT @@ROWCOUNT AS [AffectedRows];
END;

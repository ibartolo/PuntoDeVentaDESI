CREATE PROCEDURE [dbo].[sp_Marca_Actualizar]
    @EmpresaId BIGINT,
    @MarcaId BIGINT,
    @Nombre NVARCHAR(100),
    @Descripcion NVARCHAR(MAX) = NULL,
    @Actor NVARCHAR(25)
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE [dbo].[Marca]
    SET
        [Nombre] = LTRIM(RTRIM(@Nombre)),
        [Descripcion] = @Descripcion,
        [ModificadoPor] = @Actor,
        [FechaModificacion] = GETDATE()
    WHERE [EmpresaId] = @EmpresaId
      AND [Id] = @MarcaId
      AND [Estatus] = 1;

    SELECT @@ROWCOUNT AS [AffectedRows];
END;

CREATE PROCEDURE [dbo].[sp_ProductoProveedor_EliminarLogico]
    @EmpresaId BIGINT,
    @ProductoProveedorId BIGINT,
    @Actor NVARCHAR(25)
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE [dbo].[ProductoProveedor]
    SET [Estatus] = 0,
        [ModificadoPor] = @Actor,
        [FechaModificacion] = GETDATE()
    WHERE [EmpresaId] = @EmpresaId
      AND [Id] = @ProductoProveedorId
      AND [Estatus] = 1;

    SELECT @@ROWCOUNT AS [AffectedRows];
END;

CREATE PROCEDURE [dbo].[sp_ProductoProveedor_Guardar]
    @EmpresaId BIGINT,
    @ProductoId BIGINT,
    @ProveedorId BIGINT,
    @Costo DECIMAL(18,4) = NULL,
    @Actor NVARCHAR(25)
AS
BEGIN
    SET NOCOUNT ON;

    IF EXISTS
    (
        SELECT 1 FROM [dbo].[ProductoProveedor]
        WHERE [ProductoId] = @ProductoId AND [ProveedorId] = @ProveedorId
    )
    BEGIN
        UPDATE [dbo].[ProductoProveedor]
        SET [Costo] = @Costo,
            [Estatus] = 1,
            [ModificadoPor] = @Actor,
            [FechaModificacion] = GETDATE()
        WHERE [ProductoId] = @ProductoId
          AND [ProveedorId] = @ProveedorId;

        SELECT TOP (1) [Id] FROM [dbo].[ProductoProveedor]
        WHERE [ProductoId] = @ProductoId AND [ProveedorId] = @ProveedorId;
    END
    ELSE
    BEGIN
        INSERT INTO [dbo].[ProductoProveedor]
        ([EmpresaId], [ProductoId], [ProveedorId], [Costo],
         [Estatus], [CreadoPor], [FechaCreacion], [ModificadoPor], [FechaModificacion])
        VALUES
        (@EmpresaId, @ProductoId, @ProveedorId, @Costo,
         1, @Actor, GETDATE(), NULL, NULL);

        SELECT CAST(SCOPE_IDENTITY() AS BIGINT) AS [Id];
    END
END;

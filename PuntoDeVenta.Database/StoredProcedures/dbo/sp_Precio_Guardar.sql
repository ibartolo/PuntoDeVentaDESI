CREATE PROCEDURE [dbo].[sp_Precio_Guardar]
    @EmpresaId BIGINT,
    @ProductoId BIGINT,
    @PrecioCompra DECIMAL(18,4),
    @PrecioVentaSugerido DECIMAL(18,4),
    @PrecioVenta DECIMAL(18,4),
    @Actor NVARCHAR(25)
AS
BEGIN
    SET NOCOUNT ON;

    IF NOT EXISTS (SELECT 1 FROM [dbo].[Producto] WHERE [Id] = @ProductoId AND [EmpresaId] = @EmpresaId AND [Estatus] = 1)
    BEGIN
        RAISERROR('Producto no valido para operacion de Precio.', 16, 1);
        RETURN;
    END;

    -- Solo un precio activo a la vez: se cierra el vigente.
    UPDATE [dbo].[Precio]
    SET [Activo] = 0,
        [FechaFin] = GETDATE(),
        [ModificadoPor] = @Actor,
        [FechaModificacion] = GETDATE()
    WHERE [EmpresaId] = @EmpresaId
      AND [ProductoId] = @ProductoId
      AND [Activo] = 1
      AND [Estatus] = 1;

    INSERT INTO [dbo].[Precio]
    ([EmpresaId], [ProductoId], [PrecioCompra], [PrecioVentaSugerido], [PrecioVenta], [Activo],
     [FechaInicio], [FechaFin], [Estatus], [CreadoPor], [FechaCreacion], [ModificadoPor], [FechaModificacion])
    VALUES
    (@EmpresaId, @ProductoId, @PrecioCompra, @PrecioVentaSugerido, @PrecioVenta, 1,
     GETDATE(), NULL, 1, @Actor, GETDATE(), NULL, NULL);

    SELECT CAST(SCOPE_IDENTITY() AS BIGINT) AS [Id];
END;

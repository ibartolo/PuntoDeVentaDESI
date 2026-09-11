CREATE PROCEDURE [dbo].[sp_Stock_Movimiento]
    @EmpresaId BIGINT,
    @SucursalId BIGINT,
    @ProductoId BIGINT,
    @TipoMovimiento NVARCHAR(20),
    @Cantidad DECIMAL(18,4),
    @Motivo NVARCHAR(300) = NULL,
    @ReferenciaId BIGINT = NULL,
    @Actor NVARCHAR(25)
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

    IF NOT EXISTS (SELECT 1 FROM [dbo].[Producto] WHERE [Id] = @ProductoId AND [EmpresaId] = @EmpresaId AND [Estatus] = 1)
    BEGIN
        RAISERROR('Producto no valido para movimiento de stock.', 16, 1);
        RETURN;
    END;

    DECLARE @Anterior DECIMAL(18,4) = 0;
    DECLARE @Nueva DECIMAL(18,4);

    BEGIN TRY
        BEGIN TRANSACTION;

        SELECT @Anterior = [Cantidad]
        FROM [dbo].[Stock]
        WHERE [SucursalId] = @SucursalId AND [ProductoId] = @ProductoId AND [Estatus] = 1;

        IF @Anterior IS NULL
        BEGIN
            SET @Anterior = 0;
            INSERT INTO [dbo].[Stock]
            ([EmpresaId], [SucursalId], [ProductoId], [Cantidad], [StockMinimo],
             [Estatus], [CreadoPor], [FechaCreacion], [ModificadoPor], [FechaModificacion])
            VALUES
            (@EmpresaId, @SucursalId, @ProductoId, 0, 0, 1, @Actor, GETDATE(), NULL, NULL);
        END

        SET @Nueva = @Anterior + @Cantidad;
        IF @Nueva < 0 SET @Nueva = 0;

        UPDATE [dbo].[Stock]
        SET [Cantidad] = @Nueva,
            [ModificadoPor] = @Actor,
            [FechaModificacion] = GETDATE()
        WHERE [SucursalId] = @SucursalId AND [ProductoId] = @ProductoId AND [Estatus] = 1;

        INSERT INTO [dbo].[StockMovimiento]
        ([EmpresaId], [SucursalId], [ProductoId], [TipoMovimiento], [Cantidad],
         [ExistenciaAnterior], [ExistenciaNueva], [Motivo], [ReferenciaId],
         [Estatus], [CreadoPor], [FechaCreacion], [ModificadoPor], [FechaModificacion])
        VALUES
        (@EmpresaId, @SucursalId, @ProductoId, @TipoMovimiento, @Cantidad,
         @Anterior, @Nueva, @Motivo, @ReferenciaId,
         1, @Actor, GETDATE(), NULL, NULL);

        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION;
        THROW;
    END CATCH

    SELECT @Nueva AS [ExistenciaNueva];
END;

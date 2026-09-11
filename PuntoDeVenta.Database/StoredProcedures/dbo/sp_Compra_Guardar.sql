CREATE PROCEDURE [dbo].[sp_Compra_Guardar]
    @EmpresaId BIGINT,
    @SucursalId BIGINT,
    @ProveedorId BIGINT,
    @Folio NVARCHAR(50) = NULL,
    @FechaCompra DATETIME = NULL,
    @Subtotal DECIMAL(18,4) = 0,
    @Impuesto DECIMAL(18,4) = 0,
    @Total DECIMAL(18,4) = 0,
    @Observaciones NVARCHAR(MAX) = NULL,
    @DetalleJson NVARCHAR(MAX) = NULL,
    @Actor NVARCHAR(25)
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

    IF NOT EXISTS (SELECT 1 FROM [dbo].[Proveedor] WHERE [Id] = @ProveedorId AND [EmpresaId] = @EmpresaId AND [Estatus] = 1)
    BEGIN
        RAISERROR('Proveedor no valido para operacion de Compra.', 16, 1);
        RETURN;
    END;

    IF @FechaCompra IS NULL SET @FechaCompra = GETDATE();

    DECLARE @CompraId BIGINT;

    BEGIN TRY
        BEGIN TRANSACTION;

        INSERT INTO [dbo].[Compra]
        ([EmpresaId], [SucursalId], [ProveedorId], [Folio], [FechaCompra], [Subtotal], [Impuesto], [Total], [Observaciones],
         [Estatus], [CreadoPor], [FechaCreacion], [ModificadoPor], [FechaModificacion])
        VALUES
        (@EmpresaId, @SucursalId, @ProveedorId, @Folio, @FechaCompra, @Subtotal, @Impuesto, @Total, @Observaciones,
         1, @Actor, GETDATE(), NULL, NULL);

        SET @CompraId = CAST(SCOPE_IDENTITY() AS BIGINT);

        DECLARE @Detalle TABLE
        (
            ProductoId BIGINT,
            Cantidad DECIMAL(18,4),
            CostoUnitario DECIMAL(18,4),
            Importe DECIMAL(18,4)
        );

        IF @DetalleJson IS NOT NULL AND LEN(@DetalleJson) > 2
        BEGIN
            INSERT INTO @Detalle (ProductoId, Cantidad, CostoUnitario, Importe)
            SELECT ProductoId, Cantidad, CostoUnitario, Importe
            FROM OPENJSON(@DetalleJson)
            WITH
            (
                ProductoId BIGINT '$.ProductoId',
                Cantidad DECIMAL(18,4) '$.Cantidad',
                CostoUnitario DECIMAL(18,4) '$.CostoUnitario',
                Importe DECIMAL(18,4) '$.Importe'
            );
        END

        INSERT INTO [dbo].[CompraDetalle]
        ([CompraId], [ProductoId], [Cantidad], [CostoUnitario], [Importe],
         [Estatus], [CreadoPor], [FechaCreacion], [ModificadoPor], [FechaModificacion])
        SELECT @CompraId, D.[ProductoId], D.[Cantidad], D.[CostoUnitario], D.[Importe],
               1, @Actor, GETDATE(), NULL, NULL
        FROM @Detalle D;

        -- Ingreso de stock por cada linea (crea el registro si no existe).
        UPDATE S
        SET S.[Cantidad] = S.[Cantidad] + D.[Cantidad],
            S.[ModificadoPor] = @Actor,
            S.[FechaModificacion] = GETDATE()
        FROM [dbo].[Stock] S
        INNER JOIN @Detalle D ON D.[ProductoId] = S.[ProductoId]
        WHERE S.[SucursalId] = @SucursalId
          AND S.[Estatus] = 1;

        INSERT INTO [dbo].[Stock]
        ([EmpresaId], [SucursalId], [ProductoId], [Cantidad], [StockMinimo],
         [Estatus], [CreadoPor], [FechaCreacion], [ModificadoPor], [FechaModificacion])
        SELECT @EmpresaId, @SucursalId, D.[ProductoId], D.[Cantidad], 0,
               1, @Actor, GETDATE(), NULL, NULL
        FROM @Detalle D
        WHERE NOT EXISTS
        (
            SELECT 1 FROM [dbo].[Stock] S
            WHERE S.[SucursalId] = @SucursalId AND S.[ProductoId] = D.[ProductoId] AND S.[Estatus] = 1
        );

        -- Bitacora de movimientos.
        INSERT INTO [dbo].[StockMovimiento]
        ([EmpresaId], [SucursalId], [ProductoId], [TipoMovimiento], [Cantidad],
         [ExistenciaAnterior], [ExistenciaNueva], [Motivo], [ReferenciaId],
         [Estatus], [CreadoPor], [FechaCreacion], [ModificadoPor], [FechaModificacion])
        SELECT @EmpresaId, @SucursalId, D.[ProductoId], N'Ingreso', D.[Cantidad],
               ISNULL(S.[Cantidad] - D.[Cantidad], 0), ISNULL(S.[Cantidad], D.[Cantidad]), N'Compra', @CompraId,
               1, @Actor, GETDATE(), NULL, NULL
        FROM @Detalle D
        LEFT JOIN [dbo].[Stock] S ON S.[ProductoId] = D.[ProductoId] AND S.[SucursalId] = @SucursalId;

        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION;
        THROW;
    END CATCH

    SELECT @CompraId AS [Id];
END;

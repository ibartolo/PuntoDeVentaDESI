CREATE PROCEDURE [dbo].[sp_Venta_Guardar]
    @EmpresaId BIGINT,
    @SucursalId BIGINT,
    @UsuarioId BIGINT,
    @ClienteId BIGINT = NULL,
    @CajaChicaId BIGINT = NULL,
    @Folio NVARCHAR(50) = NULL,
    @MetodoPago NVARCHAR(20),
    @Subtotal DECIMAL(18,4) = 0,
    @Impuesto DECIMAL(18,4) = 0,
    @Total DECIMAL(18,4) = 0,
    @DetalleJson NVARCHAR(MAX) = NULL,
    @Actor NVARCHAR(25)
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

    IF @MetodoPago NOT IN (N'Efectivo', N'Tarjeta', N'Transferencia')
    BEGIN
        RAISERROR('Metodo de pago no valido.', 16, 1);
        RETURN;
    END;

    -- SYNC-11: la sucursal debe pertenecer a la empresa (tenant) autenticada.
    IF NOT EXISTS
    (
        SELECT 1 FROM [dbo].[Sucursal]
        WHERE [Id] = @SucursalId AND [EmpresaId] = @EmpresaId AND [Estatus] = 1
    )
    BEGIN
        RAISERROR('Sucursal no valida para la empresa.', 16, 1);
        RETURN;
    END;

    -- SYNC-11: si se informa caja chica, debe pertenecer a la empresa.
    IF @CajaChicaId IS NOT NULL AND NOT EXISTS
    (
        SELECT 1 FROM [dbo].[CajaChica]
        WHERE [Id] = @CajaChicaId AND [EmpresaId] = @EmpresaId AND [Estatus] = 1
    )
    BEGIN
        RAISERROR('Caja chica no valida para la empresa.', 16, 1);
        RETURN;
    END;

    DECLARE @VentaId BIGINT;

    BEGIN TRY
        BEGIN TRANSACTION;

        INSERT INTO [dbo].[Venta]
        ([EmpresaId], [SucursalId], [UsuarioId], [ClienteId], [CajaChicaId], [Folio], [FechaVenta], [MetodoPago],
         [Subtotal], [Impuesto], [Total], [Estatus], [CreadoPor], [FechaCreacion], [ModificadoPor], [FechaModificacion])
        VALUES
        (@EmpresaId, @SucursalId, @UsuarioId, @ClienteId, @CajaChicaId, @Folio, GETDATE(), @MetodoPago,
         @Subtotal, @Impuesto, @Total, 1, @Actor, GETDATE(), NULL, NULL);

        SET @VentaId = CAST(SCOPE_IDENTITY() AS BIGINT);

        DECLARE @Detalle TABLE
        (
            ProductoId BIGINT,
            Cantidad DECIMAL(18,4),
            PrecioUnitario DECIMAL(18,4),
            Importe DECIMAL(18,4)
        );

        IF @DetalleJson IS NOT NULL AND LEN(@DetalleJson) > 2
        BEGIN
            INSERT INTO @Detalle (ProductoId, Cantidad, PrecioUnitario, Importe)
            SELECT ProductoId, Cantidad, PrecioUnitario, Importe
            FROM OPENJSON(@DetalleJson)
            WITH
            (
                ProductoId BIGINT '$.ProductoId',
                Cantidad DECIMAL(18,4) '$.Cantidad',
                PrecioUnitario DECIMAL(18,4) '$.PrecioUnitario',
                Importe DECIMAL(18,4) '$.Importe'
            );
        END

        INSERT INTO [dbo].[VentaDetalle]
        ([VentaId], [ProductoId], [Cantidad], [PrecioUnitario], [Importe],
         [Estatus], [CreadoPor], [FechaCreacion], [ModificadoPor], [FechaModificacion])
        SELECT @VentaId, D.[ProductoId], D.[Cantidad], D.[PrecioUnitario], D.[Importe],
               1, @Actor, GETDATE(), NULL, NULL
        FROM @Detalle D;

        -- Descuenta stock de la sucursal activa, siempre dentro de la empresa (tenant).
        UPDATE S
        SET S.[Cantidad] = CASE WHEN S.[Cantidad] - D.[Cantidad] < 0 THEN 0 ELSE S.[Cantidad] - D.[Cantidad] END,
            S.[ModificadoPor] = @Actor,
            S.[FechaModificacion] = GETDATE()
        FROM [dbo].[Stock] S
        INNER JOIN @Detalle D ON D.[ProductoId] = S.[ProductoId]
        WHERE S.[SucursalId] = @SucursalId
          AND S.[EmpresaId] = @EmpresaId
          AND S.[Estatus] = 1;

        INSERT INTO [dbo].[StockMovimiento]
        ([EmpresaId], [SucursalId], [ProductoId], [TipoMovimiento], [Cantidad],
         [ExistenciaAnterior], [ExistenciaNueva], [Motivo], [ReferenciaId],
         [Estatus], [CreadoPor], [FechaCreacion], [ModificadoPor], [FechaModificacion])
        SELECT @EmpresaId, @SucursalId, D.[ProductoId], N'Venta', -D.[Cantidad],
               ISNULL(S.[Cantidad] + D.[Cantidad], D.[Cantidad]), ISNULL(S.[Cantidad], 0), N'Venta', @VentaId,
               1, @Actor, GETDATE(), NULL, NULL
        FROM @Detalle D
        LEFT JOIN [dbo].[Stock] S ON S.[ProductoId] = D.[ProductoId]
                                 AND S.[SucursalId] = @SucursalId
                                 AND S.[EmpresaId] = @EmpresaId;

        -- Registra los ingresos en la caja chica activa (validada por empresa).
        IF @CajaChicaId IS NOT NULL
        BEGIN
            UPDATE [dbo].[CajaChica]
            SET [IngresosTotales] = [IngresosTotales] + @Total,
                [ModificadoPor] = @Actor,
                [FechaModificacion] = GETDATE()
            WHERE [Id] = @CajaChicaId
              AND [EmpresaId] = @EmpresaId
              AND [Estado] = N'Abierta'
              AND [Estatus] = 1;
        END

        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION;
        THROW;
    END CATCH

    SELECT @VentaId AS [Id];
END;

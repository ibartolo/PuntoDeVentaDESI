CREATE PROCEDURE [dbo].[sp_Cancelacion_Guardar]
    @EmpresaId BIGINT,
    @SucursalId BIGINT,
    @VentaId BIGINT,
    @Tipo NVARCHAR(10),
    @Motivo NVARCHAR(300) = NULL,
    @UsuarioCancela NVARCHAR(25) = NULL,
    @UsuarioAutoriza NVARCHAR(25) = NULL,
    @TotalReembolsado DECIMAL(18,4) = 0,
    @MetodoReembolso NVARCHAR(20) = NULL,
    @DetalleJson NVARCHAR(MAX) = NULL,
    @Actor NVARCHAR(25)
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

    IF @Tipo NOT IN (N'Total', N'Parcial')
    BEGIN
        RAISERROR('Tipo de cancelacion no valido.', 16, 1);
        RETURN;
    END;

    -- SYNC-12: la venta debe existir y pertenecer a la empresa (tenant) autenticada.
    IF NOT EXISTS
    (
        SELECT 1 FROM [dbo].[Venta]
        WHERE [Id] = @VentaId AND [EmpresaId] = @EmpresaId AND [Estatus] = 1
    )
    BEGIN
        RAISERROR('Venta no valida para la empresa.', 16, 1);
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

    DECLARE @CancelacionId BIGINT;

    DECLARE @Detalle TABLE
    (
        VentaDetalleId BIGINT,
        ProductoId BIGINT,
        Cantidad DECIMAL(18,4),
        Importe DECIMAL(18,4),
        RegresaAStock BIT
    );

    IF @DetalleJson IS NOT NULL AND LEN(@DetalleJson) > 2
    BEGIN
        INSERT INTO @Detalle (VentaDetalleId, ProductoId, Cantidad, Importe, RegresaAStock)
        SELECT VentaDetalleId, ProductoId, Cantidad, Importe, RegresaAStock
        FROM OPENJSON(@DetalleJson)
        WITH
        (
            VentaDetalleId BIGINT '$.VentaDetalleId',
            ProductoId BIGINT '$.ProductoId',
            Cantidad DECIMAL(18,4) '$.Cantidad',
            Importe DECIMAL(18,4) '$.Importe',
            RegresaAStock BIT '$.RegresaAStock'
        );
    END

    -- SYNC-12: cantidades positivas.
    IF EXISTS (SELECT 1 FROM @Detalle WHERE [Cantidad] <= 0)
    BEGIN
        RAISERROR('La cantidad a devolver debe ser mayor a cero.', 16, 1);
        RETURN;
    END;

    -- SYNC-12: cada linea devuelta debe pertenecer a la venta indicada.
    IF EXISTS
    (
        SELECT 1
        FROM @Detalle D
        WHERE D.[VentaDetalleId] > 0
          AND NOT EXISTS
          (
              SELECT 1 FROM [dbo].[VentaDetalle] VD
              WHERE VD.[Id] = D.[VentaDetalleId] AND VD.[VentaId] = @VentaId
          )
    )
    BEGIN
        RAISERROR('El detalle de la cancelacion no pertenece a la venta indicada.', 16, 1);
        RETURN;
    END;

    -- SYNC-12: no permitir lineas de detalle duplicadas dentro de la misma cancelacion.
    IF EXISTS
    (
        SELECT D.[VentaDetalleId]
        FROM @Detalle D
        WHERE D.[VentaDetalleId] > 0
        GROUP BY D.[VentaDetalleId]
        HAVING COUNT(*) > 1
    )
    BEGIN
        RAISERROR('La cancelacion contiene lineas de detalle duplicadas.', 16, 1);
        RETURN;
    END;

    -- SYNC-12: la cantidad acumulada devuelta no puede exceder la vendida.
    IF EXISTS
    (
        SELECT 1
        FROM @Detalle D
        INNER JOIN [dbo].[VentaDetalle] VD ON VD.[Id] = D.[VentaDetalleId] AND VD.[VentaId] = @VentaId
        OUTER APPLY
        (
            SELECT SUM(DD.[Cantidad]) AS [Devuelto]
            FROM [dbo].[DevolucionDetalle] DD
            INNER JOIN [dbo].[Cancelacion] C ON C.[Id] = DD.[CancelacionId]
            WHERE DD.[VentaDetalleId] = D.[VentaDetalleId]
              AND DD.[Estatus] = 1
              AND C.[Estatus] = 1
              AND C.[EmpresaId] = @EmpresaId
        ) R
        WHERE D.[VentaDetalleId] > 0
          AND D.[Cantidad] > (VD.[Cantidad] - ISNULL(R.[Devuelto], 0))
    )
    BEGIN
        RAISERROR('La cantidad a devolver excede lo vendido o ya fue cancelada.', 16, 1);
        RETURN;
    END;

    BEGIN TRY
        BEGIN TRANSACTION;

        INSERT INTO [dbo].[Cancelacion]
        ([EmpresaId], [SucursalId], [VentaId], [Tipo], [Motivo], [UsuarioCancela], [UsuarioAutoriza],
         [FechaCancelacion], [TotalReembolsado], [MetodoReembolso],
         [Estatus], [CreadoPor], [FechaCreacion], [ModificadoPor], [FechaModificacion])
        VALUES
        (@EmpresaId, @SucursalId, @VentaId, @Tipo, @Motivo, @UsuarioCancela, @UsuarioAutoriza,
         GETDATE(), @TotalReembolsado, @MetodoReembolso,
         1, @Actor, GETDATE(), NULL, NULL);

        SET @CancelacionId = CAST(SCOPE_IDENTITY() AS BIGINT);

        INSERT INTO [dbo].[DevolucionDetalle]
        ([CancelacionId], [VentaDetalleId], [ProductoId], [Cantidad], [Importe], [RegresaAStock],
         [Estatus], [CreadoPor], [FechaCreacion], [ModificadoPor], [FechaModificacion])
        SELECT @CancelacionId, D.[VentaDetalleId], D.[ProductoId], D.[Cantidad], D.[Importe], D.[RegresaAStock],
               1, @Actor, GETDATE(), NULL, NULL
        FROM @Detalle D;

        -- Devolucion a stock solo de los productos marcados, dentro de la empresa (tenant).
        UPDATE S
        SET S.[Cantidad] = S.[Cantidad] + D.[Cantidad],
            S.[ModificadoPor] = @Actor,
            S.[FechaModificacion] = GETDATE()
        FROM [dbo].[Stock] S
        INNER JOIN @Detalle D ON D.[ProductoId] = S.[ProductoId]
        WHERE S.[SucursalId] = @SucursalId
          AND S.[EmpresaId] = @EmpresaId
          AND S.[Estatus] = 1
          AND D.[RegresaAStock] = 1;

        INSERT INTO [dbo].[StockMovimiento]
        ([EmpresaId], [SucursalId], [ProductoId], [TipoMovimiento], [Cantidad],
         [ExistenciaAnterior], [ExistenciaNueva], [Motivo], [ReferenciaId],
         [Estatus], [CreadoPor], [FechaCreacion], [ModificadoPor], [FechaModificacion])
        SELECT @EmpresaId, @SucursalId, D.[ProductoId], N'Devolucion', D.[Cantidad],
               ISNULL(S.[Cantidad] - D.[Cantidad], 0), ISNULL(S.[Cantidad], D.[Cantidad]), N'Devolucion', @CancelacionId,
               1, @Actor, GETDATE(), NULL, NULL
        FROM @Detalle D
        LEFT JOIN [dbo].[Stock] S ON S.[ProductoId] = D.[ProductoId]
                                 AND S.[SucursalId] = @SucursalId
                                 AND S.[EmpresaId] = @EmpresaId
        WHERE D.[RegresaAStock] = 1;

        -- Reembolso en efectivo (justificado) se registra como salida de caja si aplica.
        IF @MetodoReembolso = N'Efectivo' AND @TotalReembolsado > 0
        BEGIN
            DECLARE @CajaChicaId BIGINT;

            SELECT TOP (1) @CajaChicaId = [Id]
            FROM [dbo].[CajaChica]
            WHERE [EmpresaId] = @EmpresaId AND [SucursalId] = @SucursalId AND [Estado] = N'Abierta' AND [Estatus] = 1
            ORDER BY [FechaApertura] DESC;

            IF @CajaChicaId IS NOT NULL
            BEGIN
                INSERT INTO [dbo].[SalidaCaja]
                ([EmpresaId], [CajaChicaId], [Monto], [Comentario], [FechaHora], [Justificada], [EvidenciaUrl], [TipoSalida],
                 [Estatus], [CreadoPor], [FechaCreacion], [ModificadoPor], [FechaModificacion])
                VALUES
                (@EmpresaId, @CajaChicaId, @TotalReembolsado, N'Reembolso por cancelacion/devolucion', GETDATE(), 1, NULL, N'Reembolso',
                 1, @Actor, GETDATE(), NULL, NULL);

                UPDATE [dbo].[CajaChica]
                SET [SalidasTotales] = [SalidasTotales] + @TotalReembolsado,
                    [ModificadoPor] = @Actor,
                    [FechaModificacion] = GETDATE()
                WHERE [Id] = @CajaChicaId
                  AND [EmpresaId] = @EmpresaId;
            END
        END

        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION;
        THROW;
    END CATCH

    SELECT @CancelacionId AS [Id];
END;

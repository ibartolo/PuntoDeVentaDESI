CREATE PROCEDURE [dbo].[sp_Pagina_Guardar]
    @PaginaId BIGINT,
    @Nombre NVARCHAR(100),
    @NombreVisible NVARCHAR(150) = NULL,
    @Descripcion NVARCHAR(250) = NULL,
    @Tipo NVARCHAR(20) = N'Menu',
    @Direccion NVARCHAR(250) = NULL,
    @PermisosPadreId BIGINT = NULL,
    @Logo NVARCHAR(100) = NULL,
    @OrdenB INT = 0,
    @Actor NVARCHAR(25)
AS
BEGIN
    SET NOCOUNT ON;

    SET @Nombre = LTRIM(RTRIM(@Nombre));
    IF @Nombre IS NULL OR LEN(@Nombre) = 0
    BEGIN
        RAISERROR('El Nombre de la Página es requerido.', 16, 1);
        RETURN;
    END;

    IF @PermisosPadreId IS NOT NULL
       AND NOT EXISTS (SELECT 1 FROM [dbo].[Pagina] WHERE [Id] = @PermisosPadreId AND [Estatus] = 1)
    BEGIN
        RAISERROR('La Página padre no es válida.', 16, 1);
        RETURN;
    END;

    IF @PaginaId = 0
    BEGIN
        IF EXISTS
        (
            SELECT 1
            FROM [dbo].[Pagina]
            WHERE [Nombre] = @Nombre
              AND [Estatus] = 1
        )
        BEGIN
            SELECT CAST(-1 AS BIGINT) AS [Id];
            RETURN;
        END;

        INSERT INTO [dbo].[Pagina]
        (
            [Nombre], [NombreVisible], [Descripcion], [Tipo], [Direccion],
            [PermisosPadreId], [Logo], [OrdenB], [Estatus],
            [CreadoPor], [FechaCreacion], [ModificadoPor], [FechaModificacion]
        )
        VALUES
        (
            @Nombre, @NombreVisible, @Descripcion, @Tipo, @Direccion,
            @PermisosPadreId, @Logo, @OrdenB, 1,
            @Actor, GETDATE(), NULL, NULL
        );

        SELECT CAST(SCOPE_IDENTITY() AS BIGINT) AS [Id];
    END
    ELSE
    BEGIN
        UPDATE [dbo].[Pagina]
        SET
            [Nombre] = @Nombre,
            [NombreVisible] = @NombreVisible,
            [Descripcion] = @Descripcion,
            [Tipo] = @Tipo,
            [Direccion] = @Direccion,
            [PermisosPadreId] = @PermisosPadreId,
            [Logo] = @Logo,
            [OrdenB] = @OrdenB,
            [ModificadoPor] = @Actor,
            [FechaModificacion] = GETDATE()
        WHERE [Id] = @PaginaId
          AND [Estatus] = 1;

        SELECT CAST(CASE WHEN @@ROWCOUNT > 0 THEN @PaginaId ELSE 0 END AS BIGINT) AS [Id];
    END;
END;

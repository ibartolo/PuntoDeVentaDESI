CREATE PROCEDURE [dbo].[sp_Sucursal_Guardar]
    @EmpresaId BIGINT,
    @SucursalId BIGINT = 0,
    @Nombre NVARCHAR(100),
    @Descripcion NVARCHAR(MAX) = NULL,
    @Calle NVARCHAR(150) = NULL,
    @Ciudad NVARCHAR(100) = NULL,
    @Colonia NVARCHAR(100) = NULL,
    @CodigoPostal NVARCHAR(10) = NULL,
    @Actor NVARCHAR(25)
AS
BEGIN
    SET NOCOUNT ON;

    IF NOT EXISTS (SELECT 1 FROM [dbo].[Empresa] WHERE [Id] = @EmpresaId AND [Estatus] = 1)
    BEGIN
        RAISERROR('Empresa no valida para operacion de Sucursal.', 16, 1);
        RETURN;
    END;

    IF @SucursalId IS NULL OR @SucursalId = 0
    BEGIN
        INSERT INTO [dbo].[Sucursal]
        ([EmpresaId], [Nombre], [Descripcion], [Calle], [Ciudad], [Colonia], [CodigoPostal],
         [Estatus], [CreadoPor], [FechaCreacion], [ModificadoPor], [FechaModificacion])
        VALUES
        (@EmpresaId, LTRIM(RTRIM(@Nombre)), @Descripcion, @Calle, @Ciudad, @Colonia, @CodigoPostal,
         1, @Actor, GETDATE(), NULL, NULL);

        SELECT CAST(SCOPE_IDENTITY() AS BIGINT) AS [Id];
    END
    ELSE
    BEGIN
        UPDATE [dbo].[Sucursal]
        SET [Nombre] = LTRIM(RTRIM(@Nombre)),
            [Descripcion] = @Descripcion,
            [Calle] = @Calle,
            [Ciudad] = @Ciudad,
            [Colonia] = @Colonia,
            [CodigoPostal] = @CodigoPostal,
            [ModificadoPor] = @Actor,
            [FechaModificacion] = GETDATE()
        WHERE [EmpresaId] = @EmpresaId
          AND [Id] = @SucursalId
          AND [Estatus] = 1;

        SELECT @SucursalId AS [Id];
    END
END;

CREATE PROCEDURE [dbo].[sp_Usuario_Guardar]
    @Id BIGINT,
    @EmpresaId BIGINT,
    @NombreUsuario NVARCHAR(25),
    @ContrasenaHash NVARCHAR(256) = NULL,
    @ContrasenaSalt NVARCHAR(256) = NULL,
    @ContrasenaIteraciones INT = NULL,
    @Correo NVARCHAR(250) = NULL,
    @Telefono NVARCHAR(50) = NULL,
    @ImagenPerfil NVARCHAR(500) = NULL,
    @Actor NVARCHAR(25)
AS
BEGIN
    SET NOCOUNT ON;

    SET @NombreUsuario = LTRIM(RTRIM(@NombreUsuario));

    IF EXISTS
    (
        SELECT 1
        FROM [dbo].[Usuario]
        WHERE [NombreUsuario] = @NombreUsuario
          AND [Id] <> ISNULL(@Id, 0)
    )
    BEGIN
        SELECT CAST(-1 AS BIGINT);
        RETURN;
    END;

    IF @Id IS NULL OR @Id = 0
    BEGIN
        IF @ContrasenaHash IS NULL OR @ContrasenaSalt IS NULL OR @ContrasenaIteraciones IS NULL
        BEGIN
            SELECT CAST(0 AS BIGINT);
            RETURN;
        END;

        INSERT INTO [dbo].[Usuario]
        (
            [EmpresaId], [NombreUsuario], [ContrasenaHash], [ContrasenaSalt], [ContrasenaIteraciones],
            [Correo], [Telefono], [ImagenPerfil], [Estatus],
            [CreadoPor], [FechaCreacion], [ModificadoPor], [FechaModificacion]
        )
        VALUES
        (
            @EmpresaId, @NombreUsuario, @ContrasenaHash, @ContrasenaSalt, @ContrasenaIteraciones,
            @Correo, @Telefono, @ImagenPerfil, 1,
            @Actor, GETDATE(), NULL, NULL
        );

        SELECT CAST(SCOPE_IDENTITY() AS BIGINT);
    END
    ELSE
    BEGIN
        UPDATE [dbo].[Usuario]
        SET
            [NombreUsuario] = @NombreUsuario,
            [Correo] = @Correo,
            [Telefono] = @Telefono,
            [ImagenPerfil] = @ImagenPerfil,
            [ContrasenaHash] = CASE WHEN @ContrasenaHash IS NULL OR @ContrasenaHash = N'' THEN [ContrasenaHash] ELSE @ContrasenaHash END,
            [ContrasenaSalt] = CASE WHEN @ContrasenaSalt IS NULL OR @ContrasenaSalt = N'' THEN [ContrasenaSalt] ELSE @ContrasenaSalt END,
            [ContrasenaIteraciones] = CASE WHEN @ContrasenaIteraciones IS NULL OR @ContrasenaIteraciones = 0 THEN [ContrasenaIteraciones] ELSE @ContrasenaIteraciones END,
            [ModificadoPor] = @Actor,
            [FechaModificacion] = GETDATE()
        WHERE [Id] = @Id
          AND [EmpresaId] = @EmpresaId
          AND [Estatus] = 1;

        SELECT CAST(@Id AS BIGINT);
    END;
END;

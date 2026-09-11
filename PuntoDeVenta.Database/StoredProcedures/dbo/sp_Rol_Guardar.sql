CREATE PROCEDURE [dbo].[sp_Rol_Guardar]
    @Id BIGINT,
    @EmpresaId BIGINT,
    @Nombre NVARCHAR(50),
    @Descripcion NVARCHAR(250) = NULL,
    @PuedeAutorizar BIT = 0,
    @Actor NVARCHAR(25)
AS
BEGIN
    SET NOCOUNT ON;

    SET @Nombre = LTRIM(RTRIM(@Nombre));

    IF NOT EXISTS (SELECT 1 FROM [dbo].[Empresa] WHERE [Id] = @EmpresaId AND [Estatus] = 1)
    BEGIN
        SELECT CAST(0 AS BIGINT);
        RETURN;
    END;

    IF EXISTS
    (
        SELECT 1
        FROM [dbo].[Rol]
        WHERE [EmpresaId] = @EmpresaId
          AND [Nombre] = @Nombre
          AND [Id] <> ISNULL(@Id, 0)
          AND [Estatus] = 1
    )
    BEGIN
        SELECT CAST(-1 AS BIGINT);
        RETURN;
    END;

    IF @Id IS NULL OR @Id = 0
    BEGIN
        INSERT INTO [dbo].[Rol]
        (
            [EmpresaId], [Nombre], [Descripcion], [PuedeAutorizar], [Estatus],
            [CreadoPor], [FechaCreacion], [ModificadoPor], [FechaModificacion]
        )
        VALUES
        (
            @EmpresaId, @Nombre, @Descripcion, @PuedeAutorizar, 1,
            @Actor, GETDATE(), NULL, NULL
        );

        SELECT CAST(SCOPE_IDENTITY() AS BIGINT);
    END
    ELSE
    BEGIN
        UPDATE [dbo].[Rol]
        SET
            [Nombre] = @Nombre,
            [Descripcion] = @Descripcion,
            [PuedeAutorizar] = @PuedeAutorizar,
            [ModificadoPor] = @Actor,
            [FechaModificacion] = GETDATE()
        WHERE [Id] = @Id
          AND [EmpresaId] = @EmpresaId
          AND [Estatus] = 1;

        SELECT CAST(@Id AS BIGINT);
    END;
END;

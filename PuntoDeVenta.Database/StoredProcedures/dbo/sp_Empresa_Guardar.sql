CREATE PROCEDURE [dbo].[sp_Empresa_Guardar]
    @NombreComercial NVARCHAR(250),
    @RazonSocial NVARCHAR(250),
    @RFC NVARCHAR(50),
    @Responsable NVARCHAR(250),
    @Direccion NVARCHAR(500),
    @Ciudad NVARCHAR(100) = NULL,
    @Estado NVARCHAR(100) = NULL,
    @CodigoPostal NVARCHAR(10) = NULL,
    @Telefono NVARCHAR(50) = NULL,
    @CorreoContacto NVARCHAR(250),
    @Actor NVARCHAR(25)
AS
BEGIN
    SET NOCOUNT ON;

    IF @NombreComercial IS NULL OR LTRIM(RTRIM(@NombreComercial)) = ''
    BEGIN
        RAISERROR('El Nombre Comercial es requerido.', 16, 1);
        RETURN;
    END;

    IF @RazonSocial IS NULL OR LTRIM(RTRIM(@RazonSocial)) = ''
    BEGIN
        RAISERROR('La Razón Social es requerida.', 16, 1);
        RETURN;
    END;

    IF @RFC IS NULL OR LTRIM(RTRIM(@RFC)) = ''
    BEGIN
        RAISERROR('El RFC es requerido.', 16, 1);
        RETURN;
    END;

    IF @Responsable IS NULL OR LTRIM(RTRIM(@Responsable)) = ''
    BEGIN
        RAISERROR('El Responsable es requerido.', 16, 1);
        RETURN;
    END;

    IF @Direccion IS NULL OR LTRIM(RTRIM(@Direccion)) = ''
    BEGIN
        RAISERROR('La Dirección es requerida.', 16, 1);
        RETURN;
    END;

    IF @CorreoContacto IS NULL OR LTRIM(RTRIM(@CorreoContacto)) = ''
    BEGIN
        RAISERROR('El Correo de Contacto es requerido.', 16, 1);
        RETURN;
    END;

    IF EXISTS
    (
        SELECT 1
        FROM [dbo].[Empresa]
        WHERE [RFC] = LTRIM(RTRIM(@RFC))
          AND [Estatus] = 1
    )
    BEGIN
        RAISERROR('Ya existe una empresa activa con el mismo RFC.', 16, 1);
        RETURN;
    END;

    INSERT INTO [dbo].[Empresa]
    (
        [NombreComercial], [RazonSocial], [RFC], [Responsable], [Direccion],
        [Ciudad], [Estado], [CodigoPostal], [Telefono], [CorreoContacto],
        [FechaVigenciaInicio], [FechaVigenciaFin], [EsPeriodoPrueba], [Estatus],
        [CreadoPor], [FechaCreacion], [ModificadoPor], [FechaModificacion], [LogoUrl]
    )
    VALUES
    (
        LTRIM(RTRIM(@NombreComercial)), LTRIM(RTRIM(@RazonSocial)), LTRIM(RTRIM(@RFC)),
        LTRIM(RTRIM(@Responsable)), LTRIM(RTRIM(@Direccion)),
        @Ciudad, @Estado, @CodigoPostal, @Telefono, LTRIM(RTRIM(@CorreoContacto)),
        GETDATE(), DATEADD(DAY, 30, GETDATE()), 1, 1,
        @Actor, GETDATE(), NULL, NULL, NULL
    );

    SELECT CAST(SCOPE_IDENTITY() AS BIGINT) AS [Id];
END;

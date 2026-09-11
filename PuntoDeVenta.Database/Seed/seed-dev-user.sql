/*
  SOLO DESARROLLO.
  No usar en ambientes productivos.
  Credenciales reales y secretos productivos NO se versionan.
*/

DECLARE @Actor NVARCHAR(25) = N'seed-dev';
DECLARE @Ahora DATETIME = GETDATE();
DECLARE @EmpresaId BIGINT;

SELECT TOP (1) @EmpresaId = [Id]
FROM [dbo].[Empresa]
WHERE [RFC] = N'XAXX010101000';

IF @EmpresaId IS NULL
BEGIN
    INSERT INTO [dbo].[Empresa]
    (
        [NombreComercial],
        [RazonSocial],
        [RFC],
        [Responsable],
        [Direccion],
        [Ciudad],
        [Estado],
        [CodigoPostal],
        [Telefono],
        [CorreoContacto],
        [FechaVigenciaInicio],
        [FechaVigenciaFin],
        [EsPeriodoPrueba],
        [Estatus],
        [CreadoPor],
        [FechaCreacion],
        [ModificadoPor],
        [FechaModificacion],
        [LogoUrl]
    )
    VALUES
    (
        N'Empresa Demo Desarrollo',
        N'Empresa Demo Desarrollo SA de CV',
        N'XAXX010101000',
        N'Administrador Demo',
        N'Avenida Desarrollo 100',
        N'CDMX',
        N'Ciudad de México',
        N'01000',
        N'5550000000',
        N'dev@local.invalid',
        DATEADD(DAY, -1, @Ahora),
        DATEADD(YEAR, 1, @Ahora),
        1,
        1,
        @Actor,
        @Ahora,
        NULL,
        NULL,
        NULL
    );

    SET @EmpresaId = SCOPE_IDENTITY();
END;

IF NOT EXISTS (SELECT 1 FROM [dbo].[Usuario] WHERE [NombreUsuario] = N'dev-admin')
BEGIN
    INSERT INTO [dbo].[Usuario]
    (
        [EmpresaId],
        [NombreUsuario],
        [ContrasenaHash],
        [ContrasenaSalt],
        [ContrasenaIteraciones],
        [Estatus],
        [CreadoPor],
        [FechaCreacion],
        [ModificadoPor],
        [FechaModificacion]
    )
    VALUES
    (
        @EmpresaId,
        N'dev-admin',
        N'43EUnE9xx0vTc1+NcbTcIeawakj6mqqsJWxyOGmV78U=',
        N'qbOYMRQm/0ToMv1SbqI3Eg==',
        150000,
        1,
        @Actor,
        @Ahora,
        NULL,
        NULL
    );
END;

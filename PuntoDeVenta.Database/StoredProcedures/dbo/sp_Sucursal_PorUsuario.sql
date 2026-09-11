CREATE PROCEDURE [dbo].[sp_Sucursal_PorUsuario]
    @EmpresaId BIGINT,
    @UsuarioId BIGINT
AS
BEGIN
    SET NOCOUNT ON;

    -- Sucursales asignadas explicitamente al usuario.
    SELECT
        S.[Id], S.[EmpresaId], S.[Nombre], S.[Descripcion], S.[Calle], S.[Ciudad], S.[Colonia], S.[CodigoPostal],
        S.[Estatus], S.[CreadoPor], S.[FechaCreacion], S.[ModificadoPor], S.[FechaModificacion]
    FROM [dbo].[Sucursal] S
    INNER JOIN [dbo].[UsuarioSucursal] US ON US.[SucursalId] = S.[Id] AND US.[Estatus] = 1
    WHERE S.[EmpresaId] = @EmpresaId
      AND S.[Estatus] = 1
      AND US.[UsuarioId] = @UsuarioId
    ORDER BY S.[Nombre] ASC;

    -- Si no tiene asignaciones, se devuelven todas las de la empresa (compatibilidad).
    IF @@ROWCOUNT = 0
    BEGIN
        SELECT
            [Id], [EmpresaId], [Nombre], [Descripcion], [Calle], [Ciudad], [Colonia], [CodigoPostal],
            [Estatus], [CreadoPor], [FechaCreacion], [ModificadoPor], [FechaModificacion]
        FROM [dbo].[Sucursal]
        WHERE [EmpresaId] = @EmpresaId
          AND [Estatus] = 1
        ORDER BY [Nombre] ASC;
    END
END;

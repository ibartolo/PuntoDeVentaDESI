CREATE PROCEDURE [dbo].[sp_UsuarioRol_EliminarLogico]
    @EmpresaId BIGINT,
    @UsuarioRolId BIGINT,
    @Actor NVARCHAR(25)
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE UR
    SET
        UR.[Estatus] = 0,
        UR.[ModificadoPor] = @Actor,
        UR.[FechaModificacion] = GETDATE()
    FROM [dbo].[UsuarioRol] UR
    INNER JOIN [dbo].[Rol] R
        ON UR.[RolId] = R.[Id]
       AND R.[EmpresaId] = @EmpresaId
    WHERE UR.[Id] = @UsuarioRolId
      AND UR.[Estatus] = 1;

    SELECT @@ROWCOUNT AS [AffectedRows];
END;

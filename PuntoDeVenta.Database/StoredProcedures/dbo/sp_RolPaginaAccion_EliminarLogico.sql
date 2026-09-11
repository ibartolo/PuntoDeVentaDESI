CREATE PROCEDURE [dbo].[sp_RolPaginaAccion_EliminarLogico]
    @EmpresaId BIGINT,
    @Id BIGINT,
    @Actor NVARCHAR(25)
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE RPA
    SET
        RPA.[Estatus] = 0,
        RPA.[ModificadoPor] = @Actor,
        RPA.[FechaModificacion] = GETDATE()
    FROM [dbo].[RolPaginaAccion] RPA
    INNER JOIN [dbo].[Rol] R
        ON RPA.[RolId] = R.[Id]
       AND R.[EmpresaId] = @EmpresaId
    WHERE RPA.[Id] = @Id
      AND RPA.[Estatus] = 1;

    SELECT @@ROWCOUNT AS [AffectedRows];
END;

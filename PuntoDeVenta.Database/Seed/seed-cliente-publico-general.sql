/*
  SOLO DESARROLLO.
  No usar en ambientes productivos.
  Precarga el cliente especial "Público General" por empresa (idempotente).
  No contiene credenciales ni secretos productivos.
*/

SET NOCOUNT ON;

DECLARE @Actor NVARCHAR(25) = N'seed-cliente';
DECLARE @Ahora DATETIME = GETDATE();

INSERT INTO [dbo].[Cliente]
(
    [EmpresaId], [Nombre], [Correo], [Telefono], [Direccion], [EsPublicoGeneral], [Estatus],
    [CreadoPor], [FechaCreacion], [ModificadoPor], [FechaModificacion]
)
SELECT
    E.[Id],
    N'Público General',
    NULL,
    NULL,
    NULL,
    1,
    1,
    @Actor,
    @Ahora,
    NULL,
    NULL
FROM [dbo].[Empresa] E
WHERE E.[Estatus] = 1
  AND NOT EXISTS
  (
      SELECT 1
      FROM [dbo].[Cliente] C
      WHERE C.[EmpresaId] = E.[Id]
        AND C.[Nombre] = N'Público General'
        AND C.[Estatus] = 1
  );

-- Seed de modulos POS (idempotente).
-- Crea el cliente especial "Publico General" por empresa activa si no existe.

IF NOT EXISTS
(
    SELECT 1
    FROM [dbo].[Cliente] C
    INNER JOIN [dbo].[Empresa] E ON E.[Id] = C.[EmpresaId]
    WHERE C.[EsPublicoGeneral] = 1
      AND C.[Estatus] = 1
      AND E.[Estatus] = 1
)
BEGIN
    INSERT INTO [dbo].[Cliente]
    ([EmpresaId], [Nombre], [Correo], [Telefono], [Direccion], [EsPublicoGeneral],
     [Estatus], [CreadoPor], [FechaCreacion], [ModificadoPor], [FechaModificacion])
    SELECT
        E.[Id], N'Publico General', NULL, NULL, NULL, 1,
        1, N'seed', GETDATE(), NULL, NULL
    FROM [dbo].[Empresa] E
    WHERE E.[Estatus] = 1
      AND NOT EXISTS
      (
          SELECT 1 FROM [dbo].[Cliente] C
          WHERE C.[EmpresaId] = E.[Id] AND C.[EsPublicoGeneral] = 1
      );
END;

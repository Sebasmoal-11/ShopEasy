-- =====================================================
-- ShopEasy CRM - Datos iniciales de catalogos y administrador
-- Compatible con el modelo corregido de la base de datos shopeasy.
--
-- Este script NO crea, modifica ni elimina tablas.
-- Inserta los registros minimos para que el sistema pueda operar.
-- Puede ejecutarse varias veces sin duplicar los registros indicados.
-- =====================================================

USE `shopeasy`;

-- =====================================================
-- 1. CATALOGOS DE ACTIVOS
-- La tabla Activo utiliza idActivo como clave primaria manual.
-- =====================================================

INSERT IGNORE INTO `shopeasy`.`Activo`
    (`idActivo`, `descripcionActivo`, `Activo`)
VALUES
    (1, 'Activo', 1),
    (2, 'Inactivo', 0);

-- =====================================================
-- 2. ROLES BASE
-- =====================================================

INSERT IGNORE INTO `shopeasy`.`roles`
    (`nombre`, `descripcion`)
VALUES
    ('Administrador', 'Acceso total al sistema y gestion de usuarios y roles'),
    ('Empleado', 'Acceso operativo al CRM sin privilegios de administracion');

-- =====================================================
-- 3. USUARIO DE PRUEBA: ADMINISTRADOR
-- Cedula: 123456789
-- Correo: admin@shopeasy.com
-- Contrasena prevista: Admin-2026
--
-- IMPORTANTE:
-- Se conserva el hash proporcionado en el script original.
-- El usuario y el correo solo se insertan si no existen.
-- =====================================================

INSERT IGNORE INTO `shopeasy`.`personas`
    (`cedula`, `nombre`, `apellidos`)
VALUES
    ('123456789', 'Administrador', 'De Prueba');

INSERT IGNORE INTO `shopeasy`.`correos_persona`
    (`cedula`, `correo`, `tipo_correo`, `principal`, `Activo_idActivo`)
VALUES
    ('123456789', 'admin@shopeasy.com', 'Principal', 1, 1);

INSERT INTO `shopeasy`.`usuarios`
    (`cedula`, `id_rol`, `contrasena_hash`, `Activo_idActivo`)
SELECT
    '123456789',
    r.`id_rol`,
    '$2b$12$oYKeShR6vdlsSlAIh6ik6e5cli0CZZSXRAnwlJYEalrkziZLanXp2',
    1
FROM `shopeasy`.`roles` AS r
WHERE r.`nombre` = 'Administrador'
  AND NOT EXISTS (
      SELECT 1
      FROM `shopeasy`.`usuarios` AS u
      WHERE u.`cedula` = '123456789'
  );

-- =====================================================
-- 4. PREFERENCIAS DE PUBLICIDAD
-- El id 2 corresponde a "No acepta publicidad", segun el
-- orden de insercion inicial del catalogo.
-- =====================================================

INSERT IGNORE INTO `shopeasy`.`preferencias_publicidad`
    (`nombre`, `descripcion`)
VALUES
    ('Acepta publicidad', 'El cliente acepta recibir comunicaciones comerciales.'),
    ('No acepta publicidad', 'El cliente no desea recibir comunicaciones comerciales.'),
    ('Solo ofertas relevantes', 'El cliente acepta comunicaciones segmentadas.');

-- =====================================================
-- 5. BODEGAS
-- =====================================================

INSERT IGNORE INTO `shopeasy`.`bodegas`
    (`nombre`, `direccion`, `descripcion`, `Activo_idActivo`)
VALUES
    ('Bodega Central', 'Direccion pendiente de configurar',
     'Bodega principal del inventario.', 1);

-- =====================================================
-- 6. ESTADOS DE PEDIDO
-- =====================================================

INSERT IGNORE INTO `shopeasy`.`estados_pedido`
    (`nombre`, `descripcion`)
VALUES
    ('Pendiente', 'Pedido registrado y pendiente de preparacion.'),
    ('En proceso', 'Pedido en preparacion.'),
    ('Enviado', 'Pedido entregado al transportista.'),
    ('Entregado', 'Pedido entregado al cliente.'),
    ('Cancelado', 'Pedido cancelado.');

-- =====================================================
-- 7. TIPOS DE CASO DE ATENCION AL CLIENTE
-- =====================================================

INSERT IGNORE INTO `shopeasy`.`tipos_caso`
    (`nombre`, `descripcion`)
VALUES
    ('Queja', 'Inconformidad del cliente con el servicio o producto.'),
    ('Solicitud', 'Solicitud de informacion o de una gestion.'),
    ('Reclamo', 'Reclamo formal sobre un pedido o producto.'),
    ('Consulta', 'Consulta general del cliente.'),
    ('Sugerencia', 'Sugerencia de mejora del cliente.');

-- =====================================================
-- 8. ESTADOS DE CASO
-- =====================================================

INSERT IGNORE INTO `shopeasy`.`estados_caso`
    (`nombre`, `descripcion`)
VALUES
    ('Pendiente', 'Caso registrado sin atender.'),
    ('En proceso', 'Caso en atencion por un usuario.'),
    ('Resuelto', 'Caso atendido y resuelto.'),
    ('Cerrado', 'Caso cerrado definitivamente.');

-- =====================================================
-- 9. CANALES DE MARKETING
-- =====================================================

INSERT IGNORE INTO `shopeasy`.`canales_marketing`
    (`nombre`, `descripcion`, `Activo_idActivo`)
VALUES
    ('Correo electronico', 'Envio de campanas por correo electronico.', 1),
    ('Redes sociales', 'Publicaciones en redes sociales.', 1),
    ('SMS', 'Mensajes de texto.', 1),
    ('WhatsApp', 'Mensajes por WhatsApp.', 1);

-- =====================================================
-- FIN DE DATOS INICIALES
-- =====================================================

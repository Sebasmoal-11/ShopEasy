-- =====================================================
-- ShopEasy CRM 
-- =====================================================

SET @OLD_UNIQUE_CHECKS=@@UNIQUE_CHECKS, UNIQUE_CHECKS=0;
SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0;
SET @OLD_SQL_MODE=@@SQL_MODE, SQL_MODE='ONLY_FULL_GROUP_BY,STRICT_TRANS_TABLES,NO_ZERO_IN_DATE,NO_ZERO_DATE,ERROR_FOR_DIVISION_BY_ZERO,NO_ENGINE_SUBSTITUTION';

CREATE SCHEMA IF NOT EXISTS `ShopEasy` DEFAULT CHARACTER SET utf8mb4 ;
USE `ShopEasy` ;

-- -----------------------------------------------------
-- Table `ShopEasy`.`personas`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `ShopEasy`.`personas` (
  `cedula` VARCHAR(30) NOT NULL,
  `nombre` VARCHAR(100) NOT NULL,
  `apellidos` VARCHAR(100) NOT NULL,
  `fecha_creacion` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (`cedula`))
ENGINE = InnoDB;


-- -----------------------------------------------------
-- Table `ShopEasy`.`correos_persona`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `ShopEasy`.`correos_persona` (
  `id_correo` INT NOT NULL AUTO_INCREMENT,
  `cedula` VARCHAR(30) NOT NULL,
  `correo` VARCHAR(150) NOT NULL,
  `tipo_correo` VARCHAR(30) NOT NULL,
  `principal` TINYINT(1) NOT NULL DEFAULT 0,
  `activo` TINYINT(1) NOT NULL DEFAULT 1,
  PRIMARY KEY (`id_correo`),
  UNIQUE INDEX `uq_correos_correo` (`correo` ASC),
  INDEX `idx_correos_persona` (`cedula` ASC),
  CONSTRAINT `fk_correos_persona_personas`
    FOREIGN KEY (`cedula`)
    REFERENCES `ShopEasy`.`personas` (`cedula`)
    ON DELETE RESTRICT
    ON UPDATE CASCADE)
ENGINE = InnoDB;


-- -----------------------------------------------------
-- Table `ShopEasy`.`telefonos_persona`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `ShopEasy`.`telefonos_persona` (
  `id_telefono` INT NOT NULL AUTO_INCREMENT,
  `cedula` VARCHAR(30) NOT NULL,
  `telefono` VARCHAR(25) NOT NULL,
  `tipo_telefono` VARCHAR(30) NOT NULL,
  `principal` TINYINT(1) NOT NULL DEFAULT 0,
  `activo` TINYINT(1) NOT NULL DEFAULT 1,
  PRIMARY KEY (`id_telefono`),
  INDEX `idx_telefonos_persona` (`cedula` ASC),
  CONSTRAINT `fk_telefonos_persona_personas`
    FOREIGN KEY (`cedula`)
    REFERENCES `ShopEasy`.`personas` (`cedula`)
    ON DELETE RESTRICT
    ON UPDATE CASCADE)
ENGINE = InnoDB;


-- -----------------------------------------------------
-- Table `ShopEasy`.`roles`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `ShopEasy`.`roles` (
  `id_rol` INT NOT NULL AUTO_INCREMENT,
  `nombre` VARCHAR(50) NOT NULL,
  `descripcion` VARCHAR(255) NULL DEFAULT NULL,
  PRIMARY KEY (`id_rol`),
  UNIQUE INDEX `uq_roles_nombre` (`nombre` ASC)
) ENGINE = InnoDB;


-- -----------------------------------------------------
-- Table `ShopEasy`.`usuarios`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `ShopEasy`.`usuarios` (
  `id_usuario` INT NOT NULL AUTO_INCREMENT,
  `cedula` VARCHAR(30) NOT NULL,
  `id_rol` INT NOT NULL,
  `contrasena_hash` VARCHAR(255) NOT NULL,
  `activo` TINYINT(1) NOT NULL DEFAULT 1,
  `fecha_creacion` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (`id_usuario`),
  UNIQUE INDEX `uq_usuarios_cedula` (`cedula` ASC),
  INDEX `idx_usuarios_rol` (`id_rol` ASC),
  CONSTRAINT `fk_usuarios_personas`
    FOREIGN KEY (`cedula`)
    REFERENCES `ShopEasy`.`personas` (`cedula`)
    ON DELETE RESTRICT
    ON UPDATE CASCADE,
  CONSTRAINT `fk_usuarios_roles`
    FOREIGN KEY (`id_rol`)
    REFERENCES `ShopEasy`.`roles` (`id_rol`)
    ON DELETE RESTRICT
    ON UPDATE CASCADE)
ENGINE = InnoDB;


-- -----------------------------------------------------
-- Table `ShopEasy`.`preferencias_publicidad`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `ShopEasy`.`preferencias_publicidad` (
  `id_preferencia_publicidad` INT NOT NULL AUTO_INCREMENT,
  `nombre` VARCHAR(50) NOT NULL,
  `descripcion` VARCHAR(255) NULL DEFAULT NULL,
  PRIMARY KEY (`id_preferencia_publicidad`),
  UNIQUE INDEX `uq_preferencias_publicidad_nombre` (`nombre` ASC)
) ENGINE = InnoDB;


-- -----------------------------------------------------
-- Table `ShopEasy`.`clientes`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `ShopEasy`.`clientes` (
  `id_cliente` INT NOT NULL AUTO_INCREMENT,
  `cedula` VARCHAR(30) NOT NULL,
  `id_preferencia_publicidad` INT NOT NULL DEFAULT 2,
  `fecha_registro` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `activo` TINYINT(1) NOT NULL DEFAULT 1,
  PRIMARY KEY (`id_cliente`),
  UNIQUE INDEX `uq_clientes_cedula` (`cedula` ASC),
  INDEX `idx_clientes_preferencia` (`id_preferencia_publicidad` ASC),
  CONSTRAINT `fk_clientes_personas`
    FOREIGN KEY (`cedula`)
    REFERENCES `ShopEasy`.`personas` (`cedula`)
    ON DELETE RESTRICT
    ON UPDATE CASCADE,
  CONSTRAINT `fk_clientes_preferencias_publicidad`
    FOREIGN KEY (`id_preferencia_publicidad`)
    REFERENCES `ShopEasy`.`preferencias_publicidad` (`id_preferencia_publicidad`)
    ON DELETE RESTRICT
    ON UPDATE CASCADE)
ENGINE = InnoDB;


-- -----------------------------------------------------
-- Table `ShopEasy`.`intereses`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `ShopEasy`.`intereses` (
  `id_interes` INT NOT NULL AUTO_INCREMENT,
  `nombre` VARCHAR(80) NOT NULL,
  `descripcion` VARCHAR(255) NULL DEFAULT NULL,
  `activo` TINYINT(1) NOT NULL DEFAULT 1,
  PRIMARY KEY (`id_interes`),
  UNIQUE INDEX `uq_intereses_nombre` (`nombre` ASC))
ENGINE = InnoDB;


-- -----------------------------------------------------
-- Table `ShopEasy`.`cliente_intereses`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `ShopEasy`.`cliente_intereses` (
  `id_cliente` INT NOT NULL,
  `id_interes` INT NOT NULL,
  `fecha_asignacion` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (`id_cliente`, `id_interes`),
  INDEX `fk_cliente_intereses_intereses` (`id_interes` ASC),
  CONSTRAINT `fk_cliente_intereses_clientes`
    FOREIGN KEY (`id_cliente`)
    REFERENCES `ShopEasy`.`clientes` (`id_cliente`)
    ON DELETE RESTRICT
    ON UPDATE CASCADE,
  CONSTRAINT `fk_cliente_intereses_intereses`
    FOREIGN KEY (`id_interes`)
    REFERENCES `ShopEasy`.`intereses` (`id_interes`)
    ON DELETE RESTRICT
    ON UPDATE CASCADE)
ENGINE = InnoDB;


-- -----------------------------------------------------
-- Table `ShopEasy`.`bodegas`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `ShopEasy`.`bodegas` (
  `id_bodega` INT NOT NULL AUTO_INCREMENT,
  `nombre` VARCHAR(100) NOT NULL,
  `direccion` VARCHAR(255) NOT NULL,
  `descripcion` VARCHAR(255) NULL DEFAULT NULL,
  `activo` TINYINT(1) NOT NULL DEFAULT 1,
  PRIMARY KEY (`id_bodega`),
  UNIQUE INDEX `uq_bodegas_nombre` (`nombre` ASC))
ENGINE = InnoDB;


-- -----------------------------------------------------
-- Table `ShopEasy`.`productos`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `ShopEasy`.`productos` (
  `id_producto` INT NOT NULL AUTO_INCREMENT,
  `id_bodega` INT NOT NULL,
  `nombre` VARCHAR(150) NOT NULL,
  `descripcion` TEXT NULL DEFAULT NULL,
  `precio` DECIMAL(10,2) NOT NULL,
  `stock` INT NOT NULL DEFAULT 0,
  `activo` TINYINT(1) NOT NULL DEFAULT 1,
  `fecha_creacion` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (`id_producto`),
  INDEX `idx_productos_bodega` (`id_bodega` ASC),
  INDEX `idx_productos_nombre` (`nombre` ASC),
  CONSTRAINT `fk_productos_bodegas`
    FOREIGN KEY (`id_bodega`)
    REFERENCES `ShopEasy`.`bodegas` (`id_bodega`)
    ON DELETE RESTRICT
    ON UPDATE CASCADE)
ENGINE = InnoDB;


-- -----------------------------------------------------
-- Table `ShopEasy`.`estados_pedido`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `ShopEasy`.`estados_pedido` (
  `id_estado_pedido` INT NOT NULL AUTO_INCREMENT,
  `nombre` VARCHAR(40) NOT NULL,
  `descripcion` VARCHAR(255) NULL DEFAULT NULL,
  PRIMARY KEY (`id_estado_pedido`),
  UNIQUE INDEX `uq_estados_pedido_nombre` (`nombre` ASC))
ENGINE = InnoDB;


-- -----------------------------------------------------
-- Table `ShopEasy`.`pedidos`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `ShopEasy`.`pedidos` (
  `id_pedido` INT NOT NULL AUTO_INCREMENT,
  `id_cliente` INT NOT NULL,
  `id_estado_pedido` INT NOT NULL,
  `fecha_pedido` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `total` DECIMAL(10,2) NOT NULL DEFAULT 0.00,
  `direccion_envio` VARCHAR(255) NOT NULL,
  `observaciones` TEXT NULL DEFAULT NULL,
  PRIMARY KEY (`id_pedido`),
  INDEX `idx_pedidos_cliente_fecha` (`id_cliente` ASC, `fecha_pedido` ASC),
  INDEX `idx_pedidos_estado` (`id_estado_pedido` ASC),
  CONSTRAINT `fk_pedidos_clientes`
    FOREIGN KEY (`id_cliente`)
    REFERENCES `ShopEasy`.`clientes` (`id_cliente`)
    ON DELETE RESTRICT
    ON UPDATE CASCADE,
  CONSTRAINT `fk_pedidos_estados_pedido`
    FOREIGN KEY (`id_estado_pedido`)
    REFERENCES `ShopEasy`.`estados_pedido` (`id_estado_pedido`)
    ON DELETE RESTRICT
    ON UPDATE CASCADE)
ENGINE = InnoDB;


-- -----------------------------------------------------
-- Table `ShopEasy`.`detalle_pedido`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `ShopEasy`.`detalle_pedido` (
  `id_detalle` INT NOT NULL AUTO_INCREMENT,
  `id_pedido` INT NOT NULL,
  `id_producto` INT NOT NULL,
  `cantidad` INT NOT NULL,
  `precio_unitario` DECIMAL(10,2) NOT NULL,
  PRIMARY KEY (`id_detalle`),
  INDEX `idx_detalle_pedido` (`id_pedido` ASC),
  INDEX `idx_detalle_producto` (`id_producto` ASC),
  CONSTRAINT `fk_detalle_pedido_pedidos`
    FOREIGN KEY (`id_pedido`)
    REFERENCES `ShopEasy`.`pedidos` (`id_pedido`)
    ON DELETE RESTRICT
    ON UPDATE CASCADE,
  CONSTRAINT `fk_detalle_pedido_productos`
    FOREIGN KEY (`id_producto`)
    REFERENCES `ShopEasy`.`productos` (`id_producto`)
    ON DELETE RESTRICT
    ON UPDATE CASCADE)
ENGINE = InnoDB;


-- -----------------------------------------------------
-- Table `ShopEasy`.`historial_estados_pedido`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `ShopEasy`.`historial_estados_pedido` (
  `id_historial` INT NOT NULL AUTO_INCREMENT,
  `id_pedido` INT NOT NULL,
  `id_estado_pedido` INT NOT NULL,
  `id_usuario` INT NULL DEFAULT NULL,
  `fecha_cambio` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `observacion` VARCHAR(255) NULL DEFAULT NULL,
  PRIMARY KEY (`id_historial`),
  INDEX `idx_historial_pedido_fecha` (`id_pedido` ASC, `fecha_cambio` ASC),
  INDEX `fk_historial_pedido_estados` (`id_estado_pedido` ASC),
  INDEX `fk_historial_pedido_usuarios` (`id_usuario` ASC),
  CONSTRAINT `fk_historial_pedido_pedidos`
    FOREIGN KEY (`id_pedido`)
    REFERENCES `ShopEasy`.`pedidos` (`id_pedido`)
    ON DELETE RESTRICT
    ON UPDATE CASCADE,
  CONSTRAINT `fk_historial_pedido_estados`
    FOREIGN KEY (`id_estado_pedido`)
    REFERENCES `ShopEasy`.`estados_pedido` (`id_estado_pedido`)
    ON DELETE RESTRICT
    ON UPDATE CASCADE,
  CONSTRAINT `fk_historial_pedido_usuarios`
    FOREIGN KEY (`id_usuario`)
    REFERENCES `ShopEasy`.`usuarios` (`id_usuario`)
    ON DELETE SET NULL
    ON UPDATE CASCADE)
ENGINE = InnoDB;


-- -----------------------------------------------------
-- Table `ShopEasy`.`tipos_caso`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `ShopEasy`.`tipos_caso` (
  `id_tipo_caso` INT NOT NULL AUTO_INCREMENT,
  `nombre` VARCHAR(50) NOT NULL,
  `descripcion` VARCHAR(255) NULL DEFAULT NULL,
  PRIMARY KEY (`id_tipo_caso`),
  UNIQUE INDEX `uq_tipos_caso_nombre` (`nombre` ASC))
ENGINE = InnoDB;


-- -----------------------------------------------------
-- Table `ShopEasy`.`estados_caso`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `ShopEasy`.`estados_caso` (
  `id_estado_caso` INT NOT NULL AUTO_INCREMENT,
  `nombre` VARCHAR(40) NOT NULL,
  `descripcion` VARCHAR(255) NULL DEFAULT NULL,
  PRIMARY KEY (`id_estado_caso`),
  UNIQUE INDEX `uq_estados_caso_nombre` (`nombre` ASC))
ENGINE = InnoDB;


-- -----------------------------------------------------
-- Table `ShopEasy`.`casos_cliente`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `ShopEasy`.`casos_cliente` (
  `id_caso` INT NOT NULL AUTO_INCREMENT,
  `id_cliente` INT NOT NULL,
  `id_tipo_caso` INT NOT NULL,
  `id_estado_caso` INT NOT NULL,
  `id_usuario_asignado` INT NULL DEFAULT NULL,
  `canal` VARCHAR(40) NOT NULL,
  `asunto` VARCHAR(150) NOT NULL,
  `descripcion` TEXT NOT NULL,
  `fecha_creacion` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `fecha_resolucion` DATETIME NULL DEFAULT NULL,
  PRIMARY KEY (`id_caso`),
  INDEX `idx_casos_cliente_estado` (`id_cliente` ASC, `id_estado_caso` ASC),
  INDEX `idx_casos_asignado` (`id_usuario_asignado` ASC),
  INDEX `fk_casos_cliente_tipos` (`id_tipo_caso` ASC),
  INDEX `fk_casos_cliente_estados` (`id_estado_caso` ASC),
  CONSTRAINT `fk_casos_cliente_clientes`
    FOREIGN KEY (`id_cliente`)
    REFERENCES `ShopEasy`.`clientes` (`id_cliente`)
    ON DELETE RESTRICT
    ON UPDATE CASCADE,
  CONSTRAINT `fk_casos_cliente_tipos`
    FOREIGN KEY (`id_tipo_caso`)
    REFERENCES `ShopEasy`.`tipos_caso` (`id_tipo_caso`)
    ON DELETE RESTRICT
    ON UPDATE CASCADE,
  CONSTRAINT `fk_casos_cliente_estados`
    FOREIGN KEY (`id_estado_caso`)
    REFERENCES `ShopEasy`.`estados_caso` (`id_estado_caso`)
    ON DELETE RESTRICT
    ON UPDATE CASCADE,
  CONSTRAINT `fk_casos_cliente_usuarios`
    FOREIGN KEY (`id_usuario_asignado`)
    REFERENCES `ShopEasy`.`usuarios` (`id_usuario`)
    ON DELETE SET NULL
    ON UPDATE CASCADE)
ENGINE = InnoDB;


-- -----------------------------------------------------
-- Table `ShopEasy`.`canales_marketing`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `ShopEasy`.`canales_marketing` (
  `id_canal` INT NOT NULL AUTO_INCREMENT,
  `nombre` VARCHAR(50) NOT NULL,
  `descripcion` VARCHAR(255) NULL DEFAULT NULL,
  `activo` TINYINT(1) NOT NULL DEFAULT 1,
  PRIMARY KEY (`id_canal`),
  UNIQUE INDEX `uq_canales_marketing_nombre` (`nombre` ASC))
ENGINE = InnoDB;


-- -----------------------------------------------------
-- Table `ShopEasy`.`campanas`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `ShopEasy`.`campanas` (
  `id_campana` INT NOT NULL AUTO_INCREMENT,
  `nombre` VARCHAR(150) NOT NULL,
  `descripcion` TEXT NULL DEFAULT NULL,
  `id_canal` INT NOT NULL,
  `fecha_programada` DATETIME NOT NULL,
  `estado` VARCHAR(30) NOT NULL DEFAULT 'Borrador',
  `contenido` TEXT NOT NULL,
  `fecha_creacion` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `id_usuario_creador` INT NOT NULL,
  PRIMARY KEY (`id_campana`),
  INDEX `idx_campanas_fecha_estado` (`estado` ASC, `fecha_programada` ASC),
  INDEX `fk_campanas_canales` (`id_canal` ASC),
  INDEX `fk_campanas_usuarios` (`id_usuario_creador` ASC),
  CONSTRAINT `fk_campanas_canales`
    FOREIGN KEY (`id_canal`)
    REFERENCES `ShopEasy`.`canales_marketing` (`id_canal`)
    ON DELETE RESTRICT
    ON UPDATE CASCADE,
  CONSTRAINT `fk_campanas_usuarios`
    FOREIGN KEY (`id_usuario_creador`)
    REFERENCES `ShopEasy`.`usuarios` (`id_usuario`)
    ON DELETE RESTRICT
    ON UPDATE CASCADE)
ENGINE = InnoDB;


-- -----------------------------------------------------
-- Table `ShopEasy`.`campana_clientes`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `ShopEasy`.`campana_clientes` (
  `id_campana_cliente` INT NOT NULL AUTO_INCREMENT,
  `id_campana` INT NOT NULL,
  `id_cliente` INT NOT NULL,
  `fecha_envio` DATETIME NULL DEFAULT NULL,
  `estado_envio` VARCHAR(30) NOT NULL DEFAULT 'Pendiente',
  PRIMARY KEY (`id_campana_cliente`),
  UNIQUE INDEX `uq_campana_cliente` (`id_campana` ASC, `id_cliente` ASC),
  INDEX `idx_campana_clientes_estado` (`id_campana` ASC, `estado_envio` ASC),
  INDEX `fk_campana_clientes_clientes` (`id_cliente` ASC),
  CONSTRAINT `fk_campana_clientes_campanas`
    FOREIGN KEY (`id_campana`)
    REFERENCES `ShopEasy`.`campanas` (`id_campana`)
    ON DELETE RESTRICT
    ON UPDATE CASCADE,
  CONSTRAINT `fk_campana_clientes_clientes`
    FOREIGN KEY (`id_cliente`)
    REFERENCES `ShopEasy`.`clientes` (`id_cliente`)
    ON DELETE RESTRICT
    ON UPDATE CASCADE)
ENGINE = InnoDB;


-- -----------------------------------------------------
-- Table `ShopEasy`.`campana_intereses`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `ShopEasy`.`campana_intereses` (
  `id_campana` INT NOT NULL,
  `id_interes` INT NOT NULL,
  PRIMARY KEY (`id_campana`, `id_interes`),
  INDEX `fk_campana_intereses_intereses` (`id_interes` ASC),
  CONSTRAINT `fk_campana_intereses_campanas`
    FOREIGN KEY (`id_campana`)
    REFERENCES `ShopEasy`.`campanas` (`id_campana`)
    ON DELETE RESTRICT
    ON UPDATE CASCADE,
  CONSTRAINT `fk_campana_intereses_intereses`
    FOREIGN KEY (`id_interes`)
    REFERENCES `ShopEasy`.`intereses` (`id_interes`)
    ON DELETE RESTRICT
    ON UPDATE CASCADE)
ENGINE = InnoDB;


SET SQL_MODE=@OLD_SQL_MODE;
SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS;
SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS;


-- =====================================================
-- DATOS DE CONFIGURACION: roles base
-- =====================================================
INSERT INTO `ShopEasy`.`roles` (`nombre`, `descripcion`) VALUES
  ('Administrador', 'Acceso total al sistema y gestion de usuarios y roles'),
  ('Empleado', 'Acceso operativo al CRM sin privilegios de administracion');
  
-- =====================================================
-- USUARIO DE PRUEBA: ADMINISTRADOR
-- Cedula: 123456789
-- Correo: admin@shopeasy.com
-- Contrasena: Admin-2026
-- =====================================================

INSERT INTO `ShopEasy`.`personas`
(`cedula`, `nombre`, `apellidos`)
VALUES
('123456789', 'Administrador', 'De Prueba');

INSERT INTO `ShopEasy`.`correos_persona`
(`cedula`, `correo`, `tipo_correo`, `principal`, `activo`)
VALUES
('123456789', 'admin@shopeasy.com', 'Principal', 1, 1);

INSERT INTO `ShopEasy`.`usuarios`
(`cedula`, `id_rol`, `contrasena_hash`, `activo`)
SELECT
    '123456789',
    `id_rol`,
    '$2b$12$oYKeShR6vdlsSlAIh6ik6e5cli0CZZSXRAnwlJYEalrkziZLanXp2',
    1
FROM `ShopEasy`.`roles`
WHERE `nombre` = 'Administrador';
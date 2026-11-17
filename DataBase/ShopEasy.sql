-- MySQL Workbench Forward Engineering

SET @OLD_UNIQUE_CHECKS=@@UNIQUE_CHECKS, UNIQUE_CHECKS=0;
SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0;
SET @OLD_SQL_MODE=@@SQL_MODE, SQL_MODE='ONLY_FULL_GROUP_BY,STRICT_TRANS_TABLES,NO_ZERO_IN_DATE,NO_ZERO_DATE,ERROR_FOR_DIVISION_BY_ZERO,NO_ENGINE_SUBSTITUTION';

-- -----------------------------------------------------
-- Schema shopeasy
-- -----------------------------------------------------
CREATE SCHEMA IF NOT EXISTS `shopeasy` DEFAULT CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci ;
USE `shopeasy` ;

-- -----------------------------------------------------
-- Table `shopeasy`.`Activo`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `shopeasy`.`Activo` (
  `idActivo` INT NOT NULL,
  `descripcionActivo` VARCHAR(45) NOT NULL,
  `Activo` TINYINT(1) NOT NULL,
  PRIMARY KEY (`idActivo`))
ENGINE = InnoDB;

-- -----------------------------------------------------
-- Table `shopeasy`.`bodegas`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `shopeasy`.`bodegas` (
  `id_bodega` INT NOT NULL AUTO_INCREMENT,
  `nombre` VARCHAR(100) NOT NULL,
  `direccion` VARCHAR(255) NOT NULL,
  `descripcion` VARCHAR(255) NOT NULL,
  `Activo_idActivo` INT NOT NULL,
  PRIMARY KEY (`id_bodega`),
  UNIQUE INDEX `uq_bodegas_nombre` (`nombre` ASC) VISIBLE,
  INDEX `fk_bodegas_Activo1_idx` (`Activo_idActivo` ASC) VISIBLE,
  CONSTRAINT `fk_bodegas_Activo1`
    FOREIGN KEY (`Activo_idActivo`)
    REFERENCES `shopeasy`.`Activo` (`idActivo`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION)
ENGINE = InnoDB
DEFAULT CHARACTER SET = utf8mb4
COLLATE = utf8mb4_0900_ai_ci;


-- -----------------------------------------------------
-- Table `shopeasy`.`canales_marketing`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `shopeasy`.`canales_marketing` (
  `id_canal` INT NOT NULL AUTO_INCREMENT,
  `nombre` VARCHAR(50) NOT NULL,
  `descripcion` VARCHAR(255) NOT NULL,
  `Activo_idActivo` INT NOT NULL,
  PRIMARY KEY (`id_canal`),
  UNIQUE INDEX `uq_canales_marketing_nombre` (`nombre` ASC) VISIBLE,
  INDEX `fk_canales_marketing_Activo1_idx` (`Activo_idActivo` ASC) VISIBLE,
  CONSTRAINT `fk_canales_marketing_Activo1`
    FOREIGN KEY (`Activo_idActivo`)
    REFERENCES `shopeasy`.`Activo` (`idActivo`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION)
ENGINE = InnoDB
DEFAULT CHARACTER SET = utf8mb4
COLLATE = utf8mb4_0900_ai_ci;


-- -----------------------------------------------------
-- Table `shopeasy`.`personas`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `shopeasy`.`personas` (
  `cedula` VARCHAR(30) NOT NULL,
  `nombre` VARCHAR(100) NOT NULL,
  `apellidos` VARCHAR(100) NOT NULL,
  `fecha_creacion` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (`cedula`))
ENGINE = InnoDB
DEFAULT CHARACTER SET = utf8mb4
COLLATE = utf8mb4_0900_ai_ci;


-- -----------------------------------------------------
-- Table `shopeasy`.`roles`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `shopeasy`.`roles` (
  `id_rol` INT NOT NULL AUTO_INCREMENT,
  `nombre` VARCHAR(50) NOT NULL,
  `descripcion` VARCHAR(255) NOT NULL,
  PRIMARY KEY (`id_rol`),
  UNIQUE INDEX `uq_roles_nombre` (`nombre` ASC) VISIBLE)
ENGINE = InnoDB
AUTO_INCREMENT = 3
DEFAULT CHARACTER SET = utf8mb4
COLLATE = utf8mb4_0900_ai_ci;


-- -----------------------------------------------------
-- Table `shopeasy`.`usuarios`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `shopeasy`.`usuarios` (
  `id_usuario` INT NOT NULL AUTO_INCREMENT,
  `cedula` VARCHAR(30) NOT NULL,
  `id_rol` INT NOT NULL,
  `contrasena_hash` VARCHAR(255) NOT NULL,
  `fecha_creacion` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `Activo_idActivo` INT NOT NULL,
  PRIMARY KEY (`id_usuario`),
  UNIQUE INDEX `uq_usuarios_cedula` (`cedula` ASC) VISIBLE,
  INDEX `idx_usuarios_rol` (`id_rol` ASC) VISIBLE,
  INDEX `fk_usuarios_Activo1_idx` (`Activo_idActivo` ASC) VISIBLE,
  CONSTRAINT `fk_usuarios_personas`
    FOREIGN KEY (`cedula`)
    REFERENCES `shopeasy`.`personas` (`cedula`)
    ON DELETE RESTRICT
    ON UPDATE CASCADE,
  CONSTRAINT `fk_usuarios_roles`
    FOREIGN KEY (`id_rol`)
    REFERENCES `shopeasy`.`roles` (`id_rol`)
    ON DELETE RESTRICT
    ON UPDATE CASCADE,
  CONSTRAINT `fk_usuarios_Activo1`
    FOREIGN KEY (`Activo_idActivo`)
    REFERENCES `shopeasy`.`Activo` (`idActivo`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION)
ENGINE = InnoDB
AUTO_INCREMENT = 3
DEFAULT CHARACTER SET = utf8mb4
COLLATE = utf8mb4_0900_ai_ci;


-- -----------------------------------------------------
-- Table `shopeasy`.`campanas`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `shopeasy`.`campanas` (
  `id_campana` INT NOT NULL AUTO_INCREMENT,
  `nombre` VARCHAR(150) NOT NULL,
  `descripcion` TEXT NOT NULL,
  `id_canal` INT NOT NULL,
  `fecha_programada` DATETIME NOT NULL,
  `estado` VARCHAR(30) NOT NULL DEFAULT 'Borrador',
  `contenido` TEXT NOT NULL,
  `fecha_creacion` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `id_usuario_creador` INT NOT NULL,
  PRIMARY KEY (`id_campana`),
  INDEX `idx_campanas_fecha_estado` (`estado` ASC, `fecha_programada` ASC) VISIBLE,
  INDEX `fk_campanas_canales` (`id_canal` ASC) VISIBLE,
  INDEX `fk_campanas_usuarios` (`id_usuario_creador` ASC) VISIBLE,
  CONSTRAINT `fk_campanas_canales`
    FOREIGN KEY (`id_canal`)
    REFERENCES `shopeasy`.`canales_marketing` (`id_canal`)
    ON DELETE RESTRICT
    ON UPDATE CASCADE,
  CONSTRAINT `fk_campanas_usuarios`
    FOREIGN KEY (`id_usuario_creador`)
    REFERENCES `shopeasy`.`usuarios` (`id_usuario`)
    ON DELETE RESTRICT
    ON UPDATE CASCADE)
ENGINE = InnoDB
DEFAULT CHARACTER SET = utf8mb4
COLLATE = utf8mb4_0900_ai_ci;


-- -----------------------------------------------------
-- Table `shopeasy`.`preferencias_publicidad`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `shopeasy`.`preferencias_publicidad` (
  `id_preferencia_publicidad` INT NOT NULL AUTO_INCREMENT,
  `nombre` VARCHAR(50) NOT NULL,
  `descripcion` VARCHAR(255) NOT NULL,
  PRIMARY KEY (`id_preferencia_publicidad`),
  UNIQUE INDEX `uq_preferencias_publicidad_nombre` (`nombre` ASC) VISIBLE)
ENGINE = InnoDB
DEFAULT CHARACTER SET = utf8mb4
COLLATE = utf8mb4_0900_ai_ci;


-- -----------------------------------------------------
-- Table `shopeasy`.`clientes`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `shopeasy`.`clientes` (
  `id_cliente` INT NOT NULL AUTO_INCREMENT,
  `cedula` VARCHAR(30) NOT NULL,
  `id_preferencia_publicidad` INT NOT NULL DEFAULT '2',
  `fecha_registro` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `Activo_idActivo` INT NOT NULL,
  PRIMARY KEY (`id_cliente`),
  UNIQUE INDEX `uq_clientes_cedula` (`cedula` ASC) VISIBLE,
  INDEX `idx_clientes_preferencia` (`id_preferencia_publicidad` ASC) VISIBLE,
  INDEX `fk_clientes_Activo1_idx` (`Activo_idActivo` ASC) VISIBLE,
  CONSTRAINT `fk_clientes_personas`
    FOREIGN KEY (`cedula`)
    REFERENCES `shopeasy`.`personas` (`cedula`)
    ON DELETE RESTRICT
    ON UPDATE CASCADE,
  CONSTRAINT `fk_clientes_preferencias_publicidad`
    FOREIGN KEY (`id_preferencia_publicidad`)
    REFERENCES `shopeasy`.`preferencias_publicidad` (`id_preferencia_publicidad`)
    ON DELETE RESTRICT
    ON UPDATE CASCADE,
  CONSTRAINT `fk_clientes_Activo1`
    FOREIGN KEY (`Activo_idActivo`)
    REFERENCES `shopeasy`.`Activo` (`idActivo`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION)
ENGINE = InnoDB
DEFAULT CHARACTER SET = utf8mb4
COLLATE = utf8mb4_0900_ai_ci;


-- -----------------------------------------------------
-- Table `shopeasy`.`campana_clientes`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `shopeasy`.`campana_clientes` (
  `id_campana_cliente` INT NOT NULL AUTO_INCREMENT,
  `id_campana` INT NOT NULL,
  `id_cliente` INT NOT NULL,
  `fecha_envio` DATETIME NOT NULL,
  `estado_envio` VARCHAR(30) NOT NULL DEFAULT 'Pendiente',
  PRIMARY KEY (`id_campana_cliente`),
  UNIQUE INDEX `uq_campana_cliente` (`id_campana` ASC, `id_cliente` ASC) VISIBLE,
  INDEX `idx_campana_clientes_estado` (`id_campana` ASC, `estado_envio` ASC) VISIBLE,
  INDEX `fk_campana_clientes_clientes` (`id_cliente` ASC) VISIBLE,
  CONSTRAINT `fk_campana_clientes_campanas`
    FOREIGN KEY (`id_campana`)
    REFERENCES `shopeasy`.`campanas` (`id_campana`)
    ON DELETE RESTRICT
    ON UPDATE CASCADE,
  CONSTRAINT `fk_campana_clientes_clientes`
    FOREIGN KEY (`id_cliente`)
    REFERENCES `shopeasy`.`clientes` (`id_cliente`)
    ON DELETE RESTRICT
    ON UPDATE CASCADE)
ENGINE = InnoDB
DEFAULT CHARACTER SET = utf8mb4
COLLATE = utf8mb4_0900_ai_ci;


-- -----------------------------------------------------
-- Table `shopeasy`.`intereses`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `shopeasy`.`intereses` (
  `id_interes` INT NOT NULL AUTO_INCREMENT,
  `nombre` VARCHAR(80) NOT NULL,
  `descripcion` VARCHAR(255) NOT NULL,
  `Activo_idActivo` INT NOT NULL,
  PRIMARY KEY (`id_interes`),
  UNIQUE INDEX `uq_intereses_nombre` (`nombre` ASC) VISIBLE,
  INDEX `fk_intereses_Activo1_idx` (`Activo_idActivo` ASC) VISIBLE,
  CONSTRAINT `fk_intereses_Activo1`
    FOREIGN KEY (`Activo_idActivo`)
    REFERENCES `shopeasy`.`Activo` (`idActivo`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION)
ENGINE = InnoDB
DEFAULT CHARACTER SET = utf8mb4
COLLATE = utf8mb4_0900_ai_ci;


-- -----------------------------------------------------
-- Table `shopeasy`.`campana_intereses`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `shopeasy`.`campana_intereses` (
  `id_campana` INT NOT NULL,
  `id_interes` INT NOT NULL,
  PRIMARY KEY (`id_campana`, `id_interes`),
  INDEX `fk_campana_intereses_intereses` (`id_interes` ASC) VISIBLE,
  CONSTRAINT `fk_campana_intereses_campanas`
    FOREIGN KEY (`id_campana`)
    REFERENCES `shopeasy`.`campanas` (`id_campana`)
    ON DELETE RESTRICT
    ON UPDATE CASCADE,
  CONSTRAINT `fk_campana_intereses_intereses`
    FOREIGN KEY (`id_interes`)
    REFERENCES `shopeasy`.`intereses` (`id_interes`)
    ON DELETE RESTRICT
    ON UPDATE CASCADE)
ENGINE = InnoDB
DEFAULT CHARACTER SET = utf8mb4
COLLATE = utf8mb4_0900_ai_ci;


-- -----------------------------------------------------
-- Table `shopeasy`.`estados_caso`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `shopeasy`.`estados_caso` (
  `id_estado_caso` INT NOT NULL AUTO_INCREMENT,
  `nombre` VARCHAR(40) NOT NULL,
  `descripcion` VARCHAR(255) NOT NULL,
  PRIMARY KEY (`id_estado_caso`),
  UNIQUE INDEX `uq_estados_caso_nombre` (`nombre` ASC) VISIBLE)
ENGINE = InnoDB
DEFAULT CHARACTER SET = utf8mb4
COLLATE = utf8mb4_0900_ai_ci;


-- -----------------------------------------------------
-- Table `shopeasy`.`tipos_caso`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `shopeasy`.`tipos_caso` (
  `id_tipo_caso` INT NOT NULL AUTO_INCREMENT,
  `nombre` VARCHAR(50) NOT NULL,
  `descripcion` VARCHAR(255) NOT NULL,
  PRIMARY KEY (`id_tipo_caso`),
  UNIQUE INDEX `uq_tipos_caso_nombre` (`nombre` ASC) VISIBLE)
ENGINE = InnoDB
DEFAULT CHARACTER SET = utf8mb4
COLLATE = utf8mb4_0900_ai_ci;


-- -----------------------------------------------------
-- Table `shopeasy`.`casos_cliente`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `shopeasy`.`casos_cliente` (
  `id_caso` INT NOT NULL AUTO_INCREMENT,
  `id_cliente` INT NOT NULL,
  `id_tipo_caso` INT NOT NULL,
  `id_estado_caso` INT NOT NULL,
  `id_usuario_asignado` INT NOT NULL,
  `canal` VARCHAR(40) NOT NULL,
  `asunto` VARCHAR(150) NOT NULL,
  `descripcion` TEXT NOT NULL,
  `fecha_creacion` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `fecha_resolucion` DATETIME NOT NULL,
  PRIMARY KEY (`id_caso`),
  INDEX `idx_casos_cliente_estado` (`id_cliente` ASC, `id_estado_caso` ASC) VISIBLE,
  INDEX `idx_casos_asignado` (`id_usuario_asignado` ASC) VISIBLE,
  INDEX `fk_casos_cliente_tipos` (`id_tipo_caso` ASC) VISIBLE,
  INDEX `fk_casos_cliente_estados` (`id_estado_caso` ASC) VISIBLE,
  CONSTRAINT `fk_casos_cliente_clientes`
    FOREIGN KEY (`id_cliente`)
    REFERENCES `shopeasy`.`clientes` (`id_cliente`)
    ON DELETE RESTRICT
    ON UPDATE CASCADE,
  CONSTRAINT `fk_casos_cliente_estados`
    FOREIGN KEY (`id_estado_caso`)
    REFERENCES `shopeasy`.`estados_caso` (`id_estado_caso`)
    ON DELETE RESTRICT
    ON UPDATE CASCADE,
  CONSTRAINT `fk_casos_cliente_tipos`
    FOREIGN KEY (`id_tipo_caso`)
    REFERENCES `shopeasy`.`tipos_caso` (`id_tipo_caso`)
    ON DELETE RESTRICT
    ON UPDATE CASCADE,
  CONSTRAINT `fk_casos_cliente_usuarios`
    FOREIGN KEY (`id_usuario_asignado`)
    REFERENCES `shopeasy`.`usuarios` (`id_usuario`)
    ON DELETE RESTRICT
    ON UPDATE CASCADE)
ENGINE = InnoDB
DEFAULT CHARACTER SET = utf8mb4
COLLATE = utf8mb4_0900_ai_ci;


-- -----------------------------------------------------
-- Table `shopeasy`.`cliente_intereses`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `shopeasy`.`cliente_intereses` (
  `id_cliente` INT NOT NULL,
  `id_interes` INT NOT NULL,
  `fecha_asignacion` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (`id_cliente`, `id_interes`),
  INDEX `fk_cliente_intereses_intereses` (`id_interes` ASC) VISIBLE,
  CONSTRAINT `fk_cliente_intereses_clientes`
    FOREIGN KEY (`id_cliente`)
    REFERENCES `shopeasy`.`clientes` (`id_cliente`)
    ON DELETE RESTRICT
    ON UPDATE CASCADE,
  CONSTRAINT `fk_cliente_intereses_intereses`
    FOREIGN KEY (`id_interes`)
    REFERENCES `shopeasy`.`intereses` (`id_interes`)
    ON DELETE RESTRICT
    ON UPDATE CASCADE)
ENGINE = InnoDB
DEFAULT CHARACTER SET = utf8mb4
COLLATE = utf8mb4_0900_ai_ci;


-- -----------------------------------------------------
-- Table `shopeasy`.`correos_persona`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `shopeasy`.`correos_persona` (
  `id_correo` INT NOT NULL AUTO_INCREMENT,
  `cedula` VARCHAR(30) NOT NULL,
  `correo` VARCHAR(150) NOT NULL,
  `tipo_correo` VARCHAR(30) NOT NULL,
  `principal` TINYINT(1) NOT NULL DEFAULT '0',
  `Activo_idActivo` INT NOT NULL,
  PRIMARY KEY (`id_correo`),
  UNIQUE INDEX `uq_correos_correo` (`correo` ASC) VISIBLE,
  INDEX `idx_correos_persona` (`cedula` ASC) VISIBLE,
  INDEX `fk_correos_persona_Activo1_idx` (`Activo_idActivo` ASC) VISIBLE,
  CONSTRAINT `fk_correos_persona_personas`
    FOREIGN KEY (`cedula`)
    REFERENCES `shopeasy`.`personas` (`cedula`)
    ON DELETE RESTRICT
    ON UPDATE CASCADE,
  CONSTRAINT `fk_correos_persona_Activo1`
    FOREIGN KEY (`Activo_idActivo`)
    REFERENCES `shopeasy`.`Activo` (`idActivo`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION)
ENGINE = InnoDB
AUTO_INCREMENT = 3
DEFAULT CHARACTER SET = utf8mb4
COLLATE = utf8mb4_0900_ai_ci;


-- -----------------------------------------------------
-- Table `shopeasy`.`estados_pedido`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `shopeasy`.`estados_pedido` (
  `id_estado_pedido` INT NOT NULL AUTO_INCREMENT,
  `nombre` VARCHAR(40) NOT NULL,
  `descripcion` VARCHAR(255) NOT NULL,
  PRIMARY KEY (`id_estado_pedido`),
  UNIQUE INDEX `uq_estados_pedido_nombre` (`nombre` ASC) VISIBLE)
ENGINE = InnoDB
DEFAULT CHARACTER SET = utf8mb4
COLLATE = utf8mb4_0900_ai_ci;


-- -----------------------------------------------------
-- Table `shopeasy`.`pedidos`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `shopeasy`.`pedidos` (
  `id_pedido` INT NOT NULL AUTO_INCREMENT,
  `id_cliente` INT NOT NULL,
  `id_estado_pedido` INT NOT NULL,
  `fecha_pedido` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `total` DECIMAL(10,2) NOT NULL DEFAULT '0.00',
  `direccion_envio` VARCHAR(255) NOT NULL,
  `observaciones` TEXT NOT NULL,
  PRIMARY KEY (`id_pedido`),
  INDEX `idx_pedidos_cliente_fecha` (`id_cliente` ASC, `fecha_pedido` ASC) VISIBLE,
  INDEX `idx_pedidos_estado` (`id_estado_pedido` ASC) VISIBLE,
  CONSTRAINT `fk_pedidos_clientes`
    FOREIGN KEY (`id_cliente`)
    REFERENCES `shopeasy`.`clientes` (`id_cliente`)
    ON DELETE RESTRICT
    ON UPDATE CASCADE,
  CONSTRAINT `fk_pedidos_estados_pedido`
    FOREIGN KEY (`id_estado_pedido`)
    REFERENCES `shopeasy`.`estados_pedido` (`id_estado_pedido`)
    ON DELETE RESTRICT
    ON UPDATE CASCADE)
ENGINE = InnoDB
DEFAULT CHARACTER SET = utf8mb4
COLLATE = utf8mb4_0900_ai_ci;


-- -----------------------------------------------------
-- Table `shopeasy`.`productos`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `shopeasy`.`productos` (
  `id_producto` INT NOT NULL AUTO_INCREMENT,
  `id_bodega` INT NOT NULL,
  `nombre` VARCHAR(150) NOT NULL,
  `descripcion` TEXT NOT NULL,
  `precio` DECIMAL(10,2) NOT NULL,
  `stock` INT NOT NULL DEFAULT '0',
  `fecha_creacion` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `Activo_idActivo` INT NOT NULL,
  PRIMARY KEY (`id_producto`),
  INDEX `idx_productos_bodega` (`id_bodega` ASC) VISIBLE,
  INDEX `idx_productos_nombre` (`nombre` ASC) VISIBLE,
  INDEX `fk_productos_Activo1_idx` (`Activo_idActivo` ASC) VISIBLE,
  CONSTRAINT `fk_productos_bodegas`
    FOREIGN KEY (`id_bodega`)
    REFERENCES `shopeasy`.`bodegas` (`id_bodega`)
    ON DELETE RESTRICT
    ON UPDATE CASCADE,
  CONSTRAINT `fk_productos_Activo1`
    FOREIGN KEY (`Activo_idActivo`)
    REFERENCES `shopeasy`.`Activo` (`idActivo`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION)
ENGINE = InnoDB
DEFAULT CHARACTER SET = utf8mb4
COLLATE = utf8mb4_0900_ai_ci;


-- -----------------------------------------------------
-- Table `shopeasy`.`detalle_pedido`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `shopeasy`.`detalle_pedido` (
  `id_detalle` INT NOT NULL AUTO_INCREMENT,
  `id_pedido` INT NOT NULL,
  `id_producto` INT NOT NULL,
  `cantidad` INT NOT NULL,
  `precio_unitario` DECIMAL(10,2) NOT NULL,
  PRIMARY KEY (`id_detalle`),
  INDEX `idx_detalle_pedido` (`id_pedido` ASC) VISIBLE,
  INDEX `idx_detalle_producto` (`id_producto` ASC) VISIBLE,
  CONSTRAINT `fk_detalle_pedido_pedidos`
    FOREIGN KEY (`id_pedido`)
    REFERENCES `shopeasy`.`pedidos` (`id_pedido`)
    ON DELETE RESTRICT
    ON UPDATE CASCADE,
  CONSTRAINT `fk_detalle_pedido_productos`
    FOREIGN KEY (`id_producto`)
    REFERENCES `shopeasy`.`productos` (`id_producto`)
    ON DELETE RESTRICT
    ON UPDATE CASCADE)
ENGINE = InnoDB
DEFAULT CHARACTER SET = utf8mb4
COLLATE = utf8mb4_0900_ai_ci;


-- -----------------------------------------------------
-- Table `shopeasy`.`historial_estados_pedido`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `shopeasy`.`historial_estados_pedido` (
  `id_historial` INT NOT NULL AUTO_INCREMENT,
  `id_pedido` INT NOT NULL,
  `id_estado_pedido` INT NOT NULL,
  `id_usuario` INT NOT NULL,
  `fecha_cambio` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `observacion` VARCHAR(255) NOT NULL,
  PRIMARY KEY (`id_historial`),
  INDEX `idx_historial_pedido_fecha` (`id_pedido` ASC, `fecha_cambio` ASC) VISIBLE,
  INDEX `fk_historial_pedido_estados` (`id_estado_pedido` ASC) VISIBLE,
  INDEX `fk_historial_pedido_usuarios` (`id_usuario` ASC) VISIBLE,
  CONSTRAINT `fk_historial_pedido_estados`
    FOREIGN KEY (`id_estado_pedido`)
    REFERENCES `shopeasy`.`estados_pedido` (`id_estado_pedido`)
    ON DELETE RESTRICT
    ON UPDATE CASCADE,
  CONSTRAINT `fk_historial_pedido_pedidos`
    FOREIGN KEY (`id_pedido`)
    REFERENCES `shopeasy`.`pedidos` (`id_pedido`)
    ON DELETE RESTRICT
    ON UPDATE CASCADE,
  CONSTRAINT `fk_historial_pedido_usuarios`
    FOREIGN KEY (`id_usuario`)
    REFERENCES `shopeasy`.`usuarios` (`id_usuario`)
    ON DELETE RESTRICT
    ON UPDATE CASCADE)
ENGINE = InnoDB
DEFAULT CHARACTER SET = utf8mb4
COLLATE = utf8mb4_0900_ai_ci;


-- -----------------------------------------------------
-- Table `shopeasy`.`telefonos_persona`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `shopeasy`.`telefonos_persona` (
  `id_telefono` INT NOT NULL AUTO_INCREMENT,
  `cedula` VARCHAR(30) NOT NULL,
  `telefono` VARCHAR(25) NOT NULL,
  `tipo_telefono` VARCHAR(30) NOT NULL,
  `principal` TINYINT(1) NOT NULL DEFAULT '0',
  `Activo_idActivo` INT NOT NULL,
  PRIMARY KEY (`id_telefono`),
  INDEX `idx_telefonos_persona` (`cedula` ASC) VISIBLE,
  INDEX `fk_telefonos_persona_Activo1_idx` (`Activo_idActivo` ASC) VISIBLE,
  CONSTRAINT `fk_telefonos_persona_personas`
    FOREIGN KEY (`cedula`)
    REFERENCES `shopeasy`.`personas` (`cedula`)
    ON DELETE RESTRICT
    ON UPDATE CASCADE,
  CONSTRAINT `fk_telefonos_persona_Activo1`
    FOREIGN KEY (`Activo_idActivo`)
    REFERENCES `shopeasy`.`Activo` (`idActivo`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION)
ENGINE = InnoDB
DEFAULT CHARACTER SET = utf8mb4
COLLATE = utf8mb4_0900_ai_ci;


SET SQL_MODE=@OLD_SQL_MODE;
SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS;
SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS;
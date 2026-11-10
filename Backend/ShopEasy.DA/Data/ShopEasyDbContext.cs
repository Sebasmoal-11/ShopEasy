using Microsoft.EntityFrameworkCore;
using ShopEasy.DA.Entities;

namespace ShopEasy.DA.Data;

/// <summary>
/// DbContext mapeado contra la base de datos MySQL existente <c>ShopEasy</c>.
/// El esquema es la fuente de verdad: no se usan migraciones ni Code First.
/// </summary>
public class ShopEasyDbContext : DbContext
{
    public ShopEasyDbContext(DbContextOptions<ShopEasyDbContext> options) : base(options)
    {
    }

    public DbSet<Persona> Personas => Set<Persona>();
    public DbSet<CorreoPersona> CorreosPersona => Set<CorreoPersona>();
    public DbSet<TelefonoPersona> TelefonosPersona => Set<TelefonoPersona>();
    public DbSet<Rol> Roles => Set<Rol>();
    public DbSet<Usuario> Usuarios => Set<Usuario>();
    public DbSet<PreferenciaPublicidad> PreferenciasPublicidad => Set<PreferenciaPublicidad>();
    public DbSet<Cliente> Clientes => Set<Cliente>();
    public DbSet<Interes> Intereses => Set<Interes>();
    public DbSet<ClienteInteres> ClienteIntereses => Set<ClienteInteres>();
    public DbSet<Bodega> Bodegas => Set<Bodega>();
    public DbSet<Producto> Productos => Set<Producto>();
    public DbSet<EstadoPedido> EstadosPedido => Set<EstadoPedido>();
    public DbSet<Pedido> Pedidos => Set<Pedido>();
    public DbSet<DetallePedido> DetallePedido => Set<DetallePedido>();
    public DbSet<HistorialEstadoPedido> HistorialEstadosPedido => Set<HistorialEstadoPedido>();
    public DbSet<TipoCaso> TiposCaso => Set<TipoCaso>();
    public DbSet<EstadoCaso> EstadosCaso => Set<EstadoCaso>();
    public DbSet<CasoCliente> CasosCliente => Set<CasoCliente>();
    public DbSet<CanalMarketing> CanalesMarketing => Set<CanalMarketing>();
    public DbSet<Campana> Campanas => Set<Campana>();
    public DbSet<CampanaCliente> CampanaClientes => Set<CampanaCliente>();
    public DbSet<CampanaInteres> CampanaIntereses => Set<CampanaInteres>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        ConfigurePersonas(modelBuilder);
        ConfigureCorreosPersona(modelBuilder);
        ConfigureTelefonosPersona(modelBuilder);
        ConfigureRoles(modelBuilder);
        ConfigureUsuarios(modelBuilder);
        ConfigurePreferenciasPublicidad(modelBuilder);
        ConfigureClientes(modelBuilder);
        ConfigureIntereses(modelBuilder);
        ConfigureClienteIntereses(modelBuilder);
        ConfigureBodegas(modelBuilder);
        ConfigureProductos(modelBuilder);
        ConfigureEstadosPedido(modelBuilder);
        ConfigurePedidos(modelBuilder);
        ConfigureDetallePedido(modelBuilder);
        ConfigureHistorialEstadosPedido(modelBuilder);
        ConfigureTiposCaso(modelBuilder);
        ConfigureEstadosCaso(modelBuilder);
        ConfigureCasosCliente(modelBuilder);
        ConfigureCanalesMarketing(modelBuilder);
        ConfigureCampanas(modelBuilder);
        ConfigureCampanaClientes(modelBuilder);
        ConfigureCampanaIntereses(modelBuilder);

        // El esquema MySQL usa nombres de columna en snake_case. Se aplica la
        // convencion a todas las propiedades escalares para no repetir
        // HasColumnName en cada una.
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            foreach (var property in entityType.GetProperties())
            {
                property.SetColumnName(ToSnakeCase(property.Name));
            }
        }
    }

    private static void ConfigurePersonas(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Persona>(entity =>
        {
            entity.ToTable("personas");
            entity.HasKey(e => e.Cedula);

            entity.Property(e => e.Cedula).HasMaxLength(30);
            entity.Property(e => e.Nombre).HasMaxLength(100);
            entity.Property(e => e.Apellidos).HasMaxLength(100);
            entity.Property(e => e.FechaCreacion).HasDefaultValueSql("CURRENT_TIMESTAMP");
        });
    }

    private static void ConfigureCorreosPersona(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<CorreoPersona>(entity =>
        {
            entity.ToTable("correos_persona");
            entity.HasKey(e => e.IdCorreo);

            entity.Property(e => e.Cedula).HasMaxLength(30);
            entity.Property(e => e.Correo).HasMaxLength(150);
            entity.Property(e => e.TipoCorreo).HasMaxLength(30);

            entity.HasIndex(e => e.Correo).IsUnique().HasDatabaseName("uq_correos_correo");
            entity.HasIndex(e => e.Cedula).HasDatabaseName("idx_correos_persona");

            entity.HasOne(e => e.Persona)
                .WithMany(p => p.CorreosPersona)
                .HasForeignKey(e => e.Cedula)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_correos_persona_personas");
        });
    }

    private static void ConfigureTelefonosPersona(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<TelefonoPersona>(entity =>
        {
            entity.ToTable("telefonos_persona");
            entity.HasKey(e => e.IdTelefono);

            entity.Property(e => e.Cedula).HasMaxLength(30);
            entity.Property(e => e.Telefono).HasMaxLength(25);
            entity.Property(e => e.TipoTelefono).HasMaxLength(30);

            entity.HasIndex(e => e.Cedula).HasDatabaseName("idx_telefonos_persona");

            entity.HasOne(e => e.Persona)
                .WithMany(p => p.TelefonosPersona)
                .HasForeignKey(e => e.Cedula)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_telefonos_persona_personas");
        });
    }

    private static void ConfigureRoles(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Rol>(entity =>
        {
            entity.ToTable("roles");
            entity.HasKey(e => e.IdRol);

            entity.Property(e => e.Nombre).HasMaxLength(50);
            entity.Property(e => e.Descripcion).HasMaxLength(255);

            entity.HasIndex(e => e.Nombre).IsUnique().HasDatabaseName("uq_roles_nombre");
        });
    }

    private static void ConfigureUsuarios(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Usuario>(entity =>
        {
            entity.ToTable("usuarios");
            entity.HasKey(e => e.IdUsuario);

            entity.Property(e => e.Cedula).HasMaxLength(30);
            entity.Property(e => e.ContrasenaHash).HasMaxLength(255);
            entity.Property(e => e.FechaCreacion).HasDefaultValueSql("CURRENT_TIMESTAMP");

            entity.HasIndex(e => e.Cedula).IsUnique().HasDatabaseName("uq_usuarios_cedula");
            entity.HasIndex(e => e.IdRol).HasDatabaseName("idx_usuarios_rol");

            entity.HasOne(e => e.Persona)
                .WithOne(p => p.Usuario)
                .HasForeignKey<Usuario>(e => e.Cedula)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_usuarios_personas");

            entity.HasOne(e => e.Rol)
                .WithMany(r => r.Usuarios)
                .HasForeignKey(e => e.IdRol)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_usuarios_roles");
        });
    }

    private static void ConfigurePreferenciasPublicidad(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<PreferenciaPublicidad>(entity =>
        {
            entity.ToTable("preferencias_publicidad");
            entity.HasKey(e => e.IdPreferenciaPublicidad);

            entity.Property(e => e.Nombre).HasMaxLength(50);
            entity.Property(e => e.Descripcion).HasMaxLength(255);

            entity.HasIndex(e => e.Nombre).IsUnique().HasDatabaseName("uq_preferencias_publicidad_nombre");
        });
    }

    private static void ConfigureClientes(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Cliente>(entity =>
        {
            entity.ToTable("clientes");
            entity.HasKey(e => e.IdCliente);

            entity.Property(e => e.Cedula).HasMaxLength(30);
            entity.Property(e => e.FechaRegistro).HasDefaultValueSql("CURRENT_TIMESTAMP");

            entity.HasIndex(e => e.Cedula).IsUnique().HasDatabaseName("uq_clientes_cedula");
            entity.HasIndex(e => e.IdPreferenciaPublicidad).HasDatabaseName("idx_clientes_preferencia");

            entity.HasOne(e => e.Persona)
                .WithOne(p => p.Cliente)
                .HasForeignKey<Cliente>(e => e.Cedula)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_clientes_personas");

            entity.HasOne(e => e.PreferenciaPublicidad)
                .WithMany(p => p.Clientes)
                .HasForeignKey(e => e.IdPreferenciaPublicidad)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_clientes_preferencias_publicidad");
        });
    }

    private static void ConfigureIntereses(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Interes>(entity =>
        {
            entity.ToTable("intereses");
            entity.HasKey(e => e.IdInteres);

            entity.Property(e => e.Nombre).HasMaxLength(80);
            entity.Property(e => e.Descripcion).HasMaxLength(255);

            entity.HasIndex(e => e.Nombre).IsUnique().HasDatabaseName("uq_intereses_nombre");
        });
    }

    private static void ConfigureClienteIntereses(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ClienteInteres>(entity =>
        {
            entity.ToTable("cliente_intereses");
            entity.HasKey(e => new { e.IdCliente, e.IdInteres });

            entity.Property(e => e.FechaAsignacion).HasDefaultValueSql("CURRENT_TIMESTAMP");

            entity.HasIndex(e => e.IdInteres).HasDatabaseName("fk_cliente_intereses_intereses");

            entity.HasOne(e => e.Cliente)
                .WithMany(c => c.ClienteIntereses)
                .HasForeignKey(e => e.IdCliente)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_cliente_intereses_clientes");

            entity.HasOne(e => e.Interes)
                .WithMany(i => i.ClienteIntereses)
                .HasForeignKey(e => e.IdInteres)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_cliente_intereses_intereses");
        });
    }

    private static void ConfigureBodegas(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Bodega>(entity =>
        {
            entity.ToTable("bodegas");
            entity.HasKey(e => e.IdBodega);

            entity.Property(e => e.Nombre).HasMaxLength(100);
            entity.Property(e => e.Direccion).HasMaxLength(255);
            entity.Property(e => e.Descripcion).HasMaxLength(255);

            entity.HasIndex(e => e.Nombre).IsUnique().HasDatabaseName("uq_bodegas_nombre");
        });
    }

    private static void ConfigureProductos(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Producto>(entity =>
        {
            entity.ToTable("productos");
            entity.HasKey(e => e.IdProducto);

            entity.Property(e => e.Nombre).HasMaxLength(150);
            entity.Property(e => e.Precio).HasPrecision(10, 2);
            entity.Property(e => e.FechaCreacion).HasDefaultValueSql("CURRENT_TIMESTAMP");

            entity.HasIndex(e => e.IdBodega).HasDatabaseName("idx_productos_bodega");
            entity.HasIndex(e => e.Nombre).HasDatabaseName("idx_productos_nombre");

            entity.HasOne(e => e.Bodega)
                .WithMany(b => b.Productos)
                .HasForeignKey(e => e.IdBodega)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_productos_bodegas");
        });
    }

    private static void ConfigureEstadosPedido(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<EstadoPedido>(entity =>
        {
            entity.ToTable("estados_pedido");
            entity.HasKey(e => e.IdEstadoPedido);

            entity.Property(e => e.Nombre).HasMaxLength(40);
            entity.Property(e => e.Descripcion).HasMaxLength(255);

            entity.HasIndex(e => e.Nombre).IsUnique().HasDatabaseName("uq_estados_pedido_nombre");
        });
    }

    private static void ConfigurePedidos(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Pedido>(entity =>
        {
            entity.ToTable("pedidos");
            entity.HasKey(e => e.IdPedido);

            entity.Property(e => e.Total).HasPrecision(10, 2);
            entity.Property(e => e.DireccionEnvio).HasMaxLength(255);
            entity.Property(e => e.FechaPedido).HasDefaultValueSql("CURRENT_TIMESTAMP");

            entity.HasIndex(e => new { e.IdCliente, e.FechaPedido }).HasDatabaseName("idx_pedidos_cliente_fecha");
            entity.HasIndex(e => e.IdEstadoPedido).HasDatabaseName("idx_pedidos_estado");

            entity.HasOne(e => e.Cliente)
                .WithMany(c => c.Pedidos)
                .HasForeignKey(e => e.IdCliente)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_pedidos_clientes");

            entity.HasOne(e => e.EstadoPedido)
                .WithMany(s => s.Pedidos)
                .HasForeignKey(e => e.IdEstadoPedido)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_pedidos_estados_pedido");
        });
    }

    private static void ConfigureDetallePedido(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<DetallePedido>(entity =>
        {
            entity.ToTable("detalle_pedido");
            entity.HasKey(e => e.IdDetalle);

            entity.Property(e => e.PrecioUnitario).HasPrecision(10, 2);

            entity.HasIndex(e => e.IdPedido).HasDatabaseName("idx_detalle_pedido");
            entity.HasIndex(e => e.IdProducto).HasDatabaseName("idx_detalle_producto");

            entity.HasOne(e => e.Pedido)
                .WithMany(p => p.DetallePedido)
                .HasForeignKey(e => e.IdPedido)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_detalle_pedido_pedidos");

            entity.HasOne(e => e.Producto)
                .WithMany(p => p.DetallePedido)
                .HasForeignKey(e => e.IdProducto)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_detalle_pedido_productos");
        });
    }

    private static void ConfigureHistorialEstadosPedido(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<HistorialEstadoPedido>(entity =>
        {
            entity.ToTable("historial_estados_pedido");
            entity.HasKey(e => e.IdHistorial);

            entity.Property(e => e.Observacion).HasMaxLength(255);
            entity.Property(e => e.FechaCambio).HasDefaultValueSql("CURRENT_TIMESTAMP");

            entity.HasIndex(e => new { e.IdPedido, e.FechaCambio }).HasDatabaseName("idx_historial_pedido_fecha");
            entity.HasIndex(e => e.IdEstadoPedido).HasDatabaseName("fk_historial_pedido_estados");
            entity.HasIndex(e => e.IdUsuario).HasDatabaseName("fk_historial_pedido_usuarios");

            entity.HasOne(e => e.Pedido)
                .WithMany(p => p.HistorialEstadosPedido)
                .HasForeignKey(e => e.IdPedido)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_historial_pedido_pedidos");

            entity.HasOne(e => e.EstadoPedido)
                .WithMany(s => s.HistorialEstadosPedido)
                .HasForeignKey(e => e.IdEstadoPedido)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_historial_pedido_estados");

            entity.HasOne(e => e.Usuario)
                .WithMany(u => u.HistorialEstadosPedido)
                .HasForeignKey(e => e.IdUsuario)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_historial_pedido_usuarios");
        });
    }

    private static void ConfigureTiposCaso(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<TipoCaso>(entity =>
        {
            entity.ToTable("tipos_caso");
            entity.HasKey(e => e.IdTipoCaso);

            entity.Property(e => e.Nombre).HasMaxLength(50);
            entity.Property(e => e.Descripcion).HasMaxLength(255);

            entity.HasIndex(e => e.Nombre).IsUnique().HasDatabaseName("uq_tipos_caso_nombre");
        });
    }

    private static void ConfigureEstadosCaso(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<EstadoCaso>(entity =>
        {
            entity.ToTable("estados_caso");
            entity.HasKey(e => e.IdEstadoCaso);

            entity.Property(e => e.Nombre).HasMaxLength(40);
            entity.Property(e => e.Descripcion).HasMaxLength(255);

            entity.HasIndex(e => e.Nombre).IsUnique().HasDatabaseName("uq_estados_caso_nombre");
        });
    }

    private static void ConfigureCasosCliente(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<CasoCliente>(entity =>
        {
            entity.ToTable("casos_cliente");
            entity.HasKey(e => e.IdCaso);

            entity.Property(e => e.Canal).HasMaxLength(40);
            entity.Property(e => e.Asunto).HasMaxLength(150);
            entity.Property(e => e.FechaCreacion).HasDefaultValueSql("CURRENT_TIMESTAMP");

            entity.HasIndex(e => new { e.IdCliente, e.IdEstadoCaso }).HasDatabaseName("idx_casos_cliente_estado");
            entity.HasIndex(e => e.IdUsuarioAsignado).HasDatabaseName("idx_casos_asignado");
            entity.HasIndex(e => e.IdTipoCaso).HasDatabaseName("fk_casos_cliente_tipos");
            entity.HasIndex(e => e.IdEstadoCaso).HasDatabaseName("fk_casos_cliente_estados");

            entity.HasOne(e => e.Cliente)
                .WithMany(c => c.CasosCliente)
                .HasForeignKey(e => e.IdCliente)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_casos_cliente_clientes");

            entity.HasOne(e => e.TipoCaso)
                .WithMany(t => t.CasosCliente)
                .HasForeignKey(e => e.IdTipoCaso)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_casos_cliente_tipos");

            entity.HasOne(e => e.EstadoCaso)
                .WithMany(s => s.CasosCliente)
                .HasForeignKey(e => e.IdEstadoCaso)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_casos_cliente_estados");

            entity.HasOne(e => e.UsuarioAsignado)
                .WithMany(u => u.CasosClienteAsignados)
                .HasForeignKey(e => e.IdUsuarioAsignado)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_casos_cliente_usuarios");
        });
    }

    private static void ConfigureCanalesMarketing(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<CanalMarketing>(entity =>
        {
            entity.ToTable("canales_marketing");
            entity.HasKey(e => e.IdCanal);

            entity.Property(e => e.Nombre).HasMaxLength(50);
            entity.Property(e => e.Descripcion).HasMaxLength(255);

            entity.HasIndex(e => e.Nombre).IsUnique().HasDatabaseName("uq_canales_marketing_nombre");
        });
    }

    private static void ConfigureCampanas(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Campana>(entity =>
        {
            entity.ToTable("campanas");
            entity.HasKey(e => e.IdCampana);

            entity.Property(e => e.Nombre).HasMaxLength(150);
            entity.Property(e => e.Estado).HasMaxLength(30);
            entity.Property(e => e.FechaCreacion).HasDefaultValueSql("CURRENT_TIMESTAMP");

            entity.HasIndex(e => new { e.Estado, e.FechaProgramada }).HasDatabaseName("idx_campanas_fecha_estado");
            entity.HasIndex(e => e.IdCanal).HasDatabaseName("fk_campanas_canales");
            entity.HasIndex(e => e.IdUsuarioCreador).HasDatabaseName("fk_campanas_usuarios");

            entity.HasOne(e => e.Canal)
                .WithMany(c => c.Campanas)
                .HasForeignKey(e => e.IdCanal)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_campanas_canales");

            entity.HasOne(e => e.UsuarioCreador)
                .WithMany(u => u.CampanasCreadas)
                .HasForeignKey(e => e.IdUsuarioCreador)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_campanas_usuarios");
        });
    }

    private static void ConfigureCampanaClientes(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<CampanaCliente>(entity =>
        {
            entity.ToTable("campana_clientes");
            entity.HasKey(e => e.IdCampanaCliente);

            entity.Property(e => e.EstadoEnvio).HasMaxLength(30);

            entity.HasIndex(e => new { e.IdCampana, e.IdCliente }).IsUnique().HasDatabaseName("uq_campana_cliente");
            entity.HasIndex(e => new { e.IdCampana, e.EstadoEnvio }).HasDatabaseName("idx_campana_clientes_estado");
            entity.HasIndex(e => e.IdCliente).HasDatabaseName("fk_campana_clientes_clientes");

            entity.HasOne(e => e.Campana)
                .WithMany(c => c.CampanaClientes)
                .HasForeignKey(e => e.IdCampana)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_campana_clientes_campanas");

            entity.HasOne(e => e.Cliente)
                .WithMany(c => c.CampanaClientes)
                .HasForeignKey(e => e.IdCliente)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_campana_clientes_clientes");
        });
    }

    private static void ConfigureCampanaIntereses(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<CampanaInteres>(entity =>
        {
            entity.ToTable("campana_intereses");
            entity.HasKey(e => new { e.IdCampana, e.IdInteres });

            entity.HasIndex(e => e.IdInteres).HasDatabaseName("fk_campana_intereses_intereses");

            entity.HasOne(e => e.Campana)
                .WithMany(c => c.CampanaIntereses)
                .HasForeignKey(e => e.IdCampana)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_campana_intereses_campanas");

            entity.HasOne(e => e.Interes)
                .WithMany(i => i.CampanaIntereses)
                .HasForeignKey(e => e.IdInteres)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_campana_intereses_intereses");
        });
    }

    /// <summary>
    /// Convierte PascalCase a snake_case (IdPreferenciaPublicidad -> id_preferencia_publicidad).
    /// </summary>
    private static string ToSnakeCase(string name)
    {
        var builder = new System.Text.StringBuilder(name.Length + 8);

        for (var i = 0; i < name.Length; i++)
        {
            if (char.IsUpper(name[i]) && i > 0)
            {
                builder.Append('_');
            }

            builder.Append(char.ToLowerInvariant(name[i]));
        }

        return builder.ToString();
    }
}

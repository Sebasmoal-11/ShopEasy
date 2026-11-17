using Microsoft.EntityFrameworkCore;
using ShopEasy.DA.Entities;

namespace ShopEasy.DA.Data;

public class ClientesDA
{
    private readonly ShopEasyDbContext _context;

    public ClientesDA(ShopEasyDbContext context)
    {
        _context = context;
    }

    public async Task<List<ClienteDestacado>> ObtenerDestacadosAsync(int limite)
    {
        var filas = await (
                from cliente in _context.Clientes.AsNoTracking()
                join persona in _context.Personas.AsNoTracking()
                    on cliente.Cedula equals persona.Cedula
                join pedido in _context.Pedidos.AsNoTracking()
                    on cliente.IdCliente equals pedido.IdCliente
                group pedido by new
                {
                    cliente.IdCliente,
                    cliente.Cedula,
                    persona.Nombre,
                    persona.Apellidos
                }
                into grupo
                orderby grupo.Count() descending,
                    grupo.Sum(pedido => pedido.Total) descending
                select new
                {
                    grupo.Key.IdCliente,
                    grupo.Key.Cedula,
                    grupo.Key.Nombre,
                    grupo.Key.Apellidos,
                    CantidadPedidos = grupo.Count(),
                    MontoTotal = grupo.Sum(pedido => pedido.Total)
                })
            .Take(limite)
            .ToListAsync();

        if (filas.Count == 0)
        {
            return new List<ClienteDestacado>();
        }

        var cedulas = filas.Select(fila => fila.Cedula).ToList();

        var correos = await _context.CorreosPersona
            .AsNoTracking()
            .Where(correo => cedulas.Contains(correo.Cedula) && correo.Activo)
            .OrderByDescending(correo => correo.Principal)
            .ThenBy(correo => correo.IdCorreo)
            .Select(correo => new { correo.Cedula, correo.Correo })
            .ToListAsync();

        var mapaCorreos = correos
            .GroupBy(correo => correo.Cedula)
            .ToDictionary(grupo => grupo.Key, grupo => grupo.First().Correo);

        return filas
            .Select(fila => new ClienteDestacado(
                fila.IdCliente,
                $"{fila.Nombre} {fila.Apellidos}".Trim(),
                mapaCorreos.GetValueOrDefault(fila.Cedula),
                fila.CantidadPedidos,
                fila.MontoTotal))
            .ToList();
    }

    public async Task<PagedResult<Cliente>> ObtenerListadoAsync(
        string? buscar,
        bool? activo,
        int pagina,
        int tamano)
    {
        var consulta = _context.Clientes
            .AsNoTracking()
            .Include(cliente => cliente.Persona)
                .ThenInclude(persona => persona.CorreosPersona)
            .Include(cliente => cliente.Persona)
                .ThenInclude(persona => persona.TelefonosPersona)
            .Include(cliente => cliente.PreferenciaPublicidad)
            .AsQueryable();

        if (activo.HasValue)
        {
            consulta = consulta.Where(cliente => cliente.Activo == activo.Value);
        }

        if (!string.IsNullOrWhiteSpace(buscar))
        {
            var termino = buscar.Trim();

            consulta = consulta.Where(cliente =>
                cliente.Cedula.Contains(termino) ||
                cliente.Persona.Nombre.Contains(termino) ||
                cliente.Persona.Apellidos.Contains(termino) ||
                cliente.Persona.CorreosPersona.Any(correo =>
                    correo.Correo.Contains(termino)) ||
                cliente.Persona.TelefonosPersona.Any(telefono =>
                    telefono.Telefono.Contains(termino)));
        }

        var total = await consulta.CountAsync();

        var items = await consulta
            .OrderBy(cliente => cliente.Persona.Apellidos)
            .ThenBy(cliente => cliente.Persona.Nombre)
            .ThenBy(cliente => cliente.IdCliente)
            .Skip((pagina - 1) * tamano)
            .Take(tamano)
            .ToListAsync();

        return new PagedResult<Cliente>(total, items);
    }

    public Task<Cliente?> ObtenerDetalleAsync(int idCliente)
    {
        return _context.Clientes
            .AsNoTracking()
            .Include(cliente => cliente.Persona)
                .ThenInclude(persona => persona.CorreosPersona)
            .Include(cliente => cliente.Persona)
                .ThenInclude(persona => persona.TelefonosPersona)
            .Include(cliente => cliente.PreferenciaPublicidad)
            .FirstOrDefaultAsync(cliente => cliente.IdCliente == idCliente);
    }

    public Task<Cliente?> ObtenerParaEdicionAsync(int idCliente)
    {
        return _context.Clientes
            .Include(cliente => cliente.Persona)
                .ThenInclude(persona => persona.CorreosPersona)
            .Include(cliente => cliente.Persona)
                .ThenInclude(persona => persona.TelefonosPersona)
            .Include(cliente => cliente.PreferenciaPublicidad)
            .FirstOrDefaultAsync(cliente => cliente.IdCliente == idCliente);
    }

    public Task<Cliente?> ObtenerActivoAsync(int idCliente)
    {
        return _context.Clientes
            .AsNoTracking()
            .FirstOrDefaultAsync(cliente =>
                cliente.IdCliente == idCliente && cliente.Activo);
    }

    public Task<bool> ExisteCedulaAsync(string cedula)
    {
        return _context.Personas.AnyAsync(persona => persona.Cedula == cedula);
    }

    public Task<bool> ExisteCorreoAsync(string correo, string? cedulaExcluida = null)
    {
        var consulta = _context.CorreosPersona
            .AsNoTracking()
            .Where(correoPersona => correoPersona.Correo.ToLower() == correo);

        if (!string.IsNullOrWhiteSpace(cedulaExcluida))
        {
            consulta = consulta.Where(correoPersona =>
                correoPersona.Cedula != cedulaExcluida);
        }

        return consulta.AnyAsync();
    }

    public Task<List<PreferenciaPublicidad>> ObtenerPreferenciasAsync()
    {
        return _context.PreferenciasPublicidad
            .AsNoTracking()
            .OrderBy(preferencia => preferencia.IdPreferenciaPublicidad)
            .ToListAsync();
    }

    public Task<PreferenciaPublicidad?> ObtenerPreferenciaAsync(int idPreferencia)
    {
        return _context.PreferenciasPublicidad
            .AsNoTracking()
            .FirstOrDefaultAsync(preferencia =>
                preferencia.IdPreferenciaPublicidad == idPreferencia);
    }

    public Task<PreferenciaPublicidad?> ObtenerPrimeraPreferenciaAsync()
    {
        return _context.PreferenciasPublicidad
            .AsNoTracking()
            .OrderBy(preferencia => preferencia.IdPreferenciaPublicidad)
            .FirstOrDefaultAsync();
    }

    public async Task CrearAsync(Cliente cliente)
    {
        _context.Clientes.Add(cliente);
        await _context.SaveChangesAsync();
    }

    public Task ActualizarAsync(Cliente cliente)
    {
        return _context.SaveChangesAsync();
    }
}

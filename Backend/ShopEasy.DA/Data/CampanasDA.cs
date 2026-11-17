using Microsoft.EntityFrameworkCore;
using ShopEasy.DA.Entities;

namespace ShopEasy.DA.Data;

public class CampanasDA
{
    private readonly ShopEasyDbContext _context;

    public CampanasDA(ShopEasyDbContext context)
    {
        _context = context;
    }

    public async Task<List<ConteoEstadoCampana>> ObtenerConteosPorEstadoAsync()
    {
        var filas = await _context.Campanas
            .AsNoTracking()
            .GroupBy(campana => campana.Estado)
            .Select(grupo => new
            {
                Estado = grupo.Key,
                Cantidad = grupo.Count()
            })
            .OrderBy(fila => fila.Estado)
            .ToListAsync();

        return filas
            .Select(fila => new ConteoEstadoCampana(fila.Estado, fila.Cantidad))
            .ToList();
    }

    public async Task<List<ProximaCampana>> ObtenerProximasAsync(DateTime desde, int limite)
    {
        var filas = await _context.Campanas
            .AsNoTracking()
            .Where(campana => campana.FechaProgramada >= desde)
            .OrderBy(campana => campana.FechaProgramada)
            .Take(limite)
            .Select(campana => new
            {
                campana.IdCampana,
                campana.Nombre,
                Canal = campana.Canal.Nombre,
                campana.FechaProgramada,
                campana.Estado
            })
            .ToListAsync();

        return filas
            .Select(fila => new ProximaCampana(
                fila.IdCampana,
                fila.Nombre,
                fila.Canal,
                fila.FechaProgramada,
                fila.Estado))
            .ToList();
    }

    public async Task<PagedResult<Campana>> ObtenerListadoAsync(
        string? buscar,
        string? estado,
        int? idCanal,
        int pagina,
        int tamano)
    {
        var consulta = _context.Campanas
            .AsNoTracking()
            .Include(campana => campana.Canal)
            .Include(campana => campana.UsuarioCreador)
                .ThenInclude(usuario => usuario.Persona)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(estado))
        {
            var estadoBuscado = estado.Trim();
            consulta = consulta.Where(campana =>
                campana.Estado.ToLower() == estadoBuscado.ToLower());
        }

        if (idCanal.HasValue)
        {
            consulta = consulta.Where(campana => campana.IdCanal == idCanal.Value);
        }

        if (!string.IsNullOrWhiteSpace(buscar))
        {
            var termino = buscar.Trim();

            consulta = consulta.Where(campana =>
                campana.Nombre.Contains(termino) ||
                (campana.Descripcion != null &&
                 campana.Descripcion.Contains(termino)));
        }

        var total = await consulta.CountAsync();

        var items = await consulta
            .OrderByDescending(campana => campana.FechaProgramada)
            .ThenByDescending(campana => campana.IdCampana)
            .Skip((pagina - 1) * tamano)
            .Take(tamano)
            .ToListAsync();

        return new PagedResult<Campana>(total, items);
    }

    public Task<Campana?> ObtenerDetalleAsync(int idCampana)
    {
        return _context.Campanas
            .AsNoTracking()
            .Include(campana => campana.Canal)
            .Include(campana => campana.UsuarioCreador)
                .ThenInclude(usuario => usuario.Persona)
            .Include(campana => campana.CampanaClientes)
                .ThenInclude(campanaCliente => campanaCliente.Cliente)
                    .ThenInclude(cliente => cliente.Persona)
                        .ThenInclude(persona => persona.CorreosPersona)
            .FirstOrDefaultAsync(campana => campana.IdCampana == idCampana);
    }

    public Task<Campana?> ObtenerParaEdicionAsync(int idCampana)
    {
        return _context.Campanas
            .Include(campana => campana.Canal)
            .Include(campana => campana.UsuarioCreador)
                .ThenInclude(usuario => usuario.Persona)
            .Include(campana => campana.CampanaClientes)
                .ThenInclude(campanaCliente => campanaCliente.Cliente)
                    .ThenInclude(cliente => cliente.Persona)
                        .ThenInclude(persona => persona.CorreosPersona)
            .FirstOrDefaultAsync(campana => campana.IdCampana == idCampana);
    }

    public Task<List<CanalMarketing>> ObtenerCanalesAsync()
    {
        return _context.CanalesMarketing
            .AsNoTracking()
            .OrderBy(canal => canal.Nombre)
            .ToListAsync();
    }

    public Task<CanalMarketing?> ObtenerCanalAsync(int idCanal)
    {
        return _context.CanalesMarketing
            .AsNoTracking()
            .FirstOrDefaultAsync(canal => canal.IdCanal == idCanal);
    }

    public Task<List<string>> ObtenerEstadosAsync()
    {
        return _context.Campanas
            .AsNoTracking()
            .Select(campana => campana.Estado)
            .Distinct()
            .OrderBy(estado => estado)
            .ToListAsync();
    }

    public async Task<List<int>> ObtenerIdsClientesExistentesAsync(
        IReadOnlyCollection<int> idsClientes)
    {
        return await _context.Clientes
            .AsNoTracking()
            .Where(cliente => idsClientes.Contains(cliente.IdCliente))
            .Select(cliente => cliente.IdCliente)
            .ToListAsync();
    }

    public async Task CrearAsync(Campana campana)
    {
        _context.Campanas.Add(campana);
        await _context.SaveChangesAsync();
    }

    public Task ActualizarAsync(Campana campana)
    {
        return _context.SaveChangesAsync();
    }

    public async Task<bool> AgregarClientesAsync(
        int idCampana,
        IReadOnlyCollection<int> idsClientes)
    {
        var campana = await _context.Campanas
            .Include(c => c.CampanaClientes)
            .FirstOrDefaultAsync(c => c.IdCampana == idCampana);

        if (campana is null)
        {
            return false;
        }

        var existentes = campana.CampanaClientes
            .Select(campanaCliente => campanaCliente.IdCliente)
            .ToHashSet();

        foreach (var idCliente in idsClientes.Distinct())
        {
            if (existentes.Contains(idCliente))
            {
                continue;
            }

            campana.CampanaClientes.Add(new CampanaCliente
            {
                IdCampana = idCampana,
                IdCliente = idCliente,
                EstadoEnvio = "Pendiente"
            });
        }

        await _context.SaveChangesAsync();
        return true;
    }

    public Task<CampanaCliente?> ObtenerCampanaClienteAsync(
        int idCampana,
        int idCliente)
    {
        return _context.CampanaClientes
            .Include(campanaCliente => campanaCliente.Cliente)
                .ThenInclude(cliente => cliente.Persona)
                    .ThenInclude(persona => persona.CorreosPersona)
            .FirstOrDefaultAsync(campanaCliente =>
                campanaCliente.IdCampana == idCampana &&
                campanaCliente.IdCliente == idCliente);
    }

    public Task<List<CampanaCliente>> ObtenerClientesAsync(int idCampana)
    {
        return _context.CampanaClientes
            .AsNoTracking()
            .Include(campanaCliente => campanaCliente.Cliente)
                .ThenInclude(cliente => cliente.Persona)
                    .ThenInclude(persona => persona.CorreosPersona)
            .Where(campanaCliente => campanaCliente.IdCampana == idCampana)
            .OrderBy(campanaCliente => campanaCliente.IdCampanaCliente)
            .ToListAsync();
    }

    public Task ActualizarEnvioAsync(CampanaCliente campanaCliente)
    {
        return _context.SaveChangesAsync();
    }
}

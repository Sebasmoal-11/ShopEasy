using Microsoft.EntityFrameworkCore;
using ShopEasy.DA.Entities;

namespace ShopEasy.DA.Data;

public class AtencionClienteDA
{
    private readonly ShopEasyDbContext _context;

    public AtencionClienteDA(ShopEasyDbContext context)
    {
        _context = context;
    }

    public async Task<List<ConteoEstadoCaso>> ObtenerConteosPorEstadoAsync()
    {
        var estados = await _context.EstadosCaso
            .AsNoTracking()
            .OrderBy(estado => estado.Nombre)
            .Select(estado => new { estado.IdEstadoCaso, estado.Nombre })
            .ToListAsync();

        var conteos = await _context.CasosCliente
            .AsNoTracking()
            .GroupBy(caso => caso.IdEstadoCaso)
            .Select(grupo => new
            {
                IdEstadoCaso = grupo.Key,
                Cantidad = grupo.Count()
            })
            .ToListAsync();

        var mapa = conteos.ToDictionary(conteo => conteo.IdEstadoCaso, conteo => conteo.Cantidad);

        return estados
            .Select(estado => new ConteoEstadoCaso(
                estado.IdEstadoCaso,
                estado.Nombre,
                mapa.GetValueOrDefault(estado.IdEstadoCaso)))
            .ToList();
    }

    public async Task<List<ConteoTipoCaso>> ObtenerConteosPorTipoAsync()
    {
        var tipos = await _context.TiposCaso
            .AsNoTracking()
            .OrderBy(tipo => tipo.Nombre)
            .Select(tipo => new { tipo.IdTipoCaso, tipo.Nombre })
            .ToListAsync();

        var conteos = await _context.CasosCliente
            .AsNoTracking()
            .GroupBy(caso => caso.IdTipoCaso)
            .Select(grupo => new
            {
                IdTipoCaso = grupo.Key,
                Cantidad = grupo.Count()
            })
            .ToListAsync();

        var mapa = conteos.ToDictionary(conteo => conteo.IdTipoCaso, conteo => conteo.Cantidad);

        return tipos
            .Select(tipo => new ConteoTipoCaso(
                tipo.IdTipoCaso,
                tipo.Nombre,
                mapa.GetValueOrDefault(tipo.IdTipoCaso)))
            .ToList();
    }

    public async Task<List<ConteoCanalCaso>> ObtenerConteosPorCanalAsync()
    {
        var filas = await _context.CasosCliente
            .AsNoTracking()
            .GroupBy(caso => caso.Canal)
            .Select(grupo => new
            {
                Canal = grupo.Key,
                Cantidad = grupo.Count()
            })
            .OrderByDescending(fila => fila.Cantidad)
            .ThenBy(fila => fila.Canal)
            .ToListAsync();

        return filas
            .Select(fila => new ConteoCanalCaso(fila.Canal, fila.Cantidad))
            .ToList();
    }

    public async Task<PagedResult<CasoCliente>> ObtenerListadoAsync(
        string? buscar,
        int? idCliente,
        int? idEstadoCaso,
        int? idTipoCaso,
        int? idUsuarioAsignado,
        int pagina,
        int tamano)
    {
        var consulta = _context.CasosCliente
            .AsNoTracking()
            .Include(caso => caso.Cliente)
                .ThenInclude(cliente => cliente.Persona)
            .Include(caso => caso.TipoCaso)
            .Include(caso => caso.EstadoCaso)
            .Include(caso => caso.UsuarioAsignado)
                .ThenInclude(usuario => usuario!.Persona)
            .AsQueryable();

        if (idCliente.HasValue)
        {
            consulta = consulta.Where(caso => caso.IdCliente == idCliente.Value);
        }

        if (idEstadoCaso.HasValue)
        {
            consulta = consulta.Where(caso => caso.IdEstadoCaso == idEstadoCaso.Value);
        }

        if (idTipoCaso.HasValue)
        {
            consulta = consulta.Where(caso => caso.IdTipoCaso == idTipoCaso.Value);
        }

        if (idUsuarioAsignado.HasValue)
        {
            consulta = consulta.Where(caso =>
                caso.IdUsuarioAsignado == idUsuarioAsignado.Value);
        }

        if (!string.IsNullOrWhiteSpace(buscar))
        {
            var termino = buscar.Trim();

            consulta = consulta.Where(caso =>
                caso.Asunto.Contains(termino) ||
                caso.Canal.Contains(termino) ||
                caso.Cliente.Cedula.Contains(termino) ||
                caso.Cliente.Persona.Nombre.Contains(termino) ||
                caso.Cliente.Persona.Apellidos.Contains(termino));
        }

        var total = await consulta.CountAsync();

        var items = await consulta
            .OrderByDescending(caso => caso.FechaCreacion)
            .ThenByDescending(caso => caso.IdCaso)
            .Skip((pagina - 1) * tamano)
            .Take(tamano)
            .ToListAsync();

        return new PagedResult<CasoCliente>(total, items);
    }

    public Task<CasoCliente?> ObtenerDetalleAsync(int idCaso)
    {
        return _context.CasosCliente
            .AsNoTracking()
            .Include(caso => caso.Cliente)
                .ThenInclude(cliente => cliente.Persona)
            .Include(caso => caso.TipoCaso)
            .Include(caso => caso.EstadoCaso)
            .Include(caso => caso.UsuarioAsignado)
                .ThenInclude(usuario => usuario!.Persona)
            .FirstOrDefaultAsync(caso => caso.IdCaso == idCaso);
    }

    public Task<CasoCliente?> ObtenerParaEdicionAsync(int idCaso)
    {
        return _context.CasosCliente
            .Include(caso => caso.Cliente)
                .ThenInclude(cliente => cliente.Persona)
            .Include(caso => caso.TipoCaso)
            .Include(caso => caso.EstadoCaso)
            .Include(caso => caso.UsuarioAsignado)
                .ThenInclude(usuario => usuario!.Persona)
            .FirstOrDefaultAsync(caso => caso.IdCaso == idCaso);
    }

    public Task<List<TipoCaso>> ObtenerTiposAsync()
    {
        return _context.TiposCaso
            .AsNoTracking()
            .OrderBy(tipo => tipo.Nombre)
            .ToListAsync();
    }

    public Task<TipoCaso?> ObtenerTipoAsync(int idTipoCaso)
    {
        return _context.TiposCaso
            .AsNoTracking()
            .FirstOrDefaultAsync(tipo => tipo.IdTipoCaso == idTipoCaso);
    }

    public Task<List<EstadoCaso>> ObtenerEstadosAsync()
    {
        return _context.EstadosCaso
            .AsNoTracking()
            .OrderBy(estado => estado.Nombre)
            .ToListAsync();
    }

    public Task<EstadoCaso?> ObtenerEstadoAsync(int idEstadoCaso)
    {
        return _context.EstadosCaso
            .AsNoTracking()
            .FirstOrDefaultAsync(estado => estado.IdEstadoCaso == idEstadoCaso);
    }

    public async Task<EstadoCaso?> ObtenerEstadoInicialAsync()
    {
        var pendiente = await _context.EstadosCaso
            .AsNoTracking()
            .Where(estado => estado.Nombre.Contains("Pendient"))
            .OrderBy(estado => estado.IdEstadoCaso)
            .FirstOrDefaultAsync();

        return pendiente ?? await _context.EstadosCaso
            .AsNoTracking()
            .OrderBy(estado => estado.IdEstadoCaso)
            .FirstOrDefaultAsync();
    }

    public Task<List<UsuarioAsignable>> ObtenerUsuariosAsignablesAsync()
    {
        return _context.Usuarios
            .AsNoTracking()
            .Include(usuario => usuario.Persona)
            .Include(usuario => usuario.Rol)
            .Where(usuario => usuario.Activo)
            .OrderBy(usuario => usuario.Persona.Nombre)
            .ThenBy(usuario => usuario.Persona.Apellidos)
            .Select(usuario => new UsuarioAsignable(
                usuario.IdUsuario,
                $"{usuario.Persona.Nombre} {usuario.Persona.Apellidos}".Trim(),
                usuario.Rol.Nombre,
                usuario.Activo))
            .ToListAsync();
    }

    public Task<Usuario?> ObtenerUsuarioActivoAsync(int idUsuario)
    {
        return _context.Usuarios
            .AsNoTracking()
            .Include(usuario => usuario.Persona)
            .FirstOrDefaultAsync(usuario =>
                usuario.IdUsuario == idUsuario && usuario.Activo);
    }

    public async Task CrearAsync(CasoCliente caso)
    {
        _context.CasosCliente.Add(caso);
        await _context.SaveChangesAsync();
    }

    public Task ActualizarAsync(CasoCliente caso)
    {
        return _context.SaveChangesAsync();
    }
}

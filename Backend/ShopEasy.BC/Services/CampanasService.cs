using ShopEasy.BC.Common;
using ShopEasy.BW.Campanas;
using ShopEasy.BW.Common;
using ShopEasy.BW.Dashboard;
using ShopEasy.DA.Data;
using ShopEasy.DA.Entities;

namespace ShopEasy.BC.Services;

public class CampanasService
{
    private readonly CampanasDA _campanasDA;
    private readonly UserDA _userDA;

    public CampanasService(CampanasDA campanasDA, UserDA userDA)
    {
        _campanasDA = campanasDA;
        _userDA = userDA;
    }

    public async Task<CampanasResponse> ObtenerEstadisticasAsync(int limite)
    {
        if (limite is < 1 or > 20)
        {
            throw new ApiException(400, "El limite debe estar entre 1 y 20.");
        }

        var porEstado = await _campanasDA.ObtenerConteosPorEstadoAsync();
        var proximas = await _campanasDA.ObtenerProximasAsync(DateTime.Now, limite);

        return new CampanasResponse(
            porEstado.Sum(estado => estado.Cantidad),
            porEstado
                .Where(estado => ClasificadorEstados.EsCampanaBorrador(estado.Estado))
                .Sum(estado => estado.Cantidad),
            porEstado
                .Where(estado => ClasificadorEstados.EsCampanaProgramada(estado.Estado))
                .Sum(estado => estado.Cantidad),
            porEstado
                .Where(estado => ClasificadorEstados.EsCampanaEnviadaOFinalizada(estado.Estado))
                .Sum(estado => estado.Cantidad),
            porEstado
                .Select(estado => new CampanaPorEstadoDto(estado.Estado, estado.Cantidad))
                .ToList(),
            proximas
                .Select(campana => new ProximaCampanaDto(
                    campana.IdCampana,
                    campana.Nombre,
                    campana.Canal,
                    campana.FechaProgramada,
                    campana.Estado))
                .ToList());
    }

    public async Task<PagedResponse<CampanaResponse>> ObtenerCampanasAsync(
        string? buscar,
        string? estado,
        int? idCanal,
        int pagina,
        int tamano)
    {
        (pagina, tamano) = Validador.Paginacion(pagina, tamano);

        var resultado = await _campanasDA.ObtenerListadoAsync(
            buscar,
            estado,
            idCanal,
            pagina,
            tamano);

        return new PagedResponse<CampanaResponse>(
            resultado.Total,
            pagina,
            tamano,
            resultado.Items.Select(Mapear).ToList());
    }

    public async Task<CampanaDetalleResponse> ObtenerCampanaAsync(int idCampana)
    {
        var campana = await _campanasDA.ObtenerDetalleAsync(idCampana);

        if (campana is null)
        {
            throw new ApiException(404, "La campana no existe.");
        }

        return MapearDetalle(campana);
    }

    public async Task<CampanaDetalleResponse> CrearCampanaAsync(
        CrearCampanaRequest request,
        int idUsuario)
    {
        if (request is null)
        {
            throw new ApiException(400, "Los datos de la campana son obligatorios.");
        }

        var nombre = Validador.Texto(request.Nombre, "El nombre de la campana", 150, 2);
        var descripcion = Validador.TextoOpcional(request.Descripcion, "La descripcion", 4000);
        var contenido = Validador.Texto(request.Contenido, "El contenido", 8000, 3);
        var estado = Validador.TextoOpcional(request.Estado, "El estado", 30) ?? "Borrador";

        Validador.Fecha(request.FechaProgramada, "La fecha programada");

        var canal = await _campanasDA.ObtenerCanalAsync(request.IdCanal);

        if (canal is null || !canal.Activo)
        {
            throw new ApiException(400, "El canal de marketing no existe o esta inactivo.");
        }

        var creador = await _userDA.ObtenerActivoAsync(idUsuario);

        if (creador is null)
        {
            throw new ApiException(401, "La sesion ya no es valida.");
        }

        var idsClientes = request.IdClientes?
            .Where(id => id > 0)
            .Distinct()
            .ToList() ?? new List<int>();

        if (request.IdClientes is not null && idsClientes.Count != request.IdClientes.Distinct().Count())
        {
            throw new ApiException(400, "La lista de clientes contiene identificadores invalidos.");
        }

        await ValidarClientesAsync(idsClientes);

        var campana = new Campana
        {
            Nombre = nombre,
            Descripcion = descripcion,
            IdCanal = canal.IdCanal,
            FechaProgramada = request.FechaProgramada,
            Estado = estado,
            Contenido = contenido,
            FechaCreacion = DateTime.Now,
            IdUsuarioCreador = creador.IdUsuario
        };

        foreach (var idCliente in idsClientes)
        {
            campana.CampanaClientes.Add(new CampanaCliente
            {
                IdCliente = idCliente,
                EstadoEnvio = "Pendiente"
            });
        }

        await _campanasDA.CrearAsync(campana);

        return await ObtenerCampanaAsync(campana.IdCampana);
    }

    public async Task<CampanaDetalleResponse> ActualizarCampanaAsync(
        int idCampana,
        ActualizarCampanaRequest request)
    {
        if (request is null)
        {
            throw new ApiException(400, "Los datos de la campana son obligatorios.");
        }

        var campana = await _campanasDA.ObtenerParaEdicionAsync(idCampana);

        if (campana is null)
        {
            throw new ApiException(404, "La campana no existe.");
        }

        var nombre = Validador.Texto(request.Nombre, "El nombre de la campana", 150, 2);
        var descripcion = Validador.TextoOpcional(request.Descripcion, "La descripcion", 4000);
        var contenido = Validador.Texto(request.Contenido, "El contenido", 8000, 3);

        Validador.Fecha(request.FechaProgramada, "La fecha programada");

        var canal = await _campanasDA.ObtenerCanalAsync(request.IdCanal);

        if (canal is null || !canal.Activo)
        {
            throw new ApiException(400, "El canal de marketing no existe o esta inactivo.");
        }

        campana.Nombre = nombre;
        campana.Descripcion = descripcion;
        campana.IdCanal = canal.IdCanal;
        campana.FechaProgramada = request.FechaProgramada;
        campana.Contenido = contenido;

        await _campanasDA.ActualizarAsync(campana);

        return await ObtenerCampanaAsync(idCampana);
    }

    public async Task<CampanaDetalleResponse> CambiarEstadoAsync(
        int idCampana,
        CambiarEstadoCampanaRequest request)
    {
        if (request is null)
        {
            throw new ApiException(400, "El estado es obligatorio.");
        }

        var estado = Validador.Texto(request.Estado, "El estado", 30);

        var campana = await _campanasDA.ObtenerParaEdicionAsync(idCampana);

        if (campana is null)
        {
            throw new ApiException(404, "La campana no existe.");
        }

        campana.Estado = estado;
        await _campanasDA.ActualizarAsync(campana);

        return await ObtenerCampanaAsync(idCampana);
    }

    public async Task<CampanaDetalleResponse> AsociarClientesAsync(
        int idCampana,
        AsociarClientesCampanaRequest request)
    {
        if (request is null || request.IdClientes is null || request.IdClientes.Count == 0)
        {
            throw new ApiException(400, "Debe indicar al menos un cliente.");
        }

        var idsClientes = request.IdClientes
            .Where(id => id > 0)
            .Distinct()
            .ToList();

        if (idsClientes.Count != request.IdClientes.Distinct().Count())
        {
            throw new ApiException(400, "La lista de clientes contiene identificadores invalidos.");
        }

        await ValidarClientesAsync(idsClientes);

        var agregados = await _campanasDA.AgregarClientesAsync(idCampana, idsClientes);

        if (!agregados)
        {
            throw new ApiException(404, "La campana no existe.");
        }

        return await ObtenerCampanaAsync(idCampana);
    }

    public async Task<List<CampanaClienteDto>> ObtenerClientesAsync(int idCampana)
    {
        var campana = await _campanasDA.ObtenerDetalleAsync(idCampana);

        if (campana is null)
        {
            throw new ApiException(404, "La campana no existe.");
        }

        return campana.CampanaClientes
            .OrderBy(campanaCliente => campanaCliente.IdCampanaCliente)
            .Select(MapearCliente)
            .ToList();
    }

    public async Task<CampanaClienteDto> ActualizarEnvioAsync(
        int idCampana,
        int idCliente,
        ActualizarEnvioCampanaRequest request)
    {
        if (request is null)
        {
            throw new ApiException(400, "Los datos del envio son obligatorios.");
        }

        var estadoEnvio = Validador.Texto(request.EstadoEnvio, "El estado de envio", 30);

        var campanaCliente = await _campanasDA.ObtenerCampanaClienteAsync(
            idCampana,
            idCliente);

        if (campanaCliente is null)
        {
            throw new ApiException(404, "El cliente no esta asociado a la campana.");
        }

        campanaCliente.EstadoEnvio = estadoEnvio;
        campanaCliente.FechaEnvio = request.FechaEnvio ??
            (ClasificadorEstados.EsCampanaEnviadaOFinalizada(estadoEnvio)
                ? DateTime.Now
                : null);

        await _campanasDA.ActualizarEnvioAsync(campanaCliente);

        return MapearCliente(campanaCliente);
    }

    public async Task<List<CanalMarketingDto>> ObtenerCanalesAsync()
    {
        var canales = await _campanasDA.ObtenerCanalesAsync();

        return canales
            .Select(canal => new CanalMarketingDto(
                canal.IdCanal,
                canal.Nombre,
                canal.Descripcion,
                canal.Activo))
            .ToList();
    }

    public async Task<List<string>> ObtenerEstadosAsync()
    {
        return await _campanasDA.ObtenerEstadosAsync();
    }

    private async Task ValidarClientesAsync(IReadOnlyCollection<int> idsClientes)
    {
        if (idsClientes.Count == 0)
        {
            return;
        }

        var existentes = await _campanasDA.ObtenerIdsClientesExistentesAsync(idsClientes);

        if (existentes.Count != idsClientes.Count)
        {
            throw new ApiException(400, "Uno o mas clientes seleccionados no existen.");
        }
    }

    private static CampanaResponse Mapear(Campana campana)
    {
        return new CampanaResponse(
            campana.IdCampana,
            campana.Nombre,
            campana.Descripcion,
            campana.IdCanal,
            campana.Canal?.Nombre ?? string.Empty,
            campana.FechaProgramada,
            campana.Estado,
            campana.FechaCreacion,
            campana.IdUsuarioCreador,
            campana.UsuarioCreador is null
                ? string.Empty
                : ContactosHelper.NombreCompleto(campana.UsuarioCreador.Persona));
    }

    private static CampanaDetalleResponse MapearDetalle(Campana campana)
    {
        var clientes = campana.CampanaClientes
            .OrderBy(campanaCliente => campanaCliente.IdCampanaCliente)
            .Select(MapearCliente)
            .ToList();

        return new CampanaDetalleResponse(
            campana.IdCampana,
            campana.Nombre,
            campana.Descripcion,
            campana.IdCanal,
            campana.Canal?.Nombre ?? string.Empty,
            campana.FechaProgramada,
            campana.Estado,
            campana.Contenido,
            campana.FechaCreacion,
            campana.IdUsuarioCreador,
            campana.UsuarioCreador is null
                ? string.Empty
                : ContactosHelper.NombreCompleto(campana.UsuarioCreador.Persona),
            clientes);
    }

    private static CampanaClienteDto MapearCliente(CampanaCliente campanaCliente)
    {
        var correo = ContactosHelper.ObtenerCorreoPrincipal(campanaCliente.Cliente.Persona);

        return new CampanaClienteDto(
            campanaCliente.IdCampanaCliente,
            campanaCliente.IdCliente,
            ContactosHelper.NombreCompleto(campanaCliente.Cliente.Persona),
            string.IsNullOrWhiteSpace(correo) ? null : correo,
            campanaCliente.EstadoEnvio,
            campanaCliente.FechaEnvio);
    }
}

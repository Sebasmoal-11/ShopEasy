using ShopEasy.BC.Common;
using ShopEasy.BW.AtencionCliente;
using ShopEasy.BW.Common;
using ShopEasy.BW.Dashboard;
using ShopEasy.DA.Data;
using ShopEasy.DA.Entities;

namespace ShopEasy.BC.Services;

public class AtencionClienteService
{
    private readonly AtencionClienteDA _atencionClienteDA;
    private readonly ClientesDA _clientesDA;

    public AtencionClienteService(
        AtencionClienteDA atencionClienteDA,
        ClientesDA clientesDA)
    {
        _atencionClienteDA = atencionClienteDA;
        _clientesDA = clientesDA;
    }

    public async Task<AtencionClienteResponse> ObtenerEstadisticasAsync()
    {
        var porEstado = await _atencionClienteDA.ObtenerConteosPorEstadoAsync();
        var porTipo = await _atencionClienteDA.ObtenerConteosPorTipoAsync();
        var porCanal = await _atencionClienteDA.ObtenerConteosPorCanalAsync();

        return new AtencionClienteResponse(
            porEstado.Sum(estado => estado.Cantidad),
            porEstado
                .Where(estado => ClasificadorEstados.EsCasoPendiente(estado.Estado))
                .Sum(estado => estado.Cantidad),
            porEstado
                .Where(estado => ClasificadorEstados.EsCasoEnProceso(estado.Estado))
                .Sum(estado => estado.Cantidad),
            porEstado
                .Where(estado => ClasificadorEstados.EsCasoResuelto(estado.Estado))
                .Sum(estado => estado.Cantidad),
            porEstado
                .Select(estado => new CasoPorEstadoDto(
                    estado.IdEstadoCaso,
                    estado.Estado,
                    estado.Cantidad))
                .ToList(),
            porTipo
                .Select(tipo => new CasoPorTipoDto(
                    tipo.IdTipoCaso,
                    tipo.Tipo,
                    tipo.Cantidad))
                .ToList(),
            porCanal
                .Select(canal => new CasoPorCanalDto(
                    canal.Canal,
                    canal.Cantidad))
                .ToList());
    }

    public async Task<PagedResponse<CasoClienteResponse>> ObtenerCasosAsync(
        string? buscar,
        int? idCliente,
        int? idEstadoCaso,
        int? idTipoCaso,
        int? idUsuarioAsignado,
        int pagina,
        int tamano)
    {
        (pagina, tamano) = Validador.Paginacion(pagina, tamano);

        var resultado = await _atencionClienteDA.ObtenerListadoAsync(
            buscar,
            idCliente,
            idEstadoCaso,
            idTipoCaso,
            idUsuarioAsignado,
            pagina,
            tamano);

        return new PagedResponse<CasoClienteResponse>(
            resultado.Total,
            pagina,
            tamano,
            resultado.Items.Select(Mapear).ToList());
    }

    public async Task<CasoClienteResponse> ObtenerCasoAsync(int idCaso)
    {
        var caso = await _atencionClienteDA.ObtenerDetalleAsync(idCaso);

        if (caso is null)
        {
            throw new ApiException(404, "El caso no existe.");
        }

        return Mapear(caso);
    }

    public async Task<CasoClienteResponse> CrearCasoAsync(CrearCasoRequest request)
    {
        if (request is null)
        {
            throw new ApiException(400, "Los datos del caso son obligatorios.");
        }

        var cliente = await _clientesDA.ObtenerActivoAsync(request.IdCliente);

        if (cliente is null)
        {
            throw new ApiException(400, "El cliente no existe o esta inactivo.");
        }

        var tipo = await _atencionClienteDA.ObtenerTipoAsync(request.IdTipoCaso);

        if (tipo is null)
        {
            throw new ApiException(400, "El tipo de caso seleccionado no existe.");
        }

        await ValidarUsuarioAsignadoAsync(request.IdUsuarioAsignado);

        var estado = await _atencionClienteDA.ObtenerEstadoInicialAsync();

        if (estado is null)
        {
            throw new ApiException(400, "No hay estados de caso configurados en la base de datos.");
        }

        var canal = Validador.Texto(request.Canal, "El canal", 40);
        var asunto = Validador.Texto(request.Asunto, "El asunto", 150, 3);
        var descripcion = Validador.Texto(request.Descripcion, "La descripcion", 8000, 3);

        var caso = new CasoCliente
        {
            IdCliente = cliente.IdCliente,
            IdTipoCaso = tipo.IdTipoCaso,
            IdEstadoCaso = estado.IdEstadoCaso,
            IdUsuarioAsignado = request.IdUsuarioAsignado,
            Canal = canal,
            Asunto = asunto,
            Descripcion = descripcion,
            FechaCreacion = DateTime.Now,
            FechaResolucion = null
        };

        await _atencionClienteDA.CrearAsync(caso);

        return await ObtenerCasoAsync(caso.IdCaso);
    }

    public async Task<CasoClienteResponse> ActualizarCasoAsync(
        int idCaso,
        ActualizarCasoRequest request)
    {
        if (request is null)
        {
            throw new ApiException(400, "Los datos del caso son obligatorios.");
        }

        var caso = await _atencionClienteDA.ObtenerParaEdicionAsync(idCaso);

        if (caso is null)
        {
            throw new ApiException(404, "El caso no existe.");
        }

        var tipo = await _atencionClienteDA.ObtenerTipoAsync(request.IdTipoCaso);

        if (tipo is null)
        {
            throw new ApiException(400, "El tipo de caso seleccionado no existe.");
        }

        await ValidarUsuarioAsignadoAsync(request.IdUsuarioAsignado);

        caso.IdTipoCaso = tipo.IdTipoCaso;
        caso.IdUsuarioAsignado = request.IdUsuarioAsignado;
        caso.Canal = Validador.Texto(request.Canal, "El canal", 40);
        caso.Asunto = Validador.Texto(request.Asunto, "El asunto", 150, 3);
        caso.Descripcion = Validador.Texto(request.Descripcion, "La descripcion", 8000, 3);

        await _atencionClienteDA.ActualizarAsync(caso);

        return await ObtenerCasoAsync(idCaso);
    }

    public async Task<CasoClienteResponse> CambiarEstadoAsync(
        int idCaso,
        CambiarEstadoCasoRequest request)
    {
        if (request is null || request.IdEstadoCaso <= 0)
        {
            throw new ApiException(400, "Debe seleccionar un estado valido.");
        }

        var estado = await _atencionClienteDA.ObtenerEstadoAsync(request.IdEstadoCaso);

        if (estado is null)
        {
            throw new ApiException(400, "El estado de caso seleccionado no existe.");
        }

        var caso = await _atencionClienteDA.ObtenerParaEdicionAsync(idCaso);

        if (caso is null)
        {
            throw new ApiException(404, "El caso no existe.");
        }

        caso.IdEstadoCaso = estado.IdEstadoCaso;
        caso.FechaResolucion = ClasificadorEstados.EsCasoResuelto(estado.Nombre)
            ? caso.FechaResolucion ?? DateTime.Now
            : null;

        await _atencionClienteDA.ActualizarAsync(caso);

        return await ObtenerCasoAsync(idCaso);
    }

    public async Task<CasoClienteResponse> AsignarCasoAsync(
        int idCaso,
        AsignarCasoRequest request)
    {
        if (request is null)
        {
            throw new ApiException(400, "Los datos de asignacion son obligatorios.");
        }

        await ValidarUsuarioAsignadoAsync(request.IdUsuarioAsignado);

        var caso = await _atencionClienteDA.ObtenerParaEdicionAsync(idCaso);

        if (caso is null)
        {
            throw new ApiException(404, "El caso no existe.");
        }

        caso.IdUsuarioAsignado = request.IdUsuarioAsignado;
        await _atencionClienteDA.ActualizarAsync(caso);

        return await ObtenerCasoAsync(idCaso);
    }

    public async Task<List<TipoCasoDto>> ObtenerTiposAsync()
    {
        var tipos = await _atencionClienteDA.ObtenerTiposAsync();

        return tipos
            .Select(tipo => new TipoCasoDto(
                tipo.IdTipoCaso,
                tipo.Nombre,
                tipo.Descripcion))
            .ToList();
    }

    public async Task<List<EstadoCasoDto>> ObtenerEstadosAsync()
    {
        var estados = await _atencionClienteDA.ObtenerEstadosAsync();

        return estados
            .Select(estado => new EstadoCasoDto(
                estado.IdEstadoCaso,
                estado.Nombre,
                estado.Descripcion))
            .ToList();
    }

    public async Task<List<UsuarioAsignableDto>> ObtenerUsuariosAsignablesAsync()
    {
        var usuarios = await _atencionClienteDA.ObtenerUsuariosAsignablesAsync();

        return usuarios
            .Select(usuario => new UsuarioAsignableDto(
                usuario.IdUsuario,
                usuario.NombreCompleto,
                usuario.Rol,
                usuario.Activo))
            .ToList();
    }

    private async Task ValidarUsuarioAsignadoAsync(int? idUsuario)
    {
        if (!idUsuario.HasValue)
        {
            return;
        }

        if (idUsuario.Value <= 0 ||
            await _atencionClienteDA.ObtenerUsuarioActivoAsync(idUsuario.Value) is null)
        {
            throw new ApiException(400, "El usuario asignado no existe o esta inactivo.");
        }
    }

    private static CasoClienteResponse Mapear(CasoCliente caso)
    {
        return new CasoClienteResponse(
            caso.IdCaso,
            caso.IdCliente,
            ContactosHelper.NombreCompleto(caso.Cliente.Persona),
            caso.Cliente.Cedula,
            caso.IdTipoCaso,
            caso.TipoCaso.Nombre,
            caso.IdEstadoCaso,
            caso.EstadoCaso.Nombre,
            caso.IdUsuarioAsignado,
            caso.UsuarioAsignado is null
                ? null
                : ContactosHelper.NombreCompleto(caso.UsuarioAsignado.Persona),
            caso.Canal,
            caso.Asunto,
            caso.Descripcion,
            caso.FechaCreacion,
            caso.FechaResolucion);
    }
}

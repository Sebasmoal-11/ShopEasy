using ShopEasy.BC.Common;
using ShopEasy.BW.Clientes;
using ShopEasy.BW.Common;
using ShopEasy.BW.Dashboard;
using ShopEasy.DA.Data;
using ShopEasy.DA.Entities;

namespace ShopEasy.BC.Services;

public class ClientesService
{
    private readonly ClientesDA _clientesDA;
    private readonly PedidosDA _pedidosDA;

    public ClientesService(ClientesDA clientesDA, PedidosDA pedidosDA)
    {
        _clientesDA = clientesDA;
        _pedidosDA = pedidosDA;
    }

    public async Task<ClientesDestacadosResponse> ObtenerDestacadosAsync(int limite)
    {
        if (limite is < 1 or > 20)
        {
            throw new ApiException(400, "El limite debe estar entre 1 y 20.");
        }

        var clientes = await _clientesDA.ObtenerDestacadosAsync(limite);

        return new ClientesDestacadosResponse(
            clientes
                .Select(cliente => new ClienteDestacadoDto(
                    cliente.IdCliente,
                    cliente.NombreCompleto,
                    cliente.Correo,
                    cliente.CantidadPedidos,
                    cliente.MontoTotal))
                .ToList());
    }

    public async Task<PagedResponse<ClienteResponse>> ObtenerClientesAsync(
        string? buscar,
        bool? activo,
        int pagina,
        int tamano)
    {
        (pagina, tamano) = Validador.Paginacion(pagina, tamano);

        var resultado = await _clientesDA.ObtenerListadoAsync(
            buscar,
            activo,
            pagina,
            tamano);

        return new PagedResponse<ClienteResponse>(
            resultado.Total,
            pagina,
            tamano,
            resultado.Items
                .Select(MapearResumen)
                .ToList());
    }

    public async Task<ClienteDetalleResponse> ObtenerClienteAsync(int idCliente)
    {
        var cliente = await _clientesDA.ObtenerDetalleAsync(idCliente);

        if (cliente is null)
        {
            throw new ApiException(404, "El cliente no existe.");
        }

        return MapearDetalle(cliente);
    }

    public async Task<ClienteDetalleResponse> CrearClienteAsync(CrearClienteRequest request)
    {
        if (request is null)
        {
            throw new ApiException(400, "Los datos del cliente son obligatorios.");
        }

        var cedula = Validador.Texto(request.Cedula, "La cedula", 30);
        var nombre = Validador.Texto(request.Nombre, "El nombre", 100, 2);
        var apellidos = Validador.Texto(request.Apellidos, "Los apellidos", 100, 2);
        var correo = Validador.Correo(request.Correo);
        var telefono = Validador.TextoOpcional(request.Telefono, "El telefono", 25);
        var tipoCorreo = Validador.TextoOpcional(request.TipoCorreo, "El tipo de correo", 30)
            ?? "Personal";
        var tipoTelefono = Validador.TextoOpcional(request.TipoTelefono, "El tipo de telefono", 30)
            ?? "Personal";

        if (await _clientesDA.ExisteCedulaAsync(cedula))
        {
            throw new ApiException(409, "Ya existe una persona registrada con esa cedula.");
        }

        if (await _clientesDA.ExisteCorreoAsync(correo))
        {
            throw new ApiException(409, "El correo electronico ya esta registrado.");
        }

        var preferencia = await ObtenerPreferenciaAsync(request.IdPreferenciaPublicidad);

        var fecha = DateTime.Now;
        var persona = new Persona
        {
            Cedula = cedula,
            Nombre = nombre,
            Apellidos = apellidos,
            FechaCreacion = fecha
        };

        persona.CorreosPersona.Add(new CorreoPersona
        {
            Cedula = cedula,
            Correo = correo,
            TipoCorreo = tipoCorreo,
            Principal = true,
            Activo = true,
            Persona = persona
        });

        if (telefono is not null)
        {
            persona.TelefonosPersona.Add(new TelefonoPersona
            {
                Cedula = cedula,
                Telefono = telefono,
                TipoTelefono = tipoTelefono,
                Principal = true,
                Activo = true,
                Persona = persona
            });
        }

        var cliente = new Cliente
        {
            Cedula = cedula,
            IdPreferenciaPublicidad = preferencia.IdPreferenciaPublicidad,
            FechaRegistro = fecha,
            Activo = true,
            Persona = persona
        };

        await _clientesDA.CrearAsync(cliente);

        return await ObtenerClienteAsync(cliente.IdCliente);
    }

    public async Task<ClienteDetalleResponse> ActualizarClienteAsync(
        int idCliente,
        ActualizarClienteRequest request)
    {
        if (request is null)
        {
            throw new ApiException(400, "Los datos del cliente son obligatorios.");
        }

        var cliente = await _clientesDA.ObtenerParaEdicionAsync(idCliente);

        if (cliente is null)
        {
            throw new ApiException(404, "El cliente no existe.");
        }

        var nombre = Validador.Texto(request.Nombre, "El nombre", 100, 2);
        var apellidos = Validador.Texto(request.Apellidos, "Los apellidos", 100, 2);
        var correo = Validador.Correo(request.Correo);
        var telefono = Validador.TextoOpcional(request.Telefono, "El telefono", 25);
        var tipoCorreo = Validador.TextoOpcional(request.TipoCorreo, "El tipo de correo", 30);
        var tipoTelefono = Validador.TextoOpcional(request.TipoTelefono, "El tipo de telefono", 30);

        if (await _clientesDA.ExisteCorreoAsync(correo, cliente.Cedula))
        {
            throw new ApiException(409, "El correo electronico ya esta registrado por otra persona.");
        }

        var preferencia = await ObtenerPreferenciaAsync(request.IdPreferenciaPublicidad);

        cliente.Persona.Nombre = nombre;
        cliente.Persona.Apellidos = apellidos;
        cliente.IdPreferenciaPublicidad = preferencia.IdPreferenciaPublicidad;

        var correoActual = cliente.Persona.CorreosPersona
            .Where(c => c.Activo)
            .OrderByDescending(c => c.Principal)
            .ThenBy(c => c.IdCorreo)
            .FirstOrDefault();

        if (correoActual is null)
        {
            cliente.Persona.CorreosPersona.Add(new CorreoPersona
            {
                Cedula = cliente.Cedula,
                Correo = correo,
                TipoCorreo = tipoCorreo ?? "Personal",
                Principal = true,
                Activo = true
            });
        }
        else
        {
            if (cliente.Persona.CorreosPersona.Any(c =>
                    c.IdCorreo != correoActual.IdCorreo &&
                    c.Correo.Equals(correo, StringComparison.OrdinalIgnoreCase)))
            {
                throw new ApiException(409, "Ese correo ya esta asociado al cliente.");
            }

            correoActual.Correo = correo;
            correoActual.Principal = true;
            correoActual.Activo = true;

            if (tipoCorreo is not null)
            {
                correoActual.TipoCorreo = tipoCorreo;
            }
        }

        if (telefono is not null)
        {
            var telefonoActual = cliente.Persona.TelefonosPersona
                .Where(t => t.Activo)
                .OrderByDescending(t => t.Principal)
                .ThenBy(t => t.IdTelefono)
                .FirstOrDefault();

            if (telefonoActual is null)
            {
                cliente.Persona.TelefonosPersona.Add(new TelefonoPersona
                {
                    Cedula = cliente.Cedula,
                    Telefono = telefono,
                    TipoTelefono = tipoTelefono ?? "Personal",
                    Principal = true,
                    Activo = true
                });
            }
            else
            {
                telefonoActual.Telefono = telefono;
                telefonoActual.Principal = true;
                telefonoActual.Activo = true;

                if (tipoTelefono is not null)
                {
                    telefonoActual.TipoTelefono = tipoTelefono;
                }
            }
        }

        await _clientesDA.ActualizarAsync(cliente);

        return await ObtenerClienteAsync(idCliente);
    }

    public async Task<ClienteDetalleResponse> CambiarEstadoAsync(
        int idCliente,
        bool activo)
    {
        var cliente = await _clientesDA.ObtenerParaEdicionAsync(idCliente);

        if (cliente is null)
        {
            throw new ApiException(404, "El cliente no existe.");
        }

        cliente.Activo = activo;
        await _clientesDA.ActualizarAsync(cliente);

        return await ObtenerClienteAsync(idCliente);
    }

    public async Task<ClienteComprasResponse> ObtenerComprasAsync(int idCliente)
    {
        var cliente = await _clientesDA.ObtenerDetalleAsync(idCliente);

        if (cliente is null)
        {
            throw new ApiException(404, "El cliente no existe.");
        }

        var pedidos = await _pedidosDA.ObtenerPorClienteAsync(idCliente);
        var compras = pedidos
            .Select(PedidoMapper.MapearResumen)
            .ToList();

        return new ClienteComprasResponse(
            MapearResumen(cliente),
            compras.Count,
            Math.Round(compras.Sum(compra => compra.Total), 2),
            compras);
    }

    public async Task<List<PreferenciaPublicidadDto>> ObtenerPreferenciasAsync()
    {
        var preferencias = await _clientesDA.ObtenerPreferenciasAsync();

        return preferencias
            .Select(preferencia => new PreferenciaPublicidadDto(
                preferencia.IdPreferenciaPublicidad,
                preferencia.Nombre,
                preferencia.Descripcion))
            .ToList();
    }

    private async Task<PreferenciaPublicidad> ObtenerPreferenciaAsync(int? idPreferencia)
    {
        var preferencia = idPreferencia.HasValue && idPreferencia.Value > 0
            ? await _clientesDA.ObtenerPreferenciaAsync(idPreferencia.Value)
            : await _clientesDA.ObtenerPrimeraPreferenciaAsync();

        if (preferencia is null)
        {
            throw new ApiException(
                400,
                idPreferencia.HasValue
                    ? "La preferencia de publicidad seleccionada no existe."
                    : "No hay preferencias de publicidad configuradas en la base de datos.");
        }

        return preferencia;
    }

    private static ClienteResponse MapearResumen(Cliente cliente)
    {
        var correo = ContactosHelper.ObtenerCorreoPrincipal(cliente.Persona);
        var telefono = ContactosHelper.ObtenerTelefonoPrincipal(cliente.Persona);

        return new ClienteResponse(
            cliente.IdCliente,
            cliente.Cedula,
            cliente.Persona.Nombre,
            cliente.Persona.Apellidos,
            ContactosHelper.NombreCompleto(cliente.Persona),
            string.IsNullOrWhiteSpace(correo) ? null : correo,
            telefono,
            cliente.IdPreferenciaPublicidad,
            cliente.PreferenciaPublicidad?.Nombre ?? string.Empty,
            cliente.Activo,
            cliente.FechaRegistro);
    }

    private static ClienteDetalleResponse MapearDetalle(Cliente cliente)
    {
        var correos = cliente.Persona.CorreosPersona
            .OrderByDescending(correo => correo.Principal)
            .ThenBy(correo => correo.IdCorreo)
            .Select(correo => new CorreoPersonaDto(
                correo.IdCorreo,
                correo.Correo,
                correo.TipoCorreo,
                correo.Principal,
                correo.Activo))
            .ToList();

        var telefonos = cliente.Persona.TelefonosPersona
            .OrderByDescending(telefono => telefono.Principal)
            .ThenBy(telefono => telefono.IdTelefono)
            .Select(telefono => new TelefonoPersonaDto(
                telefono.IdTelefono,
                telefono.Telefono,
                telefono.TipoTelefono,
                telefono.Principal,
                telefono.Activo))
            .ToList();

        return new ClienteDetalleResponse(
            cliente.IdCliente,
            cliente.Cedula,
            cliente.Persona.Nombre,
            cliente.Persona.Apellidos,
            ContactosHelper.NombreCompleto(cliente.Persona),
            cliente.IdPreferenciaPublicidad,
            cliente.PreferenciaPublicidad?.Nombre ?? string.Empty,
            cliente.Activo,
            cliente.FechaRegistro,
            correos,
            telefonos);
    }
}

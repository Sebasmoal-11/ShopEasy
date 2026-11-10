using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using ShopEasy.BC.Common;
using ShopEasy.BC.Security;
using ShopEasy.BC.Settings;
using ShopEasy.BW.Auth;
using ShopEasy.DA.Data;
using ShopEasy.DA.Entities;

namespace ShopEasy.BC.Services;

public class AuthService
{
    private readonly AuthDA _authDA;
    private readonly JwtSettings _jwtSettings;

    public AuthService(AuthDA authDA, JwtSettings jwtSettings)
    {
        _authDA = authDA;
        _jwtSettings = jwtSettings;
    }

    public async Task<LoginResponse> LoginAsync(LoginRequest request)
    {
        if (request is null)
        {
            throw new ApiException(400, "Los datos de la solicitud son obligatorios.");
        }

        var correo = (request.Correo ?? string.Empty).Trim().ToLowerInvariant();
        var contrasena = request.Contrasena ?? string.Empty;

        if (string.IsNullOrWhiteSpace(correo) || string.IsNullOrWhiteSpace(contrasena))
        {
            throw new ApiException(400, "El correo y la contrasena son obligatorios.");
        }

        var usuario = await _authDA.BuscarPorCorreoAsync(correo);

        if (usuario is null)
        {
            throw CredencialesInvalidas();
        }

        var correoActivo = usuario.Persona.CorreosPersona
            .FirstOrDefault(c => c.Correo.ToLower() == correo && c.Activo);

        if (!usuario.Activo || correoActivo is null ||
            !PasswordHelper.Verify(contrasena, usuario.ContrasenaHash))
        {
            throw CredencialesInvalidas();
        }

        var nombreCompleto = $"{usuario.Persona.Nombre} {usuario.Persona.Apellidos}".Trim();
        var token = CrearToken(usuario, nombreCompleto, correoActivo.Correo);

        return new LoginResponse(
            token.Token,
            "Bearer",
            token.ExpiresAtUtc,
            new CurrentUserDto(
                usuario.IdUsuario,
                usuario.Cedula,
                nombreCompleto,
                correoActivo.Correo,
                usuario.Rol.Nombre));
    }

    private (string Token, DateTime ExpiresAtUtc) CrearToken(
        Usuario usuario,
        string nombreCompleto,
        string correo)
    {
        var expira = DateTime.UtcNow.AddMinutes(_jwtSettings.ExpiresMinutes);

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, usuario.IdUsuario.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, correo),
            new Claim("name", nombreCompleto),
            new Claim("role", usuario.Rol.Nombre),
            new Claim("cedula", usuario.Cedula),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Key));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _jwtSettings.Issuer,
            audience: _jwtSettings.Audience,
            claims: claims,
            notBefore: DateTime.UtcNow,
            expires: expira,
            signingCredentials: credentials);

        var tokenText = new JwtSecurityTokenHandler().WriteToken(token);

        return (tokenText, expira);
    }

    private static ApiException CredencialesInvalidas()
    {
        return new ApiException(401, "Correo o contrasena incorrectos.");
    }
}

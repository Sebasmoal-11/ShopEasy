using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace ShopEasy.DA.Data;

public static class BaseDeDatosExtensions
{
    public static IServiceCollection AgregarBaseDeDatos(
        this IServiceCollection services,
        string connectionString)
    {
        var version = new MySqlServerVersion(new Version(8, 0, 36));

        services.AddDbContext<ShopEasyDbContext>(options =>
            options.UseMySql(connectionString, version));

        services.AddScoped<AuthDA>();
        services.AddScoped<UserDA>();
        services.AddScoped<DashboardDA>();

        return services;
    }
}

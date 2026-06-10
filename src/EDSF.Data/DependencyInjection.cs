using EDSF.Core.Interfaces;
using EDSF.Data.Data;
using EDSF.Data.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace EDSF.Data;

public static class DependencyInjection
{
    public static IServiceCollection AddDataLayer(this IServiceCollection services, string connectionString)
    {
        services.AddHttpContextAccessor();
        services.AddScoped<AuditInterceptor>();

        services.AddDbContext<AppDbContext>((sp, options) =>
            options.UseSqlite(connectionString)
                   .AddInterceptors(sp.GetRequiredService<AuditInterceptor>()));

        services.AddScoped<IUnitOfWork, UnitOfWork>();

        return services;
    }
}

using EDSF.Core.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using System.Text.Json;

namespace EDSF.Data.Data;

public class AuditInterceptor : SaveChangesInterceptor
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        WriteIndented = false,
        ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles
    };

    private readonly IHttpContextAccessor _httpContextAccessor;

    public AuditInterceptor(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = default)
    {
        var context = eventData.Context;
        if (context == null) return base.SavingChangesAsync(eventData, result, cancellationToken);

        var entries = context.ChangeTracker.Entries()
            .Where(e => e.State == EntityState.Added ||
                        e.State == EntityState.Modified ||
                        e.State == EntityState.Deleted)
            .ToList();

        var auditLogs = new List<AuditLog>();
        var userName = _httpContextAccessor.HttpContext?.User?.Identity?.Name ?? "system";
        var ipAddress = _httpContextAccessor.HttpContext?.Connection?.RemoteIpAddress?.ToString();

        foreach (var entry in entries)
        {
            var entityType = entry.Entity.GetType();
            if (entityType == typeof(AuditLog)) continue;

            var entityName = entityType.Name;
            var entityId = entry.Property("Id")?.CurrentValue as int? ?? 0;
            string action = entry.State switch
            {
                EntityState.Added => "Create",
                EntityState.Modified => "Update",
                EntityState.Deleted => "Delete",
                _ => "Unknown"
            };

            string? oldValues = null;
            string? newValues = null;

            if (entry.State == EntityState.Modified)
            {
                var old = new Dictionary<string, object?>();
                var @new = new Dictionary<string, object?>();
                foreach (var prop in entry.Properties)
                {
                    if (prop.Metadata.IsPrimaryKey()) continue;
                    if (!Equals(prop.OriginalValue, prop.CurrentValue))
                    {
                        old[prop.Metadata.Name] = prop.OriginalValue;
                        @new[prop.Metadata.Name] = prop.CurrentValue;
                    }
                }
                if (old.Count > 0)
                {
                    oldValues = JsonSerializer.Serialize(old, JsonOptions);
                    newValues = JsonSerializer.Serialize(@new, JsonOptions);
                }
            }
            else if (entry.State == EntityState.Added)
            {
                var @new = new Dictionary<string, object?>();
                foreach (var prop in entry.Properties)
                {
                    if (prop.Metadata.IsPrimaryKey()) continue;
                    @new[prop.Metadata.Name] = prop.CurrentValue;
                }
                newValues = JsonSerializer.Serialize(@new, JsonOptions);
            }
            else if (entry.State == EntityState.Deleted)
            {
                var old = new Dictionary<string, object?>();
                foreach (var prop in entry.Properties)
                {
                    if (prop.Metadata.IsPrimaryKey()) continue;
                    old[prop.Metadata.Name] = prop.OriginalValue;
                }
                oldValues = JsonSerializer.Serialize(old, JsonOptions);
            }

            if (oldValues == null && newValues == null) continue;

            auditLogs.Add(new AuditLog
            {
                EntityName = entityName,
                EntityId = entityId,
                Action = action,
                OldValues = oldValues,
                NewValues = newValues,
                UserName = userName,
                Timestamp = DateTime.UtcNow,
                IpAddress = ipAddress
            });
        }

        if (auditLogs.Count > 0)
        {
            context.Set<AuditLog>().AddRange(auditLogs);
        }

        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }
}

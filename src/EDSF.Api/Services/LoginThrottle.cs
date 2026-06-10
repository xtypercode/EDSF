using System.Collections.Concurrent;

namespace EDSF.Api.Services;

public class LoginThrottle
{
    private static readonly ConcurrentDictionary<string, List<DateTime>> _attempts = new();
    private const int MaxAttempts = 5;
    private static readonly TimeSpan Window = TimeSpan.FromMinutes(15);

    public bool IsBlocked(string ip)
    {
        var now = DateTime.UtcNow;
        if (!_attempts.TryGetValue(ip, out var list)) return false;
        list.RemoveAll(t => now - t > Window);
        return list.Count >= MaxAttempts;
    }

    public void RecordAttempt(string ip)
    {
        var list = _attempts.GetOrAdd(ip, _ => []);
        lock (list) { list.Add(DateTime.UtcNow); }
    }

    public void Reset(string ip)
    {
        _attempts.TryRemove(ip, out _);
    }
}

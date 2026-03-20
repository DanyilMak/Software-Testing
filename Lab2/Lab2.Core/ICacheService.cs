namespace Lab2.Core;

public interface ICacheService
{
    T Get<T>(string key);
    void Set<T>(string key, T value, TimeSpan expiration);
    bool Exists(string key);
}

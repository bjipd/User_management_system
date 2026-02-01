using Application.Models;
using Application.Services;
using System.Collections.Concurrent;
using System.Threading;

public class InMemoryUserRepository : IUserRepository
{
    private readonly ConcurrentDictionary<int, User> _users = new();

    private int _nextId = 0;
    public User? GetUserById(int id)
    {
        _users.TryGetValue(id, out var user);
        return user;
    }

    public IEnumerable<User> GetAllUsers() =>  _users.Values;

    public User AddUser(User user)
    {
        var id = Interlocked.Increment(ref _nextId);
        var newUser = user with { Id = id };
        var timeCreated = DateTime.UtcNow;
        newUser = newUser with { CreatedAt = timeCreated };
        var isActiveUser = true;
        newUser = newUser with { IsActive = isActiveUser };
        _users[id] = newUser;
        return newUser;
    }

    public bool UpdateUser(int id, User user)
    {
        if (!_users.ContainsKey(id))
        {
            return false;
        }
        var timeUpdated = DateTime.UtcNow;
        user = user with { UpdatedAt = timeUpdated };
        _users[id] = user;
        return true;
    }

    public bool DeleteUser(int id)
    {
        return _users.TryRemove(id, out _);
    }
}
using System;
using Application.Models;

namespace Application.Services
{
    public interface IUserRepository
    {
        User? GetUserById(int id);
        IEnumerable<User> GetAllUsers();
        User AddUser(User user);
        bool UpdateUser(int id, User user);
        bool DeleteUser(int id);
    }
}
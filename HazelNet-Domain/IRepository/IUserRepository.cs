using HazelNet_Domain.Models;


namespace HazelNet_Domain.IRepository;


public interface IUserRepository
{
    Task<User?> Get(int userId);
    Task Update(User user);
    Task Delete(int userId);
    Task Create(User user);
}
using HazelNet_Domain.Models;

namespace HazelNet_Application.Interface;

using HazelNet_Domain;

public interface IUserRepository
{
    Task<bool> EmailExistAsync(string email);
    Task RegisterUserAsync(User user);
    Task<string?> GetPasswordHashAsync(string email);
    
}
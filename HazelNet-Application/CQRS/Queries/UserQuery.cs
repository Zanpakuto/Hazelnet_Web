using HazelNet_Domain.IRepository;
using HazelNet_Domain.Models;
using HazelNet_Application.CQRS.Abstractions;

namespace HazelNet_Application.CQRS.Queries;

//query for getting user by id
public class GetUserByIdQuery : IQuery<User?>
{
    public int UserId { get; set; }
    public GetUserByIdQuery(int userId)
    {
        UserId = userId;
    }
}

//query handler for getting user by id 
public class GetUserByIdQueryHandler : IQueryHandler<GetUserByIdQuery, User?>
{
    private readonly IUserRepository _userRepository;

    public GetUserByIdQueryHandler(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<User?> Handle(GetUserByIdQuery query)
    {
        return await _userRepository.Get(query.UserId);
    }
}

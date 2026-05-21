using HazelNet_Application.CQRS.Abstractions;
using HazelNet_Application.Interface;

namespace HazelNet_Application.CQRS.Features.User.Query;

public class GetUsernameByUserIdQueryHandler:  IQueryHandler<GetUsernameByUserIdQuery, string>
{
    private readonly IUserRepository _userRepository;

    public GetUsernameByUserIdQueryHandler(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<string> Handle(GetUsernameByUserIdQuery query)
    {
        return await _userRepository.GetUsernameByUserIdAsync(query.UserId);
    }
}
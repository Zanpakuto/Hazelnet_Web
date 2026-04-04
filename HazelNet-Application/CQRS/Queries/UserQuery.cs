using HazelNet_Domain.IRepository;
using HazelNet_Domain.Models;
using HazelNet_Application.CQRS.Abstractions;

namespace HazelNet_Application.CQRS.Queries;

/*
    HOW TO USE
    1. Inject the query handler into your controller or service where you want to execute the query
    ex:
    private readonly GetCardByIdQueryHandler _getCardByIdQueryHandler;
    public YourController(GetCardByIdQueryHandler getCardByIdQueryHandler)
    {
        _getCardByIdQueryHandler = getCardByIdQueryHandler;
    }
    2. Create an instance of the query with the required parameters and execute it using the handler
    ex:
    var query = new GetCardByIdQuery(cardId);
    var card = await _getCardByIdQueryHandler.Handle(query);
    3. Query handler will execute the logic defined and use repositories to interact with the database as needed
*/

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

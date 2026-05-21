using HazelNet_Application.CQRS.Abstractions;

namespace HazelNet_Application.CQRS.Features.User.Query;


public record GetUsernameByUserIdQuery(int UserId) : IQuery<string>;
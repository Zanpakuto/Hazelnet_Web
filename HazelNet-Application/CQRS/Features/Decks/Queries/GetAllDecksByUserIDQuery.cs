using HazelNet_Application.CQRS.Abstractions;
using HazelNet_Domain.Models;

namespace HazelNet_Application.CQRS.Features.Decks.Queries;

public record GetAllDecksByUserIDQuery(int UserId) : IQuery<List<Deck>>;
using HazelNet_Application.CQRS.Abstractions;
using HazelNet_Web.ViewModel;

namespace HazelNet_Application.CQRS.Features.Decks.Queries;


public record GetDecksQuery(int UserId) : IQuery<List<DeckViewModel>>;

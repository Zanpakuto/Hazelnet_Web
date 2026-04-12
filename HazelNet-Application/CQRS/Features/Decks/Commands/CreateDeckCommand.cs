using HazelNet_Application.CQRS.Abstractions;

namespace HazelNet_Application.CQRS.Features.Decks.Commands;

public record CreateDeckCommand
(
    int Id,
    string DeckName,
    string? DeckDescription ,
    int UserId
) : ICommand<int>;
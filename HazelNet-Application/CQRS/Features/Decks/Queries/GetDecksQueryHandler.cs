using HazelNet_Application.CQRS.Abstractions;

using HazelNet_Domain.Models;
using HazelNet_Web.ViewModel;

namespace HazelNet_Application.CQRS.Features.Decks.Queries;

public class GetDecksQueryHandler 
    : IQueryHandler<GetDecksQuery, List<DeckViewModel>>
{
    public Task<List<DeckViewModel>> Handle(GetDecksQuery query)
    {
        // fetch from DB later
        var decks = new List<Deck>
        {
            new Deck
            {
                Id = 1,
                DeckName = "System Architecture",
                DeckDescription = "Core concepts of distributed systems and microservices.",
                CreationDate = new DateTime(2026, 1, 10),
                LastAcess = DateTime.Now.AddHours(-2),
                UserId = 4
            },
            new Deck
            {
                Id = 2,
                DeckName = "Rust Fundamentals",
                DeckDescription = "Memory safety, borrowing, and lifetimes.",
                CreationDate = new DateTime(2026, 3, 5),
                LastAcess = DateTime.Now.AddDays(-1),
                UserId = 4
            },
            new Deck
            {
                Id = 3,
                DeckName = "Advanced .NET Features",
                DeckDescription = null, 
                CreationDate = new DateTime(2025, 11, 20),
                LastAcess = DateTime.Now.AddDays(-15),
                UserId = 4
            },
            new Deck
            {
                Id = 4,
                DeckName = "LeetCode Problems",
                DeckDescription = "A collection of leetcode problems and solution", 
                CreationDate = new DateTime(2025, 11, 20),
                LastAcess = DateTime.Now.AddDays(-25),
                UserId = 4
            }
        };


        var result = decks.Select(d => new DeckViewModel
        {
            Name = d.DeckName,
            Description = d.DeckDescription,
            TotalNumberOfCards = d.Cards.Count,
            LastDateAccessed = d.LastAcess
        }).ToList();

        return Task.FromResult(result);
    }
}


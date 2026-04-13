namespace HazelNet_Domain.Models;

public class Deck
{
    public int Id { get; set; }
    public string DeckName { get; set; }
    public string? DeckDescription { get; set; }
    public DateTime CreationDate { get; set; } = DateTime.UtcNow;
    public DateTime LastAcess { get; set; } = DateTime.UtcNow;
    
    public User User { get; set; }
    public int UserId { get; set; }
    public List<Card> Cards { get; set; } = new List<Card>();
}
namespace HazelNet_Domain.Models;

public class Deck
{
    public int Id { get; set; }
    public string DeckName { get; set; }
    public DateTime CreationDate { get; set; } = DateTime.Now;
    public DateTime LastAcess { get; set; } = DateTime.Now;
    
    public List<Card> Cards { get; set; } = new List<Card>();
}
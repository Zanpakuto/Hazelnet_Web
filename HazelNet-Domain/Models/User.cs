namespace HazelNet_Domain.Models;

public class User
{
    public int Id { get; set; }
    public string Username { get; set; }
    public string EmailAddress { get; set; }
    public string Password { get; set; }
    
    public List<Deck>? Decks { get; set; } =  new List<Deck>();
}
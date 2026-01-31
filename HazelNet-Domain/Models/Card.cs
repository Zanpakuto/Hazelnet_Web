namespace HazelNet_Domain.Models;

public class Card
{
    
    public int Id { get; private set; }
    public required string FrontOfCard { get; set; }
    public string? BackOfCard { get; set; }
    public DateTime CreationDate { get; set; } = DateTime.Now;
    
    //Navigation properties
    public int DeckId { get; set; }
    public Deck Deck { get; set; } =  new Deck();
    
    //FSRS PROPERTIES
    public DateTime Due { get; set; }
    public double Stability { get; set; }
    public double Difficulty { get; set; }
    public ulong ElapsedDays { get; set; }
    public ulong ScheduledDays { get; set; }
    public ulong Reps { get; set; }
    public ulong Lapses { get; set; }
    public State State { get; set; }
    public DateTime LastReview { get; set; }

    public Card()
    {
        Due = DateTime.MinValue;
        Stability = 0;
        Difficulty = 0;
        ElapsedDays = 0;
        ScheduledDays = 0;
        Reps = 0;
        Lapses = 0;
        State = State.New;
        LastReview = DateTime.MinValue;
    }

    public Card Clone()
    {
        return (Card)MemberwiseClone();
    }
}
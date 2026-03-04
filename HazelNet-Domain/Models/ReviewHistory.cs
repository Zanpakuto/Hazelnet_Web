namespace HazelNet_Domain.Models;

public class ReviewHistory
{
    public int Id { get; set; }
    //for navigation
    public Card Card { get; set; }
    public int CardId { get; set; }
    public List<ReviewLog> ReviewLogs { get; set; } = new List<ReviewLog>();

    public ReviewHistory(int cardId)
    {
        CardId = cardId;
    }
}
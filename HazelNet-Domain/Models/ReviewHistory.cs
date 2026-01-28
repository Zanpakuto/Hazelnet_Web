namespace HazelNet_Domain.Models;

public class ReviewHistory
{
    public int CardId { get; private set; }
    public ICollection<ReviewLog> ReviewLogs { get; private set; } = new List<ReviewLog>();

    public ReviewHistory(int cardId)
    {
        CardId = cardId;
    }
}
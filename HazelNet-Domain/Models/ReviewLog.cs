namespace HazelNet_Domain.Models;

public class ReviewLog
{
    public int Id { get; set; }
    public Rating Rating { get; set; }
    public ulong ScheduledDays { get; set; }
    public ulong ElapsedDays { get; set; }
    public DateTime Review { get; set; }
    public State State { get; set; }
    
    
    public ReviewHistory ReviewHistory { get; set; }
    public int ReviewHistoryId { get; set; }
}
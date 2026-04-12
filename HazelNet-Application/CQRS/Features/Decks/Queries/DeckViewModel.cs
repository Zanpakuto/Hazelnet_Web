using HazelNet_Domain.Models;

namespace HazelNet_Web.ViewModel;

public class DeckViewModel
{
    public string Name;
    public string? Description;
    public int TotalNumberOfCards = 0;
    public DateTime LastDateAccessed;
        
    //I need a function for the following 
    public int MasteredCards { get; set; }
    
    public double MasteredPercentage =>
        TotalNumberOfCards == 0 ? 0 :
            (double)MasteredCards / TotalNumberOfCards * 100;

    public int NumOfDueCards { get; set; } = 0;

    
  
}

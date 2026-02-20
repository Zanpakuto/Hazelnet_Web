using HazelNet_Domain.Models;
using HazelNet_Domain.Services.FSRS;
using System.Linq;

namespace HazelNet_Domain.Services.StudySession;

public class Study
{
    public void StudyCard(ref Card card, Parameters parameters, Rating grade)
    {
        var FSRS = Services.FSRS.FSRS.NewFSRS(parameters);
        var now = DateTime.Now;
        
        var nextInfo = FSRS.Next(card, now, grade);
        card = nextInfo.Card;
        
        var reviewLog = nextInfo.ReviewLog;
        //log result
        //di accessible db from my end for some reason
        
    }
    
    //retrieve earliest card
    public Card RetrieveCard(List<Card> cards)
    {
        return cards.First();
    }
    
    //sorts cards by time due, earliest first
    public List<Card> SortCards(List<Card> cards)
    {
        return cards.OrderBy(c => c.Due).ToList();
    }
}
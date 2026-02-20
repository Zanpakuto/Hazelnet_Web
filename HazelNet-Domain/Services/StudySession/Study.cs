using HazelNet_Domain.Models;
using HazelNet_Domain.Services.FSRS;

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
}
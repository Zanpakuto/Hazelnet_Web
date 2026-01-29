using HazelNet_Domain.Models;

namespace HazelNet_Domain.Services.FSRS;

public class Analysis
{
    //ratio of score and total reviews
    public double RecallAccuracy(Card card)
    {
        return (card.Reps - card.Lapses) / (double)card.Lapses;
    }

    //average difficulty from review history
    public Rating AvgDifficulty(ReviewHistory reviewHistory)
    {
        float totalDifficulty = 0;
        for(int i = 0; i < reviewHistory.ReviewLogs.Count; i++)
        {
            //calculate average difficulty from review logs
            totalDifficulty += (float)reviewHistory.ReviewLogs.ElementAt(i).Rating;
        }
        float avgDifficulty = totalDifficulty / reviewHistory.ReviewLogs.Count;
        avgDifficulty = float.Clamp(avgDifficulty, 2, 4);   //clamp between Hard and Easy
        return (Rating)Math.Round(avgDifficulty);
    }
}
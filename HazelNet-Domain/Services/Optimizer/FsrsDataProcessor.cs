using HazelNet.Services.Optimizer;
using HazelNet_Domain.Models;

namespace HazelNet.Service.Optimizer;

public class FsrsDataProcessor
{
    //limit review history to previous 5 reviews to ensure only "latest" data is utilized
    private const int MaxHistoryLength = 5;

    //converts domains in ReviewHistory objects into optimized numerical vectors for the trainer
    //each review history becomes a list of training sample objects
    public List<FsrsTrainingSample> ProcessHistoryToSamples(IEnumerable<ReviewHistory> allHistories)
    {
        var trainingSamples = new List<FsrsTrainingSample>();

        //assume multiple histories of cards
        foreach (var history in allHistories)
        {
            //sort logs chronoologically to ensure correct order
            var sortedLogs = history.ReviewLogs.OrderBy(x => x.Review).ToList();

            //require at least 2 reviews to create a sample
            if (sortedLogs.Count < 2) continue;

            //iterate thorugh the logs to create samples
            for (int i = 1; i < sortedLogs.Count; i++)
            {
                var currentLog = sortedLogs[i];

                // create a new sample
                var sample = new FsrsTrainingSample
                {
                    TimeHistory = new float[MaxHistoryLength],
                    RatingHistory = new int[MaxHistoryLength],
                    HistoryLength = 0,

                    // the "truth" we are training against
                    TargetInterval = (float)currentLog.ElapsedDays,
                    Label = currentLog.Rating == Rating.Again ? 0.0f : 1.0f
                };

                //window filling
                var windowStart = Math.Max(0, i - MaxHistoryLength);
                var windowCount = i - windowStart;

                sample.HistoryLength = windowCount;

                //fill the fixed arrs, If we have 3 history items, they go into indices 2, 3, 4.
                // fill arrays (Reverse order: Index 4 is most recent, Index 0 is oldest in the window)
                // note: The trainer logic loops (5 - Length) to 5. 
                // we will store them right-aligned for consistency with the original trainer
                for (int k = 0; k < windowCount; k++)
                {
                    int logIndex = windowStart + k; // index in the sortedLogs list
                    int arrayIndex = (MaxHistoryLength - windowCount) + k; // right-aligned index in the struct array

                    var pastLog = sortedLogs[logIndex];

                    sample.TimeHistory[arrayIndex] = (float)pastLog.ElapsedDays;
                    sample.RatingHistory[arrayIndex] = (int)pastLog.Rating; // enum 1-4 maps directly
                }

                trainingSamples.Add(sample);
            }
        }
        
        return trainingSamples;
    }
}
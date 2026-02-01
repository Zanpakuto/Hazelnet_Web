using HazelNet.Service.Optimizer;
using HazelNet_Domain.Models;

namespace HazelNet.Services.Optimizer;

//an "API" service for optimizing FSRS weights.

//usage: 
//var optimizer = new FsrsOptimizationService();
//var trainedWeights = optimizer.OptimizeWeights(reviewHistories);

//we can also modify the hyperparams, but defaults are reasonable so only use that.
public class FsrsOptimizationService
{
    private readonly FsrsDataProcessor _processor;
    private readonly FsrsTrainer _trainer;

    public FsrsOptimizationService(double learningRate = 0.05, double regularization = 0.05)
    {
        _processor = new FsrsDataProcessor();
        _trainer = new FsrsTrainer(learningRate, regularization);
    }

    public double[] OptimizeWeights(IEnumerable<ReviewHistory> reviewHistories, 
        int epochs = 5, int batchSize = 4096)
    {
        if (reviewHistories == null)
            throw new ArgumentNullException(nameof(reviewHistories));

        var samples = _processor.ProcessHistoryToSamples(reviewHistories);
        return _trainer.Train(samples, epochs, batchSize);
    }

    public double[] OptimizeWeights(IEnumerable<ReviewHistory> reviewHistories)
    {
        return OptimizeWeights(reviewHistories, epochs: 5, batchSize: 4096);
    }
}

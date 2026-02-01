using System.Diagnostics;

namespace HazelNet.Services.Optimizer;


public class FsrsTrainer
{
    //default fsrs weights, much safer to use this for optimization
    public static readonly double[] DefaultWeights = {
        0.212, 1.2931, 2.3065, 8.2956, 6.4133, 0.8334, 3.0194, 0.001, 
        1.8722, 0.1666, 0.796, 1.4835, 0.0614, 0.2629, 1.6483, 0.6014, 
        1.8729, 0.5425, 0.0912, 0.0658, 0.1542
    };

    //keeping most hings private and readonly, dont touch them pls
    private double[] _weights;
    private readonly double _learningRate;
    private readonly double _regularization;

    //Adam Optimizer States
    private readonly double[] _m = new double[21];
    private readonly double[] _v = new double[21];
    private readonly double _beta1 = 0.9;
    private readonly double _beta2 = 0.999;
    private readonly double _eps = 1e-8;
    private int _t = 0;

    //learning rate affects convergence speed, regularization helps prevent overfitting
    public FsrsTrainer(double learningRate = 0.05, double regularization = 0.05)
    {
        _learningRate = learningRate;
        _regularization = regularization;
        _weights = (double[])DefaultWeights.Clone();
    }

    //returns a list of weights
    public double[] Train(List<FsrsTrainingSample> dataset, int epochs = 5, int batchSize = 4096)
    {
        if (dataset == null || dataset.Count == 0)
            throw new ArgumentException("Dataset cannot be null or empty", nameof(dataset));

        Console.WriteLine($"Starting FSRS Training on {dataset.Count} samples...");
        var sw = Stopwatch.StartNew();

        // split Data (80/20) for train and val
        var rng = new Random();
        var shuffled = dataset.OrderBy(_ => rng.Next()).ToList();
        int splitIndex = (int)(dataset.Count * 0.8);
        
        var trainSet = shuffled.Take(splitIndex).ToList();
        var valSet = shuffled.Skip(splitIndex).ToList();

        // initial loss to bootstrap validaiton
        double currentValLoss = CalculateLoss(valSet, _weights);
        Console.WriteLine($"Initial Validation Loss: {currentValLoss:F5}");

        for (int e = 0; e < epochs; e++)
        {
            // mini per-batch Gradient Descent
            for (int i = 0; i < trainSet.Count; i += batchSize)
            {
                int count = Math.Min(batchSize, trainSet.Count - i);
                var batch = trainSet.GetRange(i, count);
                
                double[] grads = ComputeBatchGradients(batch);
                ApplyAdamStep(grads);
            }

            currentValLoss = CalculateLoss(valSet, _weights);
            Console.WriteLine($"Epoch {e + 1}/{epochs} | Val Loss: {currentValLoss:F5}");
        }

        sw.Stop();
        Console.WriteLine($"Training finished in {sw.Elapsed.TotalSeconds:F2}s");
        return _weights;
    }


    //compute gradients for each batch so we save a lot of compute instead of per-sample
    private double[] ComputeBatchGradients(List<FsrsTrainingSample> batch)
    {
        double[] grads = new double[21];
        double epsilon = 1e-4; 

        // parallel finite difference for Gradient Approximation
        //highkey dont undertand the hard math, just know it approximates gradients much faster and well enough
        Parallel.For(0, 21, wIdx => 
        {
            double originalVal = _weights[wIdx];
            
            // perturb Up
            _weights[wIdx] = originalVal + epsilon;
            double lossUp = CalculateLoss(batch, _weights);
            
            // perturb Down
            _weights[wIdx] = originalVal - epsilon;
            double lossDown = CalculateLoss(batch, _weights);

            // restore
            _weights[wIdx] = originalVal;
            
            // central difference
            grads[wIdx] = (lossUp - lossDown) / (2 * epsilon);
        });
        return grads;
    }
    //Calculate MSE loss with L2 Regularization
    
    //1 for 1 c# implementation of Adam
    private void ApplyAdamStep(double[] grads)
    {
        _t++;
        for (int i = 0; i < 21; i++)
        {
            double g = Math.Clamp(grads[i], -1.0, 1.0); // Gradient Clipping

            // update moments
            _m[i] = _beta1 * _m[i] + (1 - _beta1) * g;
            _v[i] = _beta2 * _v[i] + (1 - _beta2) * (g * g);

            // bias correction
            double mHat = _m[i] / (1 - Math.Pow(_beta1, _t));
            double vHat = _v[i] / (1 - Math.Pow(_beta2, _t));

            // update weights
            _weights[i] -= _learningRate * mHat / (Math.Sqrt(vHat) + _eps);

            // weight Ccipping (Constraints)
            if (i < 20) _weights[i] = Math.Clamp(_weights[i], 0.001, 30.0);
            else _weights[i] = Math.Clamp(_weights[i], 0.01, 0.9); // The retention decay parameter
        }
    }
    
    //key step, loss calc
    private double CalculateLoss(List<FsrsTrainingSample> data, double[] w)
    {
        double totalLogLoss = 0;
        object lockObj = new object();

        // calculate loss in parallel to speed things up
        Parallel.ForEach(data, () => 0.0, (sample, state, localSum) =>
        {
            // these track the simulated memory state for this specific history
            double? s = null; // stability (how long memory lasts)
            double? d = null; // difficulty (how hard the card is)
            
            // replay the history to get the current state
            for (int j = (5 - sample.HistoryLength); j < 5; j++)
            {
                float t = sample.TimeHistory[j];
                int r = sample.RatingHistory[j];
                
                // map the rating (1-4) to an array index (0-3)
                int rIdx = (r < 1 ? 1 : (r > 4 ? 4 : r)) - 1;

                if (s == null) 
                {
                    // this is the first review, so we just set the initial values
                    s = Math.Max(0.001, w[rIdx]);
                    d = Math.Clamp(w[4] - (Math.Exp(w[5] * rIdx) - 1), 1, 10);
                } 
                else 
                {
                    // check how much was remembered based on elapsed time
                    double ret = Math.Pow(1.0 + (Math.Pow(0.9, 1.0 / -w[20]) - 1.0) * t / s.Value, -w[20]);
                    
                    // update stability (s)
                    double nextS;
                    if (r == 1) // rated "again"
                    {
                        // if they forgot, cut stability in half
                        nextS = s.Value * 0.5; 
                    }
                    else // rated "hard", "good", or "easy"
                    {
                        // if they remembered, boost stability based on difficulty
                        double sInc = Math.Exp(w[8]) * (11 - d!.Value) * Math.Pow(s.Value, -w[9]) * (Math.Exp((1 - ret) * w[10]) - 1);
                        nextS = s.Value * (1 + sInc);
                    }
                    
                    // update difficulty (d)
                    // if the rating was "easy" (4), we lower difficulty
                    // if "hard" (2) or "again" (1), we raise it
                    // note: (rIdx - 2) centers the math around "good"
                    double nextD = d.Value - w[6] * (rIdx - 2);

                    // apply mean reversion
                    // this stops the difficulty from getting stuck at extreme values
                    // it gently pulls it back toward the average over time
                    nextD = (1 - w[7]) * nextD + w[7] * w[4];

                    // make sure difficulty stays within safe bounds (1 to 10)
                    d = Math.Clamp(nextD, 1.0, 10.0);

                    // save the new stability for the next loop
                    s = Math.Max(0.001, nextS);
                }
            }

            if (s.HasValue)
            {
                // predict the probability of recall at the current time
                double rPred = Math.Pow(1.0 + (Math.Pow(0.9, 1.0 / -w[20]) - 1.0) * sample.TargetInterval / s.Value, -w[20]);
                rPred = Math.Clamp(rPred, 1e-9, 1 - 1e-9);

                // calculate how wrong our prediction was (log loss)
                localSum += -(sample.Label * Math.Log(rPred) + (1 - sample.Label) * Math.Log(1 - rPred));
            }
            return localSum;
        }, 
        (localSum) => { lock (lockObj) totalLogLoss += localSum; });

        double avgLogLoss = totalLogLoss / data.Count;

        // add a penalty if weights drift too far from defaults (regularization)
        double regPenalty = 0;
        for(int i=0; i<21; i++)
        {
            double diff = w[i] - DefaultWeights[i];
            regPenalty += diff * diff;
        }
        
        return avgLogLoss + (_regularization * regPenalty);
    }

}
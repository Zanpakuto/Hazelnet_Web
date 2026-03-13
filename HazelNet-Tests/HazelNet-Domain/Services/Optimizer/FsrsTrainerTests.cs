using Xunit;
using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using HazelNet.Services.Optimizer;

namespace HazelNet.Tests.Optimizer
{
    public class FsrsTrainerTests
    {
        // ---------------------------------------------------------
        // 1. validation tests (guardrails)
        // ---------------------------------------------------------

        [Fact]
        public void Train_NullDataset_ThrowsArgumentException()
        {
            var trainer = new FsrsTrainer();
            // if we pass null, it should scream at us.
            trainer.Invoking(t => t.Train(null!)).Should().Throw<ArgumentException>();
        }

        [Fact]
        public void Train_EmptyDataset_ThrowsArgumentException()
        {
            var trainer = new FsrsTrainer();
            // training on nothing is impossible, so we expect an error here too.
            trainer.Invoking(t => t.Train(new List<FsrsTrainingSample>())).Should().Throw<ArgumentException>();
        }

        // ---------------------------------------------------------
        // 2. logic & output integrity tests
        // ---------------------------------------------------------

        [Fact]
        public void Train_ReturnsCorrectArrayLength()
        {
            var trainer = new FsrsTrainer();
            var data = GenerateMockData(count: 10);

            // just a sanity check to make sure we get back exactly 21 weights.
            var weights = trainer.Train(data, epochs: 1, batchSize: 10);

            weights.Should().NotBeNull();
            weights.Length.Should().Be(21);
        }

        [Fact]
        public void Train_WeightsRespectConstraints()
        {
            // making sure the adam optimizer keeps things sane.
            // indices 0-19 should stay within [0.001, 30.0].
            // index 20 (decay) has to be between [0.01, 0.9] or the math breaks.
            
            var trainer = new FsrsTrainer(learningRate: 0.1); 
            var data = GenerateMockData(count: 50);

            var weights = trainer.Train(data, epochs: 2);

            // checking the main stability/difficulty weights
            for (int i = 0; i < 20; i++)
            {
                weights[i].Should().BeGreaterThanOrEqualTo(0.001, $"Weight {i} ({weights[i]}) is below 0.001");
                weights[i].Should().BeLessThanOrEqualTo(30.0, $"Weight {i} ({weights[i]}) is above 30.0");
            }

            // checking the retention decay parameter
            weights[20].Should().BeGreaterThanOrEqualTo(0.01, $"Decay weight ({weights[20]}) is below 0.01");
            weights[20].Should().BeLessThanOrEqualTo(0.9, $"Decay weight ({weights[20]}) is above 0.9");
        }

        [Fact]
        public void Train_LossDoesNotReturnNaN_OnEdgeCaseData()
        {
            // this is a crucial stability test. if the interval is super tiny (like 0.0001),
            // the log calculation could explode into infinity or nan. we need to ensure the math handles it.
            var trainer = new FsrsTrainer();
            
            var data = new List<FsrsTrainingSample>
            {
                new FsrsTrainingSample
                {
                    HistoryLength = 1,
                    TimeHistory = new float[] { 0, 0, 0, 0, 1.0f }, 
                    RatingHistory = new int[] { 0, 0, 0, 0, 3 },
                    TargetInterval = 0.0001f, // super tiny interval risk
                    Label = 1.0f
                }
            };

            var weights = trainer.Train(data, epochs: 1);
            
            weights.Should().NotContain(double.NaN);
            weights.Should().NotContain(double.NegativeInfinity).And.NotContain(double.PositiveInfinity);
        }

        // ---------------------------------------------------------
        // 3. batching & concurrency
        // ---------------------------------------------------------

        [Theory]
        [InlineData(10, 100)]  // batch size bigger than dataset
        [InlineData(100, 10)]  // standard batching
        [InlineData(50, 50)]   // exact match
        public void Train_HandlesVariousBatchSizes(int datasetSize, int batchSize)
        {
            var trainer = new FsrsTrainer();
            var data = GenerateMockData(datasetSize);

            // just making sure the batching logic doesn't throw an index out of range exception
            // when the numbers don't divide perfectly.
            var exception = Record.Exception(() => trainer.Train(data, epochs: 1, batchSize: batchSize));
            
            exception.Should().BeNull();
        }

        [Fact]
        public void Train_LargeDataset_ThreadSafety()
        {
            // checking if the parallel.foreach loop is actually thread-safe.
            // if we have race conditions, we usually end up with nans or infinity in the weights.
            var trainer = new FsrsTrainer();
            var data = GenerateMockData(5000); 

            var weights = trainer.Train(data, epochs: 1, batchSize: 1024);
            
            weights.Length.Should().Be(21);
            weights.Should().NotContain(double.NegativeInfinity).And.NotContain(double.PositiveInfinity);
            weights.Should().NotContain(double.NaN);
        }
        
        // ---------------------------------------------------------
        // 4. convergence heuristic
        // ---------------------------------------------------------
        [Fact]
        public void Train_WeightsChangeWithTraining()
        {
            // basic check to see if the model is actually learning.
            // if the weights are exactly the same as the defaults after 5 epochs,
            // then the optimizer isn't stepping at all.
            
            var trainer = new FsrsTrainer(learningRate: 0.05);
            var data = GenerateMockData(100);
            
            var trainedWeights = trainer.Train(data, epochs: 5);
            
            bool hasChanged = false;
            for(int i=0; i<21; i++) 
            {
                // using a tiny epsilon to detect even small movements
                if (Math.Abs(trainedWeights[i] - FsrsTrainer.DefaultWeights[i]) > 1e-9) 
                {
                    hasChanged = true;
                    break;
                }
            }
            
            hasChanged.Should().BeTrue("Weights remained identical to defaults after training. The optimizer may be stuck.");
        }

        // ---------------------------------------------------------
        // helpers
        // ---------------------------------------------------------

        private List<FsrsTrainingSample> GenerateMockData(int count)
        {
            var list = new List<FsrsTrainingSample>();
            var rng = new Random(42); 

            for (int i = 0; i < count; i++)
            {
                var sample = new FsrsTrainingSample
                {
                    HistoryLength = rng.Next(1, 6), // random history length 1-5
                    TimeHistory = new float[5],
                    RatingHistory = new int[5],
                    TargetInterval = (float)rng.Next(1, 100),
                    Label = (float)(rng.NextDouble() > 0.5 ? 1.0 : 0.0)
                };

                // mimicking the processor logic: fill from the right side (index 4) backwards.
                // this ensures the trainer reads the most recent history correctly.
                for(int j = 0; j < sample.HistoryLength; j++)
                {
                     int idx = 4 - j;
                     sample.TimeHistory[idx] = rng.Next(1, 30);
                     sample.RatingHistory[idx] = rng.Next(1, 5); 
                }

                list.Add(sample);
            }
            return list;
        }
    }
}
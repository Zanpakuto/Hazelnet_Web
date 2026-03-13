using Xunit;
using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using HazelNet.Service.Optimizer; 
using HazelNet.Services.Optimizer;
using HazelNet_Domain.Models;

namespace HazelNet.Tests.Optimizer
{
    public class FsrsPipelineTests
    {
        // ---------------------------------------------------------
        // 1. Initialization & Sanity
        // ---------------------------------------------------------

        [Fact]
        public void Pipeline_Components_InitializeCorrectly()
        {
            var processor = new FsrsDataProcessor();
            var trainer = new FsrsTrainer();
            var optimizer = new FsrsOptimizationService(); 
            
            processor.Should().NotBeNull();
            trainer.Should().NotBeNull();
            optimizer.Should().NotBeNull();
        }

        // ---------------------------------------------------------
        // 2. Integration Tests (Processor -> Trainer)
        // ---------------------------------------------------------

        [Fact]
        public void Pipeline_DataHandover_IsShapeCompliant()
        {
            // CRITICAL TEST: Verifies that the Processor output strictly matches
            // the Trainer's input requirements (Arrays of length 5, non-null).
            
            var processor = new FsrsDataProcessor();
            
            // FIX: Use constructor and Add/AddRange instead of object initializer for read-only lists
            var historyItem = CreateReviewHistory(cardId: 1, reviewCount: 5);
            var histories = new List<ReviewHistory> { historyItem };

            // Act
            var samples = processor.ProcessHistoryToSamples(histories);

            // Assert (The Handover Inspection)
            samples.Should().NotBeEmpty();
            foreach(var sample in samples)
            {
                // Trainer will crash if these are not exactly 5
                sample.TimeHistory.Length.Should().Be(5);
                sample.RatingHistory.Length.Should().Be(5);
                
                // Trainer Math.Log() will crash if TargetInterval is <= 0
                sample.TargetInterval.Should().BeGreaterThan(0, "Pipeline passed an invalid interval to Trainer");
            }
        }

        [Fact]
        public void Integration_ProcessorToTrainer_FullFlow()
        {
            // Arrange: Real history data
            var processor = new FsrsDataProcessor();
            var trainer = new FsrsTrainer();
            
            // FIX: Create list first, then add to History object
            var logs = new List<ReviewLog>
            {
                new ReviewLog { ElapsedDays = 0, Rating = Rating.Good, Review = DateTime.Now.AddDays(-20) },
                new ReviewLog { ElapsedDays = 2, Rating = Rating.Hard, Review = DateTime.Now.AddDays(-18) },
                new ReviewLog { ElapsedDays = 5, Rating = Rating.Good, Review = DateTime.Now.AddDays(-13) },
                new ReviewLog { ElapsedDays = 10, Rating = Rating.Easy, Review = DateTime.Now.AddDays(-3) },
                new ReviewLog { ElapsedDays = 3, Rating = Rating.Again, Review = DateTime.Now }
            };

            var historyItem = new ReviewHistory(1);
            foreach (var log in logs) historyItem.ReviewLogs.Add(log);

            var history = new List<ReviewHistory> { historyItem };

            // Act 1: Process
            var dataset = processor.ProcessHistoryToSamples(history);

            // Act 2: Train
            // Run enough epochs to trigger Adam updates
            var weights = trainer.Train(dataset, epochs: 2, batchSize: 5);

            // Assert
            weights.Should().NotBeNull();
            weights.Length.Should().Be(21);
            
            // Sanity Checks
            weights.Should().NotContain(double.NaN);
            weights.Should().NotContain(double.NegativeInfinity).And.NotContain(double.PositiveInfinity);
            weights[0].Should().BeGreaterThan(0);
        }

        [Fact]
        public void Pipeline_FullFlow_ProducesValidWeights_OnGeneratedData()
        {
            // Arrange
            var processor = new FsrsDataProcessor();
            var trainer = new FsrsTrainer();
            var histories = GenerateReviewHistories(count: 20); // Sufficient data for stability

            // Act: Step 1 (Process)
            var samples = processor.ProcessHistoryToSamples(histories);
            
            // Act: Step 2 (Train)
            var weights = trainer.Train(samples, epochs: 2, batchSize: 5);

            // Assert
            weights.Should().NotBeNull();
            weights.Length.Should().Be(21);
            
            // CRITICAL: Ensure math didn't explode
            weights.Should().NotContain(double.NaN);
            weights.Should().NotContain(double.NegativeInfinity).And.NotContain(double.PositiveInfinity);
            
            // Check that the first 4 weights (Stability) are positive
            weights[0].Should().BeGreaterThan(0);
            weights[1].Should().BeGreaterThan(0);
            weights[2].Should().BeGreaterThan(0);
            weights[3].Should().BeGreaterThan(0);
        }

        // ---------------------------------------------------------
        // 3. Wrapper Service Tests (FsrsOptimizationService)
        // ---------------------------------------------------------

        [Fact]
        public void OptimizationService_EndToEnd_ReturnsOptimizedWeights()
        {
            var optimizer = new FsrsOptimizationService();
            var histories = GenerateReviewHistories(count: 50);

            var weights = optimizer.OptimizeWeights(histories, epochs: 1, batchSize: 10);
            
            weights.Should().NotBeNull();
            weights.Length.Should().Be(21);
        }

        [Fact]
        public void Pipeline_HandlesInsufficientData_Gracefully()
        {
            var processor = new FsrsDataProcessor();
            var trainer = new FsrsTrainer();
            
            // Create histories that are too short to generate samples
            var shortHistory = CreateReviewHistory(cardId: 1, reviewCount: 1); // Only 1 review
            var shortHistories = new List<ReviewHistory> { shortHistory };

            var samples = processor.ProcessHistoryToSamples(shortHistories);
            
            samples.Should().BeEmpty();
            
            // The trainer should throw if list is empty
            trainer.Invoking(t => t.Train(samples)).Should().Throw<ArgumentException>();

        }

        // ---------------------------------------------------------
        // Helpers
        // ---------------------------------------------------------

        private List<ReviewHistory> GenerateReviewHistories(int count)
        {
            var list = new List<ReviewHistory>();
            for (int i = 0; i < count; i++)
            {
                // Create with at least 3 reviews to ensure valid samples
                list.Add(CreateReviewHistory(cardId: i, reviewCount: 5));
            }
            return list;
        }

        private ReviewHistory CreateReviewHistory(int cardId, int reviewCount)
        {
            var history = new ReviewHistory(cardId);
            var rng = new Random(cardId); // Fixed seed for reproducibility
            var now = DateTime.Now;

            for (int i = 0; i < reviewCount; i++)
            {
                var rating = (Rating)(rng.Next(1, 5)); // Rating 1-4
                var reviewDate = now.AddDays(-reviewCount + i);
                // Ensure elapsed days is never 0 for the target (trainer math safety)
                var elapsedDays = (ulong)(i > 0 ? 1 : 0); 

                var log = new ReviewLog
                {
                    Review = reviewDate,
                    Rating = rating,
                    ElapsedDays = elapsedDays
                };

                history.ReviewLogs.Add(log);
            }
            return history;
        }
    }
}
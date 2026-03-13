using Xunit;
using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using HazelNet.Service.Optimizer;
using HazelNet_Domain.Models;

namespace HazelNet.Tests.Optimizer
{
    public class FsrsDataProcessorTests
    {
        private readonly FsrsDataProcessor _processor = new FsrsDataProcessor();

        // ---------------------------------------------------------
        // 1. Initialization & Stability Tests
        // ---------------------------------------------------------

        [Fact]
        public void Processor_InitializesSuccessfully()
        {
            // assert
            _processor.Should().NotBeNull();            
        }

        [Fact]
        public void ProcessHistoryToSamples_ValidData_ReturnsProcessedOutput()
        {
            var inputData = GenerateReviewHistories(count: 10);
            
            var result = _processor.ProcessHistoryToSamples(inputData);
            
            result.Should().NotBeNull().And.NotBeEmpty();
        }

        [Fact]
        public void ProcessHistoryToSamples_HandlesLargeDataset()
        {
            var histories = GenerateReviewHistories(count: 100);
            
            var samples = _processor.ProcessHistoryToSamples(histories);

            samples.Should().NotBeNull();
            samples.Count.Should().BeGreaterThan(0);
        }

        [Fact]
        public void ProcessHistoryToSamples_HandlesEmptyReviews()
        {
            var histories = new List<ReviewHistory>();
            
            var samples = _processor.ProcessHistoryToSamples(histories);

            samples.Should().NotBeNull().And.BeEmpty();
        }

        // ---------------------------------------------------------
        // 2. Logic Verification (The Math & Rules)
        // ---------------------------------------------------------

        [Fact]
        public void ProcessHistoryToSamples_ShortHistory_RightAlignsData()
        {
            var logs = new List<ReviewLog>
            {
                new ReviewLog { ElapsedDays = 0, Rating = Rating.Good, Review = DateTime.Now.AddDays(-5) },
                new ReviewLog { ElapsedDays = 5, Rating = Rating.Good, Review = DateTime.Now }
            };
            
            // FIX: Don't assign ReviewLogs via initializer. Add to the existing list.
            var historyItem = new ReviewHistory(1);
            foreach (var log in logs) historyItem.ReviewLogs.Add(log);
            
            var history = new List<ReviewHistory> { historyItem };

            var result = _processor.ProcessHistoryToSamples(history);
            var sample = result.First();

            //Assert.Equal(1, sample.HistoryLength);
            sample.HistoryLength.Should().Be(1);
            sample.RatingHistory[0].Should().Be(0);
            sample.RatingHistory[3].Should().Be(0);
            sample.RatingHistory[4].Should().Be((int)Rating.Good);
        }

        [Fact]
        public void ProcessHistoryToSamples_UnsortedInput_SortsChronologically()
        {
            var logs = new List<ReviewLog>
            {
                new ReviewLog { ElapsedDays = 5, Rating = Rating.Good, Review = DateTime.Now }, 
                new ReviewLog { ElapsedDays = 0, Rating = Rating.Good, Review = DateTime.Now.AddDays(-10) }, 
                new ReviewLog { ElapsedDays = 5, Rating = Rating.Good, Review = DateTime.Now.AddDays(-5) } 
            };
            
            // FIX: Use foreach loop instead of AddRange
            var historyItem = new ReviewHistory(1);
            foreach (var log in logs) historyItem.ReviewLogs.Add(log);

            var history = new List<ReviewHistory> { historyItem };

            var result = _processor.ProcessHistoryToSamples(history);
            
            result.Count().Should().Be(2);
            
            var lastSample = result.Last();
            
            lastSample.HistoryLength.Should().Be(2);
        }

        [Fact]
        public void ProcessHistoryToSamples_ConvertsRatingsToBinaryLabels()
        {
            var logs = new List<ReviewLog>
            {
                new ReviewLog { Rating = Rating.Good, Review = DateTime.Now.AddDays(-2) }, 
                new ReviewLog { Rating = Rating.Again, Review = DateTime.Now.AddDays(-1) }, 
                new ReviewLog { Rating = Rating.Easy, Review = DateTime.Now }              
            };
            
            // FIX: Use foreach loop instead of AddRange
            var historyItem = new ReviewHistory(1);
            foreach (var log in logs) historyItem.ReviewLogs.Add(log);

            var history = new List<ReviewHistory> { historyItem };

            var result = _processor.ProcessHistoryToSamples(history);
            
            result.Count.Should().Be(2);
            result[0].Label.Should().Be(0.0f);
            result[1].Label.Should().Be(1.0f);
        }

        [Fact]
        public void ProcessHistoryToSamples_LongHistory_TruncatesToMaxWindow()
        {
            var logs = new List<ReviewLog>();
            for(int i=0; i<10; i++)
            {
                logs.Add(new ReviewLog { ElapsedDays = 1, Rating = Rating.Good, Review = DateTime.Now.AddDays(i) });
            }
            
            // FIX: Use foreach loop instead of AddRange
            var historyItem = new ReviewHistory(1);
            foreach (var log in logs) historyItem.ReviewLogs.Add(log);

            var history = new List<ReviewHistory> { historyItem };
            
            var result = _processor.ProcessHistoryToSamples(history);

            var lastSample = result.Last();
            
            lastSample.HistoryLength.Should().Be(5);
            
            lastSample.RatingHistory.Should().NotContain(0);
        }

        // ---------------------------------------------------------
        // Helpers
        // ---------------------------------------------------------

        private List<ReviewHistory> GenerateReviewHistories(int count)
        {
            var list = new List<ReviewHistory>();
            for (int i = 0; i < count; i++)
            {
                list.Add(CreateReviewHistory(cardId: i, reviewCount: 5));
            }
            return list;
        }

        private ReviewHistory CreateReviewHistory(int cardId, int reviewCount)
        {
            var history = new ReviewHistory(cardId);
            var rng = new Random(cardId);
            var now = DateTime.Now;

            for (int i = 0; i < reviewCount; i++)
            {
                var rating = (Rating)(rng.Next(1, 5)); 
                var reviewDate = now.AddDays(-reviewCount + i);
                var elapsedDays = (ulong)(i > 0 ? 1 : 0); 

                var log = new ReviewLog
                {
                    Review = reviewDate,
                    Rating = rating,
                    ElapsedDays = elapsedDays
                };

                // FIX: Use Add instead of modifying collection directly if it was setter-based
                history.ReviewLogs.Add(log);
            }
            return history;
        }
    }
}
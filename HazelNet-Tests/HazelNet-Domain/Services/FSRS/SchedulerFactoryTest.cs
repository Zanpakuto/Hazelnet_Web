using System.Reflection;
using HazelNet_Domain.Services.FSRS;
using HazelNet_Domain.Models;

namespace HazelNet_Tests.HazelNet_Domain.Services.FSRS;
using Xunit;
using FluentAssertions;

public class SchedulerFactoryTest
{
    // SUT : SchedulerFactory.cs
    
    // arrange for all tests
    private readonly Parameters _param = new Parameters();
    private readonly Card _card = new() { FrontOfCard = "cardfront" };
    private readonly DateTime _now = DateTime.Now;

    [Fact]
    public void SchedulerFactory_SchedulerFor_ReturnsBasicScheduler_WhenShortTermEnabled()
    {
        // arrange
        _param.EnableShortTerm = true;
        // act
        var result = SchedulerFactory.SchedulerFor(_param, _card, _now);
        
            // uses GetField() to access the internal member 'impl'
        var fieldInfo = typeof(Scheduler).GetField("impl",
            BindingFlags.NonPublic | BindingFlags.Instance);
        // assert
        var actualImpl = fieldInfo.GetValue(result);
        actualImpl.Should().NotBeNull().And.BeOfType<BasicScheduler>();
    }

    [Fact]
    public void SchedulerFactory_SchedulerFor_ReturnsLongTermScheduler_WhenShortTermDisabled()
    {
        // arrange
        _param.EnableShortTerm = false;
        // act
        var result = SchedulerFactory.SchedulerFor(_param, _card, _now);

        var fieldInfo = typeof(Scheduler).GetField("impl",
            BindingFlags.NonPublic | BindingFlags.Instance);
        // assert
        var actualImpl = fieldInfo.GetValue(result);
        actualImpl.Should().NotBeNull().And.BeOfType<LongTermScheduler>();
    }
}
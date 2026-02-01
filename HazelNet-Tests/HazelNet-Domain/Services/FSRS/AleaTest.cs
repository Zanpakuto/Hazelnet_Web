using HazelNet_Domain.Services.FSRS;
using Xunit;
using FluentAssertions;

namespace HazelNet_Tests.HazelNet_Domain.Services.FSRS;

public class AleaTest
{
    /*
     * SUT: FSRS/Alea.cs
     */

    [Theory]
    [InlineData(1738.67)]
    [InlineData(337.20)]
    [InlineData("ch4rl13k1rk")]
    [InlineData("6aAWiXy")]
    [InlineData(1001)]
    [InlineData(69420)]
    [InlineData('A')]
    [InlineData('X')]
    public void Alea_New_ReturnsAleaObject(object seedValue) {
        // This test also indirectly proves the correctness of
        // the method Mash(). Hence, no test method will be written
        // for it since it is a private method of the SUT
        
        // arrange
        // act
        var result = Alea.New(seedValue);
        // assert 
        result.Should().BeOfType<Alea>();
        result.Should().NotBeNull();
    }

    [Fact]
    public void Alea_Next_ReturnsDouble() {
        // arrange
        var aleaObj = Alea.New();
        // act
        var result = aleaObj.Next();
        // assert
        result.Should().NotBe(double.NaN);
        result.Should().BeGreaterThanOrEqualTo(0).And.BeLessThan(1);
    }

    [Fact]
    public void Alea_Double_ReturnsDouble()
    {
        // arrange
        var aleaObj = Alea.New();
        // act
        var result = aleaObj.Double();
        // assert
        result.Should().NotBe(double.NaN);
        result.Should().BeGreaterThanOrEqualTo(0).And.BeLessThan(1);
    }

    [Theory]
    [InlineData(1, 0.431, 0.8910, 0.107)]
    [InlineData(1, 0.4310, 0.2609, 0.65504)]
    [InlineData(1, 0.5016, 0.5883, 0.922175)]
    public void Alea_SetState_SetsPrivateMembersValues(double c, double s0, double s1, double s2)
    {
        // arrange
        var aleaObj = Alea.New();
        AleaState sampleState = new AleaState {
            C = c, S0 = s0, S1 = s1, S2 = s2
        };
        AleaState result;
        // act
        aleaObj.SetState(sampleState);
        // assert
        result = aleaObj.GetState();
        
        result.C.Should().Be(1);
        
        result.S0.Should().NotBe(double.NaN);
        result.S0.Should().BeGreaterThan(0).And.BeLessThan(1);
        result.S0.Should().Be(s0);
        
        result.S1.Should().NotBe(double.NaN);
        result.S1.Should().BeGreaterThan(0).And.BeLessThan(1);
        result.S1.Should().Be(s1);
        
        result.S2.Should().NotBe(double.NaN);
        result.S2.Should().BeGreaterThan(0).And.BeLessThan(1);
        result.S2.Should().Be(s2);
    }
    
}
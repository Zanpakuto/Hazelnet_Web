using System.Collections.Generic;
using HazelNet_Domain.Models;

namespace HazelNet_Domain.Services.FSRS;

public class Parameters
{
    public double RequestRetention { get; set; }
    public double MaximumInterval { get; set; }
    public double[] W { get; set; }
    public double Decay { get; set; }
    public double Factor { get; set; }
    public bool EnableShortTerm { get; set; }
    public bool EnableFuzz { get; set; }
    internal string seed;

    private const double MinStability = 0.01;
    private const double MinRetrievability = 0.0001;

    public Parameters()
    {
        W = Weights.Default();
    }

    public static Parameters DefaultParam()
    {
        double DR = 0.95;
        double decay = 0.0658;
        double factor = Math.Pow(DR, 1.0 / decay) - 1;
        return new Parameters
        {
            RequestRetention = 0.9,
            MaximumInterval = 365,
            W = Weights.Default(),
            Decay = decay,
            Factor = factor,
            EnableShortTerm = true,
            EnableFuzz = false,
            seed = ""
        };
    }

    public double ForgettingCurve(double elapsedDays, double stability)
    {
        double s = Math.Max(stability, MinStability);
        double val = Math.Pow(1 + Factor * elapsedDays / stability, Decay);
        
        if(double.IsNaN(val) || double.IsInfinity(val) || val <= 0)
        {
            val = MinRetrievability;
        }

        return val;
    }

    public double InitStability(Rating r)
    {
        int idx = (int)r - 1;
        double val = W[idx];
        return Math.Max(val, MinStability);
    }

    public double InitDifficulty(Rating r)
    {
        double d = W[4] - Math.Exp(W[5] * ((float)r - 1)) + 1;
        return ConstrainDifficulty(d);
    }

    private static double ConstrainDifficulty(double d)
    {
        return Math.Min(Math.Max(d, 1.0), 10.0);
    }

    private static double LinearDamping(double deltaD, double oldD)
    {
        return (10.0 - oldD) * deltaD / 9.0;
    }

    public double NextInterval(double s, double elapsedDays)
    {
        double newInterval = s / Factor * (Math.Pow(RequestRetention, 1.0 / Decay) - 1.0);
        double rounded = Math.Round(newInterval);
        double clamped = Math.Max(Math.Min(rounded, MaximumInterval), 1);
        return ApplyFuzz(clamped, elapsedDays, EnableFuzz);
    }

    public double ApplyFuzz(double ivl, double elapsedDays, bool enableFuzz)
    {
        if (!enableFuzz || ivl < 2.5) return ivl;

        var gen = Alea.New(seed);
        double fuzzFactor = gen.Double();
        (int minIvl, int maxIvl) = GetFuzzRange(ivl, elapsedDays, MaximumInterval);
        return Math.Floor(fuzzFactor * (maxIvl - minIvl + 1)) + minIvl;
    }

    private (int, int) GetFuzzRange(double interval, double elapsedDays, double maximumInterval)
    {
        var ranges = new (double Start, double End, double Factor)[] {
            (2.5, 7.0, 0.15),
            (7.0, 20.0, 0.1),
            (20.0, double.PositiveInfinity, 0.05)
        };

        double delta = 1.0;
        foreach (var r in ranges)
        {
            delta += r.Factor * Math.Max(Math.Min(interval, r.End) - r.Start, 0.0);
        }

        interval = Math.Min(interval, maximumInterval);
        double minIvlFloat = Math.Max(2.0, Math.Round(interval - delta));
        double maxIvlFloat = Math.Min(Math.Round(interval + delta), maximumInterval);

        if (interval > elapsedDays)
        {
            minIvlFloat = Math.Max(minIvlFloat, elapsedDays + 1);
        }
        minIvlFloat = Math.Min(minIvlFloat, maxIvlFloat);

        return ((int)minIvlFloat, (int)maxIvlFloat);
    }

    public double NextDifficulty(double d, Rating r)
    {
        double deltaD = -W[6] * (float)(r - 3);
        double nextD = d + LinearDamping(deltaD, d);
        double init = InitDifficulty(Rating.Hard);
        return ConstrainDifficulty(MeanReversion(init, nextD));
    }

    public double ShortTermStability(double s, Rating r)
    {
        double baseS = Math.Max(s, MinStability);
        return baseS * Math.Exp(W[17] * ((float)(r - 3) + W[18]));
    }

    public double MeanReversion(double init, double current)
    {
        return W[7] * init + (1 - W[7]) * current;
    }

    public double NextRecallStability(double d, double s, double r, Rating rating)
    {
        double hardPenalty = rating == Rating.Hard ? W[15] : 1.0;
        double easyBonus = rating == Rating.Easy ? W[16] : 1.0;

        // clamp input stability to avoid raising zero to a negative power
        double baseS = Math.Max(s, MinStability);

        double multiplier = 1 + Math.Exp(W[8]) *
            (11 - d) *
            Math.Pow(baseS, -W[9]) *
            (Math.Exp((1 - r) * W[10]) - 1) *
            hardPenalty * easyBonus;

        double next = s * multiplier;

        // guard: ensure finite and sensible minimum
        if (double.IsNaN(next) || double.IsInfinity(next) || next <= 0)
        {
            next = MinStability;
        }

        return next;
    }

    public double NextForgetStability(double d, double s, double r)
    {
        // clamp input stability to avoid pathological inputs
        double baseS = Math.Max(s, MinStability);

        double next = W[11] *
            Math.Pow(d, -W[12]) *
            (Math.Pow(baseS + 1, W[13]) - 1) *
            Math.Exp((1 - r) * W[14]);

        if (double.IsNaN(next) || double.IsInfinity(next) || next <= 0)
        {
            next = MinStability;
        }

        return next;
    }
}
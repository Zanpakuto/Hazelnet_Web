using System;
using System.Text;

namespace HazelNet_Domain.Services.FSRS;

public struct AleaState
{
    public double C, S0, S1, S2;
}

public class Alea
{
    private double c, s0, s1, s2;

    private Alea(double c, double s0, double s1, double s2)
    {
        this.c = c; this.s0 = s0; this.s1 = s1; this.s2 = s2;
    }

    public static Alea New(object seed = null)
    {
        var mash = Mash();
        double _s0 = mash(" ");
        double _s1 = mash(" ");
        double _s2 = mash(" ");

        if (seed == null) seed = DateTime.UtcNow.Ticks;
        string seedStr = seed.ToString();

        _s0 -= mash(seedStr); if (_s0 < 0) _s0 += 1;
        _s1 -= mash(seedStr); if (_s1 < 0) _s1 += 1;
        _s2 -= mash(seedStr); if (_s2 < 0) _s2 += 1;

        return new Alea(1, _s0, _s1, _s2);
    }

    public double Next()
    {
        double t = 2091639 * s0 + c * 2.3283064365386963e-10; // 2^-32
        s0 = s1;
        s1 = s2;
        s2 = t - Math.Floor(t);
        c = Math.Floor(t);
        return s2;
    }

    public double Double()
    {
        // replicate Go implementation: prng() + uint32(prng()*0x200000)*1.1102230246251565e-16
        double a = Next();
        double b = Next();
        uint part = (uint)(b * 0x200000);
        return a + part * 1.1102230246251565e-16;
    }

    public void SetState(AleaState st)
    {
        c = st.C; s0 = st.S0; s1 = st.S1; s2 = st.S2;
    }

    public AleaState GetState()
    {
        return new AleaState { C = c, S0 = s0, S1 = s1, S2 = s2 };
    }

    private static Func<string, double> Mash()
    {
        uint n = 0xefc8249d;
        return (string data) =>
        {
            for (int i = 0; i < data.Length; i++)
            {
                n += (byte)data[i];
                double h = 0.02519603282416938 * (double)n;
                n = (uint)h;
                h -= (double)n;
                h *= (double)n;
                n = (uint)h;
                h -= (double)n;
                n += (uint)(h * 0x100000000L);
            }
            return (double)n * 2.3283064365386963e-10;
        };
    }
}
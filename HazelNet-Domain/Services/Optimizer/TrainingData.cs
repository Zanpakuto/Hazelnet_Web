using System.Net.NetworkInformation;

namespace HazelNet.Services.Optimizer;

//data struct for holding data, isolated in optim since this is it's only use case
public class FsrsTrainingSample
{
    //utilized fixed size arrays for mem optim
    public float[] TimeHistory;
    public int[] RatingHistory;
    public int HistoryLength;
    public float TargetInterval;
    public float Label;
}
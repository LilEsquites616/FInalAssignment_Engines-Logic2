using Unity.Services.Analytics;
public class WaveReachedEvent : Event
{
    public WaveReachedEvent() : base("waveReached") {}
    public int waveNumber {set{ SetParameter("waveNumber",value);}}
}

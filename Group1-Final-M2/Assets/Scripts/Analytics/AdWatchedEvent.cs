using Unity.Services.Analytics;
public class AdWatchedEvent : Event
{
    public AdWatchedEvent() : base("adWatched") {}
    public string AdType {set{ SetParameter("adType",value);}}
}

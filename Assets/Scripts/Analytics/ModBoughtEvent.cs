using Unity.Services.Analytics;
public class ModBoughtEvent : Event
{
    public ModBoughtEvent() : base("modBought") {}
    public string ModType {set{ SetParameter("modType",value);}}
}

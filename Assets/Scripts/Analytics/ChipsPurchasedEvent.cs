using Unity.Services.Analytics;
public class ChipsPurchasedEvent : Event
{
    public ChipsPurchasedEvent() : base("chipsPurchased") {}
    public int ChipAmount{set { SetParameter("chipAmount", value); }}
}

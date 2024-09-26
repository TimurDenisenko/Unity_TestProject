
public class Inventory : Storage
{
    private void Awake()
    {
        StaticSoldier.Inventory = this;
        SlotsCreating();
    }
}

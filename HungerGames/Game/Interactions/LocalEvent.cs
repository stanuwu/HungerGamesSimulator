namespace HungerGames.Game.Interactions
{
    public class LocalEvent
    {
        public static readonly List<LocalEvent> Events = new List<LocalEvent>()
        {
            new LocalEvent(player => { return new SimulationLog(new List<Player>() { player }, "%player% thinks about the dead tributes."); }),
            new LocalEvent(player =>
            {
                player.Items.Add(Item.Items.Find(i => i.Name == "Water"));
                return new SimulationLog(new List<Player>() { player }, "%player% finds a river.");
            }),
            new LocalEvent(player =>
            {
                player.Items.Add(Item.Items.Find(i => i.Name == "Food"));
                return new SimulationLog(new List<Player>() { player }, "%player% hunts for food.");
            }),
            new LocalEvent(player =>
            {
                player.Sanity -= 10;
                return new SimulationLog(new List<Player>() { player }, "%player% wakes up from a nightmare.");
            }),
            new LocalEvent(player =>
            {
                player.Sanity -= 15;
                return new SimulationLog(new List<Player>() { player }, "%player% can't fall asleep.");
            }),
            new LocalEvent(player =>
            {
                player.Sanity += 10;
                return new SimulationLog(new List<Player>() { player }, "%player% has a beautiful dream.");
            }),
            new LocalEvent(player =>
            {
                player.Sanity += 15;
                return new SimulationLog(new List<Player>() { player }, "%player% meditates.");
            }),
            new LocalEvent(player =>
            {
                player.Vitality -= 25;
                player.Sanity -= 5;
                return new SimulationLog(new List<Player>() { player }, "%player% catches a cold.");
            }),
            new LocalEvent(player =>
            {
                player.Vitality -= 10;
                player.Sanity -= 2;
                return new SimulationLog(new List<Player>() { player }, "%player% gets pricked while picking berries.");
            }),
            new LocalEvent(player =>
            {
                player.Vitality -= 30;
                return new SimulationLog(new List<Player>() { player }, "%player% falls down a tree.");
            }),
            new LocalEvent(player =>
            {
                Item item = Item.Items[GameManager.Instance.Random.Next(0, Item.Items.Count)];
                player.Items.Add(item);
                return new SimulationLog(new List<Player>() { player }, $"%player% receives {item.Name.ToLower()} from an unknown sponsor.");
            }),
            new LocalEvent(player =>
            {
                Item item = Item.Items[GameManager.Instance.Random.Next(0, Item.Items.Count)];
                player.Items.Add(item);
                return new SimulationLog(new List<Player>() { player }, $"%player% finds {item.Name.ToLower()}.");
            }),
        };

        public LocalEvent(Func<Player, SimulationLog?> run)
        {
            Run = run;
        }

        public Func<Player, SimulationLog?> Run { get; }
    }
}
namespace HungerGames.Game.Interactions
{
    public class LocalEvent
    {
        public static readonly List<LocalEvent> Events = new List<LocalEvent>()
        {
            new LocalEvent(player =>
            {
                string[] nothing = new[]
                {
                    "%player% thinks about the dead tributes.",
                    "%player% is unsure how to go on.",
                    "%player% plans %pronoun6% day.",
                    "%player% falls asleep.",
                    "%player% can't decide what to do next.",
                    "%player% is goofing off.",
                };
                return new SimulationLog(new List<Player>() { player }, nothing[GameManager.Instance.Random.Next(0, nothing.Length)]);
            }),
            new LocalEvent(player =>
            {
                player.Items.Add(Item.Items.Find(i => i.Name == "Water"));
                string[] wt = new[]
                {
                    "%player% finds a river.",
                    "%player% finds a clear lake.",
                    "%player% collects some water off of tree leaves."
                };
                return new SimulationLog(new List<Player>() { player }, wt[GameManager.Instance.Random.Next(0, wt.Length)]);
            }),
            new LocalEvent(player =>
            {
                player.Items.Add(Item.Items.Find(i => i.Name == "Food"));
                string[] wt = new[]
                {
                    "%player% hunts for food.",
                    "%player% picks some mushrooms.",
                    "%player% guts a dead animal.",
                };
                return new SimulationLog(new List<Player>() { player }, wt[GameManager.Instance.Random.Next(0, wt.Length)]);
            }),
            new LocalEvent(player =>
            {
                player.Sanity -= 10;
                string[] wt = new[]
                {
                    "%player% wakes up from a nightmare.",
                    "%player% cries.",
                };
                return new SimulationLog(new List<Player>() { player }, wt[GameManager.Instance.Random.Next(0, wt.Length)]);
            }),
            new LocalEvent(player =>
            {
                player.Sanity -= 15;
                string[] wt = new[]
                {
                    "%player% can't fall asleep.",
                    "%player% is slowly going insane.",
                };
                return new SimulationLog(new List<Player>() { player }, wt[GameManager.Instance.Random.Next(0, wt.Length)]);
            }),
            new LocalEvent(player =>
            {
                player.Sanity += 10;
                string[] wt = new[]
                {
                    "%player% has a beautiful dream.",
                    "%player% enjoys nature.",
                };
                return new SimulationLog(new List<Player>() { player }, wt[GameManager.Instance.Random.Next(0, wt.Length)]);
            }),
            new LocalEvent(player =>
            {
                player.Sanity += 15;
                string[] wt = new[]
                {
                    "%player% meditates.",
                    "%player% soaks in the sunlight.",
                };
                return new SimulationLog(new List<Player>() { player }, wt[GameManager.Instance.Random.Next(0, wt.Length)]);
            }),
            new LocalEvent(player =>
            {
                player.Vitality -= 25;
                player.Sanity -= 5;
                string[] wt = new[]
                {
                    "%player% catches a cold.",
                    "%player% trips and breaks %pronoun6% nose.",
                    "%player% hurts %pronoun4% while hunting.",
                };
                return new SimulationLog(new List<Player>() { player }, wt[GameManager.Instance.Random.Next(0, wt.Length)]);
            }),
            new LocalEvent(player =>
            {
                player.Items.Add(Item.Items.Find(i => i.Name == "Food"));
                player.Vitality -= 10;
                player.Sanity -= 2;
                return new SimulationLog(new List<Player>() { player }, "%player% gets pricked while picking berries.");
            }),
            new LocalEvent(player =>
            {
                player.Vitality -= 30;
                string[] wt = new[]
                {
                    "%player% falls down a tree.",
                    "%player% almost drowns.",
                    "%player% is hit in the head by a falling branch.",
                };
                return new SimulationLog(new List<Player>() { player }, wt[GameManager.Instance.Random.Next(0, wt.Length)]);
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
            new LocalEvent(player =>
            {
                Item item = Item.Items[GameManager.Instance.Random.Next(0, Item.Items.Count)];
                player.Items.Add(item);
                return new SimulationLog(new List<Player>() { player }, $"%player% makes {item.Name.ToLower()} from surrounding materials.");
            }),
        };

        public LocalEvent(Func<Player, SimulationLog?> run)
        {
            Run = run;
        }

        public Func<Player, SimulationLog?> Run { get; }
    }
}
namespace HungerGames.Game.Interactions
{
    public class GlobalEvent
    {
        public static readonly List<GlobalEvent> Events = new List<GlobalEvent>()
        {
            new GlobalEvent("Poison fills the Arena.", player =>
            {
                int roll = GameManager.Instance.Random.Next(0, 100);
                // common outcome
                if (roll < 50)
                {
                    return new SimulationLog(new List<Player> { player }, "%player% evades the poison and survives.");
                }

                // rare outcome 1
                if (roll < 75)
                {
                    player.Sanity -= 15;
                    return new SimulationLog(new List<Player> { player }, "%player% thinks %pronoun1% inhaled poison and lose %pronoun6% mind.");
                }

                // rare outcome 2
                string happening = "%player% inhales some poison.";
                player.Vitality -= 25;
                if (player.Vitality <= 0)
                {
                    player.Alive = false;
                    happening += " %pronoun1% succumb%pronoun5% to the poison and die%pronoun5%.";
                }

                return new SimulationLog(new List<Player> { player }, happening);
            }),
            new GlobalEvent("Wolves go wild inside the arena.", player =>
            {
                int roll = GameManager.Instance.Random.Next(0, 100);
                // common outcome
                if (roll < 50)
                {
                    return new SimulationLog(new List<Player> { player }, "%player% evades the wolves and survives.");
                }

                // rare outcome 1
                if (roll < 75 && player.Stamina >= 15)
                {
                    player.Stamina -= 15;
                    return new SimulationLog(new List<Player> { player }, "%player% runs away from the wolves.");
                }

                // rare outcome 2
                string happening = "%player% is caught and mauled by the wolves.";
                player.Vitality -= 50;
                if (player.Vitality <= 0)
                {
                    player.Alive = false;
                    happening += " %pronoun1% die%pronoun5% of %pronoun6% wounds.";
                }

                return new SimulationLog(new List<Player> { player }, happening);
            }),
            new GlobalEvent("The arena broder shrinks.", player =>
            {
                // implemented differently
                return null;
            }),
            new GlobalEvent("The arena floods with water.", player =>
            {
                int roll = GameManager.Instance.Random.Next(0, 100);
                // common outcome
                if (roll < 50)
                {
                    return new SimulationLog(new List<Player> { player }, "%player% stays on higher ground.");
                }

                // rare outcome 1
                if (roll < 75 && player.Stamina >= 30)
                {
                    player.Stamina -= 30;
                    return new SimulationLog(new List<Player> { player }, "%player% manages to stay afloat.");
                }

                // rare outcome 2
                string happening = "%player% is caught by the torrent.";
                player.Vitality -= 50;
                if (player.Vitality <= 0)
                {
                    player.Alive = false;
                    happening += " %pronoun1% drown%pronoun5%.";
                }

                return new SimulationLog(new List<Player> { player }, happening);
            }),
            new GlobalEvent("The arena catches on fire.", player =>
            {
                int roll = GameManager.Instance.Random.Next(0, 100);
                // common outcome
                if (roll < 50)
                {
                    return new SimulationLog(new List<Player> { player }, "%player% watches the fire from afar.");
                }

                // rare outcome 1
                if (roll < 75 && player.Items.Count > 0)
                {
                    player.Items.Clear();
                    return new SimulationLog(new List<Player> { player }, "%player%s supplies burn in the fire.");
                }

                // rare outcome 2
                string happening = "%player% catches fire.";
                player.Vitality -= 50;
                if (player.Vitality <= 0)
                {
                    player.Alive = false;
                    happening += " %pronoun1% burn%pronoun5% to death.";
                }

                return new SimulationLog(new List<Player> { player }, happening);
            }),
            new GlobalEvent("An earthquake shakes the arena.", player =>
            {
                int roll = GameManager.Instance.Random.Next(0, 100);
                // common outcome
                if (roll < 50)
                {
                    player.Sanity -= 10;
                    return new SimulationLog(new List<Player> { player }, "%player% is shaken.");
                }

                // rare outcome 1
                if (roll < 75)
                {
                    string happening2 = "%player% is injured by falling debris.";
                    player.Vitality -= 25;
                    if (player.Vitality <= 0)
                    {
                        player.Alive = false;
                        happening2 += " %pronoun1% die%pronoun5% from the impact.";
                    }

                    return new SimulationLog(new List<Player> { player }, happening2);
                }

                // rare outcome 2
                player.Vitality = 0;
                player.Alive = false;
                return new SimulationLog(new List<Player> { player }, "%player% becomes one with the earth.");
            }),
        };

        public GlobalEvent(string name, Func<Player, SimulationLog?> run)
        {
            Name = name;
            Run = run;
        }

        public string Name { get; }
        public Func<Player, SimulationLog?> Run { get; }
    }
}
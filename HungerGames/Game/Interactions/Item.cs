namespace HungerGames.Game.Interactions
{
    public class Item
    {
        public static readonly List<Item> Items = new List<Item>
        {
            new Item("an Axe", "%player% strikes %victim% with an axe.", 90, 0, 0, -5),
            new Item("a Spear", "%player% throws a spear at %victim%.", 75, 0, 0, -5),
            new Item("a Trident", "%player% throws a trident at %victim%.", 100, 0, 0, -5),
            new Item("a Knife", "%player% stabs %victim% with a knife.", 60, 0, 0, -5),
            new Item("a Bow", "%player% shoots an arrow at %victim%.", 65, 0, 0, -5),
            new Item("Food", "%player% eats food.", 0, 25, 10, 5, false, true),
            new Item("Water", "%player% drinks water.", 0, 20, 10, 10, false, true),
            new Item("Medical Supplies", "%player% uses medical supplies on %victim%.", 0, 10, 25, 15, positive: true),
            new Item("Poison", "%player% poisons %victim%.", 50, 0, -10, -10),
            new Item("Explosives", "%player% blows up %victim%.", 200, -10, 0, -30),
            new Item("the Bible", "%player% reads the bible to %victim%.", 0, 0, 0, 50, positive: true),
            new Item("Painkillers", "%player% takes a painkiller.", 0, 15, 15, -10, false, true),
            new Item("a Beer", "%player% cracks a cold one.", 5, -10, -10, -10, false, true),
            new Item("Matches", "%player% lights a fire.", 0, 5, 0, 10, false, true),
            new Item("an Energy Bar", "%player% eats an energy bar.", 0, 15, 5, 0, false, true),
            new Item("Cigarettes", "%player% smokes.", 0, -10, -5, 15, false, true),
            new Item("an Instrument", "%player% plays some music for %victim%.", 0, 0, 0, 15, positive: true),
            new Item("a Mace", "%player% clubs %victim% with a mace.", 70, 0, 0, -5),
        };

        public Item(string name, string usage, int damage, int stamina, int health, int sanity, bool canShare = true, bool positive = false)
        {
            Name = name;
            Usage = usage;
            Damage = damage;
            Stamina = stamina;
            Health = health;
            Sanity = sanity;
            CanShare = canShare;
            Positive = positive;
        }

        public string Name { get; }
        public string Usage { get; }
        public int Damage { get; }
        public int Stamina { get; }
        public int Health { get; }
        public int Sanity { get; }
        public bool CanShare { get; }
        public bool Positive { get; }

        public void Use(Player player, Player user)
        {
            player.Stamina += Stamina;
            player.Vitality += Health;
            player.Sanity += Sanity;
            if (player.Id != user.Id) user.Helped.Add(player);
        }

        public void UseTarget(Player player, Player user)
        {
            player.Vitality -= Damage;
            if (player.Vitality <= 0)
            {
                player.Alive = false;
                user.Killed.Add(player);
            }
            else if (Damage > 0)
            {
                user.Attacked.Add(player);
            }
        }
    }
}
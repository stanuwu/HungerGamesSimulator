using System.Drawing;
using System.Numerics;

namespace HungerGames.Game.Interactions
{
    public class Player
    {
        public Player(string name, int id, int team, Bitmap image, Personality personality, int pronouns)
        {
            Name = name;
            Id = id;
            Team = team;
            Image = image;
            Personality[] values = Enum.GetValues<Personality>();
            Personality = personality == Personality.Random ? values[GameManager.Instance.Random.Next(1, values.Length)] : personality;
            Pronouns = pronouns;
        }

        public string Name { get; }
        public int Id { get; }
        public int Team { get; }
        public Bitmap Image { get; set; }
        public Vector2 Position { get; set; }
        public Personality Personality { get; }
        public int Pronouns { get; }
        public bool Alive { get; set; } = true;

        public List<Player> Attacked = new List<Player>();
        public List<Player> Killed = new List<Player>();
        public List<Player> Helped = new List<Player>();
        public List<Item> Items = new List<Item>();

        public int Vitality { get; set; } = 100;
        public int Stamina { get; set; } = 100;
        public int Sanity { get; set; } = 100;

        public int Patience
        {
            get
            {
                int baseValue = Personality switch
                {
                    Personality.Average => 50,
                    Personality.Rational => 100,
                    Personality.Aggressive => 25,
                    Personality.Dumb => 50,
                    Personality.Reckless => 25,
                    Personality.Pacifist => 75,
                    Personality.Compassionate => 75,
                    Personality.Deranged => 0,
                    Personality.Crazy => GameManager.Instance.Random.Next(0, 101),
                    _ => 50
                };
                float modifier = (Vitality / 200f) + (Sanity / 200f);
                return (int)(baseValue * modifier);
            }
        }

        public int Recklessness
        {
            get
            {
                int baseValue = Personality switch
                {
                    Personality.Average => 50,
                    Personality.Rational => 0,
                    Personality.Aggressive => 75,
                    Personality.Dumb => 75,
                    Personality.Reckless => 100,
                    Personality.Pacifist => 50,
                    Personality.Compassionate => 50,
                    Personality.Deranged => 100,
                    Personality.Crazy => GameManager.Instance.Random.Next(0, 101),
                    _ => 50
                };
                float modifier = 0.5f + (1 - (Vitality / 200f)) + (1 - (Sanity / 200f));
                return (int)(baseValue * modifier);
            }
        }

        public int Aggression
        {
            get
            {
                int baseValue = Personality switch
                {
                    Personality.Average => 50,
                    Personality.Rational => 25,
                    Personality.Aggressive => 100,
                    Personality.Dumb => 75,
                    Personality.Reckless => 75,
                    Personality.Pacifist => 0,
                    Personality.Compassionate => 25,
                    Personality.Deranged => 100,
                    Personality.Crazy => GameManager.Instance.Random.Next(0, 101),
                    _ => 50
                };
                float modifier = Vitality / 200f + (1 - (Sanity / 200f));
                return (int)(baseValue * modifier);
            }
        }

        public int Compassion
        {
            get
            {
                int baseValue = Personality switch
                {
                    Personality.Average => 50,
                    Personality.Rational => 35,
                    Personality.Aggressive => 0,
                    Personality.Dumb => 75,
                    Personality.Reckless => 50,
                    Personality.Pacifist => 75,
                    Personality.Compassionate => 100,
                    Personality.Deranged => 0,
                    Personality.Crazy => GameManager.Instance.Random.Next(0, 101),
                    _ => 50
                };
                float modifier = (Vitality / 200f) + (Sanity / 200f);
                return (int)(baseValue * modifier);
            }
        }

        public int Fear
        {
            get
            {
                int baseValue = Personality switch
                {
                    Personality.Average => 50,
                    Personality.Rational => 25,
                    Personality.Aggressive => 25,
                    Personality.Dumb => 25,
                    Personality.Reckless => 0,
                    Personality.Pacifist => 75,
                    Personality.Compassionate => 75,
                    Personality.Deranged => 0,
                    Personality.Crazy => GameManager.Instance.Random.Next(0, 101),
                    _ => 50
                };
                float modifier = 0.5f + (1 - (Vitality / 200f)) + (1 - (Sanity / 200f));
                return (int)(baseValue * modifier);
            }
        }

        public int GetAttitude(Player player)
        {
            if (Personality == Personality.Crazy) return GameManager.Instance.Random.Next(0, 201);

            int baseValue = 50;
            if (player.Team == Team) baseValue += 25;
            baseValue += Compassion / 4;
            baseValue -= Aggression / 4;
            baseValue -= Fear / 8;
            foreach (var player1 in player.Killed)
            {
                if (player1.Team == Team) baseValue -= 20;
            }

            foreach (var player1 in player.Attacked)
            {
                if (player1.Id == Id) baseValue -= 50;
                else if (player1.Team == Team) baseValue -= 10;
            }

            foreach (var player1 in player.Helped)
            {
                if (player1.Id == Id) baseValue += 20;
            }

            baseValue *= (int)(0.5f + (Sanity / 100f));
            return baseValue;
        }

        public string GetAttitudeText(Player player)
        {
            int attitude = GetAttitude(player);
            return attitude switch
            {
                >= 200 => $"%player% is in love with {player.Name}.",
                >= 100 => $"%player% likes {player.Name}.",
                >= 50 => $"%player% is wary of {player.Name}.",
                _ => $"%player% hates {player.Name}."
            };
        }
    }
}
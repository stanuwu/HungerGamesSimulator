using System.Drawing;

namespace HungerGames.Game.Interactions
{
    public class SimulationLog : ILogItem
    {
        public SimulationLog(List<Player> participants, string events, Player? victim = null)
        {
            Participants = participants.Select(p =>
            {
                if (p.Vitality > 0)
                {
                    return new Player(p.Name, p.Id, p.Team, new Bitmap(p.Image), p.Personality, p.Pronouns);
                }

                return p;
            }).ToList();
            Events = events;
            Victim = victim;
        }

        public List<Player> Participants { get; }
        public string Events { get; }
        private Player? Victim { get; }

        // bit silly code - clean this up later
        public string GetString()
        {
            Player player = Participants[0];
            string pronoun1 = player.Pronouns switch
            {
                0 => "he",
                1 => "she",
                2 => "they",
                3 => "it",
            };
            string pronoun2 = player.Pronouns switch
            {
                0 => "him",
                1 => "her",
                2 => "them",
                3 => "it",
            };
            string pronoun3 = player.Pronouns switch
            {
                0 => "is",
                1 => "is",
                2 => "are",
                3 => "is",
            };
            string pronoun4 = player.Pronouns switch
            {
                0 => "himself",
                1 => "herself",
                2 => "themselves",
                3 => "itself",
            };
            string pronoun5 = player.Pronouns switch
            {
                0 => "s",
                1 => "s",
                2 => "",
                3 => "s",
            };
            string pronoun6 = player.Pronouns switch
            {
                0 => "his",
                1 => "her",
                2 => "their",
                3 => "its",
            };
            string victim = "";
            string victimPronoun1 = "";
            string victimPronoun2 = "";
            string victimPronoun3 = "";
            string victimPronoun4 = "";
            string victimPronoun5 = "";
            string victimPronoun6 = "";
            if (Victim != null)
            {
                if (Victim.Id == player.Id)
                {
                    victim = pronoun4;
                    victimPronoun1 = pronoun1;
                    victimPronoun2 = pronoun2;
                    victimPronoun3 = pronoun3;
                    victimPronoun4 = pronoun4;
                    victimPronoun5 = pronoun5;
                    victimPronoun6 = pronoun6;
                }
                else
                {
                    victim = Victim.Name;
                    victimPronoun1 = Victim.Pronouns switch
                    {
                        0 => "he",
                        1 => "she",
                        2 => "they",
                        3 => "it",
                    };
                    victimPronoun2 = Victim.Pronouns switch
                    {
                        0 => "him",
                        1 => "her",
                        2 => "them",
                        3 => "its",
                    };
                    victimPronoun3 = Victim.Pronouns switch
                    {
                        0 => "is",
                        1 => "is",
                        2 => "are",
                        3 => "is",
                    };
                    victimPronoun4 = Victim.Pronouns switch
                    {
                        0 => "himself",
                        1 => "herself",
                        2 => "themselves",
                        3 => "itself",
                    };
                    victimPronoun5 = player.Pronouns switch
                    {
                        0 => "s",
                        1 => "s",
                        2 => "",
                        3 => "s",
                    };
                    victimPronoun6 = player.Pronouns switch
                    {
                        0 => "his",
                        1 => "her",
                        2 => "their",
                        3 => "its",
                    };
                }
            }

            string temp = Events.Replace("%player%", player.Name);
            temp = temp.Replace("%victim%", victim);
            temp = temp.Replace("%pronoun1%", pronoun1);
            temp = temp.Replace("%pronoun2%", pronoun2);
            temp = temp.Replace("%pronoun3%", pronoun3);
            temp = temp.Replace("%pronoun4%", pronoun4);
            temp = temp.Replace("%pronoun5%", pronoun5);
            temp = temp.Replace("%pronoun6%", pronoun6);
            temp = temp.Replace("%victimPronoun1%", victimPronoun1);
            temp = temp.Replace("%victimPronoun2%", victimPronoun2);
            temp = temp.Replace("%victimPronoun3%", victimPronoun3);
            temp = temp.Replace("%victimPronoun4%", victimPronoun4);
            temp = temp.Replace("%victimPronoun5%", victimPronoun5);
            temp = temp.Replace("%victimPronoun6%", victimPronoun6);
            return temp;
        }
    }
}
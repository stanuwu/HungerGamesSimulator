using System.Drawing;
using System.Drawing.Drawing2D;
using System.Numerics;
using System.Windows.Documents;

namespace HungerGames.Game.Interactions
{
    public class Simulation
    {
        public Simulation(int size, int teamSize, Player[] players)
        {
            Size = size;
            TeamSize = teamSize;
            Players = players;
        }

        public int Size { get; }
        public int TeamSize { get; }
        public Player[] Players { get; }


        public int Round { get; set; } = 0;
        public int CenterRadius { get; } = 200;
        public int BorderRadius { get; set; } = 1000;

        public List<Player> GetNearby(Player player, float distance, float min = 0)
        {
            List<Player> list = new List<Player>();
            foreach (var player1 in Players)
            {
                if (player1.Id == player.Id) continue;
                float dist = Vector2.Distance(player1.Position, player.Position);
                if (dist <= distance && dist > min && player1.Alive) list.Add(player1);
            }

            return list;
        }

        public Zone GetZone(Player player)
        {
            if (Vector2.Distance(Vector2.Zero, player.Position) >= BorderRadius) return Zone.Border;
            if (Vector2.Distance(Vector2.Zero, player.Position) < CenterRadius) return Zone.Middle;
            if (player.Position.X > 0 && player.Position.X > Math.Abs(player.Position.Y)) return Zone.Swamp;
            if (player.Position.X < 0 && -player.Position.X > Math.Abs(player.Position.Y)) return Zone.Desert;
            if (player.Position.Y > 0) return Zone.Forest;
            return Zone.Mountains;
        }

        public void UpdateMap()
        {
            Bitmap map = new Bitmap("./assets/map.png");
            Graphics graphics = Graphics.FromImage(map);
            graphics.SmoothingMode = SmoothingMode.AntiAlias;
            graphics.DrawEllipse(new Pen(Color.Aqua, 4), 1000 - BorderRadius, 1000 - BorderRadius, BorderRadius * 2, BorderRadius * 2);
            Font nameFont = new Font(new FontFamily("Calibri Light"), 32, FontStyle.Regular, GraphicsUnit.Pixel);
            foreach (var player in Players)
            {
                if (!player.Alive) continue;
                Vector2 pos = player.Position with { Y = -player.Position.Y } + new Vector2(1000 - 25);
                string displayName = $"[{player.Team + 1}] {player.Name}";
                SizeF size = graphics.MeasureString(displayName, nameFont);
                graphics.DrawString(displayName, nameFont, Brushes.Red, pos.X - size.Width / 2 + 25, pos.Y - 12 - size.Height / 2);
                graphics.DrawImage(new Bitmap(player.Image, new Size(50, 50)), new PointF(pos));
            }

            GameManager.Instance.MainWindow?.UpdateMap(map);
        }

        public List<ILogItem> Simulate()
        {
            List<ILogItem> happenings = new List<ILogItem>();

            Round++;
            Random random = GameManager.Instance.Random;

            Dictionary<int, string> actions = new Dictionary<int, string>();
            Dictionary<int, bool> logged = new Dictionary<int, bool>();

            // Get Alive Players and create Random Order
            List<Player> roundStartPlayers = Players.Where(p => p.Alive).ToList();
            Player[] playerOrder = Players.Where(p => p.Alive).ToArray();
            GameManager.Instance.Random.Shuffle(playerOrder);

            // Recover Stats
            foreach (var player in playerOrder)
            {
                player.Stamina += 5;
                player.Sanity -= 3;
            }

            // move players
            foreach (var player in playerOrder)
            {
                // decide if player can move
                if (player.Stamina < 10)
                {
                    actions[player.Id] = "%player% is too tired to move.";
                    continue;
                }

                // decide if nearby players are targets
                List<Player> nearby = random.Next(0, 2) == 0 ? GetNearby(player, 300, 75) : new List<Player>();
                int speed = random.Next(50, 225);
                bool love = false;
                int loveNum = 100;
                Player? lovePlayer = null;
                bool hate = false;
                int hateNum = 50;
                Player? hatePlayer = null;
                foreach (var player1 in nearby)
                {
                    int baseAtt = player.GetAttitude(player1);
                    int loveCoeff = baseAtt
                                    - 10 * player.Aggression
                                    - 10 * player.Fear
                                    + 10 * player.Recklessness
                                    + 10 * player.Compassion
                                    - 10 * player.Patience;
                    int hateCoeff = baseAtt
                                    - 10 * player.Aggression
                                    - 10 * player.Recklessness
                                    + 10 * player.Compassion
                                    - 10 * player.Patience;

                    if (loveCoeff > loveNum)
                    {
                        love = true;
                        loveNum = loveCoeff;
                        lovePlayer = player1;
                    }

                    if (hateCoeff < hateNum)
                    {
                        hate = true;
                        hateNum = hateCoeff;
                        hatePlayer = player1;
                    }
                }

                // decide if hate or love is more important
                if (love && hate)
                {
                    if (loveNum - 50 > Math.Abs(hateNum + 50)) hate = false;
                    else love = false;
                }

                // decide if running or attacking
                bool run = true;
                if (hate)
                {
                    if (hateNum < 25 && player.Fear < 25) run = false;
                    if (player.Fear < 55) run = false;
                }

                if (!love && !hate)
                {
                    // do nothing
                    if (random.Next(0, 2) == 0) continue;

                    Zone zone = GetZone(player);
                    Vector2 direction = Vector2.Normalize(new Vector2(random.NextSingle(), random.NextSingle()));
                    if (float.IsNaN(direction.X) || float.IsNaN(direction.Y)) continue;

                    player.Position += direction * speed;
                    Zone newZone = GetZone(player);
                    if (newZone == Zone.Border && player.Recklessness < 100 && player.Sanity > 50)
                    {
                        player.Position -= direction * speed;
                        newZone = zone;
                    }

                    if (newZone == Zone.Border)
                    {
                        actions[player.Id] = "%player% tries to escape the border.";
                        player.Alive = false;
                        player.Vitality = 0;
                        continue;
                    }

                    if (zone != newZone)
                    {
                        actions[player.Id] = $"%player% moves into the {newZone.ToString().ToLower()}.";
                    }
                    else
                    {
                        actions[player.Id] = $"%player% moves around aimlessly.";
                    }
                }

                if (love)
                {
                    Vector2 direction = Vector2.Normalize(player.Position - lovePlayer!.Position);
                    if (float.IsNaN(direction.X) || float.IsNaN(direction.Y)) continue;
                    player.Position += direction * speed;
                    actions[player.Id] = $"%player% tries to catch up to {lovePlayer.Name}.";
                }

                if (hate)
                {
                    if (run)
                    {
                        Vector2 direction = -Vector2.Normalize(player.Position - hatePlayer!.Position);
                        if (float.IsNaN(direction.X) || float.IsNaN(direction.Y)) continue;
                        player.Position += direction * speed;
                        actions[player.Id] = $"%player% runs away from {hatePlayer.Name}.";
                    }
                    else
                    {
                        Vector2 direction = Vector2.Normalize(player.Position - hatePlayer!.Position);
                        if (float.IsNaN(direction.X) || float.IsNaN(direction.Y)) continue;
                        player.Position += direction * speed;
                        actions[player.Id] = $"%player% chases after {hatePlayer.Name}.";
                    }
                }

                player.Stamina -= 10;
            }

            playerOrder = playerOrder.Where(p => p.Alive).ToArray();

            // do global events
            ILogItem? addLast = null;
            if (Round % 5 == 0)
            {
                GlobalEvent evt = GlobalEvent.Events[random.Next(0, GlobalEvent.Events.Count)];
                addLast = new EventLog(evt.Name);
                bool shrink = false;
                foreach (var player in playerOrder)
                {
                    SimulationLog? result = evt.Run(player);
                    if (result == null)
                    {
                        shrink = true;
                        break;
                    }

                    happenings.Add(result);
                    logged[player.Id] = true;
                }

                if (shrink)
                {
                    BorderRadius -= 200;
                    if (BorderRadius < 200) BorderRadius = 200;
                    foreach (var player in roundStartPlayers)
                    {
                        if (GetZone(player) == Zone.Border)
                        {
                            player.Vitality = 0;
                            player.Alive = false;
                            happenings.Add(new SimulationLog(new List<Player> { player }, "%player% is caught by the border and dies."));
                            logged[player.Id] = true;
                        }
                    }
                }
            }

            playerOrder = playerOrder.Where(p => p.Alive).ToArray();

            // do local events
            if (Round == 1)
            {
                foreach (var player in playerOrder)
                {
                    if (logged.ContainsKey(player.Id) && logged[player.Id]) continue;
                    int roll = random.Next(0, 6);
                    switch (roll)
                    {
                        case 0:
                            Item item = Item.Items[GameManager.Instance.Random.Next(0, Item.Items.Count)];
                            player.Items.Add(item);
                            happenings.Add(new SimulationLog(new List<Player> { player }, $"%player% picks up {item.Name.ToLower()} from the cornucopia."));
                            logged[player.Id] = true;
                            break;
                        case 1:
                            happenings.Add(new SimulationLog(new List<Player> { player }, "%player% grabs an empty backpack."));
                            logged[player.Id] = true;
                            break;
                        case 2:
                            Player[] available = playerOrder.Where(p => logged.ContainsKey(p.Id) && logged[p.Id]).ToArray();
                            if (available.Length > 0)
                            {
                                Player atk = available[random.Next(0, available.Length)];
                                int atkType = random.Next(0, 3);
                                switch (atkType)
                                {
                                    case 0:
                                        atk.Vitality -= 50;
                                        happenings.Add(new SimulationLog(new List<Player> { player, atk }, "%player% attempts to strangle %victim%.", atk));
                                        break;
                                    case 1:
                                        atk.Vitality -= 25;
                                        happenings.Add(new SimulationLog(new List<Player> { player, atk }, "%player% punches %victim% in the face.", atk));
                                        break;
                                    case 2:
                                        atk.Vitality -= 10;
                                        happenings.Add(new SimulationLog(new List<Player> { player, atk }, "%player% pushes %victim% to the ground.", atk));
                                        break;
                                }

                                if (atk.Vitality < 0) atk.Alive = false;
                                logged[player.Id] = true;
                                logged[atk.Id] = true;
                            }

                            break;
                        case 3:
                        case 4:
                        case 5:
                            bool negative = random.Next(0, 2) == 0;
                            bool negative2 = random.Next(0, 2) == 0;
                            player.Position = new Vector2(random.Next(100, 250) * (negative ? -1 : 1), random.Next(100, 250) * (negative2 ? -1 : 1));
                            happenings.Add(new SimulationLog(new List<Player> { player }, "%player% runs as fast as %pronoun1% can."));
                            logged[player.Id] = true;
                            break;
                    }
                }
            }
            else
            {
                foreach (var player in playerOrder)
                {
                    if (logged.ContainsKey(player.Id) && logged[player.Id]) continue;
                    int eventType = random.Next(0, 7);

                    switch (eventType)
                    {
                        // use item
                        case 0:
                        case 1:
                            List<Item> available = player.Items.Where(item =>
                            {
                                if (player.Personality == Personality.Crazy) return true;
                                if (item.Positive) return true;
                                if (!item.Positive && player.Sanity < 15) return true;
                                return false;
                            }).ToList();
                            if (available.Count > 0)
                            {
                                Item item = available[random.Next(0, available.Count)];
                                player.Items.Remove(item);
                                item.Use(player, player);
                                item.UseTarget(player, player);
                                happenings.Add(new SimulationLog(new List<Player> { player }, item.Usage, player));
                                logged[player.Id] = true;
                            }
                            else
                            {
                                // why cant i fall through in c# :(
                                goto next1;
                            }

                            break;
                        // interact
                        case 2:
                            next1:
                            int roll = random.Next(0, 100);
                            // aggro
                            if (roll < player.Aggression)
                            {
                                List<Player> nearby = GetNearby(player, 200).Where(p => player.GetAttitude(p) < 100).ToList();
                                if (nearby.Count < 1)
                                {
                                    if (random.Next(0, 2) == 0)
                                    {
                                        happenings.Add(new SimulationLog(new List<Player> { player }, "%player% has murder on %pronoun6% mind."));
                                        logged[player.Id] = true;
                                    }
                                    else
                                    {
                                        goto next2;
                                    }
                                }
                                else
                                {
                                    int count = random.Next(1, Math.Min(nearby.Count, 4));
                                    Player[] nearbyRandom = nearby.ToArray();
                                    random.Shuffle(nearbyRandom);
                                    List<Player> targets = new List<Player>();
                                    logged[player.Id] = true;
                                    for (int i = 0; i < count; i++)
                                    {
                                        targets.Add(nearbyRandom[i]);
                                        logged[nearbyRandom[i].Id] = true;
                                    }

                                    List<Item> itemToGive = player.Items.Where(i => !i.Positive && i.CanShare).ToList();
                                    if (itemToGive.Count > 0)
                                    {
                                        foreach (var target in targets)
                                        {
                                            Item item = itemToGive[GameManager.Instance.Random.Next(0, itemToGive.Count)];
                                            player.Items.Remove(item);
                                            item.Use(target, player);
                                            item.UseTarget(target, player);
                                            happenings.Add(new SimulationLog(new List<Player> { player, target }, item.Usage, target));
                                        }
                                    }
                                    else
                                    {
                                        int choice = random.Next(0, 5);

                                        string name = string.Join(", ", targets.Select(t => t.Name).Take(targets.Count - 1));
                                        if (targets.Count > 1) name += $" and {targets.Last().Name}";
                                        List<Player> playerList = new List<Player> { player };
                                        playerList.AddRange(targets);
                                        string s = "";
                                        if (targets.Count == 1)
                                        {
                                            name = targets.First().Name;
                                            s = "s";
                                        }

                                        switch (choice)
                                        {
                                            case 0:
                                                foreach (var player1 in targets)
                                                {
                                                    player1.Vitality -= 50;
                                                    if (player1.Vitality <= 0)
                                                    {
                                                        player1.Alive = false;
                                                        player.Killed.Add(player1);
                                                    }
                                                    else
                                                    {
                                                        player.Attacked.Add(player1);
                                                    }
                                                }

                                                happenings.Add(new SimulationLog(playerList, $"{name} get{s} ambushed by %player%."));
                                                break;
                                            case 1:
                                                foreach (var player1 in targets)
                                                {
                                                    player1.Vitality -= 15;
                                                    if (player1.Vitality <= 0)
                                                    {
                                                        player1.Alive = false;
                                                        player.Killed.Add(player1);
                                                    }
                                                    else
                                                    {
                                                        player.Attacked.Add(player1);
                                                    }
                                                }

                                                happenings.Add(new SimulationLog(playerList, $"{name} get{s} hurt running away from %player%."));
                                                break;
                                            case 2:
                                                foreach (var target in targets)
                                                {
                                                    player.Attacked.Add(target);
                                                }

                                                happenings.Add(new SimulationLog(playerList, $"{name} get{s} attacked by %player% but manage to escape."));
                                                break;
                                            case 3:
                                                foreach (var player1 in playerList)
                                                {
                                                    player1.Vitality -= 50;
                                                    if (player1.Vitality <= 0)
                                                    {
                                                        player1.Alive = false;
                                                        player.Killed.Add(player1);
                                                    }
                                                    else
                                                    {
                                                        player.Attacked.Add(player1);
                                                    }
                                                }

                                                happenings.Add(new SimulationLog(playerList, $"{name} get{s} into a bloody fight with %player%."));
                                                break;
                                            case 4:
                                                player.Vitality = 0;
                                                player.Alive = false;
                                                happenings.Add(new SimulationLog(playerList,
                                                    $"{name} get{s} attacked by %player% but kill{s} %pronoun2% instead."));
                                                break;
                                        }
                                    }
                                }
                            }
                            // nice
                            else
                            {
                                List<Player> nearby = GetNearby(player, 200).Where(p => player.GetAttitude(p) > 75).ToList();
                                if (nearby.Count < 1)
                                {
                                    if (random.Next(0, 2) == 0)
                                    {
                                        happenings.Add(new SimulationLog(new List<Player> { player }, "%player% is lonely."));
                                        logged[player.Id] = true;
                                    }
                                    else
                                    {
                                        goto next2;
                                    }
                                }
                                else
                                {
                                    int count = random.Next(1, Math.Min(nearby.Count, 4));
                                    Player[] nearbyRandom = nearby.ToArray();
                                    random.Shuffle(nearbyRandom);
                                    List<Player> targets = new List<Player>();
                                    for (int i = 0; i < count; i++)
                                    {
                                        targets.Add(nearbyRandom[i]);
                                        logged[nearbyRandom[i].Id] = true;
                                    }

                                    List<Item> itemToGive = player.Items.Where(i => i.Positive && i.CanShare).ToList();
                                    if (itemToGive.Count > 0)
                                    {
                                        foreach (var target in targets)
                                        {
                                            Item item = itemToGive[GameManager.Instance.Random.Next(0, itemToGive.Count)];
                                            player.Items.Remove(item);
                                            item.Use(target, player);
                                            item.UseTarget(target, player);
                                            happenings.Add(new SimulationLog(new List<Player> { player, target }, item.Usage, target));
                                        }
                                    }
                                    else
                                    {
                                        int choice = random.Next(0, 5);

                                        string name = string.Join(", ", targets.Select(t => t.Name));
                                        name += $" and {player.Name}";
                                        List<Player> playerList = new List<Player> { player };
                                        playerList.AddRange(targets);
                                        switch (choice)
                                        {
                                            case 0:
                                                foreach (var player1 in playerList)
                                                {
                                                    player1.Sanity += 15;
                                                    player.Helped.Add(player1);
                                                }

                                                happenings.Add(new SimulationLog(playerList, $"{name} sleep in shifts."));
                                                break;
                                            case 1:
                                                foreach (var player1 in playerList)
                                                {
                                                    player1.Sanity += 5;
                                                    player.Helped.Add(player1);
                                                }

                                                happenings.Add(new SimulationLog(playerList, $"{name} talk about the other tributes."));
                                                break;
                                            case 2:
                                                happenings.Add(new SimulationLog(playerList, $"{name} can't seem to get along."));
                                                break;
                                            case 3:
                                                foreach (var player1 in playerList)
                                                {
                                                    player1.Stamina += 15;
                                                    player.Helped.Add(player1);
                                                }

                                                happenings.Add(new SimulationLog(playerList, $"{name} take turns resting."));
                                                break;
                                            case 4:
                                                happenings.Add(new SimulationLog(playerList, $"{name} hunt for other tributes."));
                                                break;
                                        }
                                    }
                                }
                            }

                            break;
                        // random event
                        case 3:
                        case 4:
                            next2:
                            LocalEvent localEvent = LocalEvent.Events[random.Next(0, LocalEvent.Events.Count)];
                            SimulationLog? res = localEvent.Run(player);
                            if (res != null)
                            {
                                if (player.Vitality <= 0) player.Alive = false;
                                happenings.Add(res);
                                logged[player.Id] = true;
                            }

                            break;
                        // zone event
                        case 5:
                            switch (GetZone(player))
                            {
                                case Zone.Middle:
                                    Item item = Item.Items[GameManager.Instance.Random.Next(0, Item.Items.Count)];
                                    player.Items.Add(item);
                                    happenings.Add(new SimulationLog(new List<Player> { player },
                                        $"%player% finds {item.Name.ToLower()} at the cornucopia."));
                                    logged[player.Id] = true;
                                    break;
                                case Zone.Forest:
                                    string start = "%player% gets attacked by a bear.";
                                    player.Vitality -= 25;
                                    if (player.Vitality <= 0)
                                    {
                                        player.Alive = false;
                                        start += " %pronoun1% bleed out from the wounds.";
                                    }

                                    happenings.Add(new SimulationLog(new List<Player> { player }, start));
                                    logged[player.Id] = true;
                                    break;
                                case Zone.Mountains:
                                    player.Vitality = 0;
                                    player.Alive = false;
                                    happenings.Add(new SimulationLog(new List<Player> { player }, "%player% falls off a cliff and dies."));
                                    logged[player.Id] = true;
                                    break;
                                case Zone.Desert:
                                    player.Sanity -= 20;
                                    happenings.Add(new SimulationLog(new List<Player> { player }, "%player% walk under the scorching sun."));
                                    logged[player.Id] = true;
                                    break;
                                case Zone.Swamp:
                                    player.Stamina -= 20;
                                    if (player.Stamina <= 0) player.Stamina = 0;
                                    happenings.Add(new SimulationLog(new List<Player> { player }, "%player% gets stuck in the mud."));
                                    logged[player.Id] = true;
                                    break;
                            }

                            break;
                        // no event
                        case 6:

                            break;
                    }
                }
            }

            playerOrder = playerOrder.Where(p => p.Alive).ToArray();

            // draw map
            UpdateMap();

            // fill empty log
            foreach (var player in playerOrder)
            {
                Player target = roundStartPlayers[random.Next(0, roundStartPlayers.Count)];
                if (!actions.ContainsKey(player.Id)) actions[player.Id] = player.GetAttitudeText(target);
            }

            // build log
            foreach (var player in roundStartPlayers)
            {
                if (!logged.ContainsKey(player.Id) || !logged[player.Id])
                {
                    if (actions.ContainsKey(player.Id)) happenings.Add(new SimulationLog(new List<Player> { player }, actions[player.Id]));
                    else happenings.Add(new SimulationLog(new List<Player> { player }, "Something went wrong here!"));
                }
            }

            // handle death image
            foreach (var roundStartPlayer in roundStartPlayers)
            {
                if (!roundStartPlayer.Alive)
                {
                    Bitmap image = new Bitmap(roundStartPlayer.Image, 200, 200);
                    for (int x = 0; x < image.Width; x++)
                    for (int y = 0; y < image.Height; y++)
                    {
                        Color pixelColor = image.GetPixel(x, y);
                        Color newColor = Color.FromArgb(pixelColor.R, 0, 0);
                        image.SetPixel(x, y, newColor);
                    }

                    roundStartPlayer.Image = image;
                }
            }

            // shuffle order (disabled)
            ILogItem[] happeningsArray = happenings.ToArray();
            // random.Shuffle(happeningsArray);
            happenings = addLast != null ? new List<ILogItem>() { addLast } : new List<ILogItem>();
            happenings.AddRange(happeningsArray);
            return happenings;
        }
    }
}
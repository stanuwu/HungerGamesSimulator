using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Net.Http;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using HungerGames.Game;
using HungerGames.Game.Interactions;
using HungerGames.Game.State;
using HungerGames.Util;
using Microsoft.Win32;
using Brushes = System.Windows.Media.Brushes;
using EventLog = HungerGames.Game.Interactions.EventLog;
using Image = System.Windows.Controls.Image;
using Page = HungerGames.Game.State.Page;
using Size = System.Drawing.Size;

namespace HungerGames;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    public MainWindow()
    {
        GameManager.Instance.MainWindow = this;
        DefaultImage = new Bitmap("./assets/default.png");
        Names = new string[96];
        for (var i = 0; i < Names.Length; i++)
        {
            Names[i] = "John Doe";
        }

        Images = new Bitmap[96];
        for (var i = 0; i < Images.Length; i++)
        {
            Images[i] = new Bitmap(DefaultImage, new Size(1000, 1000));
        }

        Pronouns = new int[96];
        for (var i = 0; i < Pronouns.Length; i++)
        {
            Pronouns[i] = 0;
        }

        Personalities = new int[96];
        foreach (var personality in Personalities)
        {
            Personalities[personality] = 0;
        }

        InitializeComponent();
        _active = true;
        CreatePlayerSelect(12, 1);
    }

    private readonly bool _active;
    private int _players;
    private int _teamSize;
    private Bitmap DefaultImage { get; }
    private string[] Names { get; }
    private Bitmap[] Images { get; }
    private int[] Pronouns { get; }
    private int[] Personalities { get; }

    private void CreditsClicked(object sender, MouseButtonEventArgs e)
    {
        Process.Start(new ProcessStartInfo(Persistent.Style.AuthorUrl) { UseShellExecute = true });
    }

    private void HideAllPages()
    {
        StartGrid.Visibility = Visibility.Hidden;
        ConfigGrid.Visibility = Visibility.Hidden;
        MatchGrid.Visibility = Visibility.Hidden;
        SummaryGrid.Visibility = Visibility.Hidden;
        EndGrid.Visibility = Visibility.Hidden;
    }

    public void ChangePage(Page page)
    {
        switch (page)
        {
            case Page.Start:
                HideAllPages();
                StartGrid.Visibility = Visibility.Visible;
                break;
            case Page.Config:
                HideAllPages();
                ConfigGrid.Visibility = Visibility.Visible;
                break;
            case Page.Match:
                HideAllPages();
                MatchGrid.Visibility = Visibility.Visible;
                break;
            case Page.Summary:
                HideAllPages();
                SummaryGrid.Visibility = Visibility.Visible;
                break;
            case Page.End:
                HideAllPages();
                EndGrid.Visibility = Visibility.Visible;
                break;
        }
    }

    private void StartButton_OnClick(object sender, RoutedEventArgs e)
    {
        GameManager.Instance.CurrentPage = Page.Config;
    }

    private void CreatePlayerSelect(int count, int teamSize)
    {
        _players = count;
        _teamSize = teamSize;
        PlayerSelectGrid.Children.Clear();
        int cursorX = 20;
        int cursorY = 20;
        for (int i = 1; i < count + 1; i++)
        {
            var i1 = i;
            Grid player = new Grid()
            {
                Name = $"Player{i}",
                Margin = new Thickness()
                {
                    Left = 0,
                    Top = cursorY,
                    Right = cursorX,
                    Bottom = 0,
                },
                HorizontalAlignment = HorizontalAlignment.Right,
                VerticalAlignment = VerticalAlignment.Top,
                Width = 150,
                Height = 305,
            };
            int ts2 = teamSize > 4 ? i - teamSize / 2 : i;
            Label district = new Label()
            {
                FontSize = 18,
                Foreground = Brushes.White,
                VerticalAlignment = VerticalAlignment.Top,
                Content = $"District {(i / teamSize + (teamSize > 4 ? 1 : 0))}",
            };
            if (ts2 % teamSize == 0) player.Children.Add(district);
            TextBox name = new TextBox()
            {
                Name = $"Player{i}Name",
                Height = 24,
                FontSize = 18,
                VerticalAlignment = VerticalAlignment.Top,
                Margin = new Thickness()
                {
                    Left = 0,
                    Top = 30,
                    Right = 0,
                    Bottom = 0,
                },
                Text = Names[i - 1],
            };
            name.TextChanged += (sender, args) => { Names[i1 - 1] = name.Text; };
            Image image = new Image()
            {
                Name = $"Player{i}Image",
                Source = Images[i - 1].ToBitmapSource(),
                VerticalAlignment = VerticalAlignment.Top,
                Margin = new Thickness()
                {
                    Left = 0,
                    Top = 65,
                    Right = 0,
                    Bottom = 0,
                },
                Width = 150,
                Height = 150,
                Stretch = Stretch.Fill
            };
            Button button1 = new Button()
            {
                Name = $"Player{i}File",
                VerticalAlignment = VerticalAlignment.Top,
                HorizontalAlignment = HorizontalAlignment.Left,
                Margin = new Thickness()
                {
                    Left = 0,
                    Top = 220,
                    Right = 0,
                    Bottom = 0,
                },
                Width = 70,
                Height = 24,
                Content = "File",
                Foreground = Brushes.White,
                Background = Brushes.Black,
                FontSize = 16
            };
            button1.Click += (sender, args) =>
            {
                // load from file
                OpenFileDialog dialog = new OpenFileDialog();
                dialog.Title = "Select Image";
                dialog.Filter = "Image Files (*.bmp;*.jpg;*.jpeg;*.png)|*.bmp;*.jpg;*.jpeg;*.png";
                bool? res = dialog.ShowDialog();
                if (res.HasValue && res.Value)
                {
                    try
                    {
                        Bitmap bmp = new Bitmap(new Bitmap(dialog.FileName), new Size(1000, 1000));
                        Images[i1 - 1] = bmp;
                        image.Source = bmp.ToBitmapSource();
                    }
                    catch (ArgumentException)
                    {
                        // invalid image format
                        MessageBox.Show("Invalid Image Format", "Image Error", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    }
                }
            };
            Button button2 = new Button()
            {
                Name = $"Player{i}Url",
                VerticalAlignment = VerticalAlignment.Top,
                HorizontalAlignment = HorizontalAlignment.Right,
                Margin = new Thickness()
                {
                    Left = 0,
                    Top = 220,
                    Right = 0,
                    Bottom = 0,
                },
                Width = 70,
                Height = 24,
                Content = "Url",
                Foreground = Brushes.White,
                Background = Brushes.Black,
                FontSize = 16
            };
            button2.Click += (sender, args) =>
            {
                // load from url

                int x = (int)(Application.Current.MainWindow!.Left + Application.Current.MainWindow.Width / 2);
                int y = (int)(Application.Current.MainWindow!.Top + Application.Current.MainWindow.Height / 2);

                string url = Microsoft.VisualBasic.Interaction.InputBox("Enter URL", "Select Image", "", x, y);
                Stream content;
                using (HttpClient client = new HttpClient())
                {
                    try
                    {
                        HttpResponseMessage res = client.Send(new HttpRequestMessage(HttpMethod.Get, url));
                        if (res.IsSuccessStatusCode) content = res.Content.ReadAsStream();
                        else return;
                    }
                    catch (InvalidOperationException)
                    {
                        // invalid url
                        MessageBox.Show("Invalid Url", "Image Error", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                        return;
                    }
                    catch (NotSupportedException)
                    {
                        // invalid url
                        MessageBox.Show("Invalid Url", "Image Error", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                        return;
                    }
                }

                try
                {
                    Bitmap bmp = new Bitmap(new Bitmap(content), new Size(1000, 1000));
                    Images[i1 - 1] = bmp;
                    image.Source = bmp.ToBitmapSource();
                }
                catch (ArgumentException)
                {
                    // invalid image format
                    MessageBox.Show("Invalid Image Format", "Image Error", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                }
            };
            ComboBox gender = new ComboBox()
            {
                Name = $"Player{i}Gender",
                VerticalAlignment = VerticalAlignment.Top,
                Margin = new Thickness()
                {
                    Left = 0,
                    Top = 250,
                    Right = 0,
                    Bottom = 0,
                },
                Width = 150,
                Height = 24,
                FontSize = 16,
                SelectedIndex = Pronouns[i - 1]
            };
            gender.SelectionChanged += (sender, args) => { Pronouns[i1 - 1] = gender.SelectedIndex; };
            ComboBoxItem item1 = new ComboBoxItem()
            {
                Name = "PgHe",
                Content = "he/him",
            };
            ComboBoxItem item2 = new ComboBoxItem()
            {
                Name = "PgShe",
                Content = "she/her",
            };
            ComboBoxItem item3 = new ComboBoxItem()
            {
                Name = "PgThey",
                Content = "they/them",
            };
            ComboBox personality = new ComboBox()
            {
                Name = $"Player{i}Gender",
                VerticalAlignment = VerticalAlignment.Top,
                Margin = new Thickness()
                {
                    Left = 0,
                    Top = 280,
                    Right = 0,
                    Bottom = 0,
                },
                Width = 150,
                Height = 24,
                FontSize = 16,
                SelectedIndex = Pronouns[i - 1]
            };
            foreach (var value in Enum.GetValues<Personality>())
            {
                ComboBoxItem item = new ComboBoxItem()
                {
                    Name = $"Ps{value}",
                    Content = value.ToString(),
                };
                personality.Items.Add(item);
            }

            personality.SelectionChanged += (sender, args) => { Personalities[i1 - 1] = personality.SelectedIndex; };
            gender.Items.Add(item1);
            gender.Items.Add(item2);
            gender.Items.Add(item3);
            player.Children.Add(name);
            player.Children.Add(image);
            player.Children.Add(button1);
            player.Children.Add(button2);
            player.Children.Add(gender);
            player.Children.Add(personality);
            PlayerSelectGrid.Children.Add(player);

            if (i % 4 == 0)
            {
                cursorX = 20;
                cursorY += 310;
            }
            else
            {
                cursorX += 180;
            }
        }
    }

    private void CreatePlayerSummary()
    {
        if (!_active) return;
        SummaryPlayers.Children.Clear();
        int cursorX = -540;
        int cursorY = 20;
        Player[] players = GameManager.Instance.Simulation!.Players;
        for (int i = 1; i < players.Length + 1; i++)
        {
            Grid player = new Grid()
            {
                Name = $"Summary{i}",
                Margin = new Thickness()
                {
                    Left = cursorX,
                    Top = cursorY,
                    Right = 0,
                    Bottom = 0,
                },
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Top,
                Width = 150,
                Height = 400,
            };
            Label name = new Label()
            {
                Name = $"Summary{i}Name",
                FontSize = 18,
                Foreground = Brushes.White,
                VerticalAlignment = VerticalAlignment.Top,
                HorizontalAlignment = HorizontalAlignment.Center,
                Content = players[i - 1].Name,
            };
            player.Children.Add(name);

            Image image = new Image()
            {
                Name = $"Summary{i}Image",
                Source = players[i - 1].Image.ToBitmapSource(),
                VerticalAlignment = VerticalAlignment.Top,
                HorizontalAlignment = HorizontalAlignment.Center,
                Margin = new Thickness()
                {
                    Left = 0,
                    Top = 32,
                    Right = 0,
                    Bottom = 0,
                },
                Width = 150,
                Height = 150,
                Stretch = Stretch.Fill
            };
            player.Children.Add(image);

            bool isAlive = players[i - 1].Alive;
            if (GameManager.Instance.DisplayStats == DisplayStats.None)
            {
                Label alive = new Label()
                {
                    Name = $"Summary{i}Alive",
                    FontSize = 18,
                    Foreground = isAlive ? Brushes.Chartreuse : Brushes.Red,
                    VerticalAlignment = VerticalAlignment.Top,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    Content = isAlive ? "Alive" : "Dead",
                    Margin = new Thickness()
                    {
                        Left = 0,
                        Top = 180,
                        Right = 0,
                        Bottom = 0,
                    },
                };
                player.Children.Add(alive);
            }

            if (GameManager.Instance.DisplayStats == DisplayStats.Basic || GameManager.Instance.DisplayStats == DisplayStats.All)
            {
                Label personality = new Label()
                {
                    Name = $"Summary{i}Personality",
                    FontSize = 18,
                    Foreground = Brushes.White,
                    VerticalAlignment = VerticalAlignment.Top,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    Content = $"{players[i - 1].Personality}",
                    Margin = new Thickness()
                    {
                        Left = 0,
                        Top = 180,
                        Right = 0,
                        Bottom = 0,
                    },
                };
                player.Children.Add(personality);

                Label vitality = new Label()
                {
                    Name = $"Summary{i}Vitality",
                    FontSize = 18,
                    Foreground = isAlive ? Brushes.Chartreuse : Brushes.Red,
                    VerticalAlignment = VerticalAlignment.Top,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    Content = $"Health: {players[i - 1].Vitality}",
                    Margin = new Thickness()
                    {
                        Left = 0,
                        Top = 200,
                        Right = 0,
                        Bottom = 0,
                    },
                };
                player.Children.Add(vitality);

                Label stamina = new Label()
                {
                    Name = $"Summary{i}Stamina",
                    FontSize = 18,
                    Foreground = Brushes.White,
                    VerticalAlignment = VerticalAlignment.Top,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    Content = $"Stamina: {players[i - 1].Stamina}",
                    Margin = new Thickness()
                    {
                        Left = 0,
                        Top = 220,
                        Right = 0,
                        Bottom = 0,
                    },
                };
                player.Children.Add(stamina);

                Label sanity = new Label()
                {
                    Name = $"Summary{i}Sanity",
                    FontSize = 18,
                    Foreground = Brushes.White,
                    VerticalAlignment = VerticalAlignment.Top,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    Content = $"Sanity: {players[i - 1].Sanity}",
                    Margin = new Thickness()
                    {
                        Left = 0,
                        Top = 240,
                        Right = 0,
                        Bottom = 0,
                    },
                };
                player.Children.Add(sanity);
            }

            if (GameManager.Instance.DisplayStats == DisplayStats.All)
            {
                Label patience = new Label()
                {
                    Name = $"Summary{i}Patience",
                    FontSize = 18,
                    Foreground = Brushes.White,
                    VerticalAlignment = VerticalAlignment.Top,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    Content = $"Patience: {players[i - 1].Patience}",
                    Margin = new Thickness()
                    {
                        Left = 0,
                        Top = 260,
                        Right = 0,
                        Bottom = 0,
                    },
                };
                player.Children.Add(patience);

                Label recklessness = new Label()
                {
                    Name = $"Summary{i}Recklessness",
                    FontSize = 18,
                    Foreground = Brushes.White,
                    VerticalAlignment = VerticalAlignment.Top,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    Content = $"Recklessness: {players[i - 1].Recklessness}",
                    Margin = new Thickness()
                    {
                        Left = 0,
                        Top = 280,
                        Right = 0,
                        Bottom = 0,
                    },
                };
                player.Children.Add(recklessness);

                Label aggression = new Label()
                {
                    Name = $"Summary{i}Aggression",
                    FontSize = 18,
                    Foreground = Brushes.White,
                    VerticalAlignment = VerticalAlignment.Top,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    Content = $"Aggression: {players[i - 1].Aggression}",
                    Margin = new Thickness()
                    {
                        Left = 0,
                        Top = 300,
                        Right = 0,
                        Bottom = 0,
                    },
                };
                player.Children.Add(aggression);

                Label compassion = new Label()
                {
                    Name = $"Summary{i}Compassion",
                    FontSize = 18,
                    Foreground = Brushes.White,
                    VerticalAlignment = VerticalAlignment.Top,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    Content = $"Compassion: {players[i - 1].Compassion}",
                    Margin = new Thickness()
                    {
                        Left = 0,
                        Top = 320,
                        Right = 0,
                        Bottom = 0,
                    },
                };
                player.Children.Add(compassion);

                Label fear = new Label()
                {
                    Name = $"Summary{i}Fear",
                    FontSize = 18,
                    Foreground = Brushes.White,
                    VerticalAlignment = VerticalAlignment.Top,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    Content = $"Fear: {players[i - 1].Fear}",
                    Margin = new Thickness()
                    {
                        Left = 0,
                        Top = 340,
                        Right = 0,
                        Bottom = 0,
                    },
                };
                player.Children.Add(fear);
            }

            SummaryPlayers.Children.Add(player);

            if (i % 4 == 0)
            {
                cursorX = -540;
                switch (GameManager.Instance.DisplayStats)
                {
                    case DisplayStats.All:
                        cursorY += 370;
                        break;
                    case DisplayStats.Basic:
                        cursorY += 270;
                        break;
                    case DisplayStats.None:
                        cursorY += 200;
                        break;
                }
            }
            else
            {
                cursorX += 180 * 2;
            }
        }
    }

    private void CreateLog(List<ILogItem> log)
    {
        if (!_active) return;
        LogGrid.Children.Clear();
        int cursorY = 20;
        RoundCounter.Content = $"Round {GameManager.Instance.Simulation!.Round}";
        for (int i = 0; i < log.Count; i++)
        {
            if (log[i] is SimulationLog item)
            {
                Grid entry = new Grid()
                {
                    Name = $"EventLog{i}",
                    Margin = new Thickness()
                    {
                        Left = 0,
                        Top = cursorY,
                        Right = 20,
                        Bottom = 0,
                    },
                    HorizontalAlignment = HorizontalAlignment.Right,
                    VerticalAlignment = VerticalAlignment.Top,
                    Height = 200,
                };
                int participants = item.Participants.Count;
                int x = 160 * participants - 160;
                foreach (var itemParticipant in item.Participants)
                {
                    Image image = new Image()
                    {
                        Source = itemParticipant.Image.ToBitmapSource(),
                        HorizontalAlignment = HorizontalAlignment.Right,
                        VerticalAlignment = VerticalAlignment.Top,
                        Margin = new Thickness()
                        {
                            Left = 0,
                            Top = 0,
                            Right = x,
                            Bottom = 0,
                        },
                        Width = 150,
                        Height = 150,
                        Stretch = Stretch.Fill
                    };
                    entry.Children.Add(image);
                    x -= 160;
                }

                Label name = new Label()
                {
                    FontSize = 18,
                    Foreground = Brushes.White,
                    HorizontalAlignment = HorizontalAlignment.Right,
                    VerticalAlignment = VerticalAlignment.Top,
                    Content = item.GetString(),
                    Margin = new Thickness()
                    {
                        Left = 0,
                        Top = 160,
                        Right = 0,
                        Bottom = 0,
                    },
                };
                entry.Children.Add(name);

                LogGrid.Children.Add(entry);

                cursorY += 300;
            }
            else if (log[i] is EventLog evt)
            {
                Grid entry = new Grid()
                {
                    Name = $"EventLog{i}",
                    Margin = new Thickness()
                    {
                        Left = 0,
                        Top = cursorY,
                        Right = 20,
                        Bottom = 0,
                    },
                    HorizontalAlignment = HorizontalAlignment.Right,
                    VerticalAlignment = VerticalAlignment.Top,
                    Height = 35,
                };
                Label name = new Label()
                {
                    FontSize = 18,
                    Foreground = Brushes.White,
                    HorizontalAlignment = HorizontalAlignment.Right,
                    VerticalAlignment = VerticalAlignment.Top,
                    Content = evt.Events,
                    Margin = new Thickness()
                    {
                        Left = 0,
                        Top = 0,
                        Right = 0,
                        Bottom = 0,
                    },
                };
                entry.Children.Add(name);
                LogGrid.Children.Add(entry);
                cursorY += 60;
            }
        }
    }

    private void PlayerCountCombo_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        // update player select
        if (!_active) return;
        ComboBoxItem item = (ComboBoxItem)e.AddedItems[0]!;
        switch (item.Name)
        {
            case "Pc6":
                CreatePlayerSelect(6, 1);
                break;
            case "Pc12":
                CreatePlayerSelect(12, 1);
                break;
            case "Pc24":
                CreatePlayerSelect(24, 2);
                break;
            case "Pc48":
                CreatePlayerSelect(48, 4);
                break;
            case "Pc96T4":
                CreatePlayerSelect(96, 4);
                break;
            case "Pc96T8":
                CreatePlayerSelect(96, 8);
                break;
        }
    }

    private void PlayerStatCombo_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (!_active) return;
        ComboBoxItem item = (ComboBoxItem)e.AddedItems[0]!;
        switch (item.Name)
        {
            case "PsBasic":
                GameManager.Instance.DisplayStats = DisplayStats.Basic;
                break;
            case "PsAll":
                GameManager.Instance.DisplayStats = DisplayStats.All;
                break;
            case "PsNone":
                GameManager.Instance.DisplayStats = DisplayStats.None;
                break;
        }
    }

    private void DoneConfigButton_OnClick(object sender, RoutedEventArgs e)
    {
        Player[] players = new Player[_players];
        for (var i = 0; i < players.Length; i++)
        {
            players[i] = new Player(Names[i], i, i / _teamSize, Images[i], (Personality)Personalities[i], Pronouns[i]);
        }

        LogGrid.Children.Clear();
        GameManager.Instance.Simulation = new Simulation(_players, _teamSize, players);
        GameManager.Instance.Simulation.UpdateMap();
        GameManager.Instance.CurrentPage = Page.Match;
    }

    public void UpdateMap(Bitmap map)
    {
        MapImage.Source = map.ToBitmapSource();
    }

    private void RoundNext_OnClick(object sender, RoutedEventArgs e)
    {
        CreatePlayerSummary();
        SummaryScroll.ScrollToTop();
        GameManager.Instance.CurrentPage = Page.Summary;
    }

    private void SummaryNext_OnClick(object sender, RoutedEventArgs e)
    {
        if (GameManager.Instance.Simulation!.Players.Count(p => p.Alive) < 2)
        {
            List<Player> winner = GameManager.Instance.Simulation!.Players.Where(p => p.Alive).ToList();
            if (winner.Count < 1) WinnerLabel.Content = "No Winners :(";
            else WinnerLabel.Content = $"{winner.First().Name} Wins";

            GameManager.Instance.CurrentPage = Page.End;
        }
        else
        {
            CreateLog(GameManager.Instance.Simulation.Simulate());
            LogScroll.ScrollToTop();
            GameManager.Instance.CurrentPage = Page.Match;
        }
    }

    private void RestartButton_OnClick(object sender, RoutedEventArgs e)
    {
        GameManager.Instance.CurrentPage = Page.Config;
    }
}
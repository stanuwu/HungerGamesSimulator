using HungerGames.Game.Interactions;
using HungerGames.Game.State;

namespace HungerGames.Game
{
    public class GameManager
    {
        public static GameManager Instance { get; } = new GameManager();
        public MainWindow? MainWindow { get; set; }
        public Random Random { get; }
        public DisplayStats DisplayStats { get; set; } = DisplayStats.Basic;
        public Simulation? Simulation { get; set; }

        private GameManager()
        {
            Random = new Random(DateTime.Now.TimeOfDay.Nanoseconds);
        }

        private Page _currentPage = Page.Start;

        public Page CurrentPage
        {
            get => _currentPage;
            set
            {
                _currentPage = value;
                MainWindow?.ChangePage(value);
            }
        }
    }
}
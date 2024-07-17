namespace HungerGames.Game.Interactions
{
    public class EventLog : ILogItem
    {
        public EventLog(string events)
        {
            Events = events;
        }

        public string Events { get; }
    }
}
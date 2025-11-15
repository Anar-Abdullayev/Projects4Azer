namespace UniversalDataCatcher.Server.Helpers
{
    public class ConsoleHelper
    {
        private string _botName;
        public ConsoleHelper(string botName)
        {
            _botName = botName;
        }

        public void PrintProgress(int position, int totalItems, int page)
        {
            BotTitle();
            Console.WriteLine($"{position}/{totalItems} - {page} page");
        }
        public void PrintText(string text)
        {
            BotTitle();
            Console.WriteLine(text);
        }
        private void BotTitle()
        {
            Console.Write($"({_botName}) ");
        }
    }
}

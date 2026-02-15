namespace AdventureGame.Core
{
    public class Program
    {
        public static void Main(string[] args)
        {
            bool gameRun = true;
            while (gameRun)
            {
                // Error in MazeGen where if percent spawns are too high, it cant find a coord slot for items and infinitly loops
                Maze maze = new Maze(height: 15);
                Player player = new Player();
                maze.MazeGen(player);

                // Main game loop
                while (player._health > 0 && !maze._win)
                {
                    MazeDisplay(maze);
                    player.Move(maze, player);
                }

                MazeDisplay(maze);

                // Player win condition handling
                Console.ForegroundColor = ConsoleColor.DarkMagenta;
                if (player._health > 0)
                {
                    Console.WriteLine("You continue deeper into the mazes for your hunt for glory...\nPress enter to continue...");
                    Console.ReadLine();
                    Console.ResetColor();
                }

                // New game continue??? | Could add new floors
                Console.ForegroundColor = ConsoleColor.Magenta;
                Console.WriteLine("New game?(Y/N): ");
                string playerResponse = Console.ReadLine().ToString().ToLower();
                Console.ResetColor();

                if (playerResponse.Contains("y"))
                    gameRun = true;
                else
                    gameRun = false;
            }
        }
        public static void MazeDisplay(Maze maze)
        {
            Console.Clear(); // ########################### DEBUG DEBUG ########################################
            Console.BackgroundColor = ConsoleColor.DarkCyan;
            foreach (ISpawnable[] itemList in maze._maze)
            {
                foreach (ISpawnable item in itemList)
                {
                    Console.Write(item._icon);
                }
                Console.Write("\n");
            }
            Console.ResetColor();
        }
    }
}
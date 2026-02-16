namespace AdventureGame.Core
{
    public class Program
    {
        public static void Main(string[] args)
        {
            bool gameRun = true;
            Player player = new Player();
            Random heightGen = new Random();
            int floorNum = 1;

            while (gameRun)
            {
                // Error in MazeGen where if percent spawns are too high, it cant find a coord slot for items and infinitly loops
                Maze maze = new Maze(height: heightGen.Next(10, 15));
                maze.MazeGen(player);

                // Main game loop
                while (player._health > 0 && !maze._win)
                {
                    MazeDisplay(maze, floorNum);
                    player.Move(maze, player);
                }

                MazeDisplay(maze, floorNum);

                // Player win condition handling
                Console.ForegroundColor = ConsoleColor.DarkMagenta;
                if (player._health > 0)
                {
                    Console.WriteLine($"\nYou continue deeper into the mazes for your hunt for glory... +{200+(2*(floorNum - 1))}points\nPress enter to continue...");
                    Console.ReadLine();
                    Console.ResetColor();
                }

                // New game continue | Could add new floors
                Console.ForegroundColor = ConsoleColor.Magenta;
                Console.WriteLine("\nContinue? (Y/N): ");
                string? playerResponse = Console.ReadLine()?.ToString().ToLower();
                Console.ResetColor();

                if (playerResponse.Contains("y"))
                {
                    gameRun = true;
                    floorNum += 1;
                    player._points += 200 + (2 * (floorNum - 1));
                }
                else
                    gameRun = false;
                if (player._health <= 0)
                {
                    player = new Player();
                    floorNum = 1;
                }
            }
        }
        public static void MazeDisplay(Maze maze, int floorNum)
        {
            Console.Clear(); // ########################### DEBUG DEBUG ########################################
            Console.ForegroundColor = ConsoleColor.DarkCyan;
            Console.WriteLine($"Floor {floorNum}\n");
            Console.ResetColor();
            Console.ForegroundColor = ConsoleColor.Gray;
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
namespace AdventureGame.Core
{
    public class Program
    {
        /**
        TODO:
            Scaleing
            Monster._health & Monster._currentDamage should scale per level
            Item._heal if a new item "max health" is added
            Weapons._damage 
            player.PointsCalc
        **/
        public static void Main(string[] args)
        {
            bool gameRun = true;
            Player player = new Player();
            Random heightGen = new Random();

            while (gameRun)
            {
                // Error in MazeGen where if percent spawns are too high, it cant find a coord slot for items and infinitly loops
                Maze maze = new Maze(height: heightGen.Next(10, 15));
                maze.MazeGen(player);

                // Main game loop
                while (player._health > 0 && !maze._win)
                {
                    MazeDisplay(maze, maze._level);
                    player.Move(maze, player);
                }

                MazeDisplay(maze, maze._level);

                // Player win condition handling
                Console.ForegroundColor = ConsoleColor.DarkMagenta;
                if (player._health > 0)
                {
                    Console.WriteLine($"\nYou continue deeper into the mazes for your hunt for glory... +{200+(2*(maze._level - 1))}points\nPress enter to continue...");
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
                    maze._level += 1;
                    player._points += 100;
                }
                else
                    gameRun = false;
                if (player._health <= 0)
                {
                    player = new Player();
                    maze._level = 1;
                }
            }
        }
        public static void MazeDisplay(Maze maze, int level)
        {
            Console.Clear(); // ########################### DEBUG DEBUG ########################################
            Console.ForegroundColor = ConsoleColor.DarkCyan;
            Console.WriteLine($"Floor {level}\n");
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
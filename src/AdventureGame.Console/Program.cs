namespace AdventureGame.Core
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Maze maze = new Maze(height: 10);
            Player player = new Player();
            maze.MazeGen(player);

            while (player._health > 0 && !maze._win)
            {
                maze.MazeDisplay();
                player.Move(maze, player);
            }

            maze.MazeDisplay();

            Console.ForegroundColor = ConsoleColor.DarkMagenta;
            if (player._health > 0)
            {
                Console.WriteLine("You continue deeper into the mazes for your hunt for glory...\nPress enter to continue...");
                Console.ReadLine();
                Console.ResetColor();
            }
            // Maybe add loop for new game?????
        }
    }
}
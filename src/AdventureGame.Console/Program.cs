namespace AdventureGame.Core
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Maze maze = new Maze();
            Player player = maze.MazeGen();

            while (player._health > 0 || maze._win)
            {
                maze.MazeDisplay();
                player.Move(maze._maze);
            }
            
            Console.WriteLine("End of game");
        }
    }
}
using System.Drawing;
using System.Dynamic;
using System.Globalization;
using System.Runtime.InteropServices.Swift;
using System.Security.Cryptography.X509Certificates;

namespace AdventureGame.Core;

public interface ICharacter
{
    void Attack(ICharacter target);
    void Damage(int damage);
}
public interface ISpawnable
{
    public string _icon { get; }
    public (int, int) _coords { get; set; }
}
public abstract class Item : ISpawnable
{
    public string? _name { get; set; }
    public string? _icon { get; set; }
    public (int, int) _coords { get; set; }
}
public class Player : ICharacter, ISpawnable
{
    public string _icon { get; private set; } = " I ";
    public int _health { get; private set; } = 100;
    public int _baseDamage { get; private set; } = 10;
    public int _currentDamage { get; private set; }
    public List<Item> _inventory { get; private set; }
    public (int, int) _coords { get; set; }
    public Player()
    {
        _currentDamage = _baseDamage;
        _inventory = new List<Item>();
    }
    public void Attack(ICharacter target)
    {
        target.Damage(_currentDamage);
    }
    public void Damage(int damage)
    {
        if (_health - damage < 0)
            _health = 0;
        else
            _health -= damage;
    }
    public void Move(Maze maze, Player player)
    {
        Console.WriteLine($"\nHP: {_health}\t\tDamage: {_currentDamage} ({_baseDamage} + {_currentDamage - _baseDamage})");
        // Check next area and mv to spot if empty or Use thing
        string playerInput = Console.ReadKey(true).Key.ToString();
        (int, int) newCoords;

        switch (playerInput)
        {
            case "W":
                newCoords = (_coords.Item1, _coords.Item2 - 1);
                break;
            case "S":
                newCoords = (_coords.Item1, _coords.Item2 + 1);
                break;
            case "A":
                newCoords = (_coords.Item1 - 1, _coords.Item2);
                break;
            case "D":
                newCoords = (_coords.Item1 + 1, _coords.Item2);
                break;
            default:
                newCoords = _coords;
                break;
        }
        // Check next point for spawnables
        ISpawnable item = maze.CheckCoordPosition(newCoords);

        // PLACEHOLDER | Replace with function that deals with different spawnables accordingly
        // Empty
        if (item.GetType() == typeof(Empty))
        {
            maze._maze[_coords.Item2][_coords.Item1] = new Empty(); // Previous character area to empty
            maze._maze[newCoords.Item2][newCoords.Item1] = player; // Set next character to empty
            player._coords = newCoords;
        }

        // Potions & Weapons
        if (item.GetType() == typeof(Weapon) || item.GetType() == typeof(Potion))
        {
            _inventory.Add((Item)item); // Change later to change to weapon or potion appropriatly
            player.Use(item);
            maze._maze[_coords.Item2][_coords.Item1] = new Empty(); // Previous character area to empty
            maze._maze[newCoords.Item2][newCoords.Item1] = player; // Set next character to empty
            player._coords = newCoords;
        }

        // Monster
        if (item.GetType() == typeof(Monster))
        {
            Monster monster = (Monster)item;
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"\nAttacking a Monster... (HP:{monster._health} | D:{monster._currentDamage})\n");
            while (monster._health > 0 && _health > 0)
            {
                player.Attack(monster);
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"You attack for {_currentDamage} damage!\n\tthe monsters health is down to {monster._health}\n");
                if (monster._health <= 0)
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("You have kill the monster!");
                    (int, int) monsterCoords = monster._coords;
                    maze._maze[monsterCoords.Item2][monsterCoords.Item1] = new Empty();
                }
                else
                {
                    monster.Attack(player);
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"The monster attacks you for {monster._currentDamage} damage!\n\t your health is at {player._health}\n");
                }
            }
            if (_health <= 0)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("You died in battle\nThe world continues on without you...");
            }
            Console.ResetColor();
            Console.WriteLine("Press enter to continue...");
            Console.ReadLine();
        }

        // ExitTile
        if (item.GetType() == typeof(ExitTile))
        {
            maze._win = true;
            maze._maze[_coords.Item2][_coords.Item1] = new Empty(); // Previous character area to empty
            maze._maze[newCoords.Item2][newCoords.Item1] = player; // Set next character to empty
            player._coords = newCoords;
        }
    }
    public void Use(ISpawnable item)
    {
        // Remove thing from mazelist 
        if (item.GetType() == typeof(Weapon))
        {
            Weapon weapon = (Weapon)item;
            if (weapon._damageBonus > (_currentDamage - _baseDamage))
                _currentDamage += weapon._damageBonus;
        }
        if (item.GetType() == typeof(Potion))
        {
            Potion potion = (Potion)item;
            _health += potion._healthAmount;
            _health = Math.Min(150, _health);
        }
    }
}
public class Monster : ICharacter, ISpawnable
{
    public string _icon { get; private set; } = " M ";
    public string _name = "Monster";
    public (int, int) _coords { get; set; }
    public int _health { get; private set; }
    public int _currentDamage { get; private set; }
    public Monster(int health)
    {
        _health = health; // PLACEHOLDER | RANDOM 30 - 50 
        _currentDamage = 10; // PLACEHOLDER | RANDOM 10 - 15
    }
    public void Attack(ICharacter target)
    {
        target.Damage(_currentDamage);
    }
    public void Damage(int damage)
    {
        if (_health - damage < 0)
            _health = 0;
        else
            _health -= damage;
    }
}
public class Weapon : Item
{
    public Weapon()
    {
        _icon = " W ";
        _name = "Weapon";
        _damageBonus = 10; // PLACEHOLDER | NEEDS TO BE RANDOM 5 - 15
    }
    public int _damageBonus { get; private set; }
}
public class Potion : Item
{
    public Potion()
    {
        _icon = " P ";
        _name = "Potion";
        _healthAmount = 20; // PLACEHOLDER | NEEDS TO BE RANDOM 20 - 30
    }
    public int _healthAmount { get; private set; }
}
public class Empty : ISpawnable
{
    public string _icon { get; private set; } = " . ";
    public (int, int) _coords { get; set; }
}
public class Wall : ISpawnable
{
    public string _icon { get; private set; } = " ■ ";
    public (int, int) _coords { get; set; }
}
public class ExitTile : ISpawnable
{
    public string _icon { get; private set; } = " E ";
    public (int, int) _coords { get; set; }
    // Method to end game? ##################################################################################
}
public class Maze
{
    private int _height;
    private int _width;
    public bool _win { get; set; } = false;

    // public List<double> _spawnChance { get; private set; } = new List<double>();
    public List<ISpawnable[]> _maze { get; set; }
    public List<(int, int)> _coordStorage { get; private set; }
    double _monsterChance;
    double _itemChance;
    double _wallChance;

    // Should replace double with float but got error
    public Maze(int height = 10, double monsterChance = .05, double itemChance = .09, double wallChance = .15)
    {
        _height = height;
        _width = height;
        _maze = new List<ISpawnable[]>();
        _coordStorage = new List<(int, int)>();
        _monsterChance = monsterChance;
        _itemChance = itemChance;
        _wallChance = wallChance;
    }
    public Player MazeGen()
    {
        // Make Empty Maze
        for (int y = 0; y < _height; y++)
        {
            // Make Solid line
            if (y == 0 || y == _width - 1)
            {
                ISpawnable[] wallLine = new ISpawnable[_width];
                for (int x = 0; x < _width; x++)
                    wallLine[x] = new Wall();
                _maze.Add(wallLine);
            }
            // Make Mid line
            else
            {
                ISpawnable[] midLine = new ISpawnable[_width];
                for (int x = 0; x < _width; x++)
                {
                    if (x == 0 || x == _width - 1)
                        midLine[x] = new Wall();
                    else
                        midLine[x] = new Empty();
                }
                _maze.Add(midLine);
            }
        }

        // Make spawnlist && Generate Player & End -> Walls -> items & monsters
        List<ISpawnable> spawnables = new List<ISpawnable>();
        spawnables.Add(new ExitTile());
        Player player = new Player();
        spawnables.Add(player);

        // PLACEHOLDER for random amount of items and monsters and walls TEMPORARY
        for (int i = 0; i < 12; i++)
            spawnables.Add(new Wall());

        for (int i = 0; i < 5; i++)
        {
            spawnables.Add(new Potion());
            spawnables.Add(new Weapon());
        }

        for (int i = 0; i < 2; i++)
            spawnables.Add(new Monster(health: 30));

        // Check Coord bank and assign spawnables
        foreach (ISpawnable spawnable in spawnables)
        {
            bool wallCheck = false;
            (int, int) newCoord = CoordGen();
            while (InCoordBank(newCoord) || wallCheck)
            {
                // if (spawnable.GetType() == typeof(Wall)) // wall check disabled for now ########### PLACEHOLDER ##############
                //     wallCheck = !CanPlaceWall(newCoord);
                newCoord = CoordGen();
            }
            spawnable._coords = newCoord;
            _maze[newCoord.Item2][newCoord.Item1] = spawnable;
            _coordStorage.Add(newCoord);
        }
        return player;
    }
    public void MazeDisplay()
    {
        Console.Clear(); // ########################### DEBUG DEBUG ########################################
        // Console.ForegroundColor = ConsoleColor.Cyan;
        Console.BackgroundColor = ConsoleColor.DarkCyan;
        foreach (ISpawnable[] itemList in _maze)
        {
            foreach (ISpawnable item in itemList)
            {
                Console.Write(item._icon);
            }
            Console.Write("\n");
        }
        Console.ResetColor();
    }
    public (int, int) CoordGen()
    {
        Random randPick = new Random();
        return (randPick.Next(1, _width - 1), randPick.Next(1, _width - 1));
    }
    public bool InCoordBank((int, int) coord)
    {
        // Returns true when coords in Coord bank, false if not
        if (_coordStorage.Contains(coord))
        {
            return true;
        }
        return false;
    }
    public ISpawnable CheckCoordPosition((int, int) coord) { return _maze[coord.Item2][coord.Item1]; }

    // Honestly Could make an algorithm that would be better than this...
    public bool CanPlaceWall((int, int) coord)
    {
        List<(int, int)> coordAround = new List<(int, int)>() // Creates a 3x3 coord list with the original coord at the center
        {
            (coord.Item1 - 1, coord.Item2 - 1), (coord.Item1, coord.Item2 - 1), (coord.Item1 + 1, coord.Item2 - 1),
            (coord.Item1 - 1, coord.Item2), coord, (coord.Item1 + 1, coord.Item2),
            (coord.Item1 - 1, coord.Item2 + 1), (coord.Item1, coord.Item2 + 1), (coord.Item1 + 1, coord.Item2 + 1)
        };

        // Checks diagonals from middle and returns false if detecting a wall | Prevents cut offs theoretically...
        for (int i = 0; i < coordAround.Count(); i++)
        {
            if (i % 2 != 0) // Checks diagonals
            {
                if (_maze[coordAround[i].Item2][coordAround[i].Item1].GetType() == typeof(Wall))
                {
                    return false;
                }
            }
        }
        return true;
    }
}

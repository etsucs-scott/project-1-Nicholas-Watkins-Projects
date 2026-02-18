using System.Drawing;
using System.Dynamic;
using System.Globalization;
using System.Runtime.InteropServices.Swift;
using System.Security.Cryptography.X509Certificates;

namespace AdventureGame.Core;

public interface ISpawnable
{
    public string _icon { get; }
    public (int, int) _coords { get; set; }
}
public interface ICharacter : ISpawnable
{
    void Attack(ICharacter target);
    void Damage(int damage);
}
public abstract class Item : ISpawnable
{
    public string? _icon { get; set; }
    public string? _name { get; set; }
    public string? _desc { get; set; }
    public (int, int) _coords { get; set; }
}
public class Player : ICharacter
{
    public string _icon { get; private set; } = " @ ";
    public int _health { get; private set; } = 100;
    public int _baseDamage { get; private set; } = 10;
    public int _currentDamage { get; private set; }
    public List<Item> _inventory { get; private set; }
    public (int, int) _coords { get; set; }
    public int _points { get; set; } = 0;
    public Player()
    {
        _currentDamage = _baseDamage;
        _inventory = new List<Item>(); // Technically could remove and just run with current method of weapon damage choice 
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
    // Moves player | Should honestly refactor later and split into more class/functions 
    public void Move(Maze maze, Player player)
    {
        Console.WriteLine($"\nHP: {_health}\t\tDamage: {_currentDamage} ({_baseDamage} + {_currentDamage - _baseDamage})\tPoints: {_points}");
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

            // Added Arrow keys
            case "UpArrow":
                newCoords = (_coords.Item1, _coords.Item2 - 1);
                break;
            case "DownArrow":
                newCoords = (_coords.Item1, _coords.Item2 + 1);
                break;
            case "LeftArrow":
                newCoords = (_coords.Item1 - 1, _coords.Item2);
                break;
            case "RightArrow":
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

        // Wall
        if (item.GetType() == typeof(Wall))
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("A wall is blocking your way...\nPress enter to continue...");
            Console.ReadLine();
            Console.ResetColor();
        }

        // Potions & Weapons
        if (item.GetType() == typeof(Weapon) || item.GetType() == typeof(Potion))
        {
            Item itemCon = (Item)item;
            _inventory.Add(itemCon); // Change later to change to weapon or potion appropriatly
            player.Use(itemCon);
            maze._maze[_coords.Item2][_coords.Item1] = new Empty(); // Previous character area to empty
            maze._maze[newCoords.Item2][newCoords.Item1] = player; // Set next character to empty
            player._coords = newCoords;
        }

        // Monster - When monster is killed add points (_points)
        if (item.GetType() == typeof(Monster))
        {
            Monster monster = (Monster)item;
            int monsterLevel = monster._health / monster._currentDamage;
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"\nAttacking a Monster... (HP:{monster._health} | D:{monster._currentDamage})\n");
            while (monster._health > 0 && _health > 0)
            {
                player.Attack(monster);
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"You attack for {_currentDamage} damage!\n\tthe monsters health is down to {monster._health}\n");
                if (monster._health <= 0)
                {
                    // Award points based on monsters health before and how low the player health and how high the sword damage is
                    int pointsEarned = PointCalc(monsterLevel, maze._level);
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine($"You have kill the monster! +{pointsEarned} points");
                    player._points += pointsEarned;
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
                Console.WriteLine("You died in battle...\nThe world continues on without you...");
                maze._maze[_coords.Item2][_coords.Item1] = new Empty();
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
    // Uses an item | Could be refactored to make logic a bit better
    public void Use(Item item)
    {
        // Remove thing from mazelist 
        if (item.GetType() == typeof(Weapon))
        {
            Weapon weapon = (Weapon)item;
            if (weapon._damageBonus > (_currentDamage - _baseDamage))
                _currentDamage = weapon._damageBonus + _baseDamage;
        }
        if (item.GetType() == typeof(Potion))
        {
            Potion potion = (Potion)item;
            _health += potion._healthAmount;
            _health = Math.Min(150, _health);
        }
        Console.ForegroundColor = ConsoleColor.DarkGray;
        Console.WriteLine("\n" + item._desc + "\nPress enter to continue...");
        Console.ReadLine();
        Console.ResetColor();
    }
    public int PointCalc(int monsterLevel, int level)
    {
        int playerHealthPoints;

        if (_health < 30)
            playerHealthPoints = 100;
        else
            playerHealthPoints = (-2 / 5) * _health + 40; //150HP -> -20 points & 50HP -> +20 points

        int pointsRewarded = (Math.Max(1, monsterLevel) * 10) + (level * 10) + playerHealthPoints;
        return pointsRewarded;
    }
}
public class Monster : ICharacter
{
    public string _icon { get; private set; } = " M ";
    public string _name = "Monster";
    public (int, int) _coords { get; set; }
    public int _health { get; private set; }
    public int _currentDamage { get; private set; }
    public Monster(int health = 30, int damage = 10)
    {
        _health = health;
        _currentDamage = damage;
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
    public Weapon(int damage = 10)
    {
        _icon = " W ";
        _name = "Weapon";
        _damageBonus = damage;
        _desc = $"You have picked up a +{_damageBonus} {_name}";
    }
    public int _damageBonus { get; private set; }
}
public class Potion : Item
{
    public Potion(int _healAmount = 20)
    {
        _icon = " P ";
        _name = "Potion";
        _desc = $"You have picked up a +{_healAmount} {_name}";
        _healthAmount = _healAmount;
    }
    public int _healthAmount { get; private set; }
}
public class Empty : ISpawnable
{
    public string _icon { get; private set; } = "   ";
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
}
public class Maze
{
    public int _height { get; set; }
    public bool _win { get; set; } = false;
    public int _level { get; set; } = 1;

    // public List<double> _spawnChance { get; private set; } = new List<double>();
    public List<ISpawnable[]> _maze { get; set; }
    public List<(int, int)> _coordStorage { get; private set; }
    double _monsterChance;
    double _itemChance;
    double _wallChance;

    // Should replace double with float but got error | Controls spawn chance for later new levels
    public Maze() 
    {
        _maze = new List<ISpawnable[]>();
        _coordStorage = new List<(int, int)>();
    }
    public void MazeGen(Player player, int height, double monsterChance = .45, double itemChance = .35, double wallChance = 1.0) // M: .45, I: .35, W: .75
    {
        // Refresh variables
        _win = false;
        _maze = new List<ISpawnable[]>();
        _coordStorage = new List<(int, int)>();
        _height = height;
        _monsterChance = monsterChance;
        _itemChance = itemChance;
        _wallChance = wallChance;
        // Make Empty Maze
        for (int y = 0; y < _height; y++)
        {
            // Make Solid line
            if (y == 0 || y == _height - 1)
            {
                ISpawnable[] wallLine = new ISpawnable[_height];
                for (int x = 0; x < _height; x++)
                    wallLine[x] = new Wall();
                _maze.Add(wallLine);
            }
            // Make Mid line
            else
            {
                ISpawnable[] midLine = new ISpawnable[_height];
                for (int x = 0; x < _height; x++)
                {
                    if (x == 0 || x == _height - 1)
                        midLine[x] = new Wall();
                    else
                        midLine[x] = new Empty();
                }
                _maze.Add(midLine);
            }
        }

        // Make spawnlist && Generate Player & End -> Walls -> items & monsters
        List<ISpawnable> spawnables = new List<ISpawnable>();
        Random randPick = new Random();

        // PLACEHOLDER, Could init better in some way probably
        int dif = _height / (4 * _height);
        // int dif = 0;
        int mazeArea = _height * _height / 5;

        int monsterAmount = (int)(mazeArea * _monsterChance) + randPick.Next(-dif, dif);
        int potionAmount = (int)(mazeArea * _itemChance) + randPick.Next(-dif, dif);
        int weaponAmount = (int)(mazeArea * _itemChance) + randPick.Next(-dif, dif);
        int wallAmount = (int)(mazeArea * _wallChance) + randPick.Next(-dif, dif);

        spawnables.Add(player);
        ExitTile exitTile = new ExitTile();
        spawnables.Add(exitTile);

        // Generate spawnables (potions & weapons)
        for (int i = 0; i < potionAmount; i++)
            spawnables.Add(new Potion(_healAmount: randPick.Next(20, 30)));

        for (int i = 0; i < weaponAmount; i++)
            spawnables.Add(new Weapon(damage: randPick.Next(5, 20)));

        // Assign Player & Exit tile & items | TODO should be changed to better algorithm below...
        foreach (ISpawnable spawnable in spawnables)
        {
            (int, int) newCoord = CoordGen();
            while (InCoordBank(newCoord))
            {
                newCoord = CoordGen();
            }
            spawnable._coords = newCoord;
            _maze[newCoord.Item2][newCoord.Item1] = spawnable;
            _coordStorage.Add(newCoord);
        }

        // Generate "blank tiles" that take up coord space to prevent "unwinable" maps
        (int, int) coordPCP = player._coords;
        (int, int) coordExit = (exitTile._coords.Item1, exitTile._coords.Item2);
        (int, int) coordDif = (player._coords.Item1 - coordExit.Item1, player._coords.Item2 - coordExit.Item2);

        // Bug where PCP needs 1+ loop to confirm with coordDif == coordExit break | Doubled playercoord values added to coordStorage
        while (coordDif.Item2 != 0 || coordDif.Item1 != 0)
        {
            if (coordPCP == coordExit)
                break;
            if (coordDif.Item1 != 0)
            {
                coordPCP.Item1 -= 1 * Math.Sign(coordDif.Item1);
                coordDif.Item1 -= 1 * Math.Sign(coordDif.Item1);
                _coordStorage.Add(coordPCP);
            }
            if (coordDif.Item2 != 0)
            {
                coordPCP.Item2 -= 1 * Math.Sign(coordDif.Item2);
                coordDif.Item2 -= 1 * Math.Sign(coordDif.Item2);
                _coordStorage.Add(coordPCP);
            }
        }
        spawnables = new List<ISpawnable>();

        // Generate spawnables (walls & monsters) | Could be better, settled for right now
        for (int i = 0; i < wallAmount; i++)
            spawnables.Add(new Wall());

        for (int i = 0; i < monsterAmount; i++)
            spawnables.Add(new Monster(health: randPick.Next(30, 50), damage: randPick.Next(10, 15)));
        
        // Check Coord bank and assign spawnables coordinates in _coordStorage
        foreach (ISpawnable spawnable in spawnables)
        {
            (int, int) newCoord = CoordGen();
            while (InCoordBank(newCoord))
            {
                newCoord = CoordGen();
            }
            spawnable._coords = newCoord;
            _maze[newCoord.Item2][newCoord.Item1] = spawnable;
            _coordStorage.Add(newCoord);
        }
    }
    public (int, int) CoordGen()
    {
        Random randPick = new Random();
        return (randPick.Next(1, _height - 1), randPick.Next(1, _height - 1));
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
}

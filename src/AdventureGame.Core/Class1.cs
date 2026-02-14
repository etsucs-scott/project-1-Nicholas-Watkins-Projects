using System.Drawing;
using System.Globalization;
using System.Runtime.InteropServices.Swift;
using System.Security.Cryptography.X509Certificates;

namespace AdventureGame.Core;

public interface ICharacter
{
    void Attack(ICharacter target);
    bool Damage(int damage);
}
public interface IUseable
{
    // Empty for now because it'll be used as a way to interact with things (monsters, items)
}
public interface ISpawnable
{
    public string _icon { get; }
}
public abstract class Item : IUseable , ISpawnable
{
    public string? _name { get; set; }
    public string? _icon { get; set; }
}
public class Player : ICharacter, ISpawnable
{
    public string _icon { get; private set; }
    public int _health { get; private set; }
    public int _currentDamage { get; private set; }
    public void Attack(ICharacter target)
    {
        target.Damage(_currentDamage);
    }
    public bool Damage(int damage)
    {
        _health -= damage;
        if (_health <= 0)
        {
            return true;
        }
        return false;
    }
    public void Move(int x, int y)
    {
        // Check next area and mv to spot if empty or Use thing
    }
    public void Use(IUseable thing)
    {
        // Remove thing from mazelist 
    }
}
public class Monster : ICharacter
{
    public Monster(int health)
    {
        _health = health; // PLACEHOLDER | RANDOM 30 - 50 
        _currentDamage = 10; // PLACEHOLDER | RANDOM 10 - 15
    }
    public string _icon = " M ";
    public string _name = "Monster";
    public int _health { get; private set; }
    public int _currentDamage { get; private set; }
    public void Attack(ICharacter target)
    {
        target.Damage(_currentDamage);
    }
    public bool Damage(int damage)
    {
        _health -= damage;
        if (_health <= 0)
        {
            return true;
        }
        return false;
    }
}
public class Weapon : Item
{
    public Weapon()
    {
        _icon = " W ";
        _name = "Weapon";
        _damageBonus = 10; // PLACEHOLDER | NEEDS TO BE RANDOM 10 - 25
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
}
public class Wall : ISpawnable
{
    public string _icon { get; private set; } = " ■ ";
}

public class Maze
{
    private int _height;
    private int _width;

    // public List<double> _spawnChance { get; private set; } = new List<double>();
    public List<ISpawnable[]> _maze { get; private set; }
    public List<(int, int)> _coordStorage { get; private set; }

    // Should replace double with float but got error
    public Maze(int height = 10)
    {
        _height = height;
        _width = height;
        _maze = new List<ISpawnable[]>();
        _coordStorage = new List<(int, int)>();
    }
    public void MazeGen()
    {
        // Make Solid line
        ISpawnable[] wallLine = new ISpawnable[_width];
        for (int x = 0; x < _width; x++)
            wallLine[x] = new Wall();

        // Make Mid line
        ISpawnable[] midLine = new ISpawnable[_width];
        for (int x = 0; x < _width; x++)
            if (x == 0 || x == _width - 1)
                midLine[x] = new Wall();
            else
                midLine[x] = new Empty();

        // Make Empty Maze
        for (int y = 0; y < _height; y++)
        {
            if (y == 0 || y == _width - 1)
                _maze.Add(wallLine);
            else
                _maze.Add(midLine);
        }

        // Generate Player & End -> Walls -> items & monsters
        Console.WriteLine($"Check = {(1, 1) == (1, 1)}");
        Console.WriteLine($"Coords = {CoordGen()}");

        
    }
    public void MazeDisplay()
    {
        // Console.Clear(); // ########################### DEBUG DEBUG ########################################
        foreach (ISpawnable[] itemList in _maze)
        {
            foreach (ISpawnable item in itemList)
            {
                Console.Write(item._icon);
            }
            Console.Write("\n");
        }
    }
    public (int, int) CoordGen()
    {
        Random randPick = new Random();
        return (randPick.Next(1, _width - 1), randPick.Next(1, _width - 1));
    }
}

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

public interface Useable
{
    // Empty for now because it'll be used as a way to interact with things (monsters, items)
}

public abstract class Item : Useable
{
    public string _name { get; set; }
    public char _icon { get; set; }
}

public class Player : ICharacter
{
    public char _playerIcon = 'I'; 
    public int[] _coords { get; set; } 
    public int _health { get; set; }
    public int _currentDamage { get; set; }
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
    public void Use(Useable thing)
    {
        // Remove thing from mazelist 
    }
}
public class Monster : ICharacter
{
    public Monster(int[] coords, int health)
    {
        _coords = coords; // PLACEHOLDER | RANDOM 
        _health = health; // PLACEHOLDER | RANDOM 30 - 50 
        _currentDamage = 10; // PLACEHOLDER | RANDOM 10 - 15
    }
    public char _icon = 'M';
    public string _name = "Monster";
    public int[] _coords { get; private set; } 
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
        _icon = 'W';
        _name = "Weapon";
        _damageBonus = 10; // PLACEHOLDER | NEEDS TO BE RANDOM 10 - 25
        _coords = [0, 0]; // PLACEHOLDER | NEEDS TO BE RANDOM 
    }
    public int _damageBonus { get; private set; }
    public int[] _coords { get; private set; }
}

public class Potion : Item
{
    public Potion()
    {
        _icon = 'P';
        _name = "Potion";
        _healthAmount = 20; // PLACEHOLDER | NEEDS TO BE RANDOM 20 - 30
        _coords = [0, 0]; // PLACEHOLDER | NEEDS TO BE RANDOM 
    }
    public int _healthAmount { get; private set; }
    public int[] _coords { get; private set; }
}

public class Maze
{
    private int _height;
    private int _width;
    List<double> _spawnChance = new List<double>();
    List<Char[]> _maze;

    // Should replace double with float but got error
    public Maze(int height = 10, double weaponAmount = .03, double potionAmount = .04, double monsterAmount = .07, double wallAmount = .2)
    {
        _height = height;
        _width = (height * 2) - 1;
        _spawnChance.Add(weaponAmount);
        _spawnChance.Add(potionAmount);
        _spawnChance.Add(monsterAmount);
        _spawnChance.Add(wallAmount);
        _maze = new List<Char[]>();
    }
    public void CreateMaze()
    {
        char[] mazeItem = new char[_width];
        char[] solidRow = new string('█', _width).ToCharArray();

        // Generate mazeItem/middle rows
        for (int i = 0; i < _width; i++)
        {
            if (i == 0 || i == _width - 1)
                mazeItem[i] = '█';

            else if (i % 2 != 0)
                mazeItem[i] = ' ';

            else
                mazeItem[i] = '_';
        }

        for (int i = 0; i < _height; i++)
        {
            if (i == 0 || i == _height - 1)
            {
                _maze.Add(solidRow);
            }
            else
            {
                _maze.Add(mazeItem);
            }
        }

        foreach (char[] row in _maze)
        {
            Console.WriteLine(String.Join("", row));
        }

        List<int[]> usedCoords = new List<int[]>();
        Random randPick = new Random();

        // Generate player and exit tile 
        int[] pcoords = { randPick.Next(1, _width - 1), randPick.Next(1, _width - 1) };
        Console.WriteLine($"{pcoords[0]}, {pcoords[1]}");

    }
}
/** 
// ############### Template for Maze class to be transfered ##################
public class Maze
{
    private List<char[]> _mazeList = new List<char[]>();
    public Maze()
    {
        
    }
    public void MazeGen(int x = 10, int y = 10)
    {
            
    }
    public void MazeDisplay(List<char[]> mazeList)
    {

    }
}
**/
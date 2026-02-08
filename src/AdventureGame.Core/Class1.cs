namespace AdventureGame.Core;

public abstract class Item
{
    public string _name = "placeholder";
    public string _pickUpMessage = "placeholder";
}

public interface ICharacter
{

}

public class Player : ICharacter
{
    public Player()
    {
        int _health = 100; // Max of 150 HP, base damage is 10
        List<Item> _inventory = new List<Item>();
    }
}

public class Monster : ICharacter
{
    // 30-50HP

}

public class Weapon : Item
{
    // Weapons stay in inventory with no size limit; highest modifier applies to attacks
}

public class Potion : Item
{
    // Heals +20 HP
}

public class Maze
{

}

// Generate the maze using Random. You decide how to make the exit reachable.

// Battle rules: player attacks first each round. Damage = base damage (10) + weapon modifier. If the monster survives, it counterattacks. Battle continues until one reaches 0 HP. No fleeing.

// Tile behavior: after a monster is defeated or an item is picked up, the tile becomes empty and can be re-entered. Item pickup is automatic.

// Input handling: moving into a wall or off-grid does nothing but prints a short error message. Monsters do not move.

// Maze size: minimum 10x10. Exit placement should be reasonable; clearly unreachable exits lose points.
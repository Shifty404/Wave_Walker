using System;
using System.Collections.Generic;

class Weapon
{
    public string name;
    public int damage;

    public Weapon(string name, int damage)
    {
        this.name = name;
        this.damage = damage;
    }

    // Method to generate random weapon
    public static Weapon GenerateRandomWeapon(Random rand)
    {
        string[] weaponNames = { "Sword", "Axe", "Bow", "Dagger", "Staff", "Fist" };
        string randomWeaponName = weaponNames[rand.Next(weaponNames.Length)];
        int weaponDamage = 0;

        switch(randomWeaponName)
        {
            case "Sword":
                weaponDamage = rand.Next(10, 21);
                break;
            case "Axe":
                weaponDamage = rand.Next(15, 26);
                break;
            case "Bow":
                weaponDamage = rand.Next(8, 18);
                break;
            case "Staff":
                weaponDamage = rand.Next(12, 22);
                break;
            case "Dagger":
                weaponDamage = rand.Next(7, 15);
                break;
            case "Fist":
                weaponDamage = 5;
                break;
        }

        return new Weapon(randomWeaponName, weaponDamage);
    }
}

class player
{
    public string name;
    public int health;
    public Weapon weapon;

    public player(string name, int health, Weapon weapon)
    {
        this.name = name;
        this.health = health;
        this.weapon = weapon;
    }
}

class Warrior : player
{
    public Warrior(string name, Random rand) : base(name, 100, null)
    {
        // Warrior uses Sword
        this.weapon = new Weapon("Sword", rand.Next(10, 25));
    }
}

class Mage : player
{
    public Mage(string name, Random rand) : base(name, 70, null)
    {
        // Mage uses Staff
        this.weapon = new Weapon("Staff", rand.Next(12, 30));
    }
}

class Ranger : player
{
    public Ranger(string name, Random rand) : base(name, 80, null)
    {
        // Ranger uses Bow
        this.weapon = new Weapon("Bow", rand.Next(8, 20));
    }
}

class Berserker : player
{
    public Berserker(string name, Random rand) : base(name, 120, null)
    {
        // Berserker uses Axe
        this.weapon = new Weapon("Axe", rand.Next(15, 35));
    }
}

class Assassin : player
{
    public Assassin(string name, Random rand) : base(name, 90, null)
    {
        // Assassin uses Dagger
        this.weapon = new Weapon("Dagger", rand.Next(7, 15));
    }
}



class MartialArtist : player
{
    public MartialArtist(string name, Random rand) : base(name, 90, null)
    {
        // Martial Artist uses Fist
        this.weapon = new Weapon("Fist", 20);
    }
}

class enemies
{
    public string type;
    public int health;
    public Weapon weapon;

    public enemies(string type, int health, Weapon weapon)
    {
        this.type = type;
        this.health = health;
        this.weapon = weapon;
    }

    public string GetDeathMessage()
    {
        switch(this.type)
        {
            case "Orc":
                return this.type + " dies howling in agony!";
            case "Elf":
                return this.type + "'s long life has ended!";
            case "Goblin":
                return this.type + " exploded into pieces!";
            case "Human":
                return this.type + " has died!";
            case "Dwarf":
                return this.type + " falls with honor!";
            default:
                return this.type + " has been defeated!";
        }
    }
}

class Program
{
    static int Add(int a, int b)
    {
        return a + b;
    }

    static void Greet(string name)
    {
        Console.WriteLine("Hi, " + name + "!");
    }

    static void GreetPlayers(string hero)
    {
        Console.WriteLine("Welcome " + hero + " to the game!");
    }

    static void Main()
    {
        // Initialize random number generator
        Random rand = new Random();
        
        // Example of user input and output
        Console.WriteLine("Enter player 1 name: ");
        string playerName = Console.ReadLine();
        
        // Using method from to greet players
        GreetPlayers(playerName);

        // Class selection system
        Console.WriteLine("\nChoose your class:");
        Console.WriteLine("1. Warrior (Sword - Balanced weapon for close combat)");
        Console.WriteLine("2. Mage (Staff - Great for magic users)");
        Console.WriteLine("3. Ranger (Bow - Perfect for ranged attacks)");
        Console.WriteLine("4. Berserker (Axe - Heavy damage but slower)");
        Console.WriteLine("5. Assassin (Dagger - Stealthy and quick attacks)");
        Console.WriteLine("6. Martial Artist (Fist - Fighting bare-handed)");
        Console.WriteLine("\nEnter class number (1-6):");
        
        string classChoice = Console.ReadLine();
        player hero = null;
        
        switch(classChoice)
        {
            case "1":
                hero = new Warrior(playerName, rand);
                Console.WriteLine("You have chosen Warrior! A balanced warrior for close combat.");
                break;
            case "2":
                hero = new Mage(playerName, rand);
                Console.WriteLine("You have chosen Mage! Master of magical arts.");
                break;
            case "3":
                hero = new Ranger(playerName, rand);
                Console.WriteLine("You have chosen Ranger! Expert in ranged combat.");
                break;
            case "4":
                hero = new Berserker(playerName, rand);
                Console.WriteLine("You have chosen Berserker! Fierce and powerful warrior.");
                break;
            case "5":
                hero = new Assassin(playerName, rand);
                Console.WriteLine("You have chosen Assassin! Stealthy and quick attacks.");
                break;
            case "6":
                hero = new MartialArtist(playerName, rand);
                Console.WriteLine("You have chosen Martial Artist! Master of hand-to-hand combat.");
                break;
            default:
                Console.WriteLine("Invalid choice! Defaulting to Martial Artist.");
                hero = new MartialArtist(playerName, rand);
                break;
        }

        Console.WriteLine("\n" + hero.name + " health: " + hero.health);

        Console.WriteLine(hero.name + "'s " + hero.weapon.name + " deals " + hero.weapon.damage + " damage per attack.");

        // Enemy generation setup
        string[] enemyTypes = { "Orc", "Elf", "Goblin", "Human", "Dwarf" };
        
        // Enemy health values by type
        Dictionary<string, int> enemyHealthByType = new Dictionary<string, int>
        {
            { "Orc", 50 },
            { "Elf", 27 },
            { "Goblin", 20 },
            { "Human", 35 },
            { "Dwarf", 30 }
        };

        // for loop example
        Console.WriteLine("\n--- Battle Start ---");
        int enemiesLeft = rand.Next(3, 5);
        for(int i = 0; i < enemiesLeft; i++)
        {
            Console.WriteLine("Enemy " + (i + 1) + " appeared!");
        }

        // while loop example - combat system
        while(enemiesLeft > 0 && hero.health > 0)
        {
            // Generate random enemy
            string randomEnemyType = enemyTypes[rand.Next(enemyTypes.Length)];
            Weapon randomEnemyWeapon = Weapon.GenerateRandomWeapon(rand);
            
            enemies currentEnemy = new enemies(
                randomEnemyType,
                enemyHealthByType[randomEnemyType],
                randomEnemyWeapon
            );
            
            Console.WriteLine("\nEnemies left: " + enemiesLeft);
            Console.WriteLine("A " + currentEnemy.type + " appeared with " + currentEnemy.weapon.name + " (Damage: " + currentEnemy.weapon.damage + ")!");
            Console.WriteLine(currentEnemy.type + " health: " + currentEnemy.health);
            Console.WriteLine(hero.name + " health: " + hero.health);
            
            // Fight until enemy is dead
            while(currentEnemy.health > 0 && hero.health > 0)
            {
                // Player attacks with weapon-specific message
                currentEnemy.health -= hero.weapon.damage;
                
                string attackMessage = "";
                switch(hero.weapon.name)
                {
                    case "Sword":
                        attackMessage = hero.name + " swings sword!";
                        break;
                    case "Bow":
                        attackMessage = hero.name + " shoots arrow!";
                        break;
                    case "Fist":
                        attackMessage = hero.name + " punched with his fist!";
                        break;
                    case "Axe":
                        attackMessage = hero.name + " swings axe with fury!";
                        break;
                    case "Staff":
                        attackMessage = hero.name + " casts spell with staff!";
                        break;
                    case "Dagger":
                        attackMessage = hero.name + " strikes swiftly with dagger!";
                        break;
                }
                
                Console.WriteLine(attackMessage);
                Console.WriteLine("Deals " + hero.weapon.damage + " damage!");
                
                if(currentEnemy.health > 0)
                {
                    Console.WriteLine(currentEnemy.type + " has " + currentEnemy.health + " health left.");
                    
                    // Enemy attacks back
                    hero.health -= currentEnemy.weapon.damage;
                    Console.WriteLine(currentEnemy.type + " attacks with " + currentEnemy.weapon.name + " for " + currentEnemy.weapon.damage + " damage!");
                    
                    if(hero.health > 0)
                    {
                        Console.WriteLine(hero.name + " health: " + hero.health);
                    }
                    else
                    {
                        Console.WriteLine(hero.name + " has been defeated!");
                    }
                }
                else
                {
                    Console.WriteLine(currentEnemy.GetDeathMessage());
                }
                
                System.Threading.Thread.Sleep(500); // Pause for readability
            }
            
            // Only decrement enemies if enemy was defeated
            if(currentEnemy.health <= 0)
            {
                enemiesLeft--;
            }
        }
        
        if(hero.health > 0)
        {
            Console.WriteLine("\n*** Victory! All enemies defeated! ***");
            Console.WriteLine(hero.name + " has " + hero.health + " health remaining.");
        }
        else
        {
            Console.WriteLine("\n*** Game Over! " + hero.name + " was defeated! ***");
        }

        // foreach loop example
        string[] items = { "potion", "elixir", "bomb" };
        Console.WriteLine("\n--- Items Collected ---");
        foreach(string item in items)
        {
            Console.WriteLine("Item: " + item);
        }

        // Example of conditional statements
        if(hero.health == 100)
        {
            Console.WriteLine(hero.name + " is at full health!");
        }
        else if (hero.health > 50)
        {
            Console.WriteLine(hero.name + " is in good health!");
        }
        else
        {
            Console.WriteLine(hero.name + " has low health!");
        }

        // Array example
        int[] scores = { 90, 85, 78, 92, 88 };
        Console.WriteLine("\nPlayer scores: " + scores[0] + ", " + scores[1] + ", " + scores[2] + ", " + scores[3] + ", " + scores[4]);

        // 2D array example 
        int[,] grid = {
            {1, 2, 3},
            {4, 5, 6},
            {7, 8, 9}
        };
        Console.WriteLine("Grid element at (1,2): " + grid[1, 2]);

    }
}
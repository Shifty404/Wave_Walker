using System;
using System.Collections.Generic;
using System.Threading;

class Weapon
{
    public string Name { get; set; }
    public int Damage { get; set; }

    public Weapon(string name, int damage)
    {
        Name = name;
        Damage = damage;
    }

    public static Weapon GenerateRandomWeapon(Random rand)
    {
        string[] weaponNames = { "Sword", "Axe", "Bow", "Dagger", "Staff", "Fist" };
        string randomWeaponName = weaponNames[rand.Next(weaponNames.Length)];
        int weaponDamage = 0;

        switch (randomWeaponName)
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

class Player
{
    public string Name { get; set; }
    public int Health { get; set; }
    public int MaxHealth { get; set; }
    public Weapon Weapon { get; set; }
    public int XP { get; set; }
    public int Level { get; set; }
    public int Potions { get; set; }
    public bool IsDefending { get; set; }
    public bool IsDodging { get; set; }
    public bool IsParrying { get; set; }

    public Player(string name, int health, Weapon weapon)
    {
        Name = name;
        Health = health;
        MaxHealth = health;
        Weapon = weapon;
        XP = 0;
        Level = 1;
        Potions = 3;
        IsDefending = false;
        IsDodging = false;
        IsParrying = false;
    }

    public void GainXP(int amount)
    {
        XP += amount;
        Console.WriteLine($"{Name} gained {amount} XP!");

        int xpNeeded = Level * 100;
        if (XP >= xpNeeded)
        {
            LevelUp();
        }
    }

    private void LevelUp()
    {
        Level++;
        XP = 0;
        Weapon.Damage += 5;
        MaxHealth += 20;
        Health = MaxHealth;

        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine("\n*** LEVEL UP! ***");
        Console.WriteLine($"{Name} is now level {Level}!");
        Console.WriteLine($"Health fully restored to {MaxHealth}!");
        Console.WriteLine($"Weapon damage increased to {Weapon.Damage}!");
        Console.ResetColor();
    }

    public void UsePotion()
    {
        if (Potions > 0)
        {
            int healAmount = Math.Min(50, MaxHealth - Health);
            Health += healAmount;
            Potions--;
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"{Name} used a potion and restored {healAmount} HP!");
            Console.ResetColor();
            Console.WriteLine($"Current health: {Health}/{MaxHealth}");
            Console.WriteLine($"Potions remaining: {Potions}");
        }
        else
        {
            Console.WriteLine("No potions left!");
        }
    }

    public void ResetCombatStates()
    {
        IsDefending = false;
        IsDodging = false;
        IsParrying = false;
    }
}

// Subclasses for different player types
class Warrior : Player
{
    public Warrior(string name, Random rand) : base(name, 100, new Weapon("Sword", rand.Next(10, 25))) { }
}

class Mage : Player
{
    public Mage(string name, Random rand) : base(name, 70, new Weapon("Staff", rand.Next(12, 30))) { }
}

class Ranger : Player
{
    public Ranger(string name, Random rand) : base(name, 80, new Weapon("Bow", rand.Next(8, 20))) { }
}

class Berserker : Player
{
    public Berserker(string name, Random rand) : base(name, 120, new Weapon("Axe", rand.Next(15, 35))) { }
}

class Assassin : Player
{
    public Assassin(string name, Random rand) : base(name, 90, new Weapon("Dagger", rand.Next(7, 15))) { }
}

class MartialArtist : Player
{
    public MartialArtist(string name, Random rand) : base(name, 90, new Weapon("Fist", 20)) { }
}

class Enemy
{
    public string Type { get; set; }
    public int Health { get; set; }
    public Weapon Weapon { get; set; }
    public int XPReward { get; set; }
    public bool IsDefending { get; set; }
    public bool IsDodging { get; set; }
    public bool IsParrying { get; set; }

    public Enemy(string type, int health, Weapon weapon, int xpReward)
    {
        Type = type;
        Health = health;
        Weapon = weapon;
        XPReward = xpReward;
        IsDefending = false;
        IsDodging = false;
        IsParrying = false;
    }

    public void ResetCombatStates()
    {
        IsDefending = false;
        IsDodging = false;
        IsParrying = false;
    }

    public string GetDeathMessage()
    {
        return Type switch
        {
            "Orc" => $"{Type} dies howling in agony!",
            "Elf" => $"{Type}'s long life has ended!",
            "Goblin" => $"{Type} exploded into pieces!",
            "Human" => $"{Type} has died!",
            "Dwarf" => $"{Type} falls with honor!",
            _ => $"{Type} has been defeated!"
        };
    }
}

class Game
{
    private Random rand;
    private Player hero;
    private int currentWave;
    private bool continueGame;

    private readonly string[] enemyTypes = { "Orc", "Elf", "Goblin", "Human", "Dwarf" };
    private readonly Dictionary<string, int> enemyHealthByType = new Dictionary<string, int>
    {
        { "Orc", 50 }, { "Elf", 27 }, { "Goblin", 20 }, { "Human", 35 }, { "Dwarf", 30 }
    };
    private readonly Dictionary<string, int> enemyXPRewards = new Dictionary<string, int>
    {
        { "Orc", 50 }, { "Elf", 30 }, { "Goblin", 25 }, { "Human", 40 }, { "Dwarf", 35 }
    };

    public Game()
    {
        rand = new Random();
        currentWave = 1;
        continueGame = true;
    }

    // Helper for retro typewriter effect
    private void Typewrite(string message, int speed = 20, bool newLine = true)
    {
        foreach (char c in message)
        {
            Console.Write(c);
            Thread.Sleep(speed);
        }
        if (newLine) Console.WriteLine();
    }

    // Helper for colored text
    private void TypewriteColor(string message, ConsoleColor color, int speed = 20, bool newLine = true)
    {
        Console.ForegroundColor = color;
        Typewrite(message, speed, newLine);
        Console.ResetColor();
    }

    public void Start()
    {
        Typewrite("Enter player's ");
        Typewrite("Name: ", 20, false);
        string playerName = Console.ReadLine();
        Typewrite($"Welcome {playerName} to the game!");

        SelectClass(playerName);
        PrintStats();

        while (continueGame && hero.Health > 0)
        {
            PlayWave();
        }

        EndGame();
    }

    private void SelectClass(string playerName)
    {
        Typewrite("\nChoose your class:");
        Console.WriteLine("1. Warrior (Sword - Balanced weapon for close combat)");
        Console.WriteLine("2. Mage (Staff - Great for magic users)");
        Console.WriteLine("3. Ranger (Bow - Perfect for ranged attacks)");
        Console.WriteLine("4. Berserker (Axe - Heavy damage but slower)");
        Console.WriteLine("5. Assassin (Dagger - Stealthy and quick attacks)");
        Console.WriteLine("6. Martial Artist (Fist - Fighting bare-handed)");
        Typewrite("\nEnter class number (1-6):");
        Typewrite("Choose an option: ", 20, false);

        string classChoice = Console.ReadLine();

        switch (classChoice)
        {
            case "1":
                hero = new Warrior(playerName, rand);
                TypewriteColor("You have chosen Warrior!", ConsoleColor.Cyan);
                break;
            case "2":
                hero = new Mage(playerName, rand);
                TypewriteColor("You have chosen Mage!", ConsoleColor.Cyan);
                break;
            case "3":
                hero = new Ranger(playerName, rand);
                TypewriteColor("You have chosen Ranger!", ConsoleColor.Cyan);
                break;
            case "4":
                hero = new Berserker(playerName, rand);
                TypewriteColor("You have chosen Berserker!", ConsoleColor.Cyan);
                break;
            case "5":
                hero = new Assassin(playerName, rand);
                TypewriteColor("You have chosen Assassin!", ConsoleColor.Cyan);
                break;
            case "6":
                hero = new MartialArtist(playerName, rand);
                TypewriteColor("You have chosen Martial Artist!", ConsoleColor.Cyan);
                break;
            default:
                TypewriteColor("Invalid choice! Defaulting to Martial Artist.", ConsoleColor.Yellow);
                hero = new MartialArtist(playerName, rand);
                break;
        }
    }

    private void PrintStats()
    {
        Console.WriteLine($"\n{hero.Name} Stats:");
        Console.WriteLine($"Level: {hero.Level}");
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine($"Health: {hero.Health}/{hero.MaxHealth}");
        Console.ResetColor();
        Console.WriteLine($"Weapon: {hero.Weapon.Name} (Damage: {hero.Weapon.Damage})");
        Console.WriteLine($"Potions: {hero.Potions}");
    }

    private void PlayWave()
    {
        TypewriteColor($"\n========== WAVE {currentWave} ===========", ConsoleColor.Magenta);
        int enemiesLeft = rand.Next(2, 5);
        Typewrite($"{enemiesLeft} enemies incoming!");

        while (enemiesLeft > 0 && hero.Health > 0)
        {
            Enemy currentEnemy = GenerateEnemy();
            Console.WriteLine($"\nEnemies left: {enemiesLeft}");
            TypewriteColor($"A {currentEnemy.Type} appeared with {currentEnemy.Weapon.Name} (Damage: {currentEnemy.Weapon.Damage})!", ConsoleColor.Red);

            CombatLoop(currentEnemy);

            if (currentEnemy.Health <= 0)
            {
                enemiesLeft--;
            }
        }

        if (hero.Health > 0)
        {
            WaveComplete();
        }
    }

    private Enemy GenerateEnemy()
    {
        string randomEnemyType = enemyTypes[rand.Next(enemyTypes.Length)];
        Weapon randomEnemyWeapon = Weapon.GenerateRandomWeapon(rand);

        int scaledHealth = enemyHealthByType[randomEnemyType] + ((currentWave - 1) * 10);
        int scaledWeaponDamage = randomEnemyWeapon.Damage + ((currentWave - 1) * 3);
        int scaledXPReward = enemyXPRewards[randomEnemyType] + ((currentWave - 1) * 15);

        randomEnemyWeapon.Damage = scaledWeaponDamage;

        return new Enemy(randomEnemyType, scaledHealth, randomEnemyWeapon, scaledXPReward);
    }

    private void CombatLoop(Enemy currentEnemy)
    {
        while (currentEnemy.Health > 0 && hero.Health > 0)
        {
            hero.ResetCombatStates();
            currentEnemy.ResetCombatStates();

            PlayerTurn(currentEnemy);

            if (currentEnemy.Health <= 0)
            {
                HandleEnemyDefeat(currentEnemy);
                break;
            }

            if (hero.Health > 0)
            {
                Thread.Sleep(1000); // Pause before enemy turn
                EnemyTurn(currentEnemy);
                Thread.Sleep(1000); // Pause after enemy turn
            }
        }
    }

    private void PlayerTurn(Enemy currentEnemy)
    {
        Console.WriteLine("\n--- Your Turn ---");
        Console.WriteLine($"{hero.Name} HP: {hero.Health}/{hero.MaxHealth} | {currentEnemy.Type} HP: {currentEnemy.Health}");
        Console.WriteLine("1. Attack | 2. Parry | 3. Dodge | 4. Defend | 5. Potion | 6. Wait");
        Typewrite("Choose an option: ", 20, false);

        string playerAction = Console.ReadLine();
        bool turnEnded = false;

        switch (playerAction)
        {
            case "1":
                PerformAttack(hero, currentEnemy);
                turnEnded = true;
                break;
            case "2":
                Typewrite($"{hero.Name} prepares to parry!");
                hero.IsParrying = true;
                turnEnded = true;
                break;
            case "3":
                Typewrite($"{hero.Name} prepares to dodge!");
                hero.IsDodging = true;
                turnEnded = true;
                break;
            case "4":
                Typewrite($"{hero.Name} takes a defensive stance!");
                hero.IsDefending = true;
                turnEnded = true;
                break;
            case "5":
                if (hero.Potions > 0)
                {
                    hero.UsePotion();
                    turnEnded = true;
                }
                else
                {
                    TypewriteColor("No potions!", ConsoleColor.Red);
                }
                break;
            case "6":
                Typewrite("Skipping turn...");
                turnEnded = true;
                break;
            default:
                TypewriteColor("Invalid action!", ConsoleColor.Yellow);
                break;
        }
        
        if (!turnEnded) PlayerTurn(currentEnemy); // Retry if invalid
    }

    private void PerformAttack(Player attacker, Enemy target)
    {
        Typewrite($"{attacker.Name} attacks with {attacker.Weapon.Name}!");
        target.Health -= attacker.Weapon.Damage;
        TypewriteColor($"Deals {attacker.Weapon.Damage} damage!", ConsoleColor.Green);
    }

    private void EnemyTurn(Enemy currentEnemy)
    {
        Console.WriteLine("\n--- Enemy Turn ---");
        Thread.Sleep(500);

        int actionRoll = rand.Next(1, 101);
        // 70% attack, 10% parry, 10% dodge, 10% defend
        if (actionRoll <= 70)
        {
            EnemyAttack(currentEnemy);
        }
        else if (actionRoll <= 80)
        {
            Typewrite($"{currentEnemy.Type} prepares to parry!");
            currentEnemy.IsParrying = true;
        }
        else if (actionRoll <= 90)
        {
            Typewrite($"{currentEnemy.Type} prepares to dodge!");
            currentEnemy.IsDodging = true;
        }
        else
        {
            Typewrite($"{currentEnemy.Type} defends!");
            currentEnemy.IsDefending = true;
        }
    }

    private void EnemyAttack(Enemy enemy)
    {
        Typewrite($"{enemy.Type} attacks!");

        if (hero.IsDodging)
        {
            if (rand.Next(1, 101) <= 60)
            {
                TypewriteColor($"{hero.Name} dodged successfully!", ConsoleColor.Cyan);
                return;
            }
            TypewriteColor("Dodge failed!", ConsoleColor.Red);
        }

        if (hero.IsParrying)
        {
            TypewriteColor($"{hero.Name} parried and counters!", ConsoleColor.Cyan);
            int counterDmg = hero.Weapon.Damage / 2;
            enemy.Health -= counterDmg;
            TypewriteColor($"Counter deals {counterDmg} damage!", ConsoleColor.Green);
            return;
        }

        int damage = enemy.Weapon.Damage;
        if (hero.IsDefending)
        {
            damage /= 2;
            TypewriteColor("Blocked! Damage reduced.", ConsoleColor.Cyan);
        }

        hero.Health -= damage;
        TypewriteColor($"{hero.Name} takes {damage} damage!", ConsoleColor.Red);
    }

    private void HandleEnemyDefeat(Enemy enemy)
    {
        TypewriteColor($"\n{enemy.GetDeathMessage()}", ConsoleColor.Yellow);
        hero.GainXP(enemy.XPReward);

        if (rand.Next(1, 21) == 1)
        {
            hero.Potions++;
            TypewriteColor("Enemy dropped a potion!", ConsoleColor.Green);
        }
    }

    private void WaveComplete()
    {
        TypewriteColor($"\n*** Wave {currentWave} Complete! ***", ConsoleColor.Magenta);
        PrintStats();

        bool menuActive = true;
        while (menuActive)
        {
            Console.WriteLine("\n1. Next Wave | 2. Inventory | 3. Quit");
            Console.Write("Choose an option: ");
            string choice = Console.ReadLine();

            switch (choice)
            {
                case "1":
                    if (hero.Health < 50)
                    {
                        TypewriteColor($"\nWARNING: Low health ({hero.Health}/{hero.MaxHealth})!", ConsoleColor.Red);
                        Console.WriteLine("Continue? (y/n)");
                        Console.Write("Choose an option: ");
                        if (Console.ReadLine().ToLower() != "y") break;
                    }
                    currentWave++;
                    menuActive = false;
                    break;

                case "2":
                    InventoryMenu();
                    break;

                case "3":
                    continueGame = false;
                    menuActive = false;
                    break;

                default:
                    Console.WriteLine("Invalid choice.");
                    break;
            }
        }
    }

    private void InventoryMenu()
    {
        bool inInventory = true;
        while (inInventory)
        {
            Console.WriteLine("\n=== Inventory ===");
            
            if (hero.Potions == 0)
            {
                TypewriteColor("Inventory empty", ConsoleColor.Gray);
                Console.WriteLine("1. Back");
                Console.Write("Choose an option: ");
                string choice = Console.ReadLine();
                if (choice == "1") inInventory = false;
            }
            else
            {
                Console.WriteLine($"Potions: {hero.Potions}");
                Console.WriteLine("1. Use Potion | 2. Back");
                Console.Write("Choose an option: ");
                
                string choice = Console.ReadLine();
                if (choice == "1")
                {
                    Console.WriteLine("Use potion? (y/n)");
                    Console.Write("Choose an option: ");
                    if (Console.ReadLine().ToLower() == "y") hero.UsePotion();
                }
                else if (choice == "2")
                {
                    inInventory = false;
                }
            }
        }
    }

    private void EndGame()
    {
        if (hero.Health <= 0)
        {
            TypewriteColor($"\n*** Game Over! {hero.Name} was defeated! ***", ConsoleColor.Red);
        }
        else
        {
            TypewriteColor("\nThanks for playing!", ConsoleColor.Cyan);
        }
        
        int wavesSurvived = hero.Health > 0 ? currentWave : currentWave - 1;
        
        TypewriteColor($"Final Level: {hero.Level} | Waves Survived: {wavesSurvived}", ConsoleColor.White);
    }
}

class Program
{
    static void Main()
    {
        Game game = new Game();
        game.Start();
    }
}
using System;
using System.Collections.Generic;
using System.Threading;
using System.Linq;
using System.IO;

class SoundManager
{
    public static void PlaySound(string type)
    {
        if (OperatingSystem.IsWindows())
        {
            switch (type)
            {
                case "ATTACK": Console.Beep(400, 100); break;
                case "HIT": Console.Beep(200, 100); break;
                case "LEVELUP": Console.Beep(400, 100); Console.Beep(500, 100); Console.Beep(600, 200); break;
                case "DEATH": Console.Beep(150, 500); break;
                case "BUY": Console.Beep(600, 100); break;
            }
        }
    }
}

class Armor
{
    public string Name { get; set; }
    public int Defense { get; set; }

    public Armor(string name, int defense)
    {
        Name = name;
        Defense = defense;
    }
}

class Weapon
{
    public string Name { get; set; }
    public int Damage { get; set; }
    public int CritChance { get; set; }

    public Weapon(string name, int damage, int critChance = 5)
    {
        Name = name;
        Damage = damage;
        CritChance = critChance;
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

        return new Weapon(randomWeaponName, weaponDamage, 10);
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
    public int Gold { get; set; }
    public int Mana { get; set; }
    public int MaxMana { get; set; }
    public Armor Armor { get; set; }
    public List<string> Inventory { get; set; }
    public bool IsDefending { get; set; }
    public bool IsDodging { get; set; }
    public bool IsParrying { get; set; }
    protected Random rand;

    public Player(string name, int health, Weapon weapon, int mana, Random rand)
    {
        Name = name;
        Health = health;
        MaxHealth = health;
        Weapon = weapon;
        XP = 0;
        Level = 1;
        Potions = 3;
        Gold = 0;
        Mana = mana;
        MaxMana = mana;
        Armor = new Armor("Cloth Tunic", 1);
        Inventory = new List<string>();
        IsDefending = false;
        IsDodging = false;
        IsParrying = false;
        this.rand = rand;
    }

    public virtual bool UseAbility(Enemy target)
    {
        Console.WriteLine($"{Name} has no special ability!");
        return false;
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
        MaxMana += 10;
        Mana = MaxMana;
        SoundManager.PlaySound("LEVELUP");

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

    public void Meditate()
    {
        // Formula: 10 + (2% of MaxMana)
        int manaRecovered = 10 + (int)(MaxMana * 0.02);
        Mana = Math.Min(MaxMana, Mana + manaRecovered);
        Console.ForegroundColor = ConsoleColor.Blue;
        Console.WriteLine($"{Name} meditates and recovers {manaRecovered} Mana!");
        Console.ResetColor();
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
    public Warrior(string name, Random rand) : base(name, 100, new Weapon("Sword", rand.Next(10, 25), 10), 20, rand) { }

    public override bool UseAbility(Enemy target)
    {
        if (Mana >= 10)
        {
            Mana -= 10;
            Console.WriteLine($"{Name} uses Shield Bash!");
            target.IsDefending = false; // Break defense
            int dmg = (int)(Weapon.Damage * 1.5);
            target.Health -= dmg;
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"Dealt {dmg} damage and broke defense!");
            Console.ResetColor();
            SoundManager.PlaySound("ATTACK");
            return true;
        }
        else
        {
            Console.WriteLine("Not enough Mana!");
            return false;
        }
    }
}

class Mage : Player
{
    public Mage(string name, Random rand) : base(name, 70, new Weapon("Staff", rand.Next(12, 30), 5), 50, rand) { }

    public override bool UseAbility(Enemy target)
    {
        if (Mana >= 20)
        {
            Mana -= 20;
            Console.WriteLine($"{Name} casts Fireball!");
            int dmg = rand.Next(30, 50);
            target.Health -= dmg;
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"Dealt {dmg} fire damage!");
            Console.ResetColor();
            SoundManager.PlaySound("ATTACK");
            return true;
        }
        else
        {
            Console.WriteLine("Not enough Mana!");
            return false;
        }
    }
}

class Ranger : Player
{
    public Ranger(string name, Random rand) : base(name, 80, new Weapon("Bow", rand.Next(8, 20), 25), 30, rand) { }

    public override bool UseAbility(Enemy target)
    {
        if (Mana >= 15)
        {
            Mana -= 15;
            Console.WriteLine($"{Name} fires a Power Shot!");
            int dmg = Weapon.Damage * 2;
            target.Health -= dmg;
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"Dealt {dmg} critical damage!");
            Console.ResetColor();
            SoundManager.PlaySound("ATTACK");
            return true;
        }
        else
        {
            Console.WriteLine("Not enough Mana!");
            return false;
        }
    }
}

class Berserker : Player
{
    public Berserker(string name, Random rand) : base(name, 120, new Weapon("Axe", rand.Next(15, 35), 15), 10, rand) { }

    public override bool UseAbility(Enemy target)
    {
        if (Mana >= 10)
        {
            Mana -= 10;
            Console.WriteLine($"{Name} goes into a Rage!");
            int dmg = Weapon.Damage + 10;
            target.Health -= dmg;
            Health -= 5; // Self damage
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"Dealt {dmg} damage but took 5 damage!");
            Console.ResetColor();
            SoundManager.PlaySound("ATTACK");
            return true;
        }
        else
        {
            Console.WriteLine("Not enough Mana!");
            return false;
        }
    }
}

class Assassin : Player
{
    public Assassin(string name, Random rand) : base(name, 90, new Weapon("Dagger", rand.Next(7, 15), 40), 30, rand) { }

    public override bool UseAbility(Enemy target)
    {
        if (Mana >= 25)
        {
            Mana -= 25;
            Console.WriteLine($"{Name} uses Backstab!");
            int dmg = Weapon.Damage * 3;
            target.Health -= dmg;
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"Dealt {dmg} massive damage!");
            Console.ResetColor();
            SoundManager.PlaySound("ATTACK");
            return true;
        }
        else
        {
            Console.WriteLine("Not enough Mana!");
            return false;
        }
    }
}

class MartialArtist : Player
{
    public MartialArtist(string name, Random rand) : base(name, 90, new Weapon("Fist", 20, 20), 40, rand) { }

    public override bool UseAbility(Enemy target)
    {
        if (Mana >= 15)
        {
            Mana -= 15;
            Console.WriteLine($"{Name} uses Flurry of Blows!");
            int dmg = Weapon.Damage + rand.Next(5, 15);
            target.Health -= dmg;
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"Dealt {dmg} combo damage!");
            Console.ResetColor();
            SoundManager.PlaySound("ATTACK");
            return true;
        }
        else
        {
            Console.WriteLine("Not enough Mana!");
            return false;
        }
    }
}

class Enemy
{
    public string Type { get; set; }
    public int Health { get; set; }
    public Weapon Weapon { get; set; }
    public int XPReward { get; set; }
    public int GoldReward { get; set; }
    public bool IsDefending { get; set; }
    public bool IsDodging { get; set; }
    public bool IsParrying { get; set; }

    public Enemy(string type, int health, Weapon weapon, int xpReward, int goldReward)
    {
        Type = type;
        Health = health;
        Weapon = weapon;
        XPReward = xpReward;
        GoldReward = goldReward;
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

    // Loot tracking for the current wave
    private int waveGoldGained;
    private List<string> waveItemsGained;

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
        waveGoldGained = 0;
        waveItemsGained = new List<string>();
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

    // Instant text helper
    private void InstantWriteColor(string message, ConsoleColor color)
    {
        Console.ForegroundColor = color;
        Console.WriteLine(message);
        Console.ResetColor();
    }

    public void Start()
    {
        Console.WriteLine("1. New Game | 2. Load Game");
        Console.Write("Choose an option: ");
        string choice = Console.ReadLine();

        bool loaded = false;
        if (choice == "2")
        {
            loaded = LoadGame();
            if (hero == null) // Load failed or no file
            {
                Console.WriteLine("Starting new game...");
                NewGameSetup();
                loaded = false;
            }
        }
        else
        {
            NewGameSetup();
        }

        // If loaded, show the menu first before starting the wave
        if (loaded)
        {
            WaveComplete(false); // false = don't show loot summary for previous wave
        }

        while (continueGame && hero.Health > 0)
        {
            PlayWave();
        }

        EndGame();
    }

    private void NewGameSetup()
    {
        Typewrite("Enter player's ");
        Typewrite("Name: ", 20, false);
        string playerName = Console.ReadLine();
        Typewrite($"Welcome {playerName} to the game!");

        SelectClass(playerName);
        PrintStats();
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
        Console.ForegroundColor = ConsoleColor.Blue;
        Console.WriteLine($"Mana: {hero.Mana}/{hero.MaxMana}");
        Console.ResetColor();
        Console.WriteLine($"Weapon: {hero.Weapon.Name} (Damage: {hero.Weapon.Damage})");
        Console.WriteLine($"Potions: {hero.Potions}");
    }

    private void PlayWave()
    {
        // Reset wave loot tracking
        waveGoldGained = 0;
        waveItemsGained.Clear();

        TypewriteColor($"\n========== WAVE {currentWave} ===========", ConsoleColor.Magenta);
        
        int enemiesLeft;
        if (currentWave % 5 == 0)
        {
            enemiesLeft = 1; // Boss wave has only 1 enemy
            TypewriteColor("WARNING: BOSS APPROACHING!", ConsoleColor.Red);
        }
        else
        {
            enemiesLeft = rand.Next(2, 5);
            Typewrite($"{enemiesLeft} enemies incoming!");
        }

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
            WaveComplete(true); // true = show loot summary
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

        int scaledGoldReward = rand.Next(10, 30) + ((currentWave - 1) * 5);
        
        // Boss Logic
        if (currentWave % 5 == 0)
        {
            randomEnemyType = "BOSS " + randomEnemyType;
            scaledHealth *= 3;
            scaledWeaponDamage = (int)(scaledWeaponDamage * 1.5);
            scaledXPReward *= 5;
            scaledGoldReward *= 5;
            randomEnemyWeapon.Name = "Legendary " + randomEnemyWeapon.Name;
        }

        return new Enemy(randomEnemyType, scaledHealth, randomEnemyWeapon, scaledXPReward, scaledGoldReward);
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
        Console.ForegroundColor = ConsoleColor.Green;
        Console.Write($"HP: {hero.Health}/{hero.MaxHealth} ");
        Console.ForegroundColor = ConsoleColor.Blue;
        Console.WriteLine($"Mana: {hero.Mana}/{hero.MaxMana}");
        Console.ResetColor();
        Console.WriteLine($"Enemy: {currentEnemy.Type} (HP: {currentEnemy.Health})");
        
        Console.WriteLine("1. Attack | 2. Parry | 3. Dodge | 4. Defend | 5. Potion | 6. Ability | 7. Meditate | 8. Wait");
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
                // UseAbility now returns true if successful, false if not (e.g. no mana)
                if (hero.UseAbility(currentEnemy))
                {
                    turnEnded = true;
                }
                break;
            case "7":
                hero.Meditate();
                turnEnded = true;
                break;
            case "8":
                Typewrite("Skipping turn...");
                turnEnded = true;
                break;
            default:
                TypewriteColor("Invalid action!", ConsoleColor.Yellow);
                break;
        }
        
        if (!turnEnded) PlayerTurn(currentEnemy); // Retry if invalid or action failed
    }

    private void PerformAttack(Player attacker, Enemy target)
    {
        Typewrite($"{attacker.Name} attacks with {attacker.Weapon.Name}!");
        
        // Crit Logic
        int damage = attacker.Weapon.Damage;
        if (rand.Next(1, 101) <= attacker.Weapon.CritChance)
        {
            damage *= 2;
            InstantWriteColor("CRITICAL HIT!", ConsoleColor.Red);
        }

        target.Health -= damage;
        InstantWriteColor($"Deals {damage} damage!", ConsoleColor.Green);
        SoundManager.PlaySound("HIT");
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
                InstantWriteColor($"{hero.Name} dodged successfully!", ConsoleColor.Cyan);
                return;
            }
            InstantWriteColor("Dodge failed!", ConsoleColor.Red);
        }

        if (hero.IsParrying)
        {
            InstantWriteColor($"{hero.Name} parried and counters!", ConsoleColor.Cyan);
            int counterDmg = hero.Weapon.Damage / 2;
            enemy.Health -= counterDmg;
            InstantWriteColor($"Counter deals {counterDmg} damage!", ConsoleColor.Green);
            return;
        }

        int damage = enemy.Weapon.Damage;
        if (hero.IsDefending)
        {
            damage /= 2;
            InstantWriteColor("Blocked! Damage reduced.", ConsoleColor.Cyan);
        }

        // Armor Reduction
        damage = Math.Max(0, damage - hero.Armor.Defense);

        hero.Health -= damage;
        InstantWriteColor($"{hero.Name} takes {damage} damage!", ConsoleColor.Red);
    }

    private void HandleEnemyDefeat(Enemy enemy)
    {
        InstantWriteColor($"\n{enemy.GetDeathMessage()}", ConsoleColor.Yellow);
        hero.GainXP(enemy.XPReward);
        hero.Gold += enemy.GoldReward;
        
        // Track loot
        waveGoldGained += enemy.GoldReward;
        // Removed immediate print of Gold found
        SoundManager.PlaySound("DEATH");

        if (rand.Next(1, 21) == 1)
        {
            hero.Potions++;
            waveItemsGained.Add("Potion");
            // Removed immediate print of Potion found
        }
        
        if (rand.Next(1, 51) == 1) // 2% chance for armor upgrade
        {
            hero.Armor.Defense++;
            waveItemsGained.Add("Armor Upgrade");
            // Removed immediate print of Armor Upgrade found
        }
    }

    private void WaveComplete(bool showSummary)
    {
        if (showSummary)
        {
            TypewriteColor($"\n*** Wave {currentWave} Complete! ***", ConsoleColor.Magenta);
            
            // Loot Summary
            Console.WriteLine("\n--- Wave Loot ---");
            Console.WriteLine($"Gold Earned: {waveGoldGained}");
            if (waveItemsGained.Count > 0)
            {
                Console.WriteLine("Items Found: " + string.Join(", ", waveItemsGained));
            }
            else
            {
                Console.WriteLine("Items Found: None");
            }
            Console.WriteLine("-----------------");
        }

        PrintStats();

        bool menuActive = true;
        while (menuActive)
        {
            Console.WriteLine("\n1. Next Wave | 2. Inventory | 3. Shop | 4. Save & Quit");
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
                    ShopMenu();
                    break;

                case "4":
                    int waveToSave = showSummary ? currentWave + 1 : currentWave;
                    SaveGame(waveToSave);
                    continueGame = false;
                    menuActive = false;
                    break;

                default:
                    Console.WriteLine("Invalid choice.");
                    break;
            }
        }
    }

    private void ShopMenu()
    {
        bool inShop = true;
        while (inShop)
        {
            Console.WriteLine($"\n=== Shop (Gold: {hero.Gold}) ===");
            Console.WriteLine("1. Potion (50 Gold) | 2. Weapon Upgrade (100 Gold) | 3. Armor Upgrade (150 Gold) | 4. Back");
            Console.Write("Choose an option: ");
            string choice = Console.ReadLine();

            switch (choice)
            {
                case "1":
                    if (hero.Gold >= 50)
                    {
                        hero.Gold -= 50;
                        hero.Potions++;
                        InstantWriteColor("Bought a Potion!", ConsoleColor.Green);
                        SoundManager.PlaySound("BUY");
                    }
                    else Console.WriteLine("Not enough Gold!");
                    break;
                case "2":
                    if (hero.Gold >= 100)
                    {
                        hero.Gold -= 100;
                        int oldDmg = hero.Weapon.Damage;
                        hero.Weapon.Damage += 5;
                        InstantWriteColor($"Weapon Upgraded! Damage: {oldDmg} -> {hero.Weapon.Damage}", ConsoleColor.Green);
                        SoundManager.PlaySound("BUY");
                    }
                    else Console.WriteLine("Not enough Gold!");
                    break;
                case "3":
                    if (hero.Gold >= 150)
                    {
                        hero.Gold -= 150;
                        int oldDef = hero.Armor.Defense;
                        hero.Armor.Defense += 1;
                        InstantWriteColor($"Armor Upgraded! Defense: {oldDef} -> {hero.Armor.Defense}", ConsoleColor.Green);
                        SoundManager.PlaySound("BUY");
                    }
                    else Console.WriteLine("Not enough Gold!");
                    break;
                case "4":
                    inShop = false;
                    break;
                default:
                    Console.WriteLine("Invalid choice.");
                    break;
            }
        }
    }

    private void SaveGame(int waveToSave)
    {
        try
        {
            using (StreamWriter writer = new StreamWriter("savegame.txt"))
            {
                writer.WriteLine(hero.Name);
                writer.WriteLine(hero.GetType().Name);
                writer.WriteLine(hero.Level);
                writer.WriteLine(hero.XP);
                writer.WriteLine(hero.Health);
                writer.WriteLine(hero.MaxHealth);
                writer.WriteLine(hero.Potions);
                writer.WriteLine(hero.Gold);
                writer.WriteLine(hero.Mana);
                writer.WriteLine(hero.MaxMana);
                writer.WriteLine(hero.Weapon.Name);
                writer.WriteLine(hero.Weapon.Damage);
                writer.WriteLine(hero.Weapon.CritChance);
                writer.WriteLine(hero.Armor.Name);
                writer.WriteLine(hero.Armor.Defense);
                writer.WriteLine(waveToSave);
            }
            InstantWriteColor("Game Saved!", ConsoleColor.Green);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error saving game: {ex.Message}");
        }
    }

    private bool LoadGame()
    {
        if (!File.Exists("savegame.txt"))
        {
            Console.WriteLine("No save file found.");
            return false;
        }

        try
        {
            using (StreamReader reader = new StreamReader("savegame.txt"))
            {
                string name = reader.ReadLine();
                string className = reader.ReadLine();
                int level = int.Parse(reader.ReadLine());
                int xp = int.Parse(reader.ReadLine());
                int health = int.Parse(reader.ReadLine());
                int maxHealth = int.Parse(reader.ReadLine());
                int potions = int.Parse(reader.ReadLine());
                int gold = int.Parse(reader.ReadLine());
                int mana = int.Parse(reader.ReadLine());
                int maxMana = int.Parse(reader.ReadLine());
                string weaponName = reader.ReadLine();
                int weaponDmg = int.Parse(reader.ReadLine());
                int weaponCrit = int.Parse(reader.ReadLine());
                string armorName = reader.ReadLine();
                int armorDef = int.Parse(reader.ReadLine());
                currentWave = int.Parse(reader.ReadLine());

                // Re-create hero based on class
                switch (className)
                {
                    case "Warrior": hero = new Warrior(name, rand); break;
                    case "Mage": hero = new Mage(name, rand); break;
                    case "Ranger": hero = new Ranger(name, rand); break;
                    case "Berserker": hero = new Berserker(name, rand); break;
                    case "Assassin": hero = new Assassin(name, rand); break;
                    case "MartialArtist": hero = new MartialArtist(name, rand); break;
                    default: hero = new Warrior(name, rand); break;
                }

                hero.Level = level;
                hero.XP = xp;
                hero.Health = health;
                hero.MaxHealth = maxHealth;
                hero.Potions = potions;
                hero.Gold = gold;
                hero.Mana = mana;
                hero.MaxMana = maxMana;
                hero.Weapon = new Weapon(weaponName, weaponDmg, weaponCrit);
                hero.Armor = new Armor(armorName, armorDef);

                InstantWriteColor("Game Loaded!", ConsoleColor.Green);
                return true;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error loading game: {ex.Message}");
            return false;
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
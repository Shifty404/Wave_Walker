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
        UI.Typewrite($"{Name} has no special ability!");
        return false;
    }

    public void GainXP(int amount)
    {
        XP += amount;
        UI.Typewrite($"{Name} gained {amount} XP!", ConsoleColor.Yellow);

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

        UI.Typewrite("\n*** LEVEL UP! ***", ConsoleColor.Yellow);
        UI.Typewrite($"{Name} is now level {Level}!", ConsoleColor.Yellow);
        UI.Typewrite($"Health fully restored to {MaxHealth}!", ConsoleColor.Green);
        UI.Typewrite($"Weapon damage increased to {Weapon.Damage}!", ConsoleColor.Cyan);
    }

    public void UsePotion()
    {
        if (Potions > 0)
        {
            int healAmount = Math.Min(50, MaxHealth - Health);
            Health += healAmount;
            Potions--;
            UI.Typewrite($"{Name} used a potion and restored {healAmount} HP!", ConsoleColor.Green);
            UI.Typewrite($"Current health: {Health}/{MaxHealth}", ConsoleColor.Green);
            UI.Typewrite($"Potions remaining: {Potions}", ConsoleColor.White);
        }
        else
        {
            UI.Typewrite("No potions left!", ConsoleColor.Red);
        }
    }

    public void Meditate()
    {
        int manaRecovered = 10 + (int)(MaxMana * 0.02);
        Mana = Math.Min(MaxMana, Mana + manaRecovered);
        UI.Typewrite($"{Name} meditates and recovers {manaRecovered} Mana!", ConsoleColor.Blue);
    }

    public void ResetCombatStates()
    {
        IsDefending = false;
        IsDodging = false;
        IsParrying = false;
    }
}

class Warrior : Player
{
    public Warrior(string name, Random rand) : base(name, 100, new Weapon("Sword", rand.Next(10, 25), 10), 20, rand) { }

    public override bool UseAbility(Enemy target)
    {
        if (Mana >= 10)
        {
            Mana -= 10;
            UI.Typewrite($"{Name} uses Shield Bash!", ConsoleColor.Cyan);
            target.IsDefending = false;
            int dmg = (int)(Weapon.Damage * 1.5);
            target.Health -= dmg;
            UI.Typewrite($"Dealt {dmg} damage and broke defense!", ConsoleColor.Green);
            SoundManager.PlaySound("ATTACK");
            return true;
        }
        else
        {
            UI.Typewrite("Not enough Mana!", ConsoleColor.Red);
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
            UI.Typewrite($"{Name} casts Fireball!", ConsoleColor.Cyan);
            int dmg = rand.Next(30, 50);
            target.Health -= dmg;
            UI.Typewrite($"Dealt {dmg} fire damage!", ConsoleColor.Green);
            SoundManager.PlaySound("ATTACK");
            return true;
        }
        else
        {
            UI.Typewrite("Not enough Mana!", ConsoleColor.Red);
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
            UI.Typewrite($"{Name} fires a Power Shot!", ConsoleColor.Cyan);
            int dmg = Weapon.Damage * 2;
            target.Health -= dmg;
            UI.Typewrite($"Dealt {dmg} critical damage!", ConsoleColor.Green);
            SoundManager.PlaySound("ATTACK");
            return true;
        }
        else
        {
            UI.Typewrite("Not enough Mana!", ConsoleColor.Red);
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
            UI.Typewrite($"{Name} goes into a Rage!", ConsoleColor.Cyan);
            int dmg = Weapon.Damage + 10;
            target.Health -= dmg;
            Health -= 5;
            UI.Typewrite($"Dealt {dmg} damage but took 5 damage!", ConsoleColor.Green);
            SoundManager.PlaySound("ATTACK");
            return true;
        }
        else
        {
            UI.Typewrite("Not enough Mana!", ConsoleColor.Red);
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
            UI.Typewrite($"{Name} uses Backstab!", ConsoleColor.Cyan);
            int dmg = Weapon.Damage * 3;
            target.Health -= dmg;
            UI.Typewrite($"Dealt {dmg} massive damage!", ConsoleColor.Green);
            SoundManager.PlaySound("ATTACK");
            return true;
        }
        else
        {
            UI.Typewrite("Not enough Mana!", ConsoleColor.Red);
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
            UI.Typewrite($"{Name} uses Flurry of Blows!", ConsoleColor.Cyan);
            int dmg = Weapon.Damage + rand.Next(5, 15);
            target.Health -= dmg;
            UI.Typewrite($"Dealt {dmg} combo damage!", ConsoleColor.Green);
            SoundManager.PlaySound("ATTACK");
            return true;
        }
        else
        {
            UI.Typewrite("Not enough Mana!", ConsoleColor.Red);
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

static class UI
{
    public static void Typewrite(string message, ConsoleColor color = ConsoleColor.White, int speed = 20, bool newLine = true)
    {
        Console.ForegroundColor = color;
        bool skip = false;
        foreach (char c in message)
        {
            Console.Write(c);
            if (!skip)
            {
                if (Console.KeyAvailable)
                {
                    Console.ReadKey(true);
                    skip = true;
                }
                else
                {
                    Thread.Sleep(speed);
                }
            }
        }
        if (newLine) Console.WriteLine();
        Console.ResetColor();
    }
}

class Game
{
    private Random rand;
    private Player hero = null!;
    private int currentWave;
    private bool continueGame;

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

    public void Start()
    {
        UI.Typewrite("1. New Game | 2. Load Game", ConsoleColor.Cyan);
        UI.Typewrite("Choose an option: ", ConsoleColor.White, 20, false);
        string choice = Console.ReadLine() ?? "";

        bool loaded = false;
        if (choice == "2")
        {
            loaded = LoadGame();
            if (hero == null)
            {
                UI.Typewrite("Starting new game...", ConsoleColor.White);
                NewGameSetup();
                loaded = false;
            }
        }
        else
        {
            NewGameSetup();
        }

        if (loaded)
        {
            WaveComplete(false);
        }

        while (continueGame && hero.Health > 0)
        {
            PlayWave();
        }

        EndGame();
    }

    private void NewGameSetup()
    {
        UI.Typewrite("Enter player's Name: ", ConsoleColor.White, 20, false);
        string playerName = Console.ReadLine() ?? "Player";
        UI.Typewrite($"Welcome {playerName} to the game!", ConsoleColor.Cyan);

        SelectClass(playerName);
        PrintStats();
    }

    private void SelectClass(string playerName)
    {
        UI.Typewrite("\nChoose your class:", ConsoleColor.Cyan);
        UI.Typewrite("1. Warrior (Sword - Balanced weapon for close combat)", ConsoleColor.White);
        UI.Typewrite("2. Mage (Staff - Great for magic users)", ConsoleColor.White);
        UI.Typewrite("3. Ranger (Bow - Perfect for ranged attacks)", ConsoleColor.White);
        UI.Typewrite("4. Berserker (Axe - Heavy damage but slower)", ConsoleColor.White);
        UI.Typewrite("5. Assassin (Dagger - Stealthy and quick attacks)", ConsoleColor.White);
        UI.Typewrite("6. Martial Artist (Fist - Fighting bare-handed)", ConsoleColor.White);
        UI.Typewrite("\nEnter class number (1-6):", ConsoleColor.Cyan);
        UI.Typewrite("Choose an option: ", ConsoleColor.White, 20, false);

        string classChoice = Console.ReadLine() ?? "";

        switch (classChoice)
        {
            case "1":
                hero = new Warrior(playerName, rand);
                UI.Typewrite("You have chosen Warrior!", ConsoleColor.Cyan);
                break;
            case "2":
                hero = new Mage(playerName, rand);
                UI.Typewrite("You have chosen Mage!", ConsoleColor.Cyan);
                break;
            case "3":
                hero = new Ranger(playerName, rand);
                UI.Typewrite("You have chosen Ranger!", ConsoleColor.Cyan);
                break;
            case "4":
                hero = new Berserker(playerName, rand);
                UI.Typewrite("You have chosen Berserker!", ConsoleColor.Cyan);
                break;
            case "5":
                hero = new Assassin(playerName, rand);
                UI.Typewrite("You have chosen Assassin!", ConsoleColor.Cyan);
                break;
            case "6":
                hero = new MartialArtist(playerName, rand);
                UI.Typewrite("You have chosen Martial Artist!", ConsoleColor.Cyan);
                break;
            default:
                UI.Typewrite("Invalid choice! Defaulting to Martial Artist.", ConsoleColor.Yellow);
                hero = new MartialArtist(playerName, rand);
                break;
        }
    }

    private void PrintStats()
    {
        UI.Typewrite($"\n{hero.Name} Stats:", ConsoleColor.White);
        UI.Typewrite($"Level: {hero.Level}", ConsoleColor.White);
        UI.Typewrite($"Health: {hero.Health}/{hero.MaxHealth}", ConsoleColor.Green);
        UI.Typewrite($"Mana: {hero.Mana}/{hero.MaxMana}", ConsoleColor.Blue);
        UI.Typewrite($"Weapon: {hero.Weapon.Name} (Damage: {hero.Weapon.Damage})", ConsoleColor.White);
        UI.Typewrite($"Potions: {hero.Potions}", ConsoleColor.White);
    }

    private void PlayWave()
    {
        waveGoldGained = 0;
        waveItemsGained.Clear();

        UI.Typewrite($"\n========== WAVE {currentWave} ===========", ConsoleColor.Magenta);
        
        int enemiesLeft;
        if (currentWave % 5 == 0)
        {
            enemiesLeft = 1;
            UI.Typewrite("WARNING: BOSS APPROACHING!", ConsoleColor.Red);
        }
        else
        {
            enemiesLeft = rand.Next(2, 5);
            UI.Typewrite($"{enemiesLeft} enemies incoming!", ConsoleColor.White);
        }

        while (enemiesLeft > 0 && hero.Health > 0)
        {
            Enemy currentEnemy = GenerateEnemy();
            UI.Typewrite($"\nEnemies left: {enemiesLeft}", ConsoleColor.White);
            UI.Typewrite($"A {currentEnemy.Type} appeared with {currentEnemy.Weapon.Name} (Damage: {currentEnemy.Weapon.Damage})!", ConsoleColor.Red);

            CombatLoop(currentEnemy);

            if (currentEnemy.Health <= 0)
            {
                enemiesLeft--;
            }
        }

        if (hero.Health > 0)
        {
            WaveComplete(true);
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
                Thread.Sleep(1000);
                EnemyTurn(currentEnemy);
                Thread.Sleep(1000);
            }
        }
    }

    private void PlayerTurn(Enemy currentEnemy)
    {
        UI.Typewrite("\n--- Your Turn ---", ConsoleColor.White);
        Console.ForegroundColor = ConsoleColor.Green;
        Console.Write($"HP: {hero.Health}/{hero.MaxHealth} ");
        Console.ForegroundColor = ConsoleColor.Blue;
        Console.WriteLine($"Mana: {hero.Mana}/{hero.MaxMana}");
        Console.ResetColor();
        
        UI.Typewrite($"Enemy: {currentEnemy.Type} (HP: {currentEnemy.Health})", ConsoleColor.Red);
        
        UI.Typewrite("1. Attack | 2. Parry | 3. Dodge | 4. Defend | 5. Potion | 6. Ability | 7. Meditate | 8. Wait", ConsoleColor.White);
        UI.Typewrite("Choose an option: ", ConsoleColor.White, 20, false);

        string playerAction = Console.ReadLine() ?? "";
        bool turnEnded = false;

        switch (playerAction)
        {
            case "1":
                PerformAttack(hero, currentEnemy);
                turnEnded = true;
                break;
            case "2":
                UI.Typewrite($"{hero.Name} prepares to parry!", ConsoleColor.Cyan);
                hero.IsParrying = true;
                turnEnded = true;
                break;
            case "3":
                UI.Typewrite($"{hero.Name} prepares to dodge!", ConsoleColor.Cyan);
                hero.IsDodging = true;
                turnEnded = true;
                break;
            case "4":
                UI.Typewrite($"{hero.Name} takes a defensive stance!", ConsoleColor.Cyan);
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
                    UI.Typewrite("No potions!", ConsoleColor.Red);
                }
                break;
            case "6":
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
                UI.Typewrite("Skipping turn...");
                turnEnded = true;
                break;
            default:
                UI.Typewrite("Invalid action!", ConsoleColor.Yellow);
                break;
        }
        
        if (!turnEnded) PlayerTurn(currentEnemy);
    }

    private void PerformAttack(Player attacker, Enemy target)
    {
        UI.Typewrite($"{attacker.Name} attacks with {attacker.Weapon.Name}!", ConsoleColor.White);
        
        int damage = attacker.Weapon.Damage;
        if (rand.Next(1, 101) <= attacker.Weapon.CritChance)
        {
            damage *= 2;
            UI.Typewrite("CRITICAL HIT!", ConsoleColor.Red, 10);
        }

        target.Health -= damage;
        UI.Typewrite($"Deals {damage} damage!", ConsoleColor.Green, 10);
        SoundManager.PlaySound("HIT");
    }

    private void EnemyTurn(Enemy currentEnemy)
    {
        UI.Typewrite("\n--- Enemy Turn ---", ConsoleColor.White);
        Thread.Sleep(500);

        int actionRoll = rand.Next(1, 101);
        if (actionRoll <= 70)
        {
            EnemyAttack(currentEnemy);
        }
        else if (actionRoll <= 80)
        {
            UI.Typewrite($"{currentEnemy.Type} prepares to parry!", ConsoleColor.Cyan);
            currentEnemy.IsParrying = true;
        }
        else if (actionRoll <= 90)
        {
            UI.Typewrite($"{currentEnemy.Type} prepares to dodge!", ConsoleColor.Cyan);
            currentEnemy.IsDodging = true;
        }
        else
        {
            UI.Typewrite($"{currentEnemy.Type} defends!", ConsoleColor.Cyan);
            currentEnemy.IsDefending = true;
        }
    }

    private void EnemyAttack(Enemy enemy)
    {
        UI.Typewrite($"{enemy.Type} attacks!", ConsoleColor.Red);

        if (hero.IsDodging)
        {
            if (rand.Next(1, 101) <= 60)
            {
                UI.Typewrite($"{hero.Name} dodged successfully!", ConsoleColor.Cyan, 10);
                return;
            }
            UI.Typewrite("Dodge failed!", ConsoleColor.Red, 10);
        }

        if (hero.IsParrying)
        {
            UI.Typewrite($"{hero.Name} parried and counters!", ConsoleColor.Cyan, 10);
            int counterDmg = hero.Weapon.Damage / 2;
            enemy.Health -= counterDmg;
            UI.Typewrite($"Counter deals {counterDmg} damage!", ConsoleColor.Green, 10);
            return;
        }

        int damage = enemy.Weapon.Damage;
        if (hero.IsDefending)
        {
            damage /= 2;
            UI.Typewrite("Blocked! Damage reduced.", ConsoleColor.Cyan, 10);
        }

        damage = Math.Max(0, damage - hero.Armor.Defense);

        hero.Health -= damage;
        UI.Typewrite($"{hero.Name} takes {damage} damage!", ConsoleColor.Red, 10);
    }

    private void HandleEnemyDefeat(Enemy enemy)
    {
        UI.Typewrite($"\n{enemy.GetDeathMessage()}", ConsoleColor.Yellow);
        hero.GainXP(enemy.XPReward);
        hero.Gold += enemy.GoldReward;
        
        waveGoldGained += enemy.GoldReward;
        SoundManager.PlaySound("DEATH");

        if (rand.Next(1, 21) == 1)
        {
            hero.Potions++;
            waveItemsGained.Add("Potion");
        }
        
        if (rand.Next(1, 51) == 1)
        {
            hero.Armor.Defense++;
            waveItemsGained.Add("Armor Upgrade");
        }
    }

    private void WaveComplete(bool showSummary)
    {
        if (showSummary)
        {
            UI.Typewrite($"\n*** Wave {currentWave} Complete! ***", ConsoleColor.Magenta);
            
            UI.Typewrite("\n--- Wave Loot ---", ConsoleColor.White);
            UI.Typewrite($"Gold Earned: {waveGoldGained}", ConsoleColor.Yellow);
            if (waveItemsGained.Count > 0)
            {
                UI.Typewrite("Items Found: " + string.Join(", ", waveItemsGained), ConsoleColor.Cyan);
            }
            else
            {
                UI.Typewrite("Items Found: None");
            }
            UI.Typewrite("-----------------", ConsoleColor.White);
        }

        PrintStats();

        bool menuActive = true;
        while (menuActive)
        {
            UI.Typewrite("\n1. Next Wave | 2. Inventory | 3. Shop | 4. Save & Quit", ConsoleColor.Cyan);
            UI.Typewrite("Choose an option: ", ConsoleColor.White, 20, false);
            string choice = Console.ReadLine() ?? "";

            switch (choice)
            {
                case "1":
                    if (hero.Health < 50)
                    {
                        UI.Typewrite($"\nWARNING: Low health ({hero.Health}/{hero.MaxHealth})!", ConsoleColor.Red);
                        UI.Typewrite("Continue? (y/n)", ConsoleColor.White);
                        UI.Typewrite("Choose an option: ", ConsoleColor.White, 20, false);
                        if ((Console.ReadLine() ?? "").ToLower() != "y") break;
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
                    UI.Typewrite("Invalid choice.", ConsoleColor.Yellow);
                    break;
            }
        }
    }

    private void ShopMenu()
    {
        bool inShop = true;
        while (inShop)
        {
            UI.Typewrite($"\n=== Shop (Gold: {hero.Gold}) ===", ConsoleColor.Yellow);
            UI.Typewrite("1. Potion (50 Gold) | 2. Weapon Upgrade (100 Gold) | 3. Armor Upgrade (150 Gold) | 4. Back", ConsoleColor.White);
            UI.Typewrite("Choose an option: ", ConsoleColor.White, 20, false);
            string choice = Console.ReadLine() ?? "";

            switch (choice)
            {
                case "1":
                    if (hero.Gold >= 50)
                    {
                        hero.Gold -= 50;
                        hero.Potions++;
                        UI.Typewrite("Bought a Potion!", ConsoleColor.Green, 10);
                        SoundManager.PlaySound("BUY");
                    }
                    else UI.Typewrite("Not enough Gold!", ConsoleColor.Red);
                    break;
                case "2":
                    if (hero.Gold >= 100)
                    {
                        hero.Gold -= 100;
                        int oldDmg = hero.Weapon.Damage;
                        hero.Weapon.Damage += 5;
                        UI.Typewrite($"Weapon Upgraded! Damage: {oldDmg} -> {hero.Weapon.Damage}", ConsoleColor.Green, 10);
                        SoundManager.PlaySound("BUY");
                    }
                    else UI.Typewrite("Not enough Gold!", ConsoleColor.Red);
                    break;
                case "3":
                    if (hero.Gold >= 150)
                    {
                        hero.Gold -= 150;
                        int oldDef = hero.Armor.Defense;
                        hero.Armor.Defense += 1;
                        UI.Typewrite($"Armor Upgraded! Defense: {oldDef} -> {hero.Armor.Defense}", ConsoleColor.Green, 10);
                        SoundManager.PlaySound("BUY");
                    }
                    else UI.Typewrite("Not enough Gold!", ConsoleColor.Red);
                    break;
                case "4":
                    inShop = false;
                    break;
                default:
                    UI.Typewrite("Invalid choice.", ConsoleColor.Yellow);
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
            UI.Typewrite("Game Saved!", ConsoleColor.Green);
        }
        catch (Exception ex)
        {
            UI.Typewrite($"Error saving game: {ex.Message}", ConsoleColor.Red);
        }
    }

    private bool LoadGame()
    {
        if (!File.Exists("savegame.txt"))
        {
            UI.Typewrite("No save file found.", ConsoleColor.Red);
            return false;
        }

        try
        {
            using (StreamReader reader = new StreamReader("savegame.txt"))
            {
                string name = reader.ReadLine() ?? "Player";
                string className = reader.ReadLine() ?? "Warrior";
                int level = int.TryParse(reader.ReadLine(), out var tmpLevel) ? tmpLevel : 1;
                int xp = int.TryParse(reader.ReadLine(), out var tmpXp) ? tmpXp : 0;
                int health = int.TryParse(reader.ReadLine(), out var tmpHealth) ? tmpHealth : 100;
                int maxHealth = int.TryParse(reader.ReadLine(), out var tmpMaxHealth) ? tmpMaxHealth : health;
                int potions = int.TryParse(reader.ReadLine(), out var tmpPotions) ? tmpPotions : 0;
                int gold = int.TryParse(reader.ReadLine(), out var tmpGold) ? tmpGold : 0;
                int mana = int.TryParse(reader.ReadLine(), out var tmpMana) ? tmpMana : 0;
                int maxMana = int.TryParse(reader.ReadLine(), out var tmpMaxMana) ? tmpMaxMana : mana;
                string weaponName = reader.ReadLine() ?? "Fist";
                int weaponDmg = int.TryParse(reader.ReadLine(), out var tmpWeaponDmg) ? tmpWeaponDmg : 5;
                int weaponCrit = int.TryParse(reader.ReadLine(), out var tmpWeaponCrit) ? tmpWeaponCrit : 5;
                string armorName = reader.ReadLine() ?? "Cloth Tunic";
                int armorDef = int.TryParse(reader.ReadLine(), out var tmpArmorDef) ? tmpArmorDef : 1;
                currentWave = int.TryParse(reader.ReadLine(), out var tmpWave) ? tmpWave : 1;

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

                UI.Typewrite("Game Loaded!", ConsoleColor.Green);
                return true;
            }
        }
        catch (Exception ex)
        {
            UI.Typewrite($"Error loading game: {ex.Message}", ConsoleColor.Red);
            return false;
        }
    }

    private void InventoryMenu()
    {
        bool inInventory = true;
        while (inInventory)
        {
            UI.Typewrite("\n=== Inventory ===", ConsoleColor.White);
            
            if (hero.Potions == 0)
            {
                UI.Typewrite("Inventory empty");
                UI.Typewrite("1. Back", ConsoleColor.White);
                UI.Typewrite("Choose an option: ", ConsoleColor.White, 20, false);
                string choice = Console.ReadLine() ?? "";
                if (choice == "1") inInventory = false;
            }
            else
            {
                UI.Typewrite($"Potions: {hero.Potions}", ConsoleColor.White);
                UI.Typewrite("1. Use Potion | 2. Back", ConsoleColor.White);
                UI.Typewrite("Choose an option: ", ConsoleColor.White, 20, false);
                
                string choice = Console.ReadLine() ?? "";
                if (choice == "1")
                {
                    UI.Typewrite("Use potion? (y/n)", ConsoleColor.White);
                    UI.Typewrite("Choose an option: ", ConsoleColor.White, 20, false);
                    if ((Console.ReadLine() ?? "").ToLower() == "y") hero.UsePotion();
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
            UI.Typewrite($"\n*** Game Over! {hero.Name} was defeated! ***", ConsoleColor.Red);
        }
        else
        {
            UI.Typewrite("\nThanks for playing!", ConsoleColor.Cyan);
        }
        
        int wavesSurvived = hero.Health > 0 ? currentWave : currentWave - 1;
        
        UI.Typewrite($"Final Level: {hero.Level} | Waves Survived: {wavesSurvived}", ConsoleColor.White);
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
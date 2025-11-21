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
    public int maxHealth;
    public Weapon weapon;
    public int xp;
    public int level;
    public int potions;
    public bool isDefending;
    public bool isDodging;
    public bool isParrying;

    public player(string name, int health, Weapon weapon)
    {
        this.name = name;
        this.health = health;
        this.maxHealth = health;
        this.weapon = weapon;
        this.xp = 0;
        this.level = 1;
        this.potions = 3; // Start with 3 potions
        this.isDefending = false;
        this.isDodging = false;
        this.isParrying = false;
    }

    public void GainXP(int amount)
    {
        this.xp += amount;
        Console.WriteLine(this.name + " gained " + amount + " XP!");
        
        // Check for level up (need 100 XP per level)
        int xpNeeded = this.level * 100;
        if(this.xp >= xpNeeded)
        {
            LevelUp();
        }
    }

    public void LevelUp()
    {
        this.level++;
        this.xp = 0;
        this.weapon.damage += 5;
        this.maxHealth += 20;
        this.health = this.maxHealth; // Full heal on level up
        
        Console.WriteLine("\n*** LEVEL UP! ***");
        Console.WriteLine(this.name + " is now level " + this.level + "!");
        Console.WriteLine("Health fully restored to " + this.maxHealth + "!");
        Console.WriteLine("Weapon damage increased to " + this.weapon.damage + "!");
    }

    public void UsePotion()
    {
        if(this.potions > 0)
        {
            int healAmount = Math.Min(50, this.maxHealth - this.health);
            this.health += healAmount;
            this.potions--;
            Console.WriteLine(this.name + " used a potion and restored " + healAmount + " HP!");
            Console.WriteLine("Current health: " + this.health + "/" + this.maxHealth);
            Console.WriteLine("Potions remaining: " + this.potions);
        }
        else
        {
            Console.WriteLine("No potions left!");
        }
    }

    public void ResetCombatStates()
    {
        this.isDefending = false;
        this.isDodging = false;
        this.isParrying = false;
    }
}

class Warrior : player
{
    public Warrior(string name, Random rand) : base(name, 100, null)
    {
        // Warrior uses Sword
        this.weapon = new Weapon("Sword", rand.Next(10, 25));
        this.maxHealth = 100;
    }
}

class Mage : player
{
    public Mage(string name, Random rand) : base(name, 70, null)
    {
        // Mage uses Staff
        this.weapon = new Weapon("Staff", rand.Next(12, 30));
        this.maxHealth = 70;
    }
}

class Ranger : player
{
    public Ranger(string name, Random rand) : base(name, 80, null)
    {
        // Ranger uses Bow
        this.weapon = new Weapon("Bow", rand.Next(8, 20));
        this.maxHealth = 80;
    }
}

class Berserker : player
{
    public Berserker(string name, Random rand) : base(name, 120, null)
    {
        // Berserker uses Axe
        this.weapon = new Weapon("Axe", rand.Next(15, 35));
        this.maxHealth = 120;
    }
}

class Assassin : player
{
    public Assassin(string name, Random rand) : base(name, 90, null)
    {
        // Assassin uses Dagger
        this.weapon = new Weapon("Dagger", rand.Next(7, 15));
        this.maxHealth = 90;
    }
}

class MartialArtist : player
{
    public MartialArtist(string name, Random rand) : base(name, 90, null)
    {
        // Martial Artist uses Fist
        this.weapon = new Weapon("Fist", 20);
        this.maxHealth = 90;
    }
}

class enemies
{
    public string type;
    public int health;
    public Weapon weapon;
    public int xpReward;
    public bool isDefending;
    public bool isDodging;
    public bool isParrying;

    public enemies(string type, int health, Weapon weapon, int xpReward)
    {
        this.type = type;
        this.health = health;
        this.weapon = weapon;
        this.xpReward = xpReward;
        this.isDefending = false;
        this.isDodging = false;
        this.isParrying = false;
    }

    public void ResetCombatStates()
    {
        this.isDefending = false;
        this.isDodging = false;
        this.isParrying = false;
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

        Console.WriteLine("\n" + hero.name + " Stats:");
        Console.WriteLine("Level: " + hero.level);
        Console.WriteLine("Health: " + hero.health + "/" + hero.maxHealth);
        Console.WriteLine("Weapon: " + hero.weapon.name + " (Damage: " + hero.weapon.damage + ")");
        Console.WriteLine("Potions: " + hero.potions);

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

        // Enemy XP rewards by type
        Dictionary<string, int> enemyXPRewards = new Dictionary<string, int>
        {
            { "Orc", 50 },
            { "Elf", 30 },
            { "Goblin", 25 },
            { "Human", 40 },
            { "Dwarf", 35 }
        };

        // Wave system
        int currentWave = 1;
        bool continueGame = true;

        while(continueGame && hero.health > 0)
        {
            Console.WriteLine("\n========== WAVE " + currentWave + " ===========");
            int enemiesLeft = rand.Next(2, 5);
            
            Console.WriteLine(enemiesLeft + " enemies incoming!");
            for(int i = 0; i < enemiesLeft; i++)
            {
                Console.WriteLine("Enemy " + (i + 1) + " appeared!");
            }

            // Combat system for current wave
            while(enemiesLeft > 0 && hero.health > 0)
            {
                // Generate random enemy
                string randomEnemyType = enemyTypes[rand.Next(enemyTypes.Length)];
                Weapon randomEnemyWeapon = Weapon.GenerateRandomWeapon(rand);
                
                // Scale enemy stats based on wave number
                int scaledHealth = enemyHealthByType[randomEnemyType] + ((currentWave - 1) * 10); // +10 HP per wave
                int scaledWeaponDamage = randomEnemyWeapon.damage + ((currentWave - 1) * 3); // +3 damage per wave
                int scaledXPReward = enemyXPRewards[randomEnemyType] + ((currentWave - 1) * 15); // +15 XP per wave
                
                // Update weapon damage
                randomEnemyWeapon.damage = scaledWeaponDamage;
                
                enemies currentEnemy = new enemies(
                    randomEnemyType,
                    scaledHealth,
                    randomEnemyWeapon,
                    scaledXPReward
                );
            
            Console.WriteLine("\nEnemies left: " + enemiesLeft);
            Console.WriteLine("A " + currentEnemy.type + " appeared with " + currentEnemy.weapon.name + " (Damage: " + currentEnemy.weapon.damage + ")!");
            Console.WriteLine(currentEnemy.type + " health: " + currentEnemy.health);
            Console.WriteLine(hero.name + " health: " + hero.health + "/" + hero.maxHealth);
            
            // Turn-based combat loop
            while(currentEnemy.health > 0 && hero.health > 0)
            {
                // Reset combat states at the start of each turn
                hero.ResetCombatStates();
                currentEnemy.ResetCombatStates();
                
                Console.WriteLine("\n--- Your Turn ---");
                Console.WriteLine(hero.name + " HP: " + hero.health + "/" + hero.maxHealth + " | " + currentEnemy.type + " HP: " + currentEnemy.health);
                Console.WriteLine("\nChoose your action:");
                Console.WriteLine("1. Attack");
                Console.WriteLine("2. Parry (Counter enemy attack)");
                Console.WriteLine("3. Dodge (Avoid enemy attack)");
                Console.WriteLine("4. Defend (Reduce damage by 50%)");
                
                if(hero.potions > 0)
                {
                    Console.WriteLine("5. Use Potion (Ends turn)");
                    Console.WriteLine("6. End Turn (Do nothing)");
                }
                else
                {
                    Console.WriteLine("5. End Turn (Do nothing)");
                }
                
                string playerAction = Console.ReadLine();
                bool playerTurnComplete = false;
                
                switch(playerAction)
                {
                    case "1": // Attack
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
                        Console.WriteLine("\n" + attackMessage);
                        currentEnemy.health -= hero.weapon.damage;
                        Console.WriteLine("Deals " + hero.weapon.damage + " damage!");
                        playerTurnComplete = true;
                        break;
                        
                    case "2": // Parry
                        Console.WriteLine("\n" + hero.name + " prepares to parry!");
                        hero.isParrying = true;
                        playerTurnComplete = true;
                        break;
                        
                    case "3": // Dodge
                        Console.WriteLine("\n" + hero.name + " prepares to dodge!");
                        hero.isDodging = true;
                        playerTurnComplete = true;
                        break;
                        
                    case "4": // Defend
                        Console.WriteLine("\n" + hero.name + " takes a defensive stance!");
                        hero.isDefending = true;
                        playerTurnComplete = true;
                        break;
                        
                    case "5":
                        if(hero.potions > 0)
                        {
                            // Use Potion
                            hero.UsePotion();
                            playerTurnComplete = true;
                        }
                        else
                        {
                            // End Turn
                            Console.WriteLine("\n" + hero.name + " does nothing in this turn.");
                            playerTurnComplete = true;
                        }
                        break;
                        
                    case "6": // End Turn (only when potions available)
                        if(hero.potions > 0)
                        {
                            Console.WriteLine("\n" + hero.name + " does nothing in this turn.");
                            playerTurnComplete = true;
                        }
                        else
                        {
                            Console.WriteLine("Invalid action! Turn skipped.");
                            playerTurnComplete = true;
                        }
                        break;
                        
                    default:
                        Console.WriteLine("Invalid action! Turn skipped.");
                        playerTurnComplete = true;
                        break;
                }
                
                if(currentEnemy.health <= 0)
                {
                    Console.WriteLine("\n" + currentEnemy.GetDeathMessage());
                    hero.GainXP(currentEnemy.xpReward);
                    
                    // Enemy potion drop (1:20 ratio = 5% chance)
                    int dropChance = rand.Next(1, 21);
                    if(dropChance == 1)
                    {
                        hero.potions++;
                        Console.WriteLine(currentEnemy.type + " dropped a potion!");
                        Console.WriteLine("Potions: " + hero.potions);
                    }
                    break;
                }
                
                // Enemy turn
                if(playerTurnComplete && hero.health > 0)
                {
                    Console.WriteLine("\n--- Enemy Turn ---");
                    System.Threading.Thread.Sleep(500);
                    
                    // Enemy AI - weighted random action (70% attack, 10% each for other actions)
                    int actionRoll = rand.Next(1, 101);
                    int enemyAction;
                    if(actionRoll <= 70)
                    {
                        enemyAction = 1; // Attack
                    }
                    else if(actionRoll <= 80)
                    {
                        enemyAction = 2; // Parry
                    }
                    else if(actionRoll <= 90)
                    {
                        enemyAction = 3; // Dodge
                    }
                    else
                    {
                        enemyAction = 4; // Defend
                    }
                    
                    switch(enemyAction)
                    {
                        case 1: // Enemy attacks
                            Console.WriteLine(currentEnemy.type + " attacks with " + currentEnemy.weapon.name + "!");
                            
                            if(hero.isDodging)
                            {
                                // Dodge attempt
                                int dodgeChance = rand.Next(1, 101);
                                if(dodgeChance <= 60) // 60% dodge success
                                {
                                    Console.WriteLine(hero.name + " dodged the attack!");
                                }
                                else
                                {
                                    hero.health -= currentEnemy.weapon.damage;
                                    Console.WriteLine("Dodge failed! " + hero.name + " takes " + currentEnemy.weapon.damage + " damage!");
                                }
                            }
                            else if(hero.isParrying)
                            {
                                // Parry - counter attack
                                Console.WriteLine(hero.name + " parried the attack and counters!");
                                int counterDamage = hero.weapon.damage / 2;
                                currentEnemy.health -= counterDamage;
                                Console.WriteLine(hero.name + " deals " + counterDamage + " counter damage!");
                            }
                            else if(hero.isDefending)
                            {
                                // Defend - reduced damage
                                int reducedDamage = currentEnemy.weapon.damage / 2;
                                hero.health -= reducedDamage;
                                Console.WriteLine(hero.name + " blocked! Only takes " + reducedDamage + " damage!");
                            }
                            else
                            {
                                // Normal attack
                                hero.health -= currentEnemy.weapon.damage;
                                Console.WriteLine(hero.name + " takes " + currentEnemy.weapon.damage + " damage!");
                            }
                            break;
                            
                        case 2: // Enemy parries
                            Console.WriteLine(currentEnemy.type + " prepares to parry!");
                            currentEnemy.isParrying = true;
                            break;
                            
                        case 3: // Enemy dodges
                            Console.WriteLine(currentEnemy.type + " prepares to dodge!");
                            currentEnemy.isDodging = true;
                            break;
                            
                        case 4: // Enemy defends
                            Console.WriteLine(currentEnemy.type + " takes a defensive stance!");
                            currentEnemy.isDefending = true;
                            break;
                    }
                    
                    if(hero.health > 0)
                    {
                        Console.WriteLine(hero.name + " HP: " + hero.health + "/" + hero.maxHealth);
                    }
                    else
                    {
                        Console.WriteLine("\n" + hero.name + " has been defeated!");
                    }
                }
                
                System.Threading.Thread.Sleep(800);
            }
            
            // Only decrement enemies if enemy was defeated
            if(currentEnemy.health <= 0)
            {
                enemiesLeft--;
            }
        }
        
        // Wave completed
        if(hero.health > 0)
        {
            Console.WriteLine("\n*** Wave " + currentWave + " Complete! ***");
            Console.WriteLine(hero.name + " has " + hero.health + "/" + hero.maxHealth + " health remaining.");
            Console.WriteLine("Level: " + hero.level + " | XP: " + hero.xp + "/" + (hero.level * 100));
            Console.WriteLine("Potions: " + hero.potions);
            
            // Wave pause menu loop
            bool menuActive = true;
            while(menuActive)
            {
                Console.WriteLine("\nWhat would you like to do?");
                Console.WriteLine("1. Continue to next wave");
                Console.WriteLine("2. Check inventory");
                Console.WriteLine("3. End game");
                
                string choice = Console.ReadLine();
                
                switch(choice)
                {
                    case "1":
                        // Check if health is low
                        if(hero.health < 50)
                        {
                            Console.WriteLine("\nWARNING: Your health is low (" + hero.health + "/" + hero.maxHealth + ")!");
                            Console.WriteLine("Are you sure you want to continue? (yes/no)");
                            string confirm = Console.ReadLine();
                            if(confirm.ToLower() == "yes" || confirm.ToLower() == "y")
                            {
                                currentWave++;
                                menuActive = false;
                            }
                        }
                        else
                        {
                            currentWave++;
                            menuActive = false;
                        }
                        break;
                    case "2":
                        Console.WriteLine("\n=== Inventory ===");
                        if(hero.potions > 0)
                        {
                            Console.WriteLine("1. Use potion (Potions: " + hero.potions + ") (Heals 50 HP)");
                            Console.WriteLine("2. Back to menu");
                            
                            string inventoryChoice = Console.ReadLine();
                            if(inventoryChoice == "1")
                            {
                                Console.WriteLine("\nAre you sure you want to use a potion? (yes/no)");
                                string confirm = Console.ReadLine();
                                if(confirm.ToLower() == "yes" || confirm.ToLower() == "y")
                                {
                                    hero.UsePotion();
                                }
                                else
                                {
                                    Console.WriteLine("Potion not used.");
                                }
                            }
                        }
                        else
                        {
                            Console.WriteLine("Potions: 0");
                            Console.WriteLine("1. Back to menu");
                            Console.ReadLine();
                        }
                        // Loop back to menu
                        break;
                    case "3":
                        continueGame = false;
                        menuActive = false;
                        Console.WriteLine("\nThanks for playing!");
                        break;
                    default:
                        Console.WriteLine("Invalid choice. Please try again.");
                        break;
                }
            }
        }
    }
        
        if(hero.health <= 0)
        {
            Console.WriteLine("\n*** Game Over! " + hero.name + " was defeated! ***");
            Console.WriteLine("You survived " + (currentWave - 1) + " waves.");
            Console.WriteLine("Final Level: " + hero.level);
        }
    }
}
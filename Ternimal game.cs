using System;
using System.Collections.Generic;

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

    static void GreetPlayers(string player1, string player2)
    {
        Console.WriteLine("Welcome " + player1 + " and " + player2 + " to the game!");
    }

    static void Main()
    {
        // Example of variable declarations
        int player1health = 100;
        int player2health = 100;
        float speed = 5.5f;
        bool alive = true;
        double score = 999.99;

        // Example of user input and output
        Console.WriteLine("Enter player 1 name: ");
        string player1Name = Console.ReadLine();
        Console.WriteLine("Enter player 2 name: ");
        string player2Name = Console.ReadLine();
        
        // Using method from to greet players
        GreetPlayers(player1Name, player2Name);

        Console.WriteLine(player1Name + " health: " + player1health);
        Console.WriteLine(player2Name + " health: " + player2health);

        // Example of conditional statements

        if(player1health == 100)
        {
            Console.WriteLine(player1Name + " is at full health!");
        }
        else if (player1health > 50)
        {
            Console.WriteLine(player1Name + " is in good health!");
        }
        else
        {
            Console.WriteLine(player1Name + " has low health!");
        }

        if(player2health == 100)
        {
            Console.WriteLine(player2Name + " is at full health!");
        }
        else if (player2health > 50)
        {
            Console.WriteLine(player2Name + " is in good health!");
        }
        else
        {
            Console.WriteLine(player2Name + " has low health!");
        }

        // List of available weapons with their damages
        Dictionary<string, int> weaponDamages = new Dictionary<string, int>
        {
            { "Sword", 10 },
            { "Axe", 15 },
            { "Bow", 8 },
            { "Staff", 12 },
            { "Fist", 5 }
        };
        
        Console.WriteLine("\nWeapons available:");
        foreach (var weapon in weaponDamages)
        {
            Console.WriteLine("- " + weapon.Key + " (Damage: " + weapon.Value + ")");
        }

        // Example of a switch statement with list validation
        Console.WriteLine("\nChoose your weapon from the list above:");
        string player1weapon = Console.ReadLine();
        
        // Check if weapon exists in the dictionary
        bool weaponFound = false;
        foreach (var weapon in weaponDamages)
        {
            if (weapon.Key.ToLower() == player1weapon.ToLower())
            {
                weaponFound = true;
                player1weapon = weapon.Key; // Set to proper case
                break;
            }
        }

        if (weaponFound)
        {
            switch(player1weapon.ToLower())
            {
                case "sword":
                    Console.WriteLine("You have chosen a Sword! A balanced weapon for close combat.");
                    break;
                case "axe":
                    Console.WriteLine("You have chosen an Axe! Heavy damage but slower.");
                    break;
                case "bow":
                    Console.WriteLine("You have chosen a Bow! Perfect for ranged attacks.");
                    break;
                case "staff":
                    Console.WriteLine("You have chosen a Staff! Great for magic users.");
                    break;
                case "fist":
                    Console.WriteLine("You have chosen your Fists! Fighting bare-handed!");
                    break;
            }
        }
        else
        {
            Console.WriteLine("Invalid weapon! Defaulting to Fist.");
            player1weapon = "Fist";
        }

        // Get weapon damage
        int weaponDamage = weaponDamages[player1weapon];
        Console.WriteLine("\nYour weapon deals " + weaponDamage + " damage per attack.");

        // for loop example
        Console.WriteLine("\n--- Battle Start ---");
        int enemiesLeft = 2;
        for(int i = 0; i < enemiesLeft; i++)
        {
            Console.WriteLine("Enemy " + (i + 1) + " appeared!");
        }

        // while loop example - combat system
        while(enemiesLeft > 0 && player1health > 0)
        {
            // Each enemy has random health between 20-50
            Random rand = new Random();
            int currentEnemyHealth = rand.Next(20, 50);
            Console.WriteLine("\nEnemies left: " + enemiesLeft);
            Console.WriteLine("Enemy " + enemiesLeft + " has " + currentEnemyHealth + " health.");
            Console.WriteLine(player1Name + " health: " + player1health);
            
            // Fight until enemy is dead
            while(currentEnemyHealth > 0 && player1health > 0)
            {
                // Player attacks
                currentEnemyHealth -= weaponDamage;
                Console.WriteLine(player1Name + " attacks with " + player1weapon + " for " + weaponDamage + " damage!");
                
                if(currentEnemyHealth > 0)
                {
                    Console.WriteLine("Enemy " + enemiesLeft + " has " + currentEnemyHealth + " health left.");
                    
                    // Enemy attacks back
                    int enemyDamage = rand.Next(5, 16);
                    player1health -= enemyDamage;
                    Console.WriteLine("Enemy " + enemiesLeft + " attacks for " + enemyDamage + " damage!");
                    
                    if(player1health > 0)
                    {
                        Console.WriteLine(player1Name + " health: " + player1health);
                    }
                    else
                    {
                        Console.WriteLine(player1Name + " has been defeated!");
                        break;
                    }
                }
                else
                {
                    Console.WriteLine("Enemy " + enemiesLeft + " has been defeated by " + player1Name + " with " + player1weapon + "!");
                }
                
                System.Threading.Thread.Sleep(500); // Pause for readability
            }
            
            // Only decrement enemies if enemy was defeated
            if(currentEnemyHealth <= 0)
            {
                enemiesLeft--;
            }
        }
        
        if(player1health > 0)
        {
            Console.WriteLine("\n*** Victory! All enemies defeated! ***");
            Console.WriteLine(player1Name + " has " + player1health + " health remaining.");
        }
        else
        {
            Console.WriteLine("\n*** Game Over! " + player1Name + " was defeated! ***");
        }

        // foreach loop example
        string[] items = { "potion", "elixir", "bomb" };
        Console.WriteLine("\n--- Items Collected ---");
        foreach(string item in items)
        {
            Console.WriteLine("Item: " + item);
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
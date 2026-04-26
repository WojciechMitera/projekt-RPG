using Godot;
using System;

/**
 * @class WaveManager
 * @brief Controls enemy wave spawning and game progression.
 * 
 * This class manages waves of enemies, including normal enemy waves,
 * boss waves, wave progression, and game completion logic.
 */
public partial class WaveManager : Node2D
{
    /**
	 * @brief Packed scene for enemy type 1.
	 */
    private PackedScene enemy1Scene = GD.Load<PackedScene>("res://enemyrespawn.tscn");

    /**
	 * @brief Packed scene for enemy type 2.
	 */
    private PackedScene enemy2Scene = GD.Load<PackedScene>("res://enemy2respawn.tscn");

    /**
	 * @brief Packed scene for enemy type 3.
	 */
    private PackedScene enemy3Scene = GD.Load<PackedScene>("res://enemy3respawn.tscn");

    /**
	 * @brief Packed scene for boss enemy.
	 */
    private PackedScene bossScene = GD.Load<PackedScene>("res://boss.tscn");

    /**
	 * @brief Reference to the player character.
	 */
    private CharacterBody2D player;

    /**
	 * @brief UI label displaying current wave number.
	 */
    private Label waveLabel;

    /**
	 * @brief Current wave index.
	 */
    public int currentWave = 1;

    /**
	 * @brief Number of enemies spawned per wave.
	 */
    public int enemiesToSpawn = 3;

    /**
	 * @brief Number of enemies currently alive.
	 */
    private int enemiesAlive = 0;

    /**
	 * @brief Interval of waves that spawn a boss.
	 */
    public int bossWaveInterval = 5;

    /**
	 * @brief Called when the node enters the scene tree.
	 * 
	 * Initializes references and starts the first wave.
	 */
    public override void _Ready()
    {
        player = GetNode<CharacterBody2D>("../player");
        waveLabel = GetNode<Label>("../../CanvasLayer/WaveLabel");
        StartWave();
    }

    /**
	 * @brief Starts the current wave.
	 * 
	 * Spawns normal enemies or a boss depending on wave number.
	 * Updates UI and enemy counters accordingly.
	 */
    public void StartWave()
    {
        GD.Print("Wave: " + currentWave);

        if (waveLabel != null)
        {
            waveLabel.Text = "Fala: " + currentWave;
        }

        if (currentWave % bossWaveInterval == 0)
        {
            SpawnBoss();
            enemiesAlive = 1;
            return;
        }

        enemiesAlive = enemiesToSpawn;

        for (int i = 0; i < enemiesToSpawn; i++)
        {
            SpawnEnemy();
        }
    }

    /**
	 * @brief Spawns a normal enemy.
	 * 
	 * Randomly selects enemy type and assigns player and wave manager references.
	 */
    public void SpawnEnemy()
    {
        PackedScene chosenScene = GetRandomEnemy();
        Node enemyNode = chosenScene.Instantiate();

        if (enemyNode is CharacterBody2D enemy)
        {
            enemy.GlobalPosition = GetRandomPosition();

            if (enemy is Enemy e1)
            {
                e1.player = player;
                e1.waveManager = this;
            }
            else if (enemy is Enemy2 e2)
            {
                e2.player = player;
                e2.waveManager = this;
            }
            else if (enemy is Enemy3 e3)
            {
                e3.player = player;
                e3.waveManager = this;
            }

            AddChild(enemy);
        }
    }

    /**
	 * @brief Spawns a boss enemy.
	 * 
	 * Assigns player and wave manager references to the boss.
	 */
    public void SpawnBoss()
    {
        var bossNode = bossScene.Instantiate();

        if (bossNode is CharacterBody2D boss)
        {
            boss.GlobalPosition = GetRandomPosition();

            if (boss is Boss b)
            {
                b.player = player;
                b.waveManager = this;
            }

            AddChild(boss);
        }
    }

    /**
	 * @brief Returns a randomly selected enemy scene.
	 * 
	 * Uses probability-based selection to choose enemy type.
	 * 
	 * @return PackedScene representing selected enemy type.
	 */
    public PackedScene GetRandomEnemy()
    {
        float rand = GD.Randf();

        if (rand < 0.5f)
            return enemy1Scene;
        else if (rand < 0.8f)
            return enemy2Scene;
        else
            return enemy3Scene;
    }

    /**
	 * @brief Called when an enemy dies.
	 * 
	 * Decreases alive enemy count and starts next wave if cleared.
	 */
    public void OnEnemyDied()
    {
        enemiesAlive--;

        if (enemiesAlive <= 0)
        {
            NextWave();
        }
    }

    /**
	 * @brief Starts the next wave after a short delay.
	 * 
	 * Increases wave difficulty and spawns new enemies.
	 */
    public async void NextWave()
    {
        await ToSignal(GetTree().CreateTimer(1.5f), "timeout");

        currentWave++;
        enemiesToSpawn += 1;

        StartWave();
    }

    /**
	 * @brief Ends the game with a win state.
	 * 
	 * Switches to the end/game over scene.
	 */
    public void GameWon()
    {
        GetTree().CallDeferred("change_scene_to_file", "res://pauza.tscn");
    }

    /**
	 * @brief Generates a random spawn position.
	 * 
	 * @return Random 2D position within predefined bounds.
	 */
    public Vector2 GetRandomPosition()
    {
        float x = GD.RandRange(175, 975);
        float y = GD.RandRange(190, 500);
        return new Vector2(x, y);
    }
}
using Godot;
using System;

public partial class WaveManager : Node2D
{
	private PackedScene enemy1Scene = GD.Load<PackedScene>("res://enemyrespawn.tscn");
	private PackedScene enemy2Scene = GD.Load<PackedScene>("res://enemy2respawn.tscn");
	private PackedScene enemy3Scene = GD.Load<PackedScene>("res://enemy3respawn.tscn");

	private CharacterBody2D player;

	public int currentWave = 1;
	public int enemiesToSpawn = 5;
	private int enemiesAlive = 0;

	public override void _Ready()
	{
		player = GetNode<CharacterBody2D>("../player");
		StartWave();
	}

	public void StartWave()
	{
		GD.Print("Wave: " + currentWave);

		enemiesAlive = enemiesToSpawn;

		for (int i = 0; i < enemiesToSpawn; i++)
		{
			SpawnEnemy();
		}
	}

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

	public void OnEnemyDied()
	{
		enemiesAlive--;

		if (enemiesAlive < 1)
		{
			NextWave();
		}
	}

	public async void NextWave()
	{
		await ToSignal(GetTree().CreateTimer(1f), "timeout");

		currentWave++;
		enemiesToSpawn += 1;	

		StartWave();
	}

	public Vector2 GetRandomPosition()
	{
		float x = GD.RandRange(175, 975);
		float y = GD.RandRange(190, 500);

		return new Vector2(x, y);
	}
}

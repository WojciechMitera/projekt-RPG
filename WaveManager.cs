using Godot;
using System;

public partial class WaveManager : Node2D
{
	private PackedScene enemy1Scene = GD.Load<PackedScene>("res://enemyrespawn.tscn");
	private PackedScene enemy2Scene = GD.Load<PackedScene>("res://enemy2respawn.tscn");
	private PackedScene enemy3Scene = GD.Load<PackedScene>("res://enemy3respawn.tscn");
	private PackedScene bossScene = GD.Load<PackedScene>("res://boss.tscn");

	private CharacterBody2D player;
	private Label waveLabel;
	public int currentWave = 1;
	public int enemiesToSpawn = 5;
	private int enemiesAlive = 0;

	public int bossWaveInterval = 5;

	public override void _Ready()
	{
		player = GetNode<CharacterBody2D>("../player");
		waveLabel = GetNode<Label>("../../CanvasLayer/WaveLabel");
		StartWave();
	}

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

		GD.Print("BOSS SPAWNED!");
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

		if (enemiesAlive <= 0)
		{
			NextWave();
		}
	}

	public async void NextWave()
	{
		await ToSignal(GetTree().CreateTimer(1.5f), "timeout");

		currentWave++;
		enemiesToSpawn += 1;

		StartWave();
	}

	public void GameWon()
	{
		GD.Print("YOU WIN!");
		GetTree().CallDeferred("change_scene_to_file", "res://win.tscn");
	}

	public Vector2 GetRandomPosition()
	{
		float x = GD.RandRange(175, 975);
		float y = GD.RandRange(190, 500);
		return new Vector2(x, y);
	}
}

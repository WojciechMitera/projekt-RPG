using Godot;
using System;
using System.Drawing;
using static Godot.TextServer;

/**
 * @class Enemy
 * @brief Represents an enemy character in the game.
 * 
 * This class handles enemy behavior such as taking damage,
 * tracking health, and dealing damage to the player upon contact.
 */
public partial class Enemy3 : CharacterBody2D
{
	/**
	 * @brief Damage dealt to the player on contact.
	 */
	public int _damage = 5;

	/**
	 * @brief Maximum health of the enemy.
	 */
	public int max_health = 30;

	/**
	 * @brief Current health of the enemy.
	 */
	private int health;

	/**
	 * @brief Indicates whether the enemy is in contact with the player.
	 */
	public bool contact = false;

	/**
	 * @brief Time interval between damage ticks while in contact.
	 */
	public float damagetime = 2f;

	public float respawntime = 1f;

	Marker2D point;

	PackedScene projectileenemytscn = GD.Load<PackedScene>("res://projectileenemy.tscn");

	PackedScene enemy3scene;
	Vector2 spawnposition;
	public const float Speed = -30.0f;
	private CharacterBody2D player;
	//public float mindist = 65f;

	/**
	 * @brief Initializes the enemy.
	 * 
	 * Sets the current health to the maximum health value.
	 */
	public override void _Ready()
	{
		health = max_health;
		enemy3scene = GD.Load<PackedScene>("res://enemy3respawn.tscn");
		spawnposition = GetRandomPosition();
		player = GetNode<CharacterBody2D>("../player");
		point = GetNode<Marker2D>("point");
		CallDeferred(nameof(Shoot));

	}

	public override void _PhysicsProcess(double delta)
	{
		if (player == null)
		{
			return;
		}
		Vector2 Direction = (player.GlobalPosition - GlobalPosition).Normalized();
		Velocity = Direction * Speed;
		//float distance = Direction.Length();

		/*if (distance > mindist)
		{
			Direction = Direction.Normalized();
			Velocity = Direction * Speed;
		}
		else
		{
			Velocity = Vector2.Zero;
			//Velocity = (GlobalPosition - player.GlobalPosition).Normalized() * Speed * 0.5f;
		}*/
		MoveAndSlide();
	}

	/**
	 * @brief Applies damage to the enemy.
	 * 
	 * Reduces the enemy's health by the given amount.
	 * If health reaches zero, the enemy is removed from the scene.
	 * 
	 * @param damage Amount of damage to apply.
	 */
	public void Damage(int damage)
	{
		health -= damage;

		if (health < 0)
		{
			health = 0;
			
		}
		else if(health == 0)
		{

			CallDeferred(nameof(SpawnEnemy3));
			//SpawnEnemy();
			QueueFree();
		} 
		GD.Print(health);
	}

	/**
	 * @brief Handles collision with another body.
	 * 
	 * If the colliding object is a player, the enemy starts
	 * dealing damage over time until the contact ends.
	 * 
	 * @param body The colliding node.
	 */
	private async void BodyCollision(Node body)
	{
		if (body is Player player)
		{
			contact = true;

			while (contact)
			{
				await ToSignal(GetTree().CreateTimer(damagetime), "timeout");

				if (contact == true)
					player.Damage(_damage);
			}
		}
	}

	/**
	 * @brief Handles when a body exits collision.
	 * 
	 * Stops dealing damage when the player is no longer in contact.
	 * 
	 * @param body The node leaving the collision.
	 */
	private void BodyCollisionOut(Node body)
	{
		if (body is Player player)
		{
			contact = false;
		}
	}
	public void SpawnEnemy3()
	{
		//await ToSignal(GetTree().CreateTimer(respawntime), "timeout");
		var enemy3 = enemy3scene.Instantiate<CharacterBody2D>();
		enemy3.GlobalPosition = spawnposition;
		GetParent().AddChild(enemy3);
		GD.Print("1");


	}

	public async void Shoot()
	{
		while (true)
		{
			await ToSignal(GetTree().CreateTimer(damagetime), "timeout");

			if (player == null)
			{
				return;
			}

			Projectileenemy projectile = projectileenemytscn.Instantiate<Projectileenemy>();
			projectile.GlobalPosition = point.GlobalPosition;

			Vector2 direction = (player.GlobalPosition - point.GlobalPosition).Normalized();
			projectile.direction = direction;

			GetParent().AddChild(projectile);

			GD.Print("strzal");
		}
	}

	public Vector2 GetRandomPosition()
	{
		float x = GD.RandRange(175, 975);
		float y = GD.RandRange(190, 500);

		return new Vector2(x, y);
	}

}

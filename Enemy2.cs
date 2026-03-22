using Godot;
using System;

/**
 * @class Enemy
 * @brief Represents an enemy character in the game.
 * 
 * This class handles enemy behavior such as taking damage,
 * tracking health, and dealing damage to the player upon contact.
 */
public partial class Enemy2 : CharacterBody2D
{
	/**
	 * @brief Damage dealt to the player on contact.
	 */
	public int _damage = 25;

	/**
	 * @brief Maximum health of the enemy.
	 */
	public int max_health = 100;

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
	public float damagetime = 1.4f;

	public float respawntime = 1f;

	PackedScene enemy2scene;
	Vector2 spawnposition;
	public const float Speed = 50.0f;
	private CharacterBody2D player;

	/**
	 * @brief Initializes the enemy.
	 * 
	 * Sets the current health to the maximum health value.
	 */
	public override void _Ready()
	{
		health = max_health;
		enemy2scene = GD.Load<PackedScene>("res://enemy2respawn.tscn");
		spawnposition = GetRandomPosition();
		player = GetNode<CharacterBody2D>("../player");
		
	}

	public override void _PhysicsProcess(double delta)
	{
		if(player == null)
		{
			return;
		}
		Vector2 Direction = (player.GlobalPosition - GlobalPosition).Normalized();
		Velocity = Direction * Speed;
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

			CallDeferred(nameof(SpawnEnemy2));
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
	public void SpawnEnemy2()
	{
		//await ToSignal(GetTree().CreateTimer(respawntime), "timeout");
		var enemy2 = enemy2scene.Instantiate<CharacterBody2D>();
		enemy2.GlobalPosition = spawnposition;
		GetParent().AddChild(enemy2);
		GD.Print("1");


	}
	public Vector2 GetRandomPosition()
	{
		float x = GD.RandRange(175, 975);
		float y = GD.RandRange(190, 500);

		return new Vector2(x, y);
	}

}

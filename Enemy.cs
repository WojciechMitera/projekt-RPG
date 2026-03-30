using Godot;
using System;
using static Godot.TextServer;

/**
 * @class Enemy
 * @brief Represents an enemy character in the game.
 * 
 * This class handles enemy behavior such as movement,
 * taking damage, respawning, and dealing damage to the player upon contact.
 */
public partial class Enemy : CharacterBody2D
{
	/**
	 * @brief Damage dealt to the player on contact.
	 */
	public int _damage = 10;

	/**
	 * @brief Maximum health of the enemy.
	 */
	public int max_health = 50;

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
	public float damagetime = 1f;

	/**
	 * @brief Time before the enemy respawns.
	 */
	public float respawntime = 1f;

	/**
	 * @brief Packed scene used to respawn the enemy.
	 */
	PackedScene enemyscene;

	/**
	 * @brief Position where the enemy will respawn.
	 */
	Vector2 spawnposition;

	/**
	 * @brief Movement speed of the enemy.
	 */
	public const float Speed = 80.0f;

	/**
	 * @brief Reference to the player object.
	 */
	private CharacterBody2D player;

	/**
	 * @brief Initializes the enemy.
	 * 
	 * Sets health, loads the respawn scene, assigns spawn position,
	 * and retrieves the player reference.
	 * 
	 * @note Make sure the player node exists at "../player".
	 */
	public override void _Ready()
	{
		health = max_health;
		enemyscene = GD.Load<PackedScene>("res://enemyrespawn.tscn");
		spawnposition = GetRandomPosition();
		player = GetNode<CharacterBody2D>("../player");
	}

	/**
	 * @brief Handles physics updates.
	 * 
	 * Moves the enemy towards the player using normalized direction
	 * and constant speed.
	 * 
	 * @param delta Time elapsed since last frame.
	 */
	public override void _PhysicsProcess(double delta)
	{
		if (player == null)
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
	 * Reduces health and triggers respawn when health reaches zero.
	 * The enemy is removed from the scene after spawning a new instance.
	 * 
	 * @param damage Amount of damage to apply.
	 */
	public void Damage(int damage)
	{
		health -= damage;

		if (health <= 0)
		{
			CallDeferred(nameof(SpawnEnemy));
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

	/**
	 * @brief Spawns a new enemy instance.
	 * 
	 * Instantiates a new enemy from the packed scene and places it
	 * at a predefined spawn position.
	 * 
	 * @note The scene "res://enemyrespawn.tscn" must exist.
	 */
	public void SpawnEnemy()
	{
		var enemy = enemyscene.Instantiate<CharacterBody2D>();
		enemy.GlobalPosition = spawnposition;
		GetParent().AddChild(enemy);

		GD.Print("Enemy respawned");
	}

	/**
	 * @brief Generates a random position within predefined bounds.
	 * 
	 * @return A Vector2 representing a random position.
	 */
	public Vector2 GetRandomPosition()
	{
		float x = GD.RandRange(175, 975);
		float y = GD.RandRange(190, 500);

		return new Vector2(x, y);
	}
}

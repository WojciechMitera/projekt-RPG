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
	 * @brief Amount of damage dealt to the player on contact.
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
	 * @brief Indicates whether the enemy is currently in contact with the player.
	 */
	public bool contact = false;

	/**
	 * @brief Time interval (in seconds) between consecutive damage ticks.
	 */
	public float damagetime = 1f;

	/**
	 * @brief Movement speed of the enemy.
	 */
	public const float Speed = 40.0f;

	/**
	 * @brief Reference to the player character.
	 */
	public CharacterBody2D player;

	/**
	 * @brief Reference to the wave manager controlling enemy waves.
	 */
	public WaveManager waveManager;

	/**
	 * @brief Called when the node enters the scene tree.
	 * Initializes enemy health.
	 */
	public override void _Ready()
	{
		health = max_health;
		//player = GetNode<CharacterBody2D>("../player");
	}

	/**
	 * @brief Called every physics frame.
	 * Handles enemy movement toward the player.
	 * 
	 * @param delta Time elapsed since the last frame.
	 */
	public override void _PhysicsProcess(double delta)
	{
		if (player == null)
			return;

		Vector2 direction = (player.GlobalPosition - GlobalPosition).Normalized();
		Velocity = direction * Speed;

		MoveAndSlide();
	}

	/**
	 * @brief Applies damage to the enemy.
	 * 
	 * Reduces health and removes the enemy if health reaches zero.
	 * Notifies the wave manager about enemy death.
	 * 
	 * @param damage Amount of damage to apply.
	 */
	public void Damage(int damage)
	{
		health -= damage;

		if (health <= 0)
		{
			waveManager?.OnEnemyDied();
			QueueFree();
		}
	}

	/**
	 * @brief Handles collision with another body.
	 * 
	 * If the body is a player, starts dealing periodic damage.
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

				if (contact)
					player.Damage(_damage);
			}
		}
	}

	/**
	 * @brief Handles exit from collision with another body.
	 * 
	 * Stops dealing damage when the player leaves contact.
	 * 
	 * @param body The node exiting collision.
	 */
	private void BodyCollisionOut(Node body)
	{
		if (body is Player)
		{
			contact = false;
		}
	}
}

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
	public int _damage = 10;
	public int max_health = 50;

	private int health;

	public bool contact = false;
	public float damagetime = 1f;

	public const float Speed = 40.0f;

	public CharacterBody2D player;

	
	public WaveManager waveManager;

	public override void _Ready()
	{
		health = max_health;
		//player = GetNode<CharacterBody2D>("../player");
	}

	public override void _PhysicsProcess(double delta)
	{
		if (player == null)
			return;

		Vector2 direction = (player.GlobalPosition - GlobalPosition).Normalized();
		Velocity = direction * Speed;

		MoveAndSlide();
	}

	public void Damage(int damage)
	{
		health -= damage;

		if (health <= 0)
		{
			
			waveManager?.OnEnemyDied();

			QueueFree();
		}
	}

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

	private void BodyCollisionOut(Node body)
	{
		if (body is Player)
		{
			contact = false;
		}
	}
}

using Godot;
using System;
using static Godot.TextServer;

/**
 * @class Enemy
 * @brief Represents a basic enemy unit in the game.
 * 
 * This class handles enemy movement towards the player,
 * health management, contact damage, and interaction with
 * the wave system when the enemy is defeated.
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
	 * @brief Movement speed of the enemy.
	 */
    public const float Speed = 40.0f;

    /**
	 * @brief Reference to the player object.
	 */
    public CharacterBody2D player;

    /**
	 * @brief Reference to the wave manager controlling enemy waves.
	 */
    public WaveManager waveManager;

    /**
	 * @brief Indicates whether the enemy can currently deal damage.
	 */
    private bool canDamage = true;

    /**
	 * @brief Called when the node enters the scene tree.
	 * 
	 * Initializes the enemy's health to its maximum value.
	 */
    public override void _Ready()
    {
        health = max_health;
    }

    /**
	 * @brief Handles physics-based movement every frame.
	 * 
	 * Moves the enemy towards the player using normalized direction
	 * and constant speed.
	 * 
	 * @param delta Time elapsed since last frame (in seconds).
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
	 * Reduces health and notifies the wave manager when the enemy dies.
	 * If health reaches zero, the enemy is removed from the scene.
	 * 
	 * @param damage Amount of damage to apply.
	 * 
	 * @note Calls WaveManager.OnEnemyDied() if assigned.
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
	 * If the colliding object is a player, the enemy starts
	 * dealing damage over time while contact persists.
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
	 * @brief Handles when a body exits collision.
	 * 
	 * Stops dealing damage when the player leaves the enemy area.
	 * 
	 * @param body The node leaving the collision.
	 */
    private void BodyCollisionOut(Node body)
    {
        if (body is Player)
        {
            contact = false;
        }
    }
}
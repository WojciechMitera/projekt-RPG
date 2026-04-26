using Godot;
using System;

/**
 * @class Boss
 * @brief Represents the main boss enemy in the game.
 * 
 * This class handles boss behavior such as movement towards the player,
 * health management, dealing contact damage, and triggering the win state
 * when defeated.
 */
public partial class Boss : CharacterBody2D
{
    /**
	 * @brief Current health of the boss.
	 */
    public int health = 300;

    /**
	 * @brief Movement speed of the boss.
	 */
    public float speed = 60;

    /**
	 * @brief Indicates whether the boss is in contact with the player.
	 */
    public bool contact = false;

    /**
	 * @brief Indicates whether the boss can currently deal damage.
	 */
    private bool canDamage = true;

    /**
	 * @brief Damage dealt to the player on contact.
	 */
    public int _damage = 30;

    /**
	 * @brief Time interval between damage ticks while in contact.
	 */
    public float damagetime = 0.8f;

    /**
	 * @brief Reference to the player character.
	 */
    public CharacterBody2D player;

    /**
	 * @brief Reference to the wave manager controlling game progression.
	 */
    public WaveManager waveManager;

    /**
	 * @brief Handles physics-based movement every frame.
	 * 
	 * Moves the boss towards the player using normalized direction
	 * and constant speed.
	 * 
	 * @param delta Time elapsed since last frame (in seconds).
	 */
    public override void _PhysicsProcess(double delta)
    {
        if (player == null) return;

        Vector2 direction = (player.GlobalPosition - GlobalPosition).Normalized();
        Velocity = direction * speed;

        MoveAndSlide();
    }

    /**
	 * @brief Applies damage to the boss.
	 * 
	 * Reduces health and triggers the win condition when health reaches zero.
	 * 
	 * @param dmg Amount of damage to apply.
	 * 
	 * @note Calls WaveManager.GameWon() if assigned.
	 */
    public void Damage(int dmg)
    {
        health -= dmg;

        if (health <= 0)
        {
            waveManager?.GameWon();
            QueueFree();
        }
    }

    /**
	 * @brief Handles collision with another body.
	 * 
	 * If the colliding object is the player, the boss starts
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
	 * Stops dealing contact damage when the player leaves the boss area.
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
using Godot;
using System;
using System.Drawing;
using static Godot.TextServer;

/**
 * @class Enemy3
 * @brief Represents a ranged enemy that shoots projectiles at the player.
 * 
 * This enemy maintains distance from the player, moves only when far enough,
 * and periodically shoots projectiles. It also interacts with the wave system
 * when defeated.
 */
public partial class Enemy3 : CharacterBody2D
{
    /**
	 * @brief Damage dealt to the player (used by projectiles or contact logic).
	 */
    public int _damage = 5;

    /**
	 * @brief Maximum health of the enemy.
	 */
    public int max_health = 15;

    /**
	 * @brief Current health of the enemy.
	 */
    private int health;

    /**
	 * @brief Time interval between shots.
	 */
    public float shootTime = 2f;

    /**
	 * @brief Movement speed of the enemy (negative for backward movement behavior).
	 */
    public const float Speed = -10.0f;

    /**
	 * @brief Reference to the player object.
	 */
    public CharacterBody2D player;

    /**
	 * @brief Reference to the wave manager controlling enemy waves.
	 */
    public WaveManager waveManager;

    /**
	 * @brief Packed scene used to spawn projectiles.
	 */
    private PackedScene projectileScene = GD.Load<PackedScene>("res://projectileenemy.tscn");

    /**
	 * @brief Marker used as projectile spawn position.
	 */
    private Marker2D point;

    /**
	 * @brief Called when the node enters the scene tree.
	 * 
	 * Initializes health, retrieves required nodes, and starts shooting loop.
	 * 
	 * @note Requires a child node named "point" (Marker2D).
	 */
    public override void _Ready()
    {
        health = max_health;
        point = GetNode<Marker2D>("point");

        CallDeferred(nameof(ShootLoop));
    }

    /**
	 * @brief Handles movement and distance-based behavior.
	 * 
	 * The enemy moves toward the player only if they are outside a defined
	 * distance threshold. Otherwise, it stays idle.
	 * 
	 * @param delta Time elapsed since last frame (in seconds).
	 */
    public override void _PhysicsProcess(double delta)
    {
        if (player == null)
            return;

        float distance = GlobalPosition.DistanceTo(player.GlobalPosition);

        if (distance > 200)
        {
            Vector2 direction = (player.GlobalPosition - GlobalPosition).Normalized();
            Velocity = direction * Speed;
        }
        else
        {
            Velocity = Vector2.Zero;
        }

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
	 * @brief Continuously handles shooting logic.
	 * 
	 * Repeats shooting at fixed intervals as long as the enemy exists
	 * and the player is available.
	 */
    public async void ShootLoop()
    {
        while (true)
        {
            await ToSignal(GetTree().CreateTimer(shootTime), "timeout");

            if (player == null)
                continue;

            Shoot();
        }
    }

    /**
	 * @brief Spawns and fires a projectile toward the player.
	 * 
	 * Instantiates a projectile, sets its position and direction,
	 * and adds it to the scene tree.
	 */
    public void Shoot()
    {
        var projectile = projectileScene.Instantiate<Projectileenemy>();

        projectile.GlobalPosition = point.GlobalPosition;

        Vector2 direction = (player.GlobalPosition - point.GlobalPosition).Normalized();
        projectile.direction = direction;

        GetParent().AddChild(projectile);
    }
}
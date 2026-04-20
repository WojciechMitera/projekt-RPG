using Godot;
using System;
using System.Drawing;
using static Godot.TextServer;

/**
 * @class Enemy3
 * @brief Represents a ranged enemy that shoots projectiles at the player.
 * 
 * This class defines an enemy with very low health that attacks
 * the player from a distance using projectiles. It also handles
 * movement, respawning, and contact-based damage.
 */
public partial class Enemy3 : CharacterBody2D
{
    /**
     * @brief Amount of damage dealt to the player (if used in other interactions).
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
     * @brief Time interval (in seconds) between consecutive shots.
     */
    public float shootTime = 2f;

    /**
     * @brief Movement speed of the enemy.
     */
    public const float Speed = -10.0f; 

    /**
     * @brief Reference to the player character.
     */
    public CharacterBody2D player;

    /**
     * @brief Reference to the wave manager controlling enemy waves.
     */
    public WaveManager waveManager;

    /**
     * @brief Scene used to instantiate projectiles.
     */
    private PackedScene projectileScene = GD.Load<PackedScene>("res://projectileenemy.tscn");

    /**
     * @brief Marker indicating the projectile spawn point.
     */
    private Marker2D point;

    /**
     * @brief Called when the node enters the scene tree.
     * Initializes health, gets the shooting point, and starts the shooting loop.
     */
    public override void _Ready()
    {
        health = max_health;
        point = GetNode<Marker2D>("point");

        CallDeferred(nameof(ShootLoop));
    }

    /**
     * @brief Called every physics frame.
     * 
     * Controls enemy movement relative to the player:
     * - Moves toward the player if too far away
     * - Stops when within shooting range
     * 
     * @param delta Time elapsed since the last frame.
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
     * @brief Handles continuous shooting behavior.
     * 
     * Periodically creates projectiles directed at the player.
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
     * @brief Spawns and launches a projectile toward the player.
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

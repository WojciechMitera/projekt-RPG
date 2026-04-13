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
    public int _damage = 5;
    public int max_health = 15;

    private int health;

    public float shootTime = 2f;

    public const float Speed = -10.0f; 

    public CharacterBody2D player;
    public WaveManager waveManager;

    private PackedScene projectileScene = GD.Load<PackedScene>("res://projectileenemy.tscn");
    private Marker2D point;

    public override void _Ready()
    {
        health = max_health;
        point = GetNode<Marker2D>("point");

        CallDeferred(nameof(ShootLoop));
    }

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

    public void Damage(int damage)
    {
        health -= damage;

        if (health <= 0)
        {
            waveManager?.OnEnemyDied();
            QueueFree();
        }
    }

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

    public void Shoot()
    {
        var projectile = projectileScene.Instantiate<Projectileenemy>();

        projectile.GlobalPosition = point.GlobalPosition;

        Vector2 direction = (player.GlobalPosition - point.GlobalPosition).Normalized();
        projectile.direction = direction;

        GetParent().AddChild(projectile);
    }

}
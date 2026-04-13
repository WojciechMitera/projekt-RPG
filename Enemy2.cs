using Godot;
using System;

/**
 * @class Enemy2
 * @brief Represents a stronger enemy variant in the game.
 * 
 * This class defines a more powerful enemy with increased health,
 * higher damage, and slower movement. It handles movement,
 * combat interactions, and respawning logic.
 */
public partial class Enemy2 : CharacterBody2D
{
    public int _damage = 25;
    public int max_health = 100;

    private int health;

    public bool contact = false;
    public float damagetime = 0.7f;

    public const float Speed = 20.0f; 

    public CharacterBody2D player;
    public WaveManager waveManager;

    public override void _Ready()
    {
        health = max_health;
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
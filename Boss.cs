using Godot;
using System;

public partial class Boss : CharacterBody2D
{
    public int health = 300;
    public float speed = 60;

    public CharacterBody2D player;
    public WaveManager waveManager;

    public override void _PhysicsProcess(double delta)
    {
        if (player == null) return;

        Vector2 direction = (player.GlobalPosition - GlobalPosition).Normalized();
        Velocity = direction * speed;

        MoveAndSlide();
    }

    public void Damage(int dmg)
    {
        health -= dmg;

        if (health <= 0)
        {
            waveManager?.GameWon(); // 🔥 KONIEC GRY
            QueueFree();
        }
    }
}
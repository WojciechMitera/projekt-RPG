using Godot;
using System;

/**
 * @class Projectile2
 * @brief Represents a fast-moving projectile variant.
 * 
 * This class defines a faster but weaker projectile used by the player.
 * It handles movement, collision detection with enemies, damage application,
 * and automatic destruction after a short lifetime.
 */
public partial class Projectile2 : CharacterBody2D
{
    /**
	 * @brief Damage dealt to enemies on hit.
	 */
    public int _damage = 5;

    /**
	 * @brief Lifetime before automatic destruction (in seconds).
	 */
    public float _time = 0.01f;

    /**
	 * @brief Movement speed of the projectile.
	 */
    public float speed = 500;

    /**
	 * @brief Normalized direction vector of the projectile.
	 */
    public Vector2 direction;

    /**
	 * @brief Handles collision with another body.
	 * 
	 * Applies damage to enemies or bosses on hit and destroys the projectile.
	 * Also destroys the projectile when colliding with the environment
	 * (walls, floor, or ceiling).
	 * 
	 * If no immediate collision occurs, the projectile is scheduled
	 * for delayed destruction.
	 * 
	 * @param body The node that the projectile collided with.
	 */
    private void BodyCollision2(Node body)
    {
        if (body is Enemy enemy)
        {
            enemy.Damage(_damage);
            QueueFree();
        }
        else if (body is Enemy2 enemy2)
        {
            enemy2.Damage(_damage);
            QueueFree();
        }
        else if (body is Enemy3 enemy3)
        {
            enemy3.Damage(_damage);
            QueueFree();
        }
        else if (body is Boss boss)
        {
            boss.Damage(_damage);
            QueueFree();
        }

        if (IsOnWallOnly() || IsOnFloorOnly() || IsOnCeilingOnly() ||
            IsOnWall() || IsOnFloor() || IsOnCeiling())
        {
            QueueFree();
        }
        else
        {
            Destroy(_time);
        }
    }

    /**
	 * @brief Updates projectile movement every physics frame.
	 * 
	 * Moves the projectile in the assigned direction with constant speed.
	 * 
	 * @param delta Time elapsed since last frame (in seconds).
	 */
    public override void _PhysicsProcess(double delta)
    {
        Velocity = direction * speed;
        MoveAndSlide();
    }

    /**
	 * @brief Destroys the projectile after a delay.
	 * 
	 * Waits for the specified time and then removes the projectile from the scene.
	 * 
	 * @param time Delay before destruction (in seconds).
	 */
    public async void Destroy(float time)
    {
        await ToSignal(GetTree().CreateTimer(time), "timeout");
        QueueFree();
    }
}
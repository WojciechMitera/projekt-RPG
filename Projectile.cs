using Godot;
using System;

/**
 * @class Projectile
 * @brief Represents a moving projectile fired by the player.
 * 
 * This class handles projectile movement, collision detection,
 * damage application, and automatic destruction after a short time.
 */
public partial class Projectile : CharacterBody2D
{
	/**
	 * @brief Damage dealt to enemies on hit.
	 */
	public int _damage = 10;

	/**
	 * @brief Lifetime before automatic destruction (in seconds).
	 */
	public float _time = 0.01f;

	/**
	 * @brief Movement speed of the projectile.
	 */
	public float speed = 300;

	/**
	 * @brief Normalized direction vector of the projectile.
	 */
	public Vector2 direction;

	/**
	 * @brief Handles collision with another body.
	 * 
	 * Applies damage to enemies and destroys the projectile on impact.
	 * Also destroys the projectile when it collides with environment
	 * surfaces such as walls, floor, or ceiling.
	 * 
	 * If no immediate collision condition is met, the projectile is
	 * scheduled for delayed destruction.
	 * 
	 * @param body The node that the projectile collided with.
	 */
	private void BodyCollision(Node body)
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
		else if (body is Boss boss) // 🔥 NOWE
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
	 * Moves the projectile in the assigned direction
	 * with constant velocity.
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
	 * Waits for the specified time and then removes
	 * the projectile from the scene.
	 * 
	 * @param time Delay before destruction (in seconds).
	 */
	public async void Destroy(float time)
	{
		await ToSignal(GetTree().CreateTimer(time), "timeout");
		QueueFree();
	}
}

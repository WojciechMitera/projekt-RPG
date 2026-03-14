using Godot;
using System;

public partial class Projectile : CharacterBody2D
{
	public int _damage = 10;
	public float _time = 0.01f;
	public float speed = 300;
	public Vector2 direction;

	private void BodyCollision(Node body)
	{
		if (body is Enemy enemy)
		{
			enemy.Damage(_damage);
			QueueFree();

		}
		if (IsOnWallOnly() || IsOnFloorOnly() || IsOnCeilingOnly() || IsOnWall() || IsOnFloor() || IsOnCeiling())
		{
			QueueFree();
		}
		else{
			Destroy(_time);
		}
	}
	
	public override void _PhysicsProcess(double delta)
	{
		Velocity = direction * speed;
		
		MoveAndSlide();
	}
	public async void Destroy(float time)
	{
		await ToSignal(GetTree().CreateTimer(time), "timeout");
		QueueFree();
	}
}

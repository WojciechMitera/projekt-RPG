using Godot;
using System;

public partial class Player : CharacterBody2D
{
	PackedScene projectiletscn = GD.Load<PackedScene>("res://projectile.tscn");

	public const float Speed = 180.0f;
	[Export]
	public int max_health = 100;
	private int health;
	private ProgressBar healthbar;
	Marker2D point;
	CharacterBody2D _player;
	public override void _Ready(){
		health = max_health;
		healthbar = GetNode<ProgressBar>("../bar");
		point = GetNode<Marker2D>("point");
		_player = GetNode<CharacterBody2D>("../player");
	}
	public void Damage(int damage)
	{
		health -= damage;
		if(health < 0)
		{
			
			//GetTree().ChangeSceneToFile("res://menu.tscn");
			health = 0;
			
		}
		else if(health == 0){
			GetTree().CallDeferred("change_scene_to_file", "res://pauza.tscn");
		}
		healthbar.Value = health;
		GD.Print(health);
	}
	public void Shoot()
	{
		Projectile projectile = projectiletscn.Instantiate<Projectile>();
		projectile.GlobalPosition = point.GlobalPosition;
		Vector2 direction = (GetGlobalMousePosition() - point.GlobalPosition).Normalized();
		projectile.direction = direction;

	

		GetParent().AddChild(projectile);
	}
	
	public override void _PhysicsProcess(double delta)
	{
		Vector2 velocity = Velocity;
		Vector2 direction = Input.GetVector("ui_left", "ui_right", "ui_up", "ui_down");
		if (direction != Vector2.Zero)
		{
			velocity.X = direction.X * Speed;
		}
		else
		{
			velocity.X = Mathf.MoveToward(Velocity.X, 0, Speed);
		}
		if (direction != Vector2.Zero)
		{
			velocity.Y = direction.Y * Speed;
		}
		else
		{
			velocity.Y = Mathf.MoveToward(Velocity.Y, 0, Speed);
		}
		
		Velocity = velocity;
		MoveAndSlide();

		if (Input.IsActionJustPressed("ui_accept"))
		{
			Shoot();
		}
	}
	
}

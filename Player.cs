using Godot;
using System;
using static Godot.SkeletonModifier3D;

/**
 * @class Player
 * @brief Controls the player character behavior.
 * 
 * This class handles player movement, shooting, health management,
 * and interaction with the game world. It extends CharacterBody2D
 * to utilize built-in physics-based movement.
 */
public partial class Player : CharacterBody2D
{
	/** @brief Packed scene used to instantiate projectiles. */
	PackedScene projectiletscn = GD.Load<PackedScene>("res://projectile.tscn");
	PackedScene projectile2tscn = GD.Load<PackedScene>("res://projectile2.tscn");

	/** @brief Movement speed of the player. */
	public const float Speed = 180.0f;

	/** @brief Maximum health value of the player. */
	[Export]
	public int max_health = 100;

	/** @brief Current health value of the player. */
	private int health;

	public int meleedamage = 8;

	/** @brief Reference to the UI health bar. */
	private ProgressBar healthbar;

	/** @brief Marker indicating where projectiles spawn. */
	Marker2D point;

	/** @brief Reference to the player node (may be redundant depending on scene structure). */
	CharacterBody2D _player;
	Area2D meleeattackarrea;

	/**
	 * @brief Called when the node enters the scene tree.
	 * 
	 * Initializes player health, UI references, and important nodes.
	 */
	public override void _Ready()
	{
		health = max_health;
		healthbar = GetNode<ProgressBar>("../bar");
		point = GetNode<Marker2D>("point");
		_player = GetNode<CharacterBody2D>("../player");
		meleeattackarrea = GetNode<Area2D>("meleeattackarea");
		meleeattackarrea.Monitoring = false;
	}

	/**
	 * @brief Applies damage to the player.
	 * 
	 * Reduces the player's health by the specified amount. If health
	 * reaches zero, the scene is changed to the pause (or game over) scene.
	 * 
	 * @param damage Amount of damage to apply.
	 */
	public void Damage(int damage)
	{
		health -= damage;

		if (health < 0)
		{
			health = 0;
		}
		else if (health == 0)
		{
			GetTree().CallDeferred("change_scene_to_file", "res://pauza.tscn");
		}

		healthbar.Value = health;
		GD.Print(health);
	}

	/**
	 * @brief Spawns and shoots a projectile toward the mouse position.
	 * 
	 * Instantiates a projectile scene, sets its position to the spawn marker,
	 * and assigns a normalized direction vector toward the mouse cursor.
	 */
	public void Shoot()
	{
		Projectile projectile = projectiletscn.Instantiate<Projectile>();
		projectile.GlobalPosition = point.GlobalPosition;

		Vector2 direction = (GetGlobalMousePosition() - point.GlobalPosition).Normalized();
		projectile.direction = direction;

		GetParent().AddChild(projectile);
	}
	public void Shoot2()
	{
		Vector2 direction = (GetGlobalMousePosition() - point.GlobalPosition).Normalized();
		float spread = 0.2f;
		Vector2[] directions = 
		{
			direction,
		direction.Rotated(spread),
		direction.Rotated(-spread)
	};
		foreach (var dir in directions)
		{
			Projectile2 projectile = projectile2tscn.Instantiate<Projectile2>();
			projectile.GlobalPosition = point.GlobalPosition;


			projectile.direction = dir;

			GetParent().AddChild(projectile);
		}
		
		
		
		
	}
	public async void Meleeattack()
	{
		meleeattackarrea.Monitoring = true;
		GD.Print("true");
		await ToSignal(GetTree().CreateTimer(0.2f), "timeout");
		meleeattackarrea.Monitoring= false;
		GD.Print("false");
	}
	public void BodyCollision(Node body)
	{
		if(body is Enemy enemy)
		{
			enemy.Damage(meleedamage);
		}
		else if (body is Enemy2 enemy2)
		{
			enemy2.Damage(meleedamage);
		}
		else if (body is Enemy3 enemy3)
		{
			enemy3.Damage(meleedamage);
		}
	}
	/**
	 * @brief Handles player movement and input every physics frame.
	 * 
	 * Processes directional input, updates velocity, changes player sprite
	 * based on movement direction, and handles shooting input.
	 * 
	 * @param delta Time elapsed since last frame (in seconds).
	 */
	public override void _PhysicsProcess(double delta)
	{
		Vector2 direction = Input.GetVector("ui_left", "ui_right", "ui_up", "ui_down");
		var sprite = GetNode<Sprite2D>("sprite_player");

		if (direction != Vector2.Zero)
		{
			direction = direction.Normalized();
			Velocity = direction * Speed;
			if (direction.X > 0)
			{
				sprite.Texture = GD.Load<Texture2D>("res://player.png");
			}
			else if (direction.X < 0)
			{
				sprite.Texture = GD.Load<Texture2D>("res://player2.png");
			}
		}
		else
		{
			Velocity = Vector2.Zero;
		}
		
		MoveAndSlide();

		if (Input.IsActionJustPressed("use_weapon_1"))
		{
			Shoot();
		}
		if (Input.IsActionJustPressed("use_weapon_2"))
		{
			Shoot2();
		}
		if (Input.IsActionJustPressed("use_weapon_3"))
		{
			Meleeattack();
		}
	}
}

using Godot;
using System;
using static Godot.SkeletonModifier3D;

/**
 * @class Player
 * @brief Controls the player character behavior.
 * 
 * This class handles player movement, shooting, melee attacks,
 * health management, and interaction with enemies. It extends
 * CharacterBody2D to utilize built-in physics-based movement.
 */
public partial class Player : CharacterBody2D
{
	/**
	 * @brief Packed scene used to instantiate the primary projectile.
	 */
	PackedScene projectiletscn = GD.Load<PackedScene>("res://projectile.tscn");

	/**
	 * @brief Packed scene used to instantiate the secondary projectile.
	 */
	PackedScene projectile2tscn = GD.Load<PackedScene>("res://projectile2.tscn");

	/**
	 * @brief Movement speed of the player.
	 */
	public const float Speed = 180.0f;

	/**
	 * @brief Maximum health value of the player.
	 */
	[Export]
	public int max_health = 100;

	/**
	 * @brief Current health value of the player.
	 */
	private int health;

	/**
	 * @brief Damage dealt by melee attacks.
	 */
	public int meleedamage = 8;

	/**
	 * @brief Reference to the UI health bar.
	 */
	private ProgressBar healthbar;

	/**
	 * @brief Marker indicating where projectiles spawn.
	 */
	Marker2D point;

	/**
	 * @brief Reference to the player node.
	 * 
	 * @note May be redundant depending on scene structure.
	 */
	CharacterBody2D _player;

	/**
	 * @brief Area used to detect melee attack collisions.
	 */
	Area2D meleeattackarrea;

	/**
	 * @brief Called when the node enters the scene tree.
	 * 
	 * Initializes player health, UI references, and required nodes.
	 * 
	 * @note Required nodes:
	 * - "../bar" (ProgressBar)
	 * - "point" (Marker2D)
	 * - "../player" (CharacterBody2D)
	 * - "meleeattackarea" (Area2D)
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
	 * Reduces the player's health. If health reaches zero,
	 * the game switches to the pause (game over) scene.
	 * 
	 * @param damage Amount of damage to apply.
	 * 
	 * @note Scene "res://pauza.tscn" must exist.
	 */
	public void Damage(int damage)
	{
		health -= damage;

		if (health <= 0)
		{
			GetTree().CallDeferred("change_scene_to_file", "res://pauza.tscn");
			GD.Print("Game Over");
		}

		healthbar.Value = health;
		GD.Print(health);
	}

	/**
	 * @brief Shoots a single projectile toward the mouse position.
	 * 
	 * Instantiates a projectile and assigns its direction
	 * based on the cursor position.
	 */
	public void Shoot()
	{
		Projectile projectile = projectiletscn.Instantiate<Projectile>();
		projectile.GlobalPosition = point.GlobalPosition;

		Vector2 direction = (GetGlobalMousePosition() - point.GlobalPosition).Normalized();
		projectile.direction = direction;

		GetParent().AddChild(projectile);
	}

	/**
	 * @brief Shoots multiple projectiles in a spread pattern.
	 * 
	 * Fires three projectiles: one straight and two slightly rotated
	 * to create a spread shot effect.
	 */
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
	public void Shoot3()
	{
		Vector2 direction = (GetGlobalMousePosition() - point.GlobalPosition).Normalized();
		float spread = 0.7f;

		Vector2[] directions =
		{
			direction,
			direction.Rotated(spread),
			direction.Rotated(-spread),

			direction.Rotated(-spread * 2),
			direction.Rotated(spread * 2),

			
			direction.Rotated(spread * 3),
			direction.Rotated(-spread * 3),

		   
			direction.Rotated(spread * 4),
			direction.Rotated(-spread * 4),

			direction.Rotated(spread * 5),
			direction.Rotated(-spread * 5)
		};

		foreach (var dir in directions)
		{
			Projectile2 projectile = projectile2tscn.Instantiate<Projectile2>();
			projectile.GlobalPosition = point.GlobalPosition;
			projectile.direction = dir;

			GetParent().AddChild(projectile);
		}
	}

	/**
	 * @brief Performs a melee attack.
	 * 
	 * Temporarily enables the melee collision area to detect
	 * and damage nearby enemies.
	 */
	public async void Meleeattack()
	{
		meleeattackarrea.Monitoring = true;
		GD.Print("Melee ON");

		await ToSignal(GetTree().CreateTimer(0.5f), "timeout");

		meleeattackarrea.Monitoring = false;
		GD.Print("Melee OFF");
	}

	/**
	 * @brief Handles collision with enemy bodies during melee attack.
	 * 
	 * Detects enemy types and applies melee damage accordingly.
	 * 
	 * @param body The colliding node.
	 */
	public void BodyCollision(Node body)
	{
		if (body is Enemy enemy)
		{
			enemy.Damage(meleedamage);
			GD.Print("Hit Enemy1");
		}
		else if (body is Enemy2 enemy2)
		{
			enemy2.Damage(meleedamage);
		}
		else if (body is Enemy3 enemy3)
		{
			enemy3.Damage(meleedamage);
		}
        else if (body is Boss boss) // 🔥 NOWE
        {
            boss.Damage(meleedamage);
        }
    }

	/**
	 * @brief Handles player movement and input every physics frame.
	 * 
	 * Processes directional input, updates velocity, changes player sprite
	 * based on movement direction, and handles weapon input.
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
			Shoot3();
		}
	}
}

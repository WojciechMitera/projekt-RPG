using Godot;
using System;

/**
 * @class Play
 * @brief Main gameplay scene controller.
 * 
 * This class is responsible for handling gameplay-related logic
 * in the main scene. It provides functionality such as pausing
 * the game and switching to the pause menu scene.
 */
public partial class Play : Node2D
{
	/**
	 * @brief Switches the current scene to the pause menu.
	 * 
	 * This method changes the active scene to the pause scene
	 * ("pauza.tscn"). It can be used when the player pauses
	 * the game (e.g., by pressing a pause button).
	 * 
	 * @note Make sure that the pause scene exists at the given path:
	 * "res://pauza.tscn".
	 */
	private void pauza(){
		GetTree().ChangeSceneToFile("res://pauza.tscn");
	}
}



using Godot;
using System;

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

	/** @brief Movement speed of the player. */
	public const float Speed = 180.0f;

	/** @brief Maximum health value of the player. */
	[Export]
	public int max_health = 100;

	/** @brief Current health value of the player. */
	private int health;

	/** @brief Reference to the UI health bar. */
	private ProgressBar healthbar;

	/** @brief Marker indicating where projectiles spawn. */
	Marker2D point;

	/** @brief Reference to the player node (may be redundant depending on scene structure). */
	CharacterBody2D _player;

	/**
	 * @brief Called when the node enters the scene tree.
	 * 
	 * Initializes player health, UI references, and important nodes.
	 */
	public override void _Ready(){
		health = max_health;
		healthbar = GetNode<ProgressBar>("../bar");
		point = GetNode<Marker2D>("point");
		_player = GetNode<CharacterBody2D>("../player");
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

		if(health < 0)
		{
			health = 0;
		}
		else if(health == 0){
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
		Vector2 velocity = Velocity;
		Vector2 direction = Input.GetVector("ui_left", "ui_right", "ui_up", "ui_down");
		var sprite = GetNode<Sprite2D>("sprite_player");

		if (direction != Vector2.Zero)
		{
			if(direction == Vector2.Right)
			{
				sprite.Texture = GD.Load<Texture2D>("res://player.png");
			}
			else if(direction == Vector2.Left)
			{
				sprite.Texture = GD.Load<Texture2D>("res://player2.png");
			}
			velocity.X = direction.X * Speed;
		}
		else
		{
			velocity.X = Mathf.MoveToward(Velocity.X, 0, Speed);
		}

		if (direction != Vector2.Zero)
		{
			if (direction == Vector2.Right)
			{
				sprite.Texture = GD.Load<Texture2D>("res://player.png");
			}
			else if (direction == Vector2.Left)
			{
				sprite.Texture = GD.Load<Texture2D>("res://player2.png");
			}
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
	/** @brief Damage dealt to enemies on hit. */
	public int _damage = 10;

	/** @brief Lifetime before automatic destruction (in seconds). */
	public float _time = 0.01f;

	/** @brief Movement speed of the projectile. */
	public float speed = 300;

	/** @brief Normalized direction vector of the projectile. */
	public Vector2 direction;

	/**
	 * @brief Handles collision with another body.
	 * 
	 * If the collided object is an Enemy, it applies damage and destroys
	 * the projectile. The projectile is also destroyed when it collides
	 * with walls, floor, or ceiling. Otherwise, it is scheduled for
	 * delayed destruction.
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
	 * Moves the projectile in the assigned direction with a constant speed.
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
	 * Uses a timer signal to wait for a specified amount of time
	 * before removing the projectile from the scene.
	 * 
	 * @param time Delay before destruction (in seconds).
	 */
	public async void Destroy(float time)
	{
		await ToSignal(GetTree().CreateTimer(time), "timeout");
		QueueFree();
	}
}

using Godot;
using System;
using static Godot.SkeletonModifier3D;

/**
 * @class Player
 * @brief Controls the player character behavior.
 * 
 * This class handles movement, shooting (multiple weapon types),
 * melee attacks, health management, and interactions with enemies
 * and the game world.
 */
public partial class Player : CharacterBody2D
{
    /**
	 * @brief Packed scene for primary projectile.
	 */
    PackedScene projectiletscn = GD.Load<PackedScene>("res://projectile.tscn");

    /**
	 * @brief Packed scene for secondary projectile.
	 */
    PackedScene projectile2tscn = GD.Load<PackedScene>("res://projectile2.tscn");

    /**
	 * @brief Movement speed of the player.
	 */
    public const float Speed = 180.0f;

    /**
	 * @brief Maximum health of the player.
	 */
    [Export]
    public int max_health = 100;

    /**
	 * @brief Current health of the player.
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
	 * @brief Projectile spawn point.
	 */
    Marker2D point;

    /**
	 * @brief Reference to player node.
	 */
    CharacterBody2D _player;

    /**
	 * @brief Area used for melee attack detection.
	 */
    Area2D meleeattackarrea;

    /**
	 * @brief Cooldown for weapon 1.
	 */
    private float cooldown1 = 0.3f;

    /**
	 * @brief Cooldown for weapon 2.
	 */
    private float cooldown2 = 0.6f;

    /**
	 * @brief Cooldown for weapon 3.
	 */
    private float cooldown3 = 1.0f;

    /**
	 * @brief Internal timer for weapon 1.
	 */
    private float timer1 = 0f;

    /**
	 * @brief Internal timer for weapon 2.
	 */
    private float timer2 = 0f;

    /**
	 * @brief Internal timer for weapon 3.
	 */
    private float timer3 = 0f;

    /**
	 * @brief Called when the node enters the scene tree.
	 * 
	 * Initializes health, UI references, and important nodes.
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
	 * Reduces health and switches to game over scene when health reaches zero.
	 * 
	 * @param damage Amount of damage to apply.
	 * 
	 * @note Scene "res://pauza.tscn" is used as game over screen.
	 */
    public void Damage(int damage)
    {
        health -= damage;

        if (health <= 0)
        {
            GetTree().CallDeferred("change_scene_to_file", "res://pauza.tscn");
        }

        healthbar.Value = health;
    }

    /**
	 * @brief Fires a single projectile towards the mouse position.
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
	 * @brief Fires a spread of 3 projectiles.
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

    /**
	 * @brief Fires a wide shotgun-style spread of projectiles.
	 */
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
	 * Temporarily enables melee hitbox for detecting enemies.
	 */
    public async void Meleeattack()
    {
        meleeattackarrea.Monitoring = true;

        await ToSignal(GetTree().CreateTimer(0.5f), "timeout");

        meleeattackarrea.Monitoring = false;
    }

    /**
	 * @brief Handles melee collision with enemies.
	 * 
	 * Applies melee damage depending on enemy type.
	 * 
	 * @param body The collided node.
	 */
    public void BodyCollision(Node body)
    {
        if (body is Enemy enemy)
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
        else if (body is Boss boss)
        {
            boss.Damage(meleedamage);
        }
    }

    /**
	 * @brief Handles movement, input, and weapon cooldowns.
	 * 
	 * Processes player movement, sprite direction changes,
	 * and weapon firing with cooldown system.
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

        timer1 -= (float)delta;
        timer2 -= (float)delta;
        timer3 -= (float)delta;

        if (Input.IsActionPressed("use_weapon_1") && timer1 <= 0f)
        {
            Shoot();
            timer1 = cooldown1;
        }

        if (Input.IsActionPressed("use_weapon_2") && timer2 <= 0f)
        {
            Shoot2();
            timer2 = cooldown2;
        }

        if (Input.IsActionPressed("use_weapon_3") && timer3 <= 0f)
        {
            Shoot3();
            timer3 = cooldown3;
        }
    }
}
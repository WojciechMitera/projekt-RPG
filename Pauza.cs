using Godot;
using System;

/**
 * @class Pauza
 * @brief Pause menu controller.
 * 
 * This class handles pause menu functionality,
 * allowing the player to return to the main menu.
 */
public partial class Pauza : Control
{
	/**
	 * @brief Loads the main menu scene.
	 * 
	 * Switches the current scene to the main menu.
	 * 
	 * @note The menu scene must exist at:
	 * "res://menu.tscn".
	 */
	private void menu()
	{
		GetTree().ChangeSceneToFile("res://menu.tscn");
	}
}

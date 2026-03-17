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
	private void pauza()
	{
		GetTree().ChangeSceneToFile("res://pauza.tscn");
	}
}

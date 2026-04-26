using Godot;
using System;

/**
 * @class Ustawienia
 * @brief Controls the settings menu, specifically audio settings.
 * 
 * This class manages the game audio volume using Godot's AudioServer.
 */
public partial class Ustawienia : Control
{
    /**
	 * @brief Audio bus index for music channel.
	 */
    private int musicBus;

    /**
	 * @brief Called when the node enters the scene tree.
	 * 
	 * Retrieves the audio bus index for music control.
	 */
    public override void _Ready()
    {
        musicBus = AudioServer.GetBusIndex("music");
    }

    /**
	 * @brief Handles volume slider changes.
	 * 
	 * Converts linear slider value to decibels and applies it
	 * to the music audio bus.
	 * 
	 * @param value Linear volume value from UI slider.
	 */
    private void ValueChanged(float value)
    {
        float db = Mathf.LinearToDb(value);
        AudioServer.SetBusVolumeDb(musicBus, db);
    }
}
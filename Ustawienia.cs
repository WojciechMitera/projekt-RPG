using Godot;
using System;

public partial class Ustawienia : Control
{
	private int musicBus;

	public override void _Ready()
	{
		musicBus = AudioServer.GetBusIndex("music");
	}

	private void ValueChanged(float value)
	{
	   

		float db = Mathf.LinearToDb(value);

		AudioServer.SetBusVolumeDb(musicBus, db);
	}
}

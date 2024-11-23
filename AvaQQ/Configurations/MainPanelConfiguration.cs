using System;

namespace AvaQQ.Configurations;

internal class MainPanelConfiguration
{
	public int Width { get; set; } = 300;

	public int Height { get; set; } = 600;

	public TimeSpan UnusedViewDestructionTime { get; set; } = TimeSpan.FromMinutes(1);
}

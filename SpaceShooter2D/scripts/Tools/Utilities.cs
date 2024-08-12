using Godot;

namespace SpaceShooter2D.scripts.Tools;

public static class Utilities
{
	public static T GetParentOfType<T>(this Node node)
	{
		Node parent;

		if ((parent = node.GetParent()) == null) return default(T);
		if (parent is T castParent)
		{
			return castParent;
		}
		else
		{
			return parent.GetParentOfType<T>();
		}
	}
}

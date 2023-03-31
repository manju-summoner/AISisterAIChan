static class CollisionParts
{
	public const string Head = "head";
	public const string Cheek = "cheek";
	public const string Mouth = "mouth";
	public const string Ribbon = "ribbon";
	public const string TwinTail = "twintail";
	public const string Bust = "bust";
	public const string Skirt = "skirt";
	public const string Shoulder = "shoulder";
	public const string Leg = "leg";

	public static string GetCollisionPartsName(string parts)
	{
		switch (parts)
		{
			case Head:
				return "頭";
			case Cheek:
				return "ほっぺた";
			case Mouth:
				return "唇";
			case Ribbon:
				return "リボン";
			case TwinTail:
				return "ツインテール";
			case Bust:
				return "胸";
			case Skirt:
				return "スカート";
			case Shoulder:
				return "肩";
			case Leg:
				return "太もも";
			default:
				return null;
		}
	}
}
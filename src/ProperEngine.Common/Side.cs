using System;

namespace ProperEngine.Common
{
	[Flags]
	public enum Side : byte
	{
		None   = 0b00,
		Client = 0b01,
		Server = 0b10,
		Both   = Client | Server
	}
	
	public static class SideExtensions
	{
		public static Side Opposite(this Side side)
		{
			switch (side) {
				case Side.None:   return Side.None;
				case Side.Client: return Side.Server;
				case Side.Server: return Side.Client;
				case Side.Both:   return Side.Both;
				default: throw new ArgumentException(
					$"Invalid side '{ side }'", nameof(side));
			}
		}
		
		public static bool IsValid(this Side side, Side required)
			=> ((side & required) != Side.None);
	}
}

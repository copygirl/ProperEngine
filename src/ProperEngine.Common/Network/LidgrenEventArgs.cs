using System;
using Lidgren.Network;

namespace ProperEngine.Common.Network
{
	public abstract class LidgrenEventArgs
		: EventArgs
	{
		public NetIncomingMessage LidgrenMessage { get; }
		
		public NetConnection Connection => LidgrenMessage.SenderConnection;
		
		internal LidgrenEventArgs(NetIncomingMessage lidgrenMessage)
			=> LidgrenMessage = lidgrenMessage;
	}
	
	public class DiagMessageEventArgs
		: LidgrenEventArgs
	{
		public DiagMessageLevel Level { get; }
		public string Message { get; }
		
		internal DiagMessageEventArgs(NetIncomingMessage lidgrenMessage)
			: base(lidgrenMessage)
		{
			switch (lidgrenMessage.MessageType) {
				case NetIncomingMessageType.VerboseDebugMessage:
					Level = DiagMessageLevel.Verbose; break;
				case NetIncomingMessageType.DebugMessage:
					Level = DiagMessageLevel.Debug; break;
				case NetIncomingMessageType.WarningMessage:
					Level = DiagMessageLevel.Warning; break;
				case NetIncomingMessageType.ErrorMessage:
					Level = DiagMessageLevel.Error; break;
				default: throw new InvalidOperationException();
			}
			Message = lidgrenMessage.ReadString();
		}
	}
	
	public enum DiagMessageLevel
	{
		Verbose,
		Debug,
		Warning,
		Error
	}
	
	public class StatusChangedEventArgs
		: LidgrenEventArgs
	{
		public NetConnectionStatus Status { get; }
		public string Reason { get; }
		
		internal StatusChangedEventArgs(NetIncomingMessage lidgrenMessage)
			: base(lidgrenMessage)
		{
			Status = (NetConnectionStatus)lidgrenMessage.ReadByte();
			Reason = lidgrenMessage.ReadString();
		}
	}
	
	public class MessageEventArgs
		: LidgrenEventArgs
	{
		public IMessage Message { get; }
		
		internal MessageEventArgs(NetIncomingMessage lidgrenMessage,
		                          IMessage message)
			: base(lidgrenMessage)
				=> Message = message;
	}
}

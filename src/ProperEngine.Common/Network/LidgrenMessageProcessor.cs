using System;
using System.Collections.Generic;
using Lidgren.Network;
using MessagePack;

namespace ProperEngine.Common.Network
{
	public delegate void MessageReceived<T>(T message) where T : IMessage;
	
	public class LidgrenMessageProcessor
	{
		public Side Side { get; }
		public NetPeer Peer { get; }
		public MessageRegistry Registry { get; }
		
		public event EventHandler<StatusChangedEventArgs> StatusChanged;
		public event EventHandler<DiagMessageEventArgs> DiagMessageReceived;
		public event EventHandler<MessageEventArgs> MessageReceived;
		
		public LidgrenMessageProcessor(Side side, NetPeer peer,
		                               MessageRegistry registry)
		{
			Side     = side;
			Peer     = peer;
			Registry = registry;
		}
		
		public void Update()
		{
			NetIncomingMessage inMsg;
			while ((inMsg = Peer.ReadMessage()) != null) {
				switch (inMsg.MessageType) {
					case NetIncomingMessageType.StatusChanged:
						StatusChanged?.Invoke(this, new StatusChangedEventArgs(inMsg));
						break;
					
					case NetIncomingMessageType.VerboseDebugMessage:
					case NetIncomingMessageType.DebugMessage:
					case NetIncomingMessageType.WarningMessage:
					case NetIncomingMessageType.ErrorMessage:
						DiagMessageReceived?.Invoke(this, new DiagMessageEventArgs(inMsg));
						break;
					
					case NetIncomingMessageType.Data:
						var position = inMsg.PositionInBytes;
						var length   = inMsg.LengthBytes - position;
						var data     = new ArraySegment<byte>(inMsg.Data, position, length);
						var message  = Registry.Read(Side, data);
						MessageReceived?.Invoke(this, new MessageEventArgs(inMsg, message));
						break;
					
					default: throw new NotSupportedException(
						$"Unhandled message type '{ inMsg.MessageType }'");
				}
				Peer.Recycle(inMsg);
			}
		}
	}
}

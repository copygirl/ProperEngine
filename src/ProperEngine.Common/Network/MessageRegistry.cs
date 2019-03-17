using System;
using System.Collections.Generic;
using Lidgren.Network;
using MessagePack;

namespace ProperEngine.Common.Network
{
	public class MessageRegistry
	{
		private readonly Dictionary<byte, Entry> _byID
			= new Dictionary<byte, Entry>();
		private readonly Dictionary<Type, Entry> _byType
			= new Dictionary<Type, Entry>();
		
		
		public void Register<T>(byte id, Side validReceivingSide,
		                               IMessageDeSerializer<T> deSerializer)
			where T : IMessage
		{
			if (deSerializer == null) throw new ArgumentNullException(nameof(deSerializer));
			if (_byID.ContainsKey(id)) throw new ArgumentException(
				$"Duplicate registry for message ID '{ id }'");
			if (_byType.ContainsKey(typeof(T))) throw new ArgumentException(
				$"Duplicate registry for message Type '{ typeof(T) }'");
			
			var entry = new Entry(id, typeof(T), validReceivingSide, deSerializer);
			_byID.Add(id, entry);
			_byType.Add(typeof(T), entry);
		}
		
		public void Register<T>(byte id, Side validReceivingSide)
			where T : IMessage
				=> Register(id, validReceivingSide, new DefaultDeSerializer<T>());
		
		
		public NetOutgoingMessage Write<T>(
				Side side, NetPeer peer, T message)
			where T : IMessage
		{
			if (!_byType.TryGetValue(typeof(T), out var entry)) throw new ArgumentException(
				$"Message type '{ typeof(T) } has not been registered", nameof(T));
			if (!side.IsValid(entry.ValidSendingSide)) throw new InvalidOperationException(
				$"Can't send message '{ typeof(T) }' from side '{ side }'");
			
			var serializer = (IMessageDeSerializer<T>)entry.DeSerializer;
			var data       = serializer.Write(message);
			var outMsg     = peer.CreateMessage(1 + data.Count);
			outMsg.Write(entry.ID);
			outMsg.Write(data.Array, data.Offset, data.Count);
			return outMsg;
		}
		
		public IMessage Read(Side side, ArraySegment<byte> data)
		{
			var id = data.Array[0];
			if (!_byID.TryGetValue(id, out var entry))
				throw new InvalidOperationException(
					$"Message ID '{ id }' not registered");
			if (!side.IsValid(entry.ValidReceivingSide)) throw new InvalidOperationException(
				$"Can't receive message '{ entry.Type }' on side '{ side }'");
			
			return entry.DeSerializer.Read(new ArraySegment<byte>(
				data.Array, data.Offset + 1, data.Count - 1));
		}
		
		
		public struct Entry
		{
			public byte ID { get; }
			public Type Type { get; }
			public Side ValidReceivingSide { get; }
			public Side ValidSendingSide { get; }
			public IMessageDeSerializer DeSerializer { get; }
			
			public Entry(byte id, Type type, Side validReceivingSide,
			             IMessageDeSerializer deSerializer)
			{
				ID   = id;
				Type = type;
				ValidReceivingSide = validReceivingSide;
				ValidSendingSide   = validReceivingSide.Opposite();
				DeSerializer       = deSerializer;
			}
		}
		
		
		public interface IMessageDeSerializer
		{
			IMessage Read(ArraySegment<byte> data);
		}
		
		public interface IMessageDeSerializer<T>
				: IMessageDeSerializer
			where T : IMessage
		{
			ArraySegment<byte> Write(T message);
		}
		
		private class DefaultDeSerializer<T> : IMessageDeSerializer<T>
			where T : IMessage
		{
			public IMessage Read(ArraySegment<byte> data)
				=> MessagePackSerializer.Deserialize<T>(data);
			public ArraySegment<byte> Write(T message)
				=> MessagePackSerializer.SerializeUnsafe(message);
		}
	}
}

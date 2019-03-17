using System.Runtime.Serialization;
using MessagePack;

namespace ProperEngine.Common.Network
{
	/// <summary>
	/// Interface for types which are meant to be sent over the network as messages,
	/// containing arbitrary payload. Implementing types must be marked with
	/// <see cref="DataContractAttribute"/> or <see cref="MessagePackObjectAttribute"/>, and
	/// fields / properties as <see cref="DataMemberAttribute"/> or <see cref="KeyAttribute"/>.
	/// </summary>
	public interface IMessage
	{
	}
}

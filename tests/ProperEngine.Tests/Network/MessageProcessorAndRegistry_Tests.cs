using System;
using System.Runtime.Serialization;
using System.Threading;
using Lidgren.Network;
using ProperEngine.Common;
using ProperEngine.Common.Network;
using Xunit;

namespace ProperEngine.Tests.Network
{
	public class MessageProcessorAndRegistry_Tests : IDisposable
	{
		public NetServer Server { get; }
		public NetClient Client { get; }
		public MessageRegistry Registry { get; }
		
		public MessageProcessorAndRegistry_Tests()
		{
			var port = 27272;
			Server = new NetServer(new NetPeerConfiguration("ProperEngine.Tests") { Port = port });
			Client = new NetClient(new NetPeerConfiguration("ProperEngine.Tests"));
			
			Registry = new MessageRegistry();
			Registry.Register<TestMessage>(0, Side.Both);
			Registry.Register<TestMessageToClient>(1, Side.Client);
			Registry.Register<TestMessageToServer>(2, Side.Server);
			
			Server.Start();
			Client.Start();
			Client.Connect("127.0.0.1", port);
			
			// Wait until server and client are fully connected to each other.
			while (Client.ConnectionStatus != NetConnectionStatus.Connected) {
				Server.WaitMessage(100);
				Client.WaitMessage(100);
			}
		}
		
		public void Dispose()
		{
			Server.Shutdown("");
			Client.Shutdown("");
			
			// Wait for server to fully shut down socket.
			while (Server.Status != NetPeerStatus.NotRunning)
				Thread.Sleep(5);
		}
		
		[Fact]
		public void Client_To_Server()
		{
			var processor = new LidgrenMessageProcessor(Side.Server, Server, Registry);
			var outMsg1 = Registry.Write(Side.Client, Client, new TestMessageToServer { Text = "Bar", Number = 30 });
			var outMsg2 = Registry.Write(Side.Client, Client, new TestMessage { Text = "Foo", Number = 20 });
			Assert.Throws<InvalidOperationException>(() => Registry.Write(Side.Client, Client, new TestMessageToClient()));
			
			Client.SendMessage(outMsg1, NetDeliveryMethod.ReliableOrdered);
			Client.SendMessage(outMsg2, NetDeliveryMethod.ReliableOrdered);
			
			int numMessage = 0;
			processor.MessageReceived += (_, args) => {
				numMessage++;
				switch (args.Message) {
					case TestMessageToServer msg:
						Assert.Equal(1, numMessage);
						Assert.Equal("Bar", msg.Text);
						Assert.Equal(30, msg.Number);
						break;
					case TestMessage msg:
						Assert.Equal(2, numMessage);
						Assert.Equal("Foo", msg.Text);
						Assert.Equal(20, msg.Number);
						break;
				}
			};
			
			// Wait a maximum of 25ms for the messages to arrive.
			// (Unsure if necessary, but.. just in case.)
			for (var i = 0; (i < 5) && (numMessage < 2); i++) {
				processor.Update();
				Thread.Sleep(5);
			}
			
			Assert.Equal(2, numMessage);
		}
		
		[Fact]
		public void Server_To_Client()
		{
			var processor = new LidgrenMessageProcessor(Side.Client, Client, Registry);
			var outMsg1 = Registry.Write(Side.Server, Server, new TestMessageToClient { Text = "Bar", Number = 30 });
			var outMsg2 = Registry.Write(Side.Server, Server, new TestMessage { Text = "Foo", Number = 20 });
			Assert.Throws<InvalidOperationException>(() => Registry.Write(Side.Server, Server, new TestMessageToServer()));
			
			var recipient = Server.Connections[0];
			Server.SendMessage(outMsg1, recipient, NetDeliveryMethod.ReliableOrdered);
			Server.SendMessage(outMsg2, recipient, NetDeliveryMethod.ReliableOrdered);
			
			int numMessage = 0;
			processor.MessageReceived += (_, args) => {
				numMessage++;
				switch (args.Message) {
					case TestMessageToClient msg:
						Assert.Equal(1, numMessage);
						Assert.Equal("Bar", msg.Text);
						Assert.Equal(30, msg.Number);
						break;
					case TestMessage msg:
						Assert.Equal(2, numMessage);
						Assert.Equal("Foo", msg.Text);
						Assert.Equal(20, msg.Number);
						break;
				}
			};
			
			// Wait a maximum of 25ms for the messages to arrive.
			// (Unsure if necessary, but.. just in case.)
			for (var i = 0; (i < 5) && (numMessage < 2); i++) {
				processor.Update();
				Thread.Sleep(5);
			}
			
			Assert.Equal(2, numMessage);
		}
		
		[DataContract]
		public class TestMessage : IMessage
		{
			[DataMember] public string Text { get; set; }
			[DataMember] public int Number { get; set; }
		}
		[DataContract]
		public class TestMessageToClient : TestMessage {  }
		[DataContract]
		public class TestMessageToServer : TestMessage {  }
	}
}

namespace Synthicate
{
	public class ConnectionRequest
	{
		public string ipAddress {get;}
		public ushort port {get;}
		public string playerName {get;}
		public ConnectionRequest(string ipAddress, ushort port, string playerName)
		{
			this.ipAddress = ipAddress;
			this.port = port;
			this.playerName = playerName;
		}
	}
}
namespace Synthicate
{
	public class ConnectionRequest
	{
		public string ipAddress {get;}
		public uint port {get;}
		public string playerName {get;}
		public ConnectionRequest(string ipAddress, uint port, string playerName)
		{
			this.ipAddress = ipAddress;
			this.port = port;
			this.playerName = playerName;
		}
	}
}
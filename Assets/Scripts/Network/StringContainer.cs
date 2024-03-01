using Unity.Netcode;

public class StringContainer : INetworkSerializable
{
	public string text;
	public StringContainer()
	{
		this.text = "";
	}
	public StringContainer(string text)
	{
		this.text = text;
	}
	public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
	{
		if (serializer.IsWriter)
		{
			serializer.GetFastBufferWriter().WriteValueSafe(text);
		}
		else
		{
			serializer.GetFastBufferReader().ReadValueSafe(out text);
		}
	}
}
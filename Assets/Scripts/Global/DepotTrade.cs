namespace Synthicate
{
	public struct DepotTradeRequest
	{
		public ResourceType requestedResource;
		public uint amount;

		public DepotTradeRequest(ResourceType requestedResource, uint amount)
		{
			this.requestedResource = requestedResource;
			this.amount = amount;
		}
	}
	public struct DepotTrade
	{
		public ResourceType requestedResource;
		public ResourceType offeredResource;
		public uint offeredAmount;

		public DepotTrade(ResourceType requestedResource, ResourceType offeredResource, uint offeredAmount)
		{
			this.requestedResource = requestedResource;
			this.offeredResource = offeredResource;
			this.offeredAmount = offeredAmount;
		}
	}
	
	public struct DepotSelection
	{
		public int DepotId;
		public ResourceType Resource;
		public int RequiredAmount;
		
		public DepotSelection(int id, ResourceType resource, int amount)
		{
			DepotId = id;
			Resource = resource;
			RequiredAmount = amount;
		}

	}
	
}
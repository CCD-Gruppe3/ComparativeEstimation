namespace ComparativeEstimation
{
	public class PairRanking
	{
		public int LeftId { get; }
		public int RightId { get; }

		public PairRanking(int leftId, int rightId)
		{
			LeftId = leftId;
			RightId = rightId;
		}
	}
}
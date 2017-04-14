using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Contracts;

namespace ComparativeEstimation
{
	public class Pair : IEquatable<Pair>
	{
		public readonly IUserStory Left;
		public readonly IUserStory Right;

		public Pair(IUserStory left, IUserStory right)
		{
			this.Left = left;
			this.Right = right;
		}

		public override string ToString()
		{
			return $"{Left} - {Right}";
		}

		public bool Equals(Pair other)
		{
			if (ReferenceEquals(null, other)) return false;
			if (ReferenceEquals(this, other)) return true;
			return Equals(Left, other.Left) && Equals(Right, other.Right);
		}

		#region Comparer 
		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			if (obj.GetType() != this.GetType()) return false;
			return Equals((Pair) obj);
		}

		public override int GetHashCode()
		{
			unchecked
			{
				return ((Left != null ? Left.GetHashCode() : 0)*397) ^ (Right != null ? Right.GetHashCode() : 0);
			}
		}

		public static bool operator ==(Pair left, Pair right)
		{
			return Equals(left, right);
		}

		public static bool operator !=(Pair left, Pair right)
		{
			return !Equals(left, right);
		}


		private sealed class LeftRightEqualityComparer : IEqualityComparer<Pair>
		{
			public bool Equals(Pair x, Pair y)
			{
				if (ReferenceEquals(x, y)) return true;
				if (ReferenceEquals(x, null)) return false;
				if (ReferenceEquals(y, null)) return false;
				if (x.GetType() != y.GetType()) return false;
				return Equals(x.Left, y.Left) && Equals(x.Right, y.Right);
			}

			public int GetHashCode(Pair obj)
			{
				unchecked
				{
					return ((obj.Left != null ? obj.Left.GetHashCode() : 0)*397) ^ (obj.Right != null ? obj.Right.GetHashCode() : 0);
				}
			}
		}

		private static readonly IEqualityComparer<Pair> LeftRightComparerInstance = new LeftRightEqualityComparer();

		public static IEqualityComparer<Pair> LeftRightComparer
		{
			get { return LeftRightComparerInstance; }
		}
		#endregion

	}
}

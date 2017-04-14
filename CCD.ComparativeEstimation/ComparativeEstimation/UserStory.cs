using System;
using System.Collections.Generic;
using Contracts;

namespace ComparativeEstimation
{
	public class UserStory : IUserStory, IEquatable<UserStory>
	{
		public int Id { get; set; }
		public string Title { get; set; }

		public UserStory(int id, string title)
		{
			Id = id;
			Title = title;
		}

		public override string ToString()
		{
			return $"[{Id}] {Title}";
		}


		public bool Equals(UserStory other)
		{
			if (ReferenceEquals(null, other)) return false;
			if (ReferenceEquals(this, other)) return true;
			return Id == other.Id;
		}

		#region comparer
		private sealed class IdEqualityComparer : IEqualityComparer<UserStory>
		{
			public bool Equals(UserStory x, UserStory y)
			{
				if (ReferenceEquals(x, y)) return true;
				if (ReferenceEquals(x, null)) return false;
				if (ReferenceEquals(y, null)) return false;
				if (x.GetType() != y.GetType()) return false;
				return x.Id == y.Id;
			}

			public int GetHashCode(UserStory obj)
			{
				return obj.Id;
			}
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			if (obj.GetType() != this.GetType()) return false;
			return Equals((UserStory) obj);
		}

		public override int GetHashCode()
		{
			return Id;
		}

		public static bool operator ==(UserStory left, UserStory right)
		{
			return Equals(left, right);
		}

		public static bool operator !=(UserStory left, UserStory right)
		{
			return !Equals(left, right);
		}

		private static readonly IEqualityComparer<UserStory> IdComparerInstance = new IdEqualityComparer();

		public static IEqualityComparer<UserStory> IdComparer
		{
			get { return IdComparerInstance; }
		}
		#endregion
	}
}
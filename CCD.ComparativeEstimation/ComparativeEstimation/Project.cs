using Contracts;
using Newtonsoft.Json;

namespace ComparativeEstimation
{
	public class Project : IProject
	{

		[JsonConstructor]
		public Project(string id, int number, string title, string email, IUserStory[] userStories)
		{
			Id = id;
			Number = number;
			Title = title;
			Email = email;
			UserStories = userStories;
		}

		public Project(int number, string title, string email, IUserStory[] userStories)
		{
			Number = number;
			Title = title;
			Email = email;
			UserStories = userStories;
		}
		
		public string Id { get; set; }
		public int Number { get; set; }
		public string Title { get; set; }
		public string Email { get; set; }
		public IUserStory[] UserStories { get; set; }
	}
}
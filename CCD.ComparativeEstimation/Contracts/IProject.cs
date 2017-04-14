namespace Contracts
{
	public interface IProjectInfo
	{
		string Id { get; set; }
		int Number { get; set; }
		string Title { get; set; }
	}

	public interface IProject : IProjectInfo
	{
		IUserStory[] UserStories { get; set; }
		string Email { get; set; }
	}
}
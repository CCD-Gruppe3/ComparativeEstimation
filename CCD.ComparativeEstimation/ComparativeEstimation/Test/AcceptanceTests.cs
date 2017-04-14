using System;
using System.Collections.Generic;
using System.Linq;
using Contracts;
using Moq;
using Xunit;

namespace ComparativeEstimation.Test
{
	public class AcceptanceTests
	{
		DbProvider.DbProvider Provider = new DbProvider.DbProvider(@"Test\TestData");

		[Fact]
		public void TestLogin()
		{
			var state = new SessionState();
			var handler = new RequestHandler(state, null);


			Assert.True(handler.Login("jo@cp.ag"));
			Assert.Equal("jo@cp.ag", state.Email);

			Assert.True(handler.Login("tj@cp.ag"));
			Assert.Equal("tj@cp.ag", state.Email);

			Assert.False(handler.Login("123"));
			Assert.Equal(null, state.Email);
		}


		[Fact]
		public void TestGetQueryGetPairs()
		{
			var handler = new RequestHandler(null, Provider);

			var id = "project_1";
			var userStoryA = new UserStory(1, "Blaa");
			var userStoryB = new UserStory(2, "Blubb");
			var userStoryC = new UserStory(3, "Laber");
			var userStoryD = new UserStory(4, "Rhabarber");
			
			var result =
				new[]
				{
					new Pair(userStoryA, userStoryB),
					new Pair(userStoryA, userStoryC),
					new Pair(userStoryA, userStoryD),
					new Pair(userStoryB, userStoryC),
					new Pair(userStoryB, userStoryD),
					new Pair(userStoryC, userStoryD)
				};
			var queryGetPairs = handler.QueryGetPairs(id).ToArray();
			Assert.Equal(result[0], queryGetPairs[0]);
			Assert.Equal(result, queryGetPairs);
		}

		[Fact]
		public void TestRankingAbgeben()
		{
			var state = new SessionState();
			var handler = new RequestHandler(state, Provider);

			handler.Login("bro@cp.ag");

			var goodRankings = new[]
			{
				new PairRanking(1, 2),
				new PairRanking(3, 2),
				new PairRanking(3, 1),
				new PairRanking(2, 4),
				new PairRanking(3, 4),
				new PairRanking(1, 4)
			};

			Assert.Equal(InputResult.Successful, handler.RankingAbgeben("Project_1", goodRankings));
			
			var badRankings = new[]
			{
				new PairRanking(1, 2),
				new PairRanking(3, 2),
				new PairRanking(3, 1),
				new PairRanking(2, 4),
				new PairRanking(4, 3), //fehler
				new PairRanking(4, 1) //fehler
			};

			// Wegen Dummyimplenetierung ist der Test hier rot!!!
			Assert.Equal(InputResult.Inconsistent, handler.RankingAbgeben("Project_1", badRankings));
		}

		[Fact]
		public void TestNewProject()
		{
			var state = new SessionState();
			var handler = new RequestHandler(state, Provider);

			handler.Login("jo@cp.ag");

			var projectIdsBeforeAdd = handler.ListProjects().Select(p => p.Id).ToList();
			var projectNumbersBeforeAdd = handler.ListProjects().Select(p => p.Number).ToList();

			var newId = handler.NewProject("neues Projekt", new List<UserStory>
			{
				new UserStory(1, "UserStory 1"),
				new UserStory(2, "UserStory 2")
			});

			Assert.False(string.IsNullOrEmpty(newId));
			Assert.False(projectIdsBeforeAdd.Contains(newId));

			var project = handler.LoadProject(newId);
			Assert.NotNull(project);
			Assert.Equal(newId, project.Id);
			Assert.False(projectNumbersBeforeAdd.Contains(project.Number));

			Assert.Equal("neues Projekt", project.Title);
			Assert.Equal(state.Email, project.Email);
			Assert.Equal(2, project.UserStories.Count());
		}

		[Fact]
		public void TestPersoehnlichesRankingLesen()
		{
			//Arrange
			var state = new SessionState();
			var handler = new RequestHandler(state, Provider);

			var userStoryA = new UserStory(1, "Blaa");
			var userStoryB = new UserStory(2, "Blubb");
			var userStoryC = new UserStory(3, "Laber");
			var userStoryD = new UserStory(4, "Rhabarber");

			var expected = new List<IUserStory>() { userStoryA, userStoryC, userStoryD, userStoryB};
			var projectId = "Project_1";

			//Act
			handler.Login("jo@cp.ag");

			IList<IUserStory> result = handler.LoadPersonalRanking(projectId);

			//Assert
			Assert.Equal(expected ,result);
		}

	}
}
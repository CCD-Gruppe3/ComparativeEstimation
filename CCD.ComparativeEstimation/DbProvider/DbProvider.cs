using System;
using System.Collections.Generic;
using System.IO;
using Contracts;
using Newtonsoft.Json;

namespace DbProvider
{
    public class DbProvider
    {

	    private readonly string rankingPath = "Rankings";
	    private readonly string basePath;

	    public DbProvider(string basePath)
	    {
		    this.basePath = basePath;
	    }


	    public IEnumerable<int[]> LoadProjectRankings(string id)
	    {
		    var path = Path.Combine(BuildProjectPath(id), rankingPath);
			
			var dirInfo = new DirectoryInfo(path);

		    foreach (var fileInfo in dirInfo.GetFiles())
		    {
			    var fileContent = File.ReadAllText(fileInfo.FullName);
			    yield return JsonConvert.DeserializeObject<int[]>(fileContent);
		    }
	    }

		public void SaveProjectRanking(string email, string id, int[] userRanking)
		{
			var path = Path.Combine(BuildProjectPath(id), rankingPath, email + ".txt");
			var fileContent = JsonConvert.SerializeObject(userRanking);
			File.WriteAllText(path, fileContent);
		}

	    public int[] LoadUserRanking(string email, string id)
	    {
			var path = Path.Combine(BuildProjectPath(id), rankingPath, email + ".txt");
			var fileContent = File.ReadAllText(path);
			return JsonConvert.DeserializeObject<int[]>(fileContent);
		}

	    private string BuildProjectPath(string id) => Path.Combine(basePath, id);

	    private string BuildProjectFilePath(string id) => Path.Combine(basePath, id, "project.txt");
	    
	    public IProject LoadProject(string id)
	    {
		    var content =  File.ReadAllText(BuildProjectFilePath(id));

		    var setting = new JsonSerializerSettings
		    {
			    TypeNameHandling = TypeNameHandling.Objects
		    };

		    return JsonConvert.DeserializeObject<IProject>(content, setting);
	    }

	    public string SaveProject(IProject project)
	    {

		    var id = Guid.NewGuid().ToString();
			project.Id = id;

		    var setting = new JsonSerializerSettings
		    {
				TypeNameHandling = TypeNameHandling.Objects
		    };

		    var content =  JsonConvert.SerializeObject(project, setting);
			Directory.CreateDirectory(BuildProjectPath(id));

		    File.WriteAllText(BuildProjectFilePath(id), content);

		    return id;
	    }

	    public IEnumerable<IProject> LoadProjects(string email)
	    {
			var dir = new DirectoryInfo(basePath);
		    foreach (var projectDirectory in dir.GetDirectories())
		    {
			    var project = LoadProject(projectDirectory.Name);
			    if (project.Email == email)
				    yield return project;
		    }
	    }

	    public string GetProjectId(int projectNumber)
	    {
			var dir = new DirectoryInfo(basePath);
			foreach (var projectDirectory in dir.GetDirectories())
			{
				var project = LoadProject(projectDirectory.Name);
				if (project.Number == projectNumber)
					return project.Id;
			}

		    return null;
	    }
    }
}

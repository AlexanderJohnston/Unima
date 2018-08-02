﻿using System.IO;
using Anotar.Log4Net;
using Cama.Core.Models.Mutation;
using Newtonsoft.Json;

namespace Cama.Core.Services.Project
{
    public class ProjectService : ICreateProjectService, IOpenProjectService
    {
        public void CreateProject(CamaConfig config)
        {
            var directoryPath = Path.GetDirectoryName(config.ProjectPath);
            LogTo.Info($"Creating project directory at {directoryPath}");
            Directory.CreateDirectory(directoryPath);
            LogTo.Info("Creating neccessary project files");
            File.WriteAllText(config.ProjectPath, JsonConvert.SerializeObject(config));
        }

        public CamaConfig OpenProject(string path)
        {
            LogTo.Info($"Opening project at {path}");
            return JsonConvert.DeserializeObject<CamaConfig>(File.ReadAllText(path));
        }
    }
}
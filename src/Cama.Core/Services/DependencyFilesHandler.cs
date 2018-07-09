﻿using System.IO;
using System.Linq;

namespace Cama.Core.Services
{
    public class DependencyFilesHandler
    {
        public void CopyDependencies(string path)
        {
            var files = Directory.GetFiles(@"D:\Programmering\Testura\Testura.Code\src\Testura.Code.Tests\bin\Debug");
            foreach (var file in files.Where(f => f.EndsWith(".dll") && !f.Contains("Testura.Code.dll")))
            {
                File.Copy(file, Path.Combine(path, Path.GetFileName(file)), true);
            }
        }
    }
}

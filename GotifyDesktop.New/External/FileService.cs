using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace GotifyDesktop.New.External
{
    public class FileService
    {
        public FileService()
        {

        }

        public async Task SaveSettingsFileAsync(string path, string fileToWrite)
        {
            await File.WriteAllTextAsync(path, fileToWrite);
        }

        public async Task<string> LoadSettingsFileAsync(string path) 
        {
            return await File.ReadAllTextAsync(path);
        }
    }
}

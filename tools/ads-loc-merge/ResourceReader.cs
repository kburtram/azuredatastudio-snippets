//
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
//

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using Newtonsoft.Json;

namespace AzureDataStudio.Localization
{
    /// <summary>
    /// Resource Reader
    /// </summary>
    public class ResourceReader
    {
        private string directory;
        private PathMap pathMap;

        private List<string> resourceFiles = new List<string>();

        public Dictionary<string, Dictionary<string, object>> resourceList = new Dictionary<string, Dictionary<string, object>>();

        public ResourceReader(string directory, PathMap pathMap)
        {
            this.directory = directory.ToLower();
            this.pathMap = pathMap;
        }

        public void Read()
        {
            DirectoryWalk(this.directory);
        }

        private void DirectoryWalk(string dir)
        {
            try
            {
                foreach (string d in Directory.GetDirectories(dir))
                {
                    foreach (string f in Directory.GetFiles(d))
                    {
                        string extension = ".i18n.json";
                        if (f.EndsWith(extension))
                        {
                            int startIndex = this.directory.Length + ("\\src\\".Length);
                            int length = f.Length - startIndex - extension.Length;
                            string parsedFile = f.Substring(startIndex, length);
                            parsedFile = parsedFile.Replace("\\", "/").ToLower();
                            parsedFile = this.pathMap.Map(parsedFile);
                            string resourceContent = File.ReadAllText(f);
                            Dictionary<string, object> resourceMap = JsonConvert.DeserializeObject<Dictionary<string, object>>(resourceContent);
                            if (resourceMap != null && resourceMap.Keys.Count > 0)
                            {
                                resourceList.Add(parsedFile, resourceMap);
                            }

                            resourceFiles.Add(parsedFile);
                            Console.WriteLine(f);
                        }
                    }
                    DirectoryWalk(d);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

    }
}

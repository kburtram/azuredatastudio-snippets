//
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
//

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Xml.Serialization;
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
        private string directoryPrefix;
        private string extension;

        private List<string> resourceFiles = new List<string>();

        public Dictionary<string, Dictionary<string, object>> resourceList = new Dictionary<string, Dictionary<string, object>>();
        
        public Dictionary<string, Xliff> xlfResourceList = new Dictionary<string, Xliff>();

        public ResourceReader(string directory, PathMap pathMap = null, string directoryPrefix = "\\src\\", string fileExtension = ".i18n.json")
        {
            this.directory = directory;
            this.pathMap = pathMap;
            this.directoryPrefix = directoryPrefix;
            this.extension = fileExtension;
        }

        public void Read(bool expectJson = true)
        {
            DirectoryWalk(this.directory + directoryPrefix, directoryPrefix.Length, expectJson);
        }

        public string[] FindResources(string filePath)
        {
            string fileName = Path.GetFileNameWithoutExtension(filePath);
            List<string> matches = new List<string>();
            foreach (var resourceFile in resourceFiles)
            {
                string resourceName = resourceFile;
                int slashPos = resourceFile.LastIndexOf("/");
                if (slashPos > 0)
                {
                    resourceName = resourceName.Substring(slashPos + 1);
                }

                if (string.Equals(fileName, resourceName, StringComparison.OrdinalIgnoreCase))
                {
                    matches.Add(resourceFile);
                }                        
            }
            return matches.ToArray();
        }

        public string CleanUpPath(int startIndex, string f, string extension)
        {            
            int length = f.Length - startIndex - extension.Length;
            string parsedFile = f.Substring(startIndex, length);
            parsedFile = parsedFile.Replace("\\", "/");
            return parsedFile;
        }

        private void DirectoryWalk(string dir, int prefixLen, bool expectJson = true)
        {
            try
            {
                // check files at current level before going into lower level directories 
                foreach (string f in Directory.GetFiles(dir))
                {
                    if (f.EndsWith(extension, StringComparison.OrdinalIgnoreCase))
                    {
                        int startIndex = this.directory.Length + (prefixLen);
                        string parsedFile = CleanUpPath(startIndex, f, extension);
                        if (this.pathMap != null)
                        {
                            parsedFile = this.pathMap.Map(parsedFile);
                        }

                        string resourceContent = File.ReadAllText(f);
                        if (expectJson)
                        {
                            Dictionary<string, object> resourceMap = JsonConvert.DeserializeObject<Dictionary<string, object>>(resourceContent);
                            if (resourceMap != null && resourceMap.Keys.Count > 0)
                            {
                                resourceList.Add(parsedFile, resourceMap);
                            }
                        }
                        else
                        {
                            XmlRootAttribute xRoot = new XmlRootAttribute();
                            xRoot.ElementName = "xliff";
                            xRoot.Namespace = "urn:oasis:names:tc:xliff:document:1.2";
                            xRoot.IsNullable = true;

                            System.IO.StreamReader file = new System.IO.StreamReader(f);
                            System.Xml.Serialization.XmlSerializer reader = new System.Xml.Serialization.XmlSerializer(typeof(Xliff), xRoot);
                            Xliff obj = (Xliff)reader.Deserialize(file);
                            if(obj != null)
                            {
                                this.xlfResourceList.Add(parsedFile, obj);
                            }
                        }

                        resourceFiles.Add(parsedFile);
                        Console.WriteLine(f);
                    }
                }
                // repeat for lower levels
                foreach (string d in Directory.GetDirectories(dir))
                {
                    DirectoryWalk(d, prefixLen);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}

//
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using Newtonsoft.Json;

namespace AzureDataStudio.Localization
{
    public class Mapping
    {
        public string To { get; set; }

        public string From { get; set; }
    }

    public class PathMap
    {
        public Mapping[] Mappings { get; set; }

        public string Map(string path)
        {
            foreach (Mapping mapping in this.Mappings)
            {
                if (string.Equals(mapping.From, path, StringComparison.OrdinalIgnoreCase))
                {
                    return mapping.To;
                }
            }
            return path;
        }
    }

    public class PathMapper
    {
        private string sourcePath;
        private ResourceReader resourceReader;

        private int files = 0;

        public PathMapper(string sourcePath, ResourceReader resourceReader)
        {
            this.sourcePath = sourcePath;
            this.resourceReader = resourceReader;
        }

        public void Write(string path)
        {
            files = 0;

            StringBuilder sb = new StringBuilder();

            sb.AppendLine("{" + Environment.NewLine + "  \"mappings\": [");

            DirectoryWalk(sb, this.sourcePath);
            
            sb.AppendLine(Environment.NewLine + "  ]" + Environment.NewLine + "}");

            if (File.Exists(path))
            {
                File.Delete(path);
            }

            File.WriteAllText(path, sb.ToString());
        }        

        private void DirectoryWalk(StringBuilder sb, string dir)
        {
            try
            {
                foreach (string d in Directory.GetDirectories(dir))
                {                    
                    foreach (string f in Directory.GetFiles(d))
                    {
                        int startIndex = this.sourcePath.Length - "sql".Length;
                        string parsedPath = this.resourceReader.CleanUpPath(startIndex, f, Path.GetExtension(f));
                        string[] matches = this.resourceReader.FindResources(f);
                        int matchesCount = 0;
                        foreach (var match in matches)
                        {                                              
                            if (!string.Equals(parsedPath, match, StringComparison.OrdinalIgnoreCase)) 
                            {   
                                string prefix = string.Empty;
                                if (matchesCount++ > 0)
                                {
                                    prefix = "****-";
                                }

                                if (files++ > 0)
                                {
                                    sb.AppendLine(",");
                                }
                                sb.AppendLine("    { \"from\": \"" + prefix + match + "\", ");
                                sb.Append("      \"to\": \"" + parsedPath + "\" }");
                            }
                        }
                    }
                    DirectoryWalk(sb, d);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}
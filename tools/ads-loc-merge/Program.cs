//
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
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

    /// <summary>
    /// Main application classs
    /// </summary>
    internal class Program
    {
        /// <summary>
        /// Main entry point into the SQL Tools API Service Layer
        /// </summary>
        internal static void Main(string[] args)
        {
            // read command-line arguments
            CommandOptions commandOptions = new CommandOptions(args, "ADS Localization Resource Merge");
            if (commandOptions.ShouldExit)
            {
                return;
            }

            if (string.Equals(commandOptions.Action, CommandOptions.DefaultAction, StringComparison.OrdinalIgnoreCase))
            {
                PathMap mappings = null;
                string mappingPath = commandOptions.PathMapping;
                if (File.Exists(mappingPath))
                {
                    string mappingsContent = File.ReadAllText(mappingPath);
                    mappings = JsonConvert.DeserializeObject<PathMap>(mappingsContent);
                    Console.WriteLine("name = " + mappings.ToString());               
                }

                ResourceReader resourceReader = new ResourceReader(
                    commandOptions.ResourceDirectoryPath, 
                    mappings);
                resourceReader.Read();

                LangPackMerger merger = new LangPackMerger(commandOptions.LanguagePackDirectory);
                merger.Merge(resourceReader);
            }
            else
            {
                Console.WriteLine("Unknown action");
            }
        }
    }
}

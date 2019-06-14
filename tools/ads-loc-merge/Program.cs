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

            if (string.Equals(
                commandOptions.Action, 
                CommandOptions.DefaultAction, 
                StringComparison.OrdinalIgnoreCase))
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
            else if (string.Equals(
                commandOptions.Action,
                CommandOptions.PathMapAction,
                StringComparison.OrdinalIgnoreCase))
            {
                ResourceReader resourceReader = new ResourceReader(commandOptions.ResourceDirectoryPath);
                resourceReader.Read();

                PathMapper mapper = new PathMapper(commandOptions.SourceDirectoryPath, resourceReader);
                mapper.Write(commandOptions.PathMapping);
            }
            else
            {
                Console.WriteLine("Unknown action");
            }
        }
    }
}

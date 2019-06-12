//
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
//

using System;
using System.Globalization;
using System.IO;

namespace AzureDataStudio.Localization
{
    /// <summary>
    /// The command-line options helper class.
    /// </summary>
    public class CommandOptions
    {
        public static readonly string DefaultAction = "default";

        /// <summary>
        /// Construct and parse command line options from the arguments array
        /// </summary>
        public CommandOptions(string[] args, string serviceName)
        {
            this.ErrorMessage = string.Empty;
            this.ServiceName = serviceName;
            this.Action = CommandOptions.DefaultAction;

            try
            {
                for (int i = 0; i < args.Length; ++i)
                {
                    string arg = args[i];
                    if (arg != null && arg.StartsWith("--") && (i + 1) < args.Length)
                    {
                        // Extracting arguments and properties
                        arg = arg.Substring(2).ToLowerInvariant();
                        string argName = arg;

                        switch (argName)
                        {
                            case "action":
                                this.Action = args[++i];
                                break;       
                            case "langpack-dir":
                                this.LanguagePackDirectory = args[++i];
                                break;
                            case "resource-dir":
                                this.ResourceDirectoryPath = args[++i];
                                break;
                            case "path-mapping":
                                this.PathMapping = args[++i];
                                break;                      
                            default:
                                ErrorMessage += string.Format("Unknown argument \"{0}\"" + Environment.NewLine, argName);
                                break;
                        }
                    }
                }

                if (string.IsNullOrWhiteSpace(this.LanguagePackDirectory)
                    || string.IsNullOrWhiteSpace(this.ResourceDirectoryPath)
                    || string.IsNullOrWhiteSpace(this.PathMapping))
                {
                    this.ErrorMessage = "Missing required arguments";
                }
            }
            catch (Exception ex)
            {
                this.ErrorMessage += ex.ToString();
                return;
            }
            finally
            {
                if (!string.IsNullOrEmpty(ErrorMessage) || this.ShouldExit)
                {
                    Console.WriteLine(Usage);
                    this.ShouldExit = true;
                }
            }
        }

        /// <summary>
        /// Whether the program should exit immediately. Set to true when the usage is printed.
        /// </summary>
        public bool ShouldExit { get; private set; }

        /// <summary>
        /// Name of service that is receiving command options
        /// </summary>
        public string ServiceName { get; private set; }

        /// <summary>
        /// LangPack directory path
        /// </summary>
        public string LanguagePackDirectory { get; private set; }

        /// <summary>
        /// resource directory path
        /// </summary>
        public string ResourceDirectoryPath { get; private set; }

        /// <summary>
        /// application action
        /// </summary>
        public string Action { get; private set; }

        /// <summary>
        /// Contains any error messages during execution
        /// </summary>
        public string ErrorMessage { get; private set; }

        /// <summary>
        /// Path mappings for files that have moved
        /// </summary>
        public string PathMapping { get; private set; }

        /// <summary>
        /// Get the usage string describing command-line arguments for the program
        /// </summary>
        public string Usage
        {
            get
            {
                var str = string.Format("{0}" + Environment.NewLine +
                    ServiceName + " " + Environment.NewLine +
                    "   Options:" + Environment.NewLine +
                    "        --langpack-dir [DIRECTORY]" + Environment.NewLine +
                    "        --resource-dir [DIRECTORY]" + Environment.NewLine,
                    "        --path-mapping [FILE]" + Environment.NewLine,               
                    this.ErrorMessage);
                return str;
            }
        }
    }
}

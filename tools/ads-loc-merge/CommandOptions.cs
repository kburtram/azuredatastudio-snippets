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

        public static readonly string PathMapAction = "pathmap";

        public static readonly string ConvertAction = "convert";

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
                            case "source-dir":
                                this.SourceDirectoryPath = args[++i];
                                break;
                            case "path-mapping":
                                this.PathMapping = args[++i];
                                break;
                            case "xlf-dir":
                                this.XlfDirectoryPath = args[++i];
                                break;
                            default:
                                ErrorMessage += string.Format("Unknown argument \"{0}\"" + Environment.NewLine, argName);
                                break;
                        }
                    }
                }

                if (!CheckRequiredArguments())
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

        private bool CheckRequiredArguments()
        {
            if (string.Equals(this.Action, CommandOptions.DefaultAction, StringComparison.OrdinalIgnoreCase)
                && (string.IsNullOrWhiteSpace(this.LanguagePackDirectory)
                || string.IsNullOrWhiteSpace(this.ResourceDirectoryPath)
                || string.IsNullOrWhiteSpace(this.PathMapping)))
            {
                return false;
            }

            if (string.Equals(this.Action, CommandOptions.PathMapAction, StringComparison.OrdinalIgnoreCase)
                && (string.IsNullOrWhiteSpace(this.SourceDirectoryPath)
                || string.IsNullOrWhiteSpace(this.ResourceDirectoryPath)
                || string.IsNullOrWhiteSpace(this.PathMapping)))
            {
                return false;
            }

            if (string.Equals(this.Action, CommandOptions.ConvertAction, StringComparison.OrdinalIgnoreCase)
                && (string.IsNullOrWhiteSpace(this.XlfDirectoryPath)
                || string.IsNullOrWhiteSpace(this.LanguagePackDirectory)))
            {
                return false;
            }

            return true;
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
        /// source directory path
        /// </summary>
        public string SourceDirectoryPath { get; private set; }

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
        /// Folder to look for xlf files
        /// </summary>
        public string XlfDirectoryPath { get; private set; }
        
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
                    "        --resource-dir [DIRECTORY]" + Environment.NewLine +
                    "        --path-mapping [FILE]" + Environment.NewLine +
                    "        --source-mapping [FILE]" + Environment.NewLine +
                    "        --xlf-dir [DIRECTORY]" + Environment.NewLine + 
                    "        --action [default|pathmap|convert]" + Environment.NewLine,
                    this.ErrorMessage);
                return str;
            }
        }
    }
}

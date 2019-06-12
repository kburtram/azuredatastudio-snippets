//
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
//

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace AzureDataStudio.Localization
{
    /// <summary>
    /// Language Pack Reader
    /// </summary>
    public class LangPackMerger
    {
        private string directory;

        private Dictionary<string, object> resourceMap;

        public LangPackMerger(string directory)
        {
            this.directory = directory;
        }

        private void Write(string filePath)
        {
            if (this.resourceMap == null)
            {
                return;
            }

            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }

            string json = JsonConvert.SerializeObject(resourceMap, Formatting.Indented);
            File.WriteAllText(filePath, json);
        }

        public void Merge(ResourceReader resources)
        {
            string mainFile = Path.Combine(this.directory, "translations\\main.i18n.json");
            if (!File.Exists(mainFile))
            {
                Console.WriteLine("Could not find file " + mainFile);
                return;
            }

            string mainFileContent = File.ReadAllText(mainFile);
            resourceMap = JsonConvert.DeserializeObject<Dictionary<string, object>>(mainFileContent);

            JToken contents = resourceMap["contents"] as JToken;
            foreach (var file in resources.resourceList.Keys)
            {
                StringBuilder sb = new StringBuilder();
                sb.AppendLine("{\"" + file + "\": { ");
                Dictionary<string, object> filemap = resources.resourceList[file];
                if (filemap.Keys.Count <= 1)
                {
                    continue;
                }

                int i = 0;
                int skip = 1;
                foreach (var propName in filemap.Keys)
                {
                    if (string.IsNullOrWhiteSpace(propName))
                    {
                        ++skip;
                        continue;
                    }
                    string propValue = filemap[propName] as string;
                    sb.Append("\"" + propName + "\": "  + JsonConvert.ToString(propValue));
                    if (i < filemap.Keys.Count - skip)
                    {
                       sb.AppendLine(","); 
                    }
                    else 
                    {
                        sb.AppendLine(""); 
                    }
                    ++i;
                }
                sb.Append("}}");

                var resourceObject = JObject.Parse(sb.ToString());

                contents.Last.AddAfterSelf(resourceObject.First);
            }

            Write(mainFile + ".output");
        }
    }
}

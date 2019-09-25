//
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
//

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Cryptography;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;
using Newtonsoft.Json;

namespace AzureDataStudio.Localization
{
    /// <summary>
    /// Xml Object to Json Converter
    /// </summary>
    public class XmlTojsonConverter
    {
        private string sourceFolder;
        private string destinationFolder;
        private ResourceReader resourceReader;

        public XmlTojsonConverter(string source, string destination, ResourceReader reader)
        {
            sourceFolder = source;
            destinationFolder = destination;
            resourceReader = reader;
        }

        public void Convert()
        {
            this.resourceReader.Read(expectJson: false);
            foreach (var resource in this.resourceReader.xlfResourceList)
            {
                try
                {
                    string output = this.GetOutputFile(resource.Key);
                    this.Convert(resource.Value, output);
                }
                catch (Exception ex)
                {
                    // Example case: noticed this case because two keys have same string value in spark job submissed (To fix)
                    Console.WriteLine($"---------Failed to convert {resource.Key}----------");
                    Console.WriteLine(ex.Message);
                }
            }
        }

        private string GetOutputFile(string inputfilename)
        {
            FileInfo fInfo = new FileInfo(inputfilename);
            return Path.Combine(destinationFolder, "translations\\extensions", fInfo.Name + "i18n.json");
        }

        public void Convert(Xliff xlifobject, string jsonFile)
        {
            if (xlifobject != null && xlifobject.Files != null)
            {
                // convert format
                foreach (var f in xlifobject.Files)
                {
                    f.keyValuePairs = f.body.transunits.ToDictionary(x => x.Key, x => x.Value);
                    f.body = null;
                }

                var translationunit = xlifobject.Files.ToDictionary(x => x.File, x => x.keyValuePairs);

                ExtensionTranslatedObject finaloutput = new ExtensionTranslatedObject();
                finaloutput.contents = translationunit;

                JsonSerializerSettings setting = new JsonSerializerSettings()
                {
                    NullValueHandling = NullValueHandling.Ignore
                };
                string jsonString = JsonConvert.SerializeObject(finaloutput, setting);
                File.WriteAllText(jsonFile, jsonString);
            }
        }
    }
}

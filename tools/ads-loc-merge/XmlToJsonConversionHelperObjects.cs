﻿//
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
//

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using Newtonsoft.Json;

namespace AzureDataStudio.Localization
{
    /// <summary>
    /// This and all classes in this file depend on the exact structure of localization xlf/json files
    /// </summary>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public class Xliff
    {
        [XmlElement(ElementName = "file")]
        public List<FileRef> Files { get; set; }
    }

    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public class FileRef
    {
        [XmlAttribute(AttributeName = "original")]
        public string File { get; set; }

        [XmlElement(ElementName = "body")]
        public Body body { get; set; }

        [XmlIgnore]
        public Dictionary<string, string> keyValuePairs { get; set; }
    }

    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public class Body
    {
        [XmlElement(ElementName = "trans-unit")]
        public List<TransUnit> transunits { get; set; }
    }

    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public class TransUnit
    {
        [XmlAttribute(AttributeName = "id")]
        public string Key { get; set; }

        [XmlElement(ElementName = "target")]
        public string Value { get; set; }
    }

    /// <summary>
    /// This is json output object
    /// </summary>
    public class ExtensionTranslatedObject
    {
        public string[] Copyright = {
    "--------------------------------------------------------------------------------------------",
    "Copyright (c) Microsoft Corporation. All rights reserved.",
    "Licensed under the MIT License. See License.txt in the project root for license information.",
    "--------------------------------------------------------------------------------------------",
    "Do not edit this file. It is machine generated." };

        public string version = "1.0.0.0";

        public Dictionary<string, Dictionary<string, string>> contents { get; set; }
    }
}
﻿using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace Capgemini.DataMigration.Core.Model
{
    /// <summary>
    /// Contains details of the field to be obfuscated.
    /// </summary>
    public class FieldToBeObfuscated
    {
        /// <summary>
        /// Gets or Sets the field name.
        /// </summary>
        public string FieldName { get; set; }

        /// <summary>
        /// Gets or Sets the formatting to be used when obfuscating the data.
        /// </summary>
        public string ObfuscationFormat { get; set; }

        /// <summary>
        /// Gets the function that will be used to generate the obfuscated values.
        /// </summary>
        public List<ObfuscationFormatOption> ObfuscationFormatArgs { get; private set; } = new List<ObfuscationFormatOption>();

        [JsonIgnore]
        public bool CanBeFormatted
        {
            get
            {
                if (!string.IsNullOrWhiteSpace(this.ObfuscationFormat) && this.ObfuscationFormatArgs.Any())
                {
                    return true;
                }

                return false;
            }
        }
    }
}
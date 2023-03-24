using System;

namespace Sitemplate
{
    /// <summary>
    /// Descriptor of the project
    /// </summary>
    [Serializable]
    public class Descriptor
    {
        /// <summary>
        /// Project name - meaningless, just to have it :)
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Where the content of the site is
        /// </summary>
        public string InputFolder { get; set; }

        /// <summary>
        /// Where to put the results
        /// </summary>
        public string OutputFolder { get; set; }

        /// <summary>
        /// Files to be included and copied to the results.
        /// Supports wildcards.
        /// </summary>
        public string[] Include { get; set; }

        /// <summary>
        /// Files not to clean in output.
        /// Useful if you have to place the results in root folder and have sources in subfolder - don't delete it.
        /// </summary>
        public string[] IgnoreInOutput { get; set; }

        /// <summary>
        /// Files to be deleted additionally to "Include".
        /// Supports wildcards.
        /// </summary>
        public string[] Clean { get; set; }

        /// <summary>
        /// Templates to be included
        /// </summary>
        public string[] Templates { get; set; }
    }
}

namespace Aksio.Microservices.Configuration
{
    /// <summary>
    /// Attribute used to adorn configuration objects
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class ConfigurationAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of <see cref="ConfigurationAttribute"/>.
        /// </summary>
        /// <param name="fileName">Name of the configuration file.</param>
        /// <param name="optional">Whether or not the file is optional - default = false</param>
        public ConfigurationAttribute(string fileName, bool optional = false)
        {
            FileName = fileName;
            Optional = optional;
        }

        /// <summary>
        /// Gets the name of the configuration file.
        /// </summary>
        public string FileName { get; }

        /// <summary>
        /// Gets whether or not the file is optional.
        /// </summary>
        public bool Optional { get; }
    }
}
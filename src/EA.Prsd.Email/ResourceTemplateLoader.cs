using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace EA.Prsd.Email
{
    /// <summary>
    /// Loads email templates from embedded resources.
    /// </summary>
    public class ResourceTemplateLoader : ITemplateLoader
    {
        private readonly Assembly assembly;
        private readonly string prefix;

        /// <summary>
        /// Creates a new instance of <see cref="ResourceTemplateLoader"/>.
        /// </summary>
        /// <param name="assembly">The assembly in which the resources are embedded.</param>
        /// <param name="prefix">This will be the "namespace" to the resource within the assembly.
        /// The namespace is normally defined as the dot-separated path to the resource.
        /// For example: "EA.SomeProject.Email.Templates"</param>
        public ResourceTemplateLoader(Assembly assembly, string prefix)
        {
            this.assembly = assembly;
            this.prefix = prefix;
        }

        public string LoadTemplate(string name)
        {
            string resourceName = GetResourceName(name);

            using (Stream stream = assembly.GetManifestResourceStream(resourceName))
            {
                if (stream == null)
                {
                    string errorMessage = string.Format(
                        "An email template was not found in assembly \"{0}\" with resource name \"{1}\". " +
                        "Check the file name and ensure the file's Build Action has been set to \"Embedded Resource\".",
                        assembly.FullName,
                        resourceName);
                    
                    throw new InvalidOperationException(errorMessage);
                }

                using (StreamReader streamReader = new StreamReader(stream))
                {
                    return streamReader.ReadToEnd();
                }
            }
        }

        public string GetResourceName(string name)
        {
            return string.Format("{0}.{1}", prefix, name);
        }
    }
}

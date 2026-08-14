using System;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using Abp.BlobStoring;
using Abp.Dependency;
using Abp.Localization;

namespace Eaf.BlobStoring.Naming
{
    /// <summary>
    /// Normalizador padrão de nomes de BLOBs e contêineres do EAF.
    /// </summary>
    public class EafDefaultBlobNamingNormalizer : IBlobNamingNormalizer, ITransientDependency
    {
        /// <inheritdoc />
        public virtual string NormalizeContainerName(string containerName)
        {
            using (CultureInfoHelper.Use(CultureInfo.InvariantCulture))
            {
                containerName = containerName?.ToLowerInvariant() ?? string.Empty;

                if (containerName.Length > 63)
                {
                    containerName = containerName.Substring(0, 63);
                }

                containerName = Regex.Replace(containerName, "[^a-z0-9-]", "-");
                containerName = Regex.Replace(containerName, "-{2,}", "-");
                containerName = Regex.Replace(containerName, "^-", string.Empty);
                containerName = Regex.Replace(containerName, "-$", string.Empty);

                if (containerName.Length < 3)
                {
                    containerName = containerName.PadRight(3, '0');
                }

                return containerName;
            }
        }

        /// <inheritdoc />
        public virtual string NormalizeBlobName(string blobName)
        {
            if (string.IsNullOrWhiteSpace(blobName))
            {
                return blobName;
            }

            var parts = blobName
                .Replace('\\', '/')
                .Split(new[] { '/' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(p => p.Trim())
                .Where(p => p != "." && p != ".." && !string.IsNullOrWhiteSpace(p))
                .ToArray();

            return string.Join("/", parts);
        }
    }
}

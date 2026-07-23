using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using OpenTelemetry;

namespace Eaf.AspNetCore.Configuration
{
    /// <summary>
    /// Remove ou mascara dados sensíveis (PII) de atividades e spans do OpenTelemetry.
    /// </summary>
    public class PiiRedactionProcessor : BaseProcessor<Activity>
    {
        private static readonly string[] SensitiveKeys = { "Authorization", "Cookie", "password", "token", "secret", "apikey", "api_key", "authorization" };

        public override void OnEnd(Activity activity)
        {
            var tags = activity.Tags.ToList();

            foreach (var (key, value) in tags)
            {
                if (IsSensitive(key))
                {
                    activity.SetTag(key, "[REDACTED]");
                    continue;
                }

                if (value is string stringValue && ContainsSensitivePattern(stringValue))
                {
                    activity.SetTag(key, "[REDACTED]");
                }
            }

            base.OnEnd(activity);
        }

        private static bool IsSensitive(string key)
        {
            if (string.IsNullOrWhiteSpace(key))
                return false;

            var lowerKey = key.ToLowerInvariant();
            return SensitiveKeys.Any(sensitive => lowerKey.Contains(sensitive));
        }

        private static bool ContainsSensitivePattern(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return false;

            return SensitiveKeys.Any(sensitive =>
                value.Contains(sensitive, System.StringComparison.OrdinalIgnoreCase));
        }
    }
}

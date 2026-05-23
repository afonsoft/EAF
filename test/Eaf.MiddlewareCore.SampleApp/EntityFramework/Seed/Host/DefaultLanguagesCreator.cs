using Abp.Localization;
using System.Collections.Generic;
using System.Linq;

namespace Eaf.MiddlewareCore.SampleApp.EntityFramework.Seed.Host
{
    public class DefaultLanguagesCreator
    {
        private readonly SampleAppDbContext _context;

        public DefaultLanguagesCreator(SampleAppDbContext context)
        {
            _context = context;
        }

        public static List<ApplicationLanguage> InitialLanguages => GetInitialLanguages();

        public void Create()
        {
            CreateLanguages();
        }

        private static List<ApplicationLanguage> GetInitialLanguages()
        {
            return new List<ApplicationLanguage>
            {
                new ApplicationLanguage(null, "en", "English", "famfamfam-flags gb"),
                new ApplicationLanguage(null, "tr", "Türkçe", "famfamfam-flags tr")
            };
        }

        private void AddLanguageIfNotExists(ApplicationLanguage language)
        {
            if (_context.Languages.Any(l => l.TenantId == language.TenantId && l.Name == language.Name))
            {
                return;
            }

            _context.Languages.Add(language);

            _context.SaveChanges();
        }

        private void CreateLanguages()
        {
            foreach (var language in InitialLanguages)
            {
                AddLanguageIfNotExists(language);
            }
        }
    }
}
namespace book_shop.Helpers.EmailHelper
{
    public class EmailTemplateLoader
    {
        private readonly string _basePath;

        public EmailTemplateLoader()
        {
            _basePath = Path.Combine(Directory.GetCurrentDirectory(), "EmailTemplates");
        }

        public string LoadTemplate(string relativePath, Dictionary<string, string> placeholders)
        {
            var fullPath = Path.Combine(_basePath, relativePath);
            var content = File.ReadAllText(fullPath);

            foreach (var pair in placeholders)
            {
                content = content.Replace($"{{{{{pair.Key}}}}}", pair.Value);
            }

            return content;
        }

        public string WrapWithLayout(string content)
        {
            var layoutPath = Path.Combine(_basePath, "Layouts", "EmailLayout.html");
            var layout = File.ReadAllText(layoutPath);
            return layout.Replace("{{BodyContent}}", content);
        }
    }
}

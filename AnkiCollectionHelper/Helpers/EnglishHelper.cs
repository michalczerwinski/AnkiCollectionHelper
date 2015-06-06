namespace AnkiCollectionHelper.Helpers
{
    public static class EnglishHelper
    {
        public static string GetNormalizedVocabulary(string vocabulary)
        {
            vocabulary = HtmlAgilityHelper.ExtractText(vocabulary);
            vocabulary = vocabulary
                .ToLower()
                .Trim()
                .TrimPrefix("to ")
                .TrimPrefix("the ")
                .TrimPrefix("a ")
                .TrimPrefix("an ");
            
            return vocabulary;
        }
    }
}
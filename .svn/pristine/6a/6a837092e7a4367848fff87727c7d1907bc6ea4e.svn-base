using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Slugify;
namespace ApiWebsite.Helper
{
    public static class MySlugHelper
    {
        /// <summary>  
        /// Removes all accents from the input string.  
        /// </summary>  
        /// <param name="text">The input string.</param>  
        /// <returns></returns>  
        public static string RemoveAccents(this string text)
        {
            if (string.IsNullOrWhiteSpace(text))
                return text;

            text = text.Normalize(NormalizationForm.FormD);
            char[] chars = text
                .Where(c => CharUnicodeInfo.GetUnicodeCategory(c)
                != UnicodeCategory.NonSpacingMark).ToArray();

            return new string(chars).Normalize(NormalizationForm.FormC);
        }

        /// <summary>  
        /// Turn a string into a slug by removing all accents,   
        /// special characters, additional spaces, substituting   
        /// spaces with hyphens & making it lower-case.  
        /// </summary>  
        /// <param name="phrase">The string to turn into a slug.</param>  
        /// <returns></returns>  
        public static string Slugify(this string phrase)
        {
            // Remove all accents and make the string lower case.  
            string output = phrase.RemoveAccents().ToLower();

            output = Regex.Replace(output, "đ", "d");

            // Remove all special characters from the string.  
            output = Regex.Replace(output, @"[^A-Za-z0-9\s-]", "");

            // Remove all additional spaces in favour of just one.  
            output = Regex.Replace(output, @"\s+", " ").Trim();

            // Replace all spaces with the hyphen.  
            output = Regex.Replace(output, @"\s", "-");

            // Return the slug.  
            return output;
        }

        public static SlugHelper helper;
        public static SlugHelper GetInstant()
        {
            if (helper is null)
            {
                // Creating a configuration object
                var config = new SlugHelperConfiguration();

                // Replace spaces with a dash
                config.StringReplacements.Add("đ", "d");
                config.StringReplacements.Add(@"[^A-Za-z0-9\s-]", "");
                config.StringReplacements.Add(@"\s+", " ");
                config.StringReplacements.Add(@"\s", "-");

                // We want a lowercase Slug
                config.ForceLowerCase = true;

                // Will collapse multiple seqential dashes down to a single one
                config.CollapseDashes = true;

                // Will trim leading and trailing whitespace
                config.TrimWhitespace = true;

                // Colapse consecutive whitespace chars into one
                config.CollapseWhiteSpace = true;

                // Remove everything that's not a letter, number, hyphen, dot, or underscore
                // config.DeniedCharactersRegex = @"[^A-Za-z0-9\s-]";

                // Create a helper instance with our new configuration
                helper = new SlugHelper(config);
            }
            return helper;
        }
    }

}
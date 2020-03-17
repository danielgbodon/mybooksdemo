using MyBooks.Models;
using System.Collections.Generic;

namespace MyBooks.Core
{
    public static class AppEnvironment
    {
        public static Dictionary<string, string> SupportedCultures = new Dictionary<string, string>()
        {
            { "es", "es-es" },
            { "gb", "en-gb" }
        };

        public const string WRITER_PROFILE = "writer";
        public const string READER_PROFILE = "reader";
        public static List<string> UserProfiles = new List<string>() { 
            AppEnvironment.READER_PROFILE, 
            AppEnvironment.WRITER_PROFILE 
        };
        public const string BOOK_SEPARATOR_ID = "_mb_";
        public const string BOOK_PREFIX_BD = "int1";
        public const string BOOK_PREFIX_GOOGLE = "ext1";

        public const string CLAIM_KEY_CULTURE = "culture";
        public static UserModel CurrentUser;
        
        public static Dictionary<string, string> DefaultBookLists = new Dictionary<string, string>()
        {
            { "MB_BookListPendientes",  "#3f51b5"},
            { "MB_BookListLeyendo",  "#ffc107"},
            { "MB_BookListLeidos",  "#4caf50"},
        };
    }
}

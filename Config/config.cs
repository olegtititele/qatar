using System.Globalization;
using PostgreSQL;
using ProjectFunctions;

namespace ConfigFile
{
    public class Config
    {
        public static string Bot_Token = "5906299723:AAEZHyFLuUzivC4Ci623JmeObb3XyPoDitQ";
        public static string menuPhoto = "background.jpg";
        public static string warningPhoto = "warning.jpg";       
        public static string errorMessage = "Произошла неизвестная ошибка. Попробуйте еще раз.";
        public static long[] adminChatsId = {5740336806};
        public static CultureInfo culture = new CultureInfo("ru-RU", false);
        public static string[] Countries = {"dba.dk", "homeless.co.il", "gumtree.co.za", "ebay-kleinanzeigen.de", "bolha.com", "olx.pt", "olx.pl", "olx.ro", "olx.bg", "sbazar.cz", "kijiji.it", "jofogas.hu", "oglasnik.hr", "tutti.ch", "bazar.bg", "quoka.de", "anibis.ch", "gumtree.uk", "guloggratis.dk", "bazar.sk", "bazos.pl", "olx.qa", "olx.com.om", "list.am", "kijiji.ca", "lajumate.ro", "om.opensooq.com", "ye.opensooq.com", "ae.opensooq.com", "iq.opensooq.com", "eg.opensooq.com", "lb.opensooq.com", "ly.opensooq.com", "sa.opensooq.com", "sd.opensooq.com", "bh.opensooq.com", "kuldnebors.ee", "olx.com.kw", "olx.com.bh", "qatarliving.com"};
    }
}
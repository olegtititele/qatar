using ConfigFile;


namespace Modules
{
    public static class CountryLink
    {      
        public static string GetCountryLink(string country)
        {
            string text = "";
            
            switch (country)
            {
                case "olx.com.om":
                    text = $"<b>📒 Введите вашу ссылку с категорией или ключевое слово.</b>\n\n<b>✔️ Пример ссылки: </b>{Links.olx_com_om_link}\n\n<b>✔️ Пример слова:</b> <u>apple</u>";
                    break;
                case "olx.com.bh":
                    text = $"<b>📒 Введите вашу ссылку с категорией или ключевое слово.</b>\n\n<b>✔️ Пример ссылки: </b>{Links.olx_com_bh_link}\n\n<b>✔️ Пример слова:</b> <u>apple</u>";
                    break;
                case "olx.com.kw":
                    text = $"<b>📒 Введите вашу ссылку с категорией или ключевое слово.</b>\n\n<b>✔️ Пример ссылки: </b>{Links.olx_com_kw_link}\n\n<b>✔️ Пример слова:</b> <u>apple</u>";
                    break;
            }
            return text;
        }
    }
}
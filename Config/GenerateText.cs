using PostgreSQL;

namespace ConfigFile
{
    public static class GenerateMessageText
    {
        public static string MenuText(long chatId)
        {
            string text = $"🆔 <b>ID:</b> <code>{chatId}</code>";
            return text;
        }

        public static string ConfigurationText(long chatId)
        {
            string sellerRegDate = DB.GetSellerRegDate(chatId);
            DateTime dt;

            if(DateTime.TryParse(sellerRegDate, out dt))
            {
                sellerRegDate = dt.ToString("dd.MM.yyyy");
            }

            string text = $"<b>📌 Количество объявлений продавца — </b><code>{DB.GetSellerTotalAds(chatId)}</code>\n<b>📌 Рейтинг продавца — </b><code>{DB.GetSellerRating(chatId)}</code>\n<b>📌 Дата регистрации продавца — </b><code>{sellerRegDate}</code>";

            return text;
        }

        public static string SettingsText(long chatId)
        {
            string text = $"⚒ Ваши настройки:\n\n<u><b>Текст для WhatsApp:</b></u> <code>{DB.GetWhatsappText(chatId)}</code>\n\n<u><b>Продавцов в ЧС:</b></u> <code>{DB.BlacklistLength(chatId)}</code>\n\n<u><b>Тайм-аут:</b></u> <code>{DB.GetTimeout(chatId)}</code>\n\n<u><b>Ссылок в ЧС:</b></u> <code>{DB.GetUserBlacklistLinks(chatId).Count()}</code>";

            return text;
        }
    }
}
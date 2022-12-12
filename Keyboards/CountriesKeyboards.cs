using Telegram.Bot.Types.ReplyMarkups;


namespace Bot_Keyboards
{
    public class CountriesKeyboards
    {
        // Клавиатура сервисов
        public static InlineKeyboardMarkup CountriesSitesKb = new(new []
        {
            new []
            {
                InlineKeyboardButton.WithCallbackData(text: "🇶🇦 qatarliving.com 🇶🇦", callbackData: "qatarliving.com"),
            },
            // new []
            // {
            //     InlineKeyboardButton.WithCallbackData(text: "🇶🇦 olx.qa 🇶🇦", callbackData: "olx.qa"),
            //     InlineKeyboardButton.WithCallbackData(text: "🇴🇲 olx.com.om 🇴🇲", callbackData: "olx.com.om"),
            // },
            // new []
            // {
            //     InlineKeyboardButton.WithCallbackData(text: "olx.com.bh", callbackData: "olx.com.bh"),
            //     InlineKeyboardButton.WithCallbackData(text: "olx.com.kw", callbackData: "olx.com.kw"),
            // },

            // new []
            // {
            //     InlineKeyboardButton.WithCallbackData(text: "🌐 OPENSOOQ 🌐", callbackData: "opensooq"),
            // },
            // new []
            // {
            //     InlineKeyboardButton.WithCallbackData(text: "🇩🇰 dba.dk 🇩🇰", callbackData: "dba.dk"),
            //     InlineKeyboardButton.WithCallbackData(text: "🇸🇮 bolha.com 🇸🇮", callbackData: "bolha.com"),
            // },
            // new []
            // {
            //     InlineKeyboardButton.WithCallbackData(text: "🇨🇿 sbazar.cz 🇨🇿", callbackData: "sbazar.cz"),
            //     InlineKeyboardButton.WithCallbackData(text: "🇮🇱 homeless.co.il 🇮🇱", callbackData: "homeless.co.il"), 
            // },
            // new []
            // {
            //     InlineKeyboardButton.WithCallbackData(text: "🇭🇺 jofogas.hu 🇭🇺", callbackData: "jofogas.hu"),
            //     InlineKeyboardButton.WithCallbackData(text: "🇭🇷 oglasnik.hr 🇭🇷", callbackData: "oglasnik.hr"), 
            // },
            // new []
            // {
            //     InlineKeyboardButton.WithCallbackData(text: "🇨🇭 anibis.ch 🇨🇭", callbackData: "anibis.ch"),
            //     InlineKeyboardButton.WithCallbackData(text: "🇧🇬 bazar.bg 🇧🇬", callbackData: "bazar.bg"),
            // },
            // new []
            // {
            //     InlineKeyboardButton.WithCallbackData(text: "🇩🇪 quoka.de 🇩🇪", callbackData: "quoka.de"),
            //     InlineKeyboardButton.WithCallbackData(text: "🇸🇰 bazar.sk 🇸🇰", callbackData: "bazar.sk"),
            // },
            // new []
            // {
            //     InlineKeyboardButton.WithCallbackData(text: "🇨🇦 kijiji.ca 🇨🇦", callbackData: "kijiji.ca"),
            //     InlineKeyboardButton.WithCallbackData(text: "󠁧🇷🇴 lajumate.ro 🇷🇴", callbackData: "lajumate.ro"),
            // },
            // new []
            // {
            //     InlineKeyboardButton.WithCallbackData(text: "🇦🇲 list.am 🇦🇲", callbackData: "list.am"),
            //     InlineKeyboardButton.WithCallbackData(text: "🇩🇰 guloggratis.dk 🇩🇰", callbackData: "guloggratis.dk"), 
            // },
            // new []
            // {
            //     InlineKeyboardButton.WithCallbackData(text: "🇪🇪 kuldnebors.ee 🇪🇪", callbackData: "kuldnebors.ee"),
            // },
            
            // back btn
            new []
            {
                InlineKeyboardButton.WithCallbackData(text: "🏚 Вернуться в меню", callbackData: "back_to_menu"),
            },
        });

        public static InlineKeyboardMarkup OPENSOOQKb = new(new []
        {
            
            new []
            {
                InlineKeyboardButton.WithCallbackData(text: "🇴🇲 om.opensooq.com 🇴🇲", callbackData: "om.opensooq.com"),
                InlineKeyboardButton.WithCallbackData(text: "🇾🇪 ye.opensooq.com 🇾🇪", callbackData: "ye.opensooq.com"),
            },
            new []
            {
                InlineKeyboardButton.WithCallbackData(text: "🇦🇪 ae.opensooq.com 🇦🇪", callbackData: "ae.opensooq.com"),
                InlineKeyboardButton.WithCallbackData(text: "🇮🇶 iq.opensooq.com 🇮🇶", callbackData: "iq.opensooq.com"),
            },
            new []
            {
                InlineKeyboardButton.WithCallbackData(text: "🇪🇬 eg.opensooq.com 🇪🇬", callbackData: "eg.opensooq.com"),
                InlineKeyboardButton.WithCallbackData(text: "🇱🇧 lb.opensooq.com 🇱🇧", callbackData: "lb.opensooq.com"),
            },
            new []
            {
                InlineKeyboardButton.WithCallbackData(text: "🇱🇾 ly.opensooq.com 🇱🇾", callbackData: "ly.opensooq.com"),
                InlineKeyboardButton.WithCallbackData(text: "🇸🇦 sa.opensooq.com 🇸🇦", callbackData: "sa.opensooq.com"),
            },
            new []
            {
                InlineKeyboardButton.WithCallbackData(text: "🇸🇩 sd.opensooq.com 🇸🇩", callbackData: "sd.opensooq.com"),
                InlineKeyboardButton.WithCallbackData(text: "🇧🇭 bh.opensooq.com 🇧🇭", callbackData: "bh.opensooq.com"),
            },
            new []
            {
                InlineKeyboardButton.WithCallbackData(text: "🔙 Назад", callbackData: "back_to_countries"),
            },
        });


        public static InlineKeyboardMarkup OLXKb = new(new []
        {
            
            new []
            {
                InlineKeyboardButton.WithCallbackData(text: "🇶🇦 olx.qa 🇶🇦", callbackData: "olx.qa"),
                InlineKeyboardButton.WithCallbackData(text: "🇴🇲 olx.com.om 🇴🇲", callbackData: "olx.com.om"),
            },
            new []
            {
                InlineKeyboardButton.WithCallbackData(text: "🇵🇹 olx.pt 🇵🇹", callbackData: "olx.pt"),
                InlineKeyboardButton.WithCallbackData(text: "🇵🇱 olx.pl 🇵🇱", callbackData: "olx.pl"),
            },
            new []
            {
                InlineKeyboardButton.WithCallbackData(text: "🇧🇬 olx.bg 🇧🇬", callbackData: "olx.bg"),
                InlineKeyboardButton.WithCallbackData(text: "🇷🇴 olx.ro 🇷🇴", callbackData: "olx.ro"),
            },
            new []
            {
                InlineKeyboardButton.WithCallbackData(text: "🔙 Назад", callbackData: "back_to_countries"),
            },
        });
    }
}
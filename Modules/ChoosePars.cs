using Parser;
using PostgreSQL;
using Bot_Keyboards;
using ConfigFile;

using Telegram.Bot;
using Telegram.Bot.Types.Enums;

namespace Modules
{
    public static class Platforms
    {
        public static void GetParser(ITelegramBotClient botClient, long chatId, string platform)
        {
            Thread load = new Thread(()=>Loading(botClient, chatId));
            switch(platform)
            {
                case "olx.com.om":    
                    new Thread(()=>ArabOLX.StartParsing(botClient, chatId, DateTime.Now)).Start();
                    load.Start();
                    break;
                case "olx.com.bh":    
                    new Thread(()=>ArabOLX.StartParsing(botClient, chatId, DateTime.Now)).Start();
                    load.Start();
                    break;
                case "olx.com.kw":    
                    new Thread(()=>ArabOLX.StartParsing(botClient, chatId, DateTime.Now)).Start();
                    load.Start();
                    break;
                case "qatarliving.com":    
                    new Thread(()=>QatarLiving.StartParsing(botClient, chatId, DateTime.Now)).Start();
                    load.Start();
                    break;
            }
        }

        static async void Loading(ITelegramBotClient botClient, long chatId)
        {
            string mainMenuPhoto = Config.menuPhoto;
            while(true)
            {
                if(DB.GetParser(chatId) == "Stop")
                {
                    await botClient.SendTextMessageAsync(
                        chatId: chatId,
                        text: "<b>⛔️ Парсинг остановлен!</b>",
                        parseMode: ParseMode.Html,
                        replyMarkup: Keyboards.MenuKb()
                    );
                    return;
                }
                else
                {
                    System.Threading.Thread.Sleep(5000);
                }
            }
        }
    }
}
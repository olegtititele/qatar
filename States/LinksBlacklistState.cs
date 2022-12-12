using PostgreSQL;
using Bot_Keyboards;
using ConfigFile;

using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.InputFiles;

namespace States
{
    public static class LinksBlacklistState
    {
        public static async void CallBackHandler(ITelegramBotClient botClient, CallbackQuery callbackQuery, long chatId, int messageId)
        {
            try
            {
                if(callbackQuery.Data == "delete_all_links")
                {
                    DB.DeleteAllLinks(chatId);
                    await botClient.EditMessageCaptionAsync(
                        chatId: chatId,
                        messageId: messageId,
                        caption: $"<b>Все ссылки удалены.</b>",
                        replyMarkup: Keyboards.BackToSettings,
                        parseMode: ParseMode.Html
                    );
                }

                string state = "MainMenu";
                DB.UpdateState(chatId, state);
            }
            catch
            {
                await botClient.AnswerCallbackQueryAsync(
                    callbackQueryId: callbackQuery.Id,
                    text: Config.errorMessage,
                    showAlert:false
                );
            }
        }
        
        public static async void MessageHandler(ITelegramBotClient botClient, string messageText, long chatId, int messageId, string mainMenuPhoto)
        {
            try
            {
                FileStream fileStream = new FileStream(mainMenuPhoto, FileMode.Open, FileAccess.Read, FileShare.Read);

                string[] blacklistLinks = messageText.Split(',');
                int blacklistLinksCount = 0;
                foreach(string blacklistLink in blacklistLinks)
                {
                    if(!DB.GetUserBlacklistLinks(chatId).Contains(blacklistLink.Trim()))
                    {
                        DB.InsertNewLink(chatId, blacklistLink.Trim());
                        blacklistLinksCount++;
                    }
                }

                await botClient.SendPhotoAsync(
                    chatId: chatId,
                    photo: new InputOnlineFile(fileStream),
                    caption: $"<b>Добавлено новых ссылок:</b> <code>{blacklistLinksCount}</code>",
                    parseMode: ParseMode.Html,
                    replyMarkup: Keyboards.BackToSettings
                );

                string state = "MainMenu";
                DB.UpdateState(chatId, state);
            }
            catch
            {
                return;
            }
        }
    }
}
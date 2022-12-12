using PostgreSQL;
using Bot_Keyboards;

using Telegram.Bot;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.InputFiles;


namespace States
{
    public static class WhatsappTextState
    {
        public static async void MessageHandler(ITelegramBotClient botClient, dynamic messageText, long chatId, int messageId, string mainMenuPhoto)
        {
            try
            {
                FileStream fileStream = new FileStream(mainMenuPhoto, FileMode.Open, FileAccess.Read, FileShare.Read);
                string oldWhatsappText = DB.GetWhatsappText(chatId);

                if(messageText.Length > 500)
                {
                    await botClient.SendPhotoAsync(
                        chatId: chatId,
                        photo: new InputOnlineFile(fileStream),
                        caption: $"<b>❗️ Введите корректный текст для WhatsApp.</b>",
                        parseMode: ParseMode.Html,
                        replyMarkup: Keyboards.BackToSettings
                    );
                }
                else
                {
                    DB.UpdateWhatsappText(chatId, messageText!);
                    await botClient.SendPhotoAsync(
                        chatId: chatId,
                        photo: new InputOnlineFile(fileStream),
                        caption: $"<b>Текст для WhatsApp обновлен на:</b> <code>{DB.GetWhatsappText(chatId)}</code>",
                        parseMode: ParseMode.Html,
                        replyMarkup: Keyboards.BackToSettings
                    );

                    string state = "MainMenu";
                    DB.UpdateState(chatId, state);
                }
            }
            catch
            {
                return;
            }
        }
    }
}
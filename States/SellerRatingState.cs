using PostgreSQL;
using Bot_Keyboards;
using ConfigFile;

using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.InputFiles;

namespace States
{
    public static class SellerRatingState
    {
        public static async void CallBackHandler(ITelegramBotClient botClient, CallbackQuery callbackQuery, long chatId, int messageId)
        {
            try
            {
                double rating = 0;
                switch(callbackQuery.Data)
                {
                    case "0":
                        rating = 0;
                        break;
                    case "1":
                        rating = 1;
                        break;
                    case "2":
                        rating = 2;
                        break;
                    case "3":
                        rating = 3;
                        break;
                    case "4":
                        rating = 4;
                        break;
                    case "5":
                        rating = 5;
                        break;
                }
                
                DB.UpdateSellerRating(chatId, rating);
                await botClient.EditMessageCaptionAsync(
                    chatId: chatId,
                    messageId: messageId,
                    caption: $"<b>Рейтинг продавца обновлен на:</b> <code>{DB.GetSellerRating(chatId)}</code>",
                    replyMarkup: Keyboards.BackToConfiguration,
                    parseMode: ParseMode.Html
                );
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

                if(double.TryParse(messageText.Replace('.', ','), out double number))
                {
                    if(number <= 5 && number >= 0)
                    {
                        DB.UpdateSellerRating(chatId, number);

                        await botClient.SendPhotoAsync(
                            chatId: chatId,
                            photo: new InputOnlineFile(fileStream),
                            caption: $"<b>Рейтинг продавца обновлен на:</b> <code>{DB.GetSellerRating(chatId)}</code>",
                            parseMode: ParseMode.Html,
                            replyMarkup: Keyboards.BackToConfiguration
                        );

                        string state = "MainMenu";
                        DB.UpdateState(chatId, state);
                    }
                    else
                    {
                        await botClient.SendPhotoAsync(
                            chatId: chatId,
                            photo: new InputOnlineFile(fileStream),
                            caption: "<b>❗️ Рейтинг продавца должен быть в промежутке от 0 до 5. Введите повторно.</b>",
                            parseMode: ParseMode.Html,
                            replyMarkup: Keyboards.BackToConfiguration
                        );
                    }
                }
                else
                {
                    await botClient.SendPhotoAsync(
                        chatId: chatId,
                        photo: new InputOnlineFile(fileStream),
                        caption: "<b>❗️ Должно быть цифрой. Введите повторно.</b>",
                        parseMode: ParseMode.Html,
                        replyMarkup: Keyboards.BackToConfiguration
                    );
                }
            }
            catch
            {
                return;
            }
        }
    }
}
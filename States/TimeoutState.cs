using PostgreSQL;
using Bot_Keyboards;
using ConfigFile;

using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.InputFiles;

namespace States
{
    public static class TimeoutState
    {
        // public static async void CallBackHandler(ITelegramBotClient botClient, CallbackQuery callbackQuery, long chatId, int messageId)
        // {
        //     try
        //     {
        //         if(callbackQuery.Data == "disable_seller_total_ads")
        //         {
        //             DB.UpdateSellerTotalAds(chatId, "Отключить");
        //             await botClient.EditMessageCaptionAsync(
        //                 chatId: chatId,
        //                 messageId: messageId,
        //                 caption: $"<b>Фильтр \"Количество объялений продавца\" отключен.</b>",
        //                 replyMarkup: Keyboards.BackToConfiguration,
        //                 parseMode: ParseMode.Html
        //             );
        //         }

        //         string state = "MainMenu";
        //         DB.UpdateState(chatId, state);
        //     }
        //     catch
        //     {
        //         await botClient.AnswerCallbackQueryAsync(
        //             callbackQueryId: callbackQuery.Id,
        //             text: Config.errorMessage,
        //             showAlert:false
        //         );
        //     }
        // }
        
        public static async void MessageHandler(ITelegramBotClient botClient, string messageText, long chatId, int messageId, string mainMenuPhoto)
        {
            try
            {
                FileStream fileStream = new FileStream(mainMenuPhoto, FileMode.Open, FileAccess.Read, FileShare.Read);

                if(int.TryParse(messageText, out int number))
                {
                    if(number >= 10 && number <= 30)
                    {
                        DB.UpdateTimeout(chatId, number);
                        await botClient.SendPhotoAsync(
                            chatId: chatId,
                            photo: new InputOnlineFile(fileStream),
                            caption: $"<b>Тайм-аут изменен на:</b> <code>{DB.GetTimeout(chatId)}</code>",
                            parseMode: ParseMode.Html,
                            replyMarkup: Keyboards.BackToSettings
                        );

                        string state = "MainMenu";
                        DB.UpdateState(chatId, state);
                    }
                    else
                    {
                        await botClient.SendPhotoAsync(
                            chatId: chatId,
                            photo: new InputOnlineFile(fileStream),
                            caption: "<b>❗️ Тайм-аут не должен быть меньше 10 секунд и превышать 30 секунд. Введите повторно.</b>",
                            parseMode: ParseMode.Html,
                            replyMarkup: Keyboards.BackToSettings
                        ); 
                    }
                }
                else
                {
                    await botClient.SendPhotoAsync(
                        chatId: chatId,
                        photo: new InputOnlineFile(fileStream),
                        caption: "<b>❗️ Тайм-аут должен быть цифрой. Введите повторно.</b>",
                        parseMode: ParseMode.Html,
                        replyMarkup: Keyboards.BackToSettings
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
using PostgreSQL;
using Bot_Keyboards;
using ConfigFile;

using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.InputFiles;

namespace States
{
    public static class SellerAdCountState
    {
        public static async void CallBackHandler(ITelegramBotClient botClient, CallbackQuery callbackQuery, long chatId, int messageId)
        {
            try
            {
                if(callbackQuery.Data == "disable_seller_total_ads")
                {
                    DB.UpdateSellerTotalAds(chatId, "Отключить");
                    await botClient.EditMessageCaptionAsync(
                        chatId: chatId,
                        messageId: messageId,
                        caption: $"<b>Фильтр \"Количество объялений продавца\" отключен.</b>",
                        replyMarkup: Keyboards.BackToConfiguration,
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

                if(int.TryParse(messageText, out int number))
                {
                    DB.UpdateSellerTotalAds(chatId, messageText);
                    await botClient.SendPhotoAsync(
                        chatId: chatId,
                        photo: new InputOnlineFile(fileStream),
                        caption: $"<b>Количество объялений продавца обновлено на:</b> <code>{DB.GetSellerTotalAds(chatId)}</code>",
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
                        caption: "<b>❗️ Количество объялений продавца должно быть цифрой. Введите повторно.</b>",
                        parseMode: ParseMode.Html,
                        replyMarkup: Keyboards.sellerTotalAdsKb
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
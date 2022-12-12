using PostgreSQL;
using Bot_Keyboards;
using ConfigFile;

using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.InputFiles;

namespace States
{
    public static class SellerRegDateState
    {
        public static async void CallBackHandler(ITelegramBotClient botClient, CallbackQuery callbackQuery, long chatId, int messageId)
        {
            try
            {
                if(callbackQuery.Data == "today_date")
                {
                    DateTime today = DateTime.Today;

                    DB.UpdateSellerRegDate(chatId, today.ToString());
                    await botClient.EditMessageCaptionAsync(
                        chatId: chatId,
                        messageId: messageId,
                        caption: $"<b>Дата регистрации продавца обновлена на:</b> <code>{today.ToString("dd.MM.yyyy")}</code>",
                        replyMarkup: Keyboards.BackToConfiguration,
                        parseMode: ParseMode.Html
                    );
                }
                else if(callbackQuery.Data == "disable_reg_date")
                {
                    DB.UpdateSellerRegDate(chatId, "Отключить");
                    await botClient.EditMessageCaptionAsync(
                        chatId: chatId,
                        messageId: messageId,
                        caption: $"<b>Фильтр \"Дата регистрации продавца\" отключен.</b>",
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

                if(DateTime.TryParse(messageText, out DateTime dt))
                {
                    DB.UpdateSellerRegDate(chatId, messageText);
                    await botClient.SendPhotoAsync(
                        chatId: chatId,
                        photo: new InputOnlineFile(fileStream),
                        caption: $"<b>Дата регистрации продавца обновлена на:</b> <code>{dt.ToString("dd.MM.yyyy")}</code>",
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
                        caption: "<b>❗️ Дата должна быть строго формата \"ДД.ММ.ГГГГ\".\n\nВведите дату регистрации продавца повторно.</b>",
                        parseMode: ParseMode.Html,
                        replyMarkup: Keyboards.RegDateKb()
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
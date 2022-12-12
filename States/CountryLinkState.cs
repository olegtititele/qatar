using PostgreSQL;
using Bot_Keyboards;
using ConfigFile;
using ProjectFunctions;

using Telegram.Bot;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.InputFiles;


namespace States
{
    public static class CountryLinkState
    {
        public static async void MessageHandler(ITelegramBotClient botClient, dynamic messageText, long chatId, int messageId)
        {
            try
            {
                string oldLink = DB.GetLink(chatId);

                if(Functions.StringIsNumber(messageText))
                {
                    using (var fileStream = new FileStream(Config.menuPhoto, FileMode.Open, FileAccess.Read, FileShare.Read))
                    {
                        await botClient.SendPhotoAsync(
                            chatId: chatId,
                            photo: new InputOnlineFile(fileStream),
                            caption: "<b>❗️ Ссылка не должна быть числом. Введите повторно.</b>",
                            parseMode: ParseMode.Html,
                            replyMarkup: Keyboards.BackToCountries
                        );
                    } 
                }
                else
                {
                    try
                    {
                        DB.UpdateLink(chatId, messageText!);

                        string platform = DB.GetPlatform(chatId);
                        string link = DB.GetLink(chatId);
                        string sellerTotalAds = DB.GetSellerTotalAds(chatId);
                        string sellerRegDate = DB.GetSellerRegDate(chatId);
                        decimal sellerRating = DB.GetSellerRating(chatId);
                        string sellerType = DB.GetSellerType(chatId);
                        string blackList = DB.GetBlackList(chatId);

                        if(DateTime.TryParse(sellerRegDate, out DateTime dt))
                        {
                            sellerRegDate = dt.ToString("dd.MM.yyyy");
                        }
                        using (var fileStream = new FileStream(Config.warningPhoto, FileMode.Open, FileAccess.Read, FileShare.Read))
                        {
                            await botClient.SendPhotoAsync(
                                chatId: chatId,
                                photo: new InputOnlineFile(fileStream),
                                caption: $"<b><u>Ваши параметры для парсинга: </u></b>\n\n<b>🛍 Площадка ⇒ </b>{platform}\n\n<b>📨 Ссылка ⇒ </b>{link}\n\n<b>🗄 Кол-во объявлений ⇒ </b>{sellerTotalAds}\n\n<b>📆 Регистрация продавца ⇒ </b>{sellerRegDate}\n\n<b>⭐️ Рейтинг продавца ⇒ </b>{sellerRating}\n\n<b>👤 Тип аккаунтов ⇒ </b>{sellerType}\n\n<b>📂 ЧС ⇒ </b>{blackList}",
                                parseMode: ParseMode.Html,
                                replyMarkup: Keyboards.StartPars
                            );
                        }
                        DB.UpdateState(chatId, "MainMenu");
                    }
                    catch
                    {
                        DB.UpdateLink(chatId, oldLink);
                        using (var fileStream = new FileStream(Config.menuPhoto, FileMode.Open, FileAccess.Read, FileShare.Read))
                        {
                            await botClient.SendPhotoAsync(
                                chatId: chatId,
                                photo: new InputOnlineFile(fileStream),
                                caption: "<b>❗️ Введите ссылку корректно.</b>",
                                parseMode: ParseMode.Html,
                                replyMarkup: Keyboards.BackToCountries
                            );
                        }
                    }
                }
            }
            catch
            {
                return;
            }
        }
    }
}
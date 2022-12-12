using PostgreSQL;
using Bot_Keyboards;
using ConfigFile;
using Modules;
using Parser;

using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;


namespace States
{
    public static class MainMenu
    {
        public static async void CallBack(ITelegramBotClient botClient, CallbackQuery callbackQuery, long chatId, int messageId, string firstName, string mainMenuPhoto)
        {
            string state;

            try
            {
                foreach(string country in Config.Countries)
                {
                    
                    if(callbackQuery.Data == country)
                    {
                        if(DB.GetParser(chatId) == "Start")
                        {
                            await botClient.AnswerCallbackQueryAsync(
                                callbackQueryId: callbackQuery.Id,
                                text: "У вас уже запущен парсинг.",
                                showAlert: true
                            );
                            return;
                        }
                        DB.UpdatePlatform(chatId, callbackQuery.Data);

                        await botClient.EditMessageCaptionAsync(
                            chatId: chatId,
                            messageId: messageId,
                            caption: "<b>🌐 Выберите категорию парсинга:</b>",
                            replyMarkup: Keyboards.ChooseTypePars(),
                            parseMode: ParseMode.Html
                        );
                    }
                }
                
                switch(callbackQuery.Data)
                {
                    case "parse_all_categories":
                    // ПАРСИТЬ ВСЕ КАТЕГОРИИ
                        DB.UpdateParserCategory(chatId, "all-categories");
                        string platform = DB.GetPlatform(chatId);
                        string link = DB.GetLink(chatId);
                        string sellerTotalAds = DB.GetSellerTotalAds(chatId);
                        string sellerRegDate = DB.GetSellerRegDate(chatId);
                        decimal sellerRating = DB.GetSellerRating(chatId);
                        string sellerType = DB.GetSellerType(chatId);
                        string blackList = DB.GetBlackList(chatId);

                        DateTime dt;

                        if(DateTime.TryParse(sellerRegDate, out dt))
                        {
                            sellerRegDate = dt.ToString("dd.MM.yyyy");
                        }

                        await botClient.EditMessageCaptionAsync(
                            chatId: chatId,
                            messageId: messageId,
                            caption: $"<b><u>Ваши параметры для парсинга: </u></b>\n\n<b>🛍 Площадка ⇒ </b>{platform}\n\n<b>📨 Ссылка ⇒ </b>Все категории\n\n<b>🗄 Кол-во объявлений ⇒ </b>{sellerTotalAds}\n\n<b>📆 Регистрация продавца ⇒ </b>{sellerRegDate}\n\n<b>⭐️ Рейтинг продавца ⇒ </b>{sellerRating}\n\n<b>👤 Тип аккаунтов ⇒ </b>{sellerType}\n\n<b>📂 ЧС ⇒ </b>{blackList}",
                            parseMode: ParseMode.Html,
                            replyMarkup: Keyboards.StartPars
                        );
                        return;
                    case "parse_category":
                    // ПАРСИТЬ КАТЕГОРИЮ
                        DB.UpdateParserCategory(chatId, "category");
                        DB.UpdateState(chatId, "CountryLink");

                        await botClient.EditMessageCaptionAsync(
                            chatId: chatId,
                            messageId: messageId,
                            caption: CountryLink.GetCountryLink(DB.GetPlatform(chatId)),
                            replyMarkup: Keyboards.BackToCountries,
                            parseMode: ParseMode.Html
                        );
                        return;
                    case "start_pars":
                        DB.UpdateParser(chatId, "Start");
                        Platforms.GetParser(botClient, chatId, DB.GetPlatform(chatId));

                        await botClient.EditMessageCaptionAsync(
                            chatId: chatId,
                            messageId: messageId,
                            caption: $"<b>Парсинг начался. Парсер будет отслеживать самые свежие объявления начиная с текущего времнеми.</b>\n\n<b>❕ Для остановки парсера напишите </b><code>Стоп</code>",
                            parseMode: ParseMode.Html
                        );
                        return;
                    case "show_services":
                    // ПОСМОТРЕТЬ СЕРВИСЫ
                        await botClient.EditMessageCaptionAsync(
                            chatId: chatId,
                            messageId: messageId,
                            caption: "<b>🌍 Выберите площадку, где вы хотите найти объявления.</b>",
                            parseMode: ParseMode.Html,
                            replyMarkup: CountriesKeyboards.CountriesSitesKb
                        );
                        return;
                    case "settings":
                    // НАСТРОЙКИ
                        await botClient.EditMessageCaptionAsync(
                            chatId: chatId,
                            messageId: messageId,
                            caption: GenerateMessageText.SettingsText(chatId),
                            parseMode: ParseMode.Html,
                            replyMarkup: Keyboards.SettingsKb(chatId)
                        );
                        return;
                    case "configuration":
                    // КОНФИГУРАЦИЯ
                        await botClient.EditMessageCaptionAsync(
                            chatId: chatId,
                            messageId: messageId,
                            caption: GenerateMessageText.ConfigurationText(chatId),
                            parseMode: ParseMode.Html,
                            replyMarkup: Keyboards.ConfigurationKb(chatId)
                        );
                        return;
                    case "account_type":
                    // ВЫБОР ТИПА АККАУНТА
                        if(DB.GetSellerType(chatId) == "Частное лицо")
                        {
                            DB.UpdateSellerType(chatId);
                            await botClient.EditMessageReplyMarkupAsync(
                                chatId: chatId,
                                messageId: messageId,
                                replyMarkup: Keyboards.ConfigurationKb(chatId)
                            );
                        }
                        else
                        {
                            DB.UpdateSellerType(chatId);
                            await botClient.EditMessageReplyMarkupAsync(
                                chatId: chatId,
                                messageId: messageId,
                                replyMarkup: Keyboards.ConfigurationKb(chatId)
                            );
                        }
                        return;
                    case "seller_announ_count":
                    // КОЛ-ВО ОБЪЯВЛЕНИЙ ПРОДАВЦА
                        state = "SellerAdvCount";
                        DB.UpdateState(chatId, state);
                        await botClient.EditMessageCaptionAsync(
                            chatId: chatId,
                            messageId: messageId,
                            caption: "<b>📙 Введите число количество объявлений продавца.\n\n✔️ Пример: </b><u>10</u> (парсер будет искать продавцов у которых кол-во объявлений не будет превышать 10)",
                            parseMode: ParseMode.Html,
                            replyMarkup: Keyboards.sellerTotalAdsKb
                        );
                        return;
                    case "seller_rating":
                    // РЕЙТИНГ ПРОДАВЦА
                        state = "SellerRating";
                        DB.UpdateState(chatId, state);
                        await botClient.EditMessageCaptionAsync(
                            chatId: chatId,
                            messageId: messageId,
                            caption: "<b>📝 Введите рейтинг продавца(Oт 0 до 5).\n\n✔️ Пример: </b><u>2,5</u> (парсер будет искать продавцов у которых рейтинг не будет превышать 2.5)",
                            parseMode: ParseMode.Html,
                            replyMarkup: Keyboards.sellerRatingKb
                        );
                        return;
                    case "seller_reg":
                    // ДАТА РЕГИСТРАЦИИ ПРОДАВЦА
                        state = "SellerRegData";
                        DB.UpdateState(chatId, state);
                        await botClient.EditMessageCaptionAsync(
                            chatId: chatId,
                            messageId: messageId,
                            caption: "<b>📗 Укажите дату регистрации продавца.\n\n✔️ Пример: </b><u>01.01.2022</u> (парсер будет искать продавцов, которые зарегистрировались с 01.01.2022 по текущую дату)",
                            parseMode: ParseMode.Html,
                            replyMarkup: Keyboards.RegDateKb()
                        );
                        return;
                    case "whatsapp_text":
                    // ТЕКСТ ДЛЯ WHATSAPP
                        string text = "<b>🖊 Введите текст для WhatsApp (<u>макс. 500 символов</u>):</b>\n\n<b>Ключевые слова для вставки:</b>\n<code>@adlink</code>-<i>Подставит текущую ссылку</i>\n<code>@adname</code>-<i>Подставит название объявления</i>\n<code>@adprice</code>-<i>Подставит цену объявления</i>\n<code>@adlocation</code>-<i>Подставит местоположение объявления</i>\n<code>@sellername</code>-<i>Подставит имя продавца</i>\n\n✔️ <b>Вставьте ключевое слово в текст и вместо него подставится нужная информация.</b>";
                        state = "WhatsappText";
                        DB.UpdateState(chatId, state);
                        await botClient.EditMessageCaptionAsync(
                            chatId: chatId,
                            messageId: messageId,
                            caption: text,
                            parseMode: ParseMode.Html,
                            replyMarkup: Keyboards.BackToSettings
                        );
                        return;
                    case "timeout":
                    // ТАЙМ-АУТ
                        text = "<b>Установите таймаут от 10 до 30 секунд:</b>";
                        state = "Timeout";
                        DB.UpdateState(chatId, state);
                        await botClient.EditMessageCaptionAsync(
                            chatId: chatId,
                            messageId: messageId,
                            caption: text,
                            parseMode: ParseMode.Html,
                            replyMarkup: Keyboards.BackToSettings
                        );
                        return;
                    case "black_list":
                    // ЧЕРНЫЙ СПИСОК
                        DB.UpdateBlackList(chatId);
                        await botClient.EditMessageReplyMarkupAsync(
                            chatId: chatId,
                            messageId: messageId,
                            replyMarkup: Keyboards.SettingsKb(chatId)
                        );
                        return;
                    case "links_blacklist":
                    // ЧЕРНЫЙ СПИСОК ССЫЛОК
                        text = "<b>Введите ссылки через запятую для добавления в черный список.</b>";
                        state = "LinksBlacklist";
                        DB.UpdateState(chatId, state);
                        await botClient.EditMessageCaptionAsync(
                            chatId: chatId,
                            messageId: messageId,
                            caption: text,
                            parseMode: ParseMode.Html,
                            replyMarkup: Keyboards.blacklistLinksKb
                        );
                        return;
                    case "back_from_pars":
                        using (var fileStream = new FileStream(Config.menuPhoto, FileMode.Open, FileAccess.Read, FileShare.Read))
                        {
                            await botClient.EditMessageMediaAsync(
                                chatId: chatId,
                                messageId: messageId,
                                media: new InputMediaPhoto(new InputMedia(fileStream, Config.menuPhoto))
                            );
                        }
                
                        await botClient.EditMessageCaptionAsync(
                            chatId: chatId,
                            messageId: messageId,
                            caption: GenerateMessageText.MenuText(chatId),
                            parseMode: ParseMode.Html,
                            replyMarkup: Keyboards.MainMenuButtons
                        );
                        return;
                }
            }
            catch(Exception e)
            {
                Console.WriteLine(e);
                await botClient.AnswerCallbackQueryAsync(
                    callbackQueryId: callbackQuery.Id,
                    text: Config.errorMessage,
                    showAlert:false
                );
            }
        }

        // Оставить только цифры
        private static string leave_only_numbers(string block)
        {
            string new_block = "";
            for (int i = 0; i < block.Length; i++)
            {
                if (Char.IsDigit(block[i]))
                {
                    new_block += block[i];
                }
                else
                {
                    continue;
                }
            }
            return new_block;
        }
    }
}
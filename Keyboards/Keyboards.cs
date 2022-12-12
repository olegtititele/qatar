using Telegram.Bot.Types.ReplyMarkups;
using PostgreSQL;
using ConfigFile;

namespace Bot_Keyboards
{
    public static class Keyboards
    {

        // MENU
        public static InlineKeyboardMarkup MainMenuButtons = new(new []
        {
            new []
            {
                InlineKeyboardButton.WithCallbackData(text: "🔌 Парсинг", callbackData: "show_services"),
            },
            new []
            {
                InlineKeyboardButton.WithCallbackData(text: "⚙️ Настройки", callbackData: "settings"),
                InlineKeyboardButton.WithCallbackData(text: "🛠 Конфигурация", callbackData: "configuration"),
            },
        });

        
        public static ReplyKeyboardMarkup MenuKb()
        {
            ReplyKeyboardMarkup Keyboard;
            Keyboard = new(new []
            {
                new KeyboardButton[] {"Меню"},
            })
            {
                ResizeKeyboard = true,
            };
            return Keyboard;
        }

        // SETTINGS
        public static InlineKeyboardMarkup SettingsKb(long chatId)
        {
            string blackList = DB.GetBlackList(chatId);
            InlineKeyboardMarkup blackListKb;


            if(blackList == "Включить")
            {
                blackListKb = new(new []
                {
                    new []
                    {
                        InlineKeyboardButton.WithCallbackData(text: "🟢 Чёрный список", callbackData: "black_list"),
                    },
                    new []
                    {
                        InlineKeyboardButton.WithCallbackData(text: "Черный список ссылок", callbackData: "links_blacklist"),  
                    },
                    new []
                    {
                        InlineKeyboardButton.WithCallbackData(text: "Текст для WhatsApp", callbackData: "whatsapp_text"),  
                    },
                    new []
                    {
                        InlineKeyboardButton.WithCallbackData(text: "Тайм-аут", callbackData: "timeout"),  
                    },
                    new []
                    {
                        InlineKeyboardButton.WithCallbackData(text: "🏚 Вернуться в меню", callbackData: "back_to_menu"),
                    },
                });
            }
            else
            {
                blackListKb = new(new []
                {
                    new []
                    {
                        InlineKeyboardButton.WithCallbackData(text: "⚪️ Чёрный список", callbackData: "black_list"),
                    },
                    new []
                    {
                        InlineKeyboardButton.WithCallbackData(text: "Черный список ссылок", callbackData: "links_blacklist"),  
                    },
                    new []
                    {
                        InlineKeyboardButton.WithCallbackData(text: "Текст для WhatsApp", callbackData: "whatsapp_text"),  
                    },
                    new []
                    {
                        InlineKeyboardButton.WithCallbackData(text: "Тайм-аут", callbackData: "timeout"),  
                    },
                    new []
                    {
                        InlineKeyboardButton.WithCallbackData(text: "🏚 Вернуться в меню", callbackData: "back_to_menu"),
                    },
                });
            }

            return blackListKb;
        }

        public static InlineKeyboardMarkup ConfigurationKb(long chatId)
        {
            string sellerType = DB.GetSellerType(chatId);
            InlineKeyboardMarkup configurationKb;
            if(sellerType == "Частное лицо")
            {
                configurationKb = new(new []
                {
                    new []
                    {
                        InlineKeyboardButton.WithCallbackData(text: "Парсить: только \"Частные лица\"", callbackData: "account_type"),
                    },
                    new []
                    {
                        InlineKeyboardButton.WithCallbackData(text: "Кол-во объявлений продавца", callbackData: "seller_announ_count"),
                    },
                    new []
                    {
                        InlineKeyboardButton.WithCallbackData(text: "Рейтинг продавца", callbackData: "seller_rating"),
                    },
                    new []
                    {
                        InlineKeyboardButton.WithCallbackData(text: "Дата регистрации продавца", callbackData: "seller_reg"),
                    },
                    new []
                    {
                        InlineKeyboardButton.WithCallbackData(text: "🏚 Вернуться в меню", callbackData: "back_to_menu"),
                    },
                });
            }
            else
            {
                configurationKb = new(new []
                {
                    new []
                    {
                        InlineKeyboardButton.WithCallbackData(text: "Парсить: \"Все типы аккаунтов\"", callbackData: "account_type"),
                    },
                    new []
                    {
                        InlineKeyboardButton.WithCallbackData(text: "Кол-во объявлений продавца", callbackData: "seller_announ_count"),
                    },
                    new []
                    {
                        InlineKeyboardButton.WithCallbackData(text: "Рейтинг продавца", callbackData: "seller_rating"),
                    },
                    new []
                    {
                        InlineKeyboardButton.WithCallbackData(text: "Дата регистрации продавца", callbackData: "seller_reg"),
                    },
                    new []
                    {
                        InlineKeyboardButton.WithCallbackData(text: "🏚 Вернуться в меню", callbackData: "back_to_menu"),
                    },
                });
            }

            return configurationKb;
        }

        public static InlineKeyboardMarkup ChooseTypePars()
        {
            InlineKeyboardMarkup keyboard = new(new []
            {
                new []
                {
                    InlineKeyboardButton.WithCallbackData(text: $"Парсить все категории", callbackData: "parse_all_categories"),
                },
                new []
                {
                    InlineKeyboardButton.WithCallbackData(text: $"Ввести ссылку", callbackData: "parse_category"),
                },
                new []
                {
                    InlineKeyboardButton.WithCallbackData(text: "🔙 Назад", callbackData: "back_to_countries"),
                }
            });
            return keyboard;
        }

        public static InlineKeyboardMarkup RegDateKb()
        {
            DateTime today = DateTime.Today;
            InlineKeyboardMarkup regDateKb;

            regDateKb = new(new []
            {
                new []
                {
                    InlineKeyboardButton.WithCallbackData(text: $"Сегодня - {today.ToString("dd.MM.yyyy")}", callbackData: "today_date"),
                },
                new []
                {
                    InlineKeyboardButton.WithCallbackData(text: $"Отключить", callbackData: "disable_reg_date"),
                },
                new []
                {
                    InlineKeyboardButton.WithCallbackData(text: "🔙 Назад", callbackData: "back_to_configuration"),
                }
            });

            return regDateKb;
        }

        public static InlineKeyboardMarkup sellerTotalAdsKb = new(new []
        {
            new []
            {
                InlineKeyboardButton.WithCallbackData(text: $"Отключить", callbackData: "disable_seller_total_ads"),
            },
            new []
            {
                InlineKeyboardButton.WithCallbackData(text: "🔙 Назад", callbackData: "back_to_configuration"),
            }
        });

        public static InlineKeyboardMarkup sellerRatingKb = new(new []
        {
            new []
            {
                InlineKeyboardButton.WithCallbackData(text: $"0", callbackData: "0"),
                InlineKeyboardButton.WithCallbackData(text: $"1", callbackData: "1"),
                InlineKeyboardButton.WithCallbackData(text: $"2", callbackData: "2"),
                InlineKeyboardButton.WithCallbackData(text: $"3", callbackData: "3"),
                InlineKeyboardButton.WithCallbackData(text: $"4", callbackData: "4"),
                InlineKeyboardButton.WithCallbackData(text: $"5", callbackData: "5"),
            },
            new []
            {
                InlineKeyboardButton.WithCallbackData(text: "🔙 Назад", callbackData: "back_to_configuration"),
            }
        });

        public static InlineKeyboardMarkup blacklistLinksKb = new(new []
        {
            new []
            {
                InlineKeyboardButton.WithCallbackData(text: "🗑 Удалить все ссылки", callbackData: "delete_all_links"),
            },
            new []
            {
                InlineKeyboardButton.WithCallbackData(text: "🔙 Назад", callbackData: "back_to_settings"),
            },
        });


        // PARSER
        public static InlineKeyboardMarkup StartPars = new(new []
        {
            new []
            {
                InlineKeyboardButton.WithCallbackData(text: "🏠 В меню", callbackData: "back_from_pars"),
                InlineKeyboardButton.WithCallbackData(text: "🪐 Начать парсинг", callbackData: "start_pars"),
            }
        });


        public static ReplyKeyboardMarkup StopPars()
        {
            ReplyKeyboardMarkup Keyboard;
            Keyboard = new(new []
            {
                new KeyboardButton[] {"Стоп"},
            })
            {
                ResizeKeyboard = true,
            };
            return Keyboard;
        }


        // BACK KEYBOARDS
        public static InlineKeyboardMarkup BackBtnKb = new(new []
        {
            new []
            {
                InlineKeyboardButton.WithCallbackData(text: "🏚 Вернуться в меню", callbackData: "back_to_menu"),
            },
        });
        public static InlineKeyboardMarkup BackToProfile = new(new []
        {
            new []
            {
                InlineKeyboardButton.WithCallbackData(text: "🏚 Вернуться в меню", callbackData: "back_to_profile"),
            },
        });

        public static InlineKeyboardMarkup BackFromParse = new(new []
        {
            new []
            {
                InlineKeyboardButton.WithCallbackData(text: "🏚 Вернуться в меню", callbackData: "back_from_pars"),
            },
        });

        public static InlineKeyboardMarkup BackToSettings= new(new []
        {
            new []
            {
                InlineKeyboardButton.WithCallbackData(text: "🔙 Назад", callbackData: "back_to_settings"),
            },
        });
        public static InlineKeyboardMarkup BackToConfiguration= new(new []
        {
            new []
            {
                InlineKeyboardButton.WithCallbackData(text: "🔙 Назад", callbackData: "back_to_configuration"),
            },
        });

        public static InlineKeyboardMarkup BackToCountries = new(new []
        {
            new []
            {
                InlineKeyboardButton.WithCallbackData(text: "🔙 Назад", callbackData: "back_to_countries"),
            },
        });
    }
}
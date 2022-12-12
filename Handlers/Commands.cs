using PostgreSQL;
using Bot_Keyboards;
using ConfigFile;
using Handlers;

using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.InputFiles;


namespace CommandsSpace
{
    public static class Commands
    {
        public static async void StartCommand(ITelegramBotClient botClient, string messageText, long chatId, int messageId, string firstName, string mainMenuPhoto, string username)
        {
            if(DB.CheckUser(chatId))
            {
                string state = "MainMenu";
                DB.UpdateState(chatId, state);
                await botClient.SendTextMessageAsync(
                    chatId: chatId,
                    text: $"⚡️",
                    replyMarkup: Keyboards.MenuKb()
                );
                
                using (var fileStream = new FileStream(mainMenuPhoto, FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    await botClient.SendPhotoAsync(
                        chatId: chatId,
                        photo: new InputOnlineFile(fileStream),
                        caption: GenerateMessageText.MenuText(chatId),
                        parseMode: ParseMode.Html,
                        replyMarkup: Keyboards.MainMenuButtons
                    );
                }
            }
            else
            {
                try
                {
                    DB.CreateUser(chatId, username);
                    DB.CreateBlackListTable(chatId);
                    

                    await botClient.SendTextMessageAsync(
                        chatId: chatId,
                        text: $"⚡️",
                        replyMarkup: Keyboards.MenuKb()
                    );

                    using (var fileStream = new FileStream(mainMenuPhoto, FileMode.Open, FileAccess.Read, FileShare.Read))
                    {
                        await botClient.SendPhotoAsync(
                            chatId: chatId,
                            photo: new InputOnlineFile(fileStream),
                            caption: GenerateMessageText.MenuText(chatId),
                            parseMode: ParseMode.Html,
                            replyMarkup: Keyboards.MainMenuButtons
                        );
                    }
                }
                catch(Exception e)
                {
                    Console.WriteLine(e);
                    await botClient.SendTextMessageAsync(
                        chatId: chatId,
                        text: $"<b>Чтобы пользоваться ботом нужно указать @username.\n\n</b>",
                        parseMode: ParseMode.Html
                    );
                }
            }
            return;
        }
    }
}
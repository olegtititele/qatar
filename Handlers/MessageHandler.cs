using PostgreSQL;
using Bot_Keyboards;
using ConfigFile;
using CommandsSpace;
using States;

using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.InputFiles;


namespace Handlers
{
    public static class MessHandler
    {
        private static string mainMenuPhoto = Config.menuPhoto;
        public static async Task BotOnMessageReceived(ITelegramBotClient botClient, Message message)
        {

            try
            {
                long chatId = message.Chat.Id;
                string? messageText = message.Text;
                int messageId = message.MessageId;
                string? username = message.Chat.Username;
                string? firstName = message.Chat.FirstName;
                string state = DB.GetState(chatId);
                
                if(chatId.ToString()[0]=='-')
                {
                    return;
                }

                if(messageText == null)
                {
                    messageText = "-------";
                }

                if(DB.GetState(chatId) == "Parser")
                {
                    try
                    {
                        await botClient.DeleteMessageAsync(
                            chatId: chatId,
                            messageId: messageId
                        );
                    }
                    catch{ }
                    return;
                }

                if(messageText![0]=='/')
                {
                    if(messageText!.Contains("/start"))
                    {
                        Commands.StartCommand(botClient, messageText!, chatId, messageId, firstName!, mainMenuPhoto, username!);
                        return;
                    }
                }
                else
                {
                    if(messageText == "Меню")
                    {
                        state = "MainMenu";
                        DB.UpdateState(chatId, state);
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
                        return;
                    }
                    else if(messageText.ToLower() == "стоп")
                    {
                        if(DB.GetParser(chatId) == "Start")
                        {
                            DB.UpdateParser(chatId, "Stop");
                        }
                        return;
                    }

                    switch (state)
                    {
                        case "WhatsappText":
                            WhatsappTextState.MessageHandler(botClient, messageText!, chatId, messageId, mainMenuPhoto);  
                            return;
                        case "CountryLink":
                            CountryLinkState.MessageHandler(botClient, messageText!, chatId, messageId); 
                            return;
                        case "SellerAdvCount":
                            SellerAdCountState.MessageHandler(botClient, messageText!, chatId, messageId, mainMenuPhoto);
                            return;
                        case "SellerRegData":
                            SellerRegDateState.MessageHandler(botClient, messageText!, chatId, messageId, mainMenuPhoto);
                            return;
                        case "SellerRating":
                            SellerRatingState.MessageHandler(botClient, messageText!, chatId, messageId, mainMenuPhoto);
                            return;
                        case "Timeout":
                            TimeoutState.MessageHandler(botClient, messageText!, chatId, messageId, mainMenuPhoto);
                            return;
                        case "LinksBlacklist":
                            LinksBlacklistState.MessageHandler(botClient, messageText!, chatId, messageId, mainMenuPhoto);
                            return;
                        default:
                            return;
                    }
                }
            }   
            catch(Exception)
            {
                return;
            }
        }
        public static bool CheckSubChannel(string chatMember)
        {
            if(chatMember == "Left")
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
using System.Globalization;

using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Extensions.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

using System.Net;
using ConfigFile;
using PostgreSQL;
using Handlers;
using Modules;



namespace Bot
{
    public static class Handlers
    {
        public static TelegramBotClient? botClient;
        public static async Task Main()
        {
            CultureInfo.CurrentCulture = Config.culture;
            ServicePointManager.Expect100Continue = true;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            botClient = new TelegramBotClient(Config.Bot_Token);

            // DB.AddColumn();
            // DB.DropUserTable();
            // DB.DropBlacklistTable(5409089881);
            // DB.DropBlacklistTable(5740336806);
            // Console.WriteLine(DB.GetSellerRegDate(5409089881));
            // DB.aaaa(5409089881);
            DB.DropBlacklistTable(5740336806);
            DB.CreateBlacklistLinksTable();
            DB.CreateUsersTable();
            DB.UpdateStates();
            

            using var cts = new CancellationTokenSource();

            // StartReceiving does not block the caller thread. Receiving is done on the ThreadPool.
            ReceiverOptions receiverOptions = new ReceiverOptions
            {
                AllowedUpdates = { } // receive all update types
            };
            botClient.StartReceiving(
                HandleUpdateAsync,
                HandleErrorAsync,
                receiverOptions,
                cancellationToken: cts.Token);

            User me = await botClient.GetMeAsync();


            Console.WriteLine($"Start listening for @{me.Username}");
            Console.ReadLine();

            // Send cancellation request to stop bot
            cts.Cancel();
        }

        public static Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
        {
            string ErrorMessage = exception switch
            {
                ApiRequestException apiRequestException
                    => $"Telegram API Error:\n[{apiRequestException.ErrorCode}]\n{apiRequestException.Message}",
                _ => exception.ToString()
            };

            Console.WriteLine(ErrorMessage);
            return Task.CompletedTask;
        }

        public static async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            try
            {
                Task handler = update.Type switch
                {
                    
                    UpdateType.Message            => MessHandler.BotOnMessageReceived(botClient, update.Message!),
                    UpdateType.EditedMessage      => MessHandler.BotOnMessageReceived(botClient, update.EditedMessage!),
                    UpdateType.CallbackQuery      => CallHandler.BotOnCallbackQueryReceived(botClient, update.CallbackQuery!),
                    UpdateType.Unknown            => UnknownUpdateHandlerAsync(botClient, update),
                    _                             => UnknownUpdateHandlerAsync(botClient, update)
                };

            
                await handler;
            }
            catch (Exception exception)
            {
                await HandleErrorAsync(botClient, exception, cancellationToken);
            }
        }

        private static Task UnknownUpdateHandlerAsync(ITelegramBotClient botClient, Update update)
        {
            Console.WriteLine($"Unknown update type: {update.Type}");
            return Task.CompletedTask;
        }
    }
}
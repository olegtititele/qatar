using PostgreSQL;

using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;


namespace States
{
    public static class AdminAlertState
    {
        async public static void Alert(ITelegramBotClient botClient, Message message, long chatId)
        {
            try
            {
                List<long> users = DB.GetAllUsersId();
                int success = 0, error = 0, total = 0;
                try
                {
                    string fileId = message.Photo![^1].FileId;
                    string caption = message.Caption!;
                    foreach(long userId in users)
                    {
                        total++;
                        try
                        {
                            await botClient.SendPhotoAsync(
                                chatId:userId,
                                photo: fileId,
                                caption: caption,
                                parseMode: ParseMode.Html
                            );
                            success++;
                        }
                        catch(Exception)
                        {
                            error++;
                        }
                    }
                }
                catch(Exception)
                {
                    string messageText = message.Text!;
                    foreach(long userId in users)
                    {
                        total++;
                        try
                        {
                            await botClient.SendTextMessageAsync(
                                chatId: userId,
                                text: messageText,
                                parseMode: ParseMode.Html
                            );
                            success++;
                        }
                        catch(Exception)
                        {
                            error++;
                        }
                    }
                }

                string state = "MainMenu";
                DB.UpdateState(chatId, state);

                await botClient.SendTextMessageAsync(
                    chatId: chatId,
                    text: $"<b>Всего рассылок:</b> <code>{total}</code>\n\n<b>✅ Успешно:</b> <code>{success}</code>\n<b>❌ Неудачно:</b> <code>{error}</code>",
                    parseMode: ParseMode.Html
                );
            }
            catch
            {
                return;
            }
        }
    }
}
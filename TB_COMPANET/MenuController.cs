using Telegram.Bot;
using Telegram.Bot.Extensions.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
namespace TB_COMPANET
{
    public static class MenuController
    {
        public static async Task ShowMenu(ITelegramBotClient botClient, Update update,
            CancellationToken cancellationToken, bool flag = true)
        {
            var inlineKeyboard = new InlineKeyboardMarkup(new[]
            {
                new[]
                {
                    InlineKeyboardButton.WithCallbackData("Add new payment", "add"),
                },
                new[]
                {
                    InlineKeyboardButton.WithCallbackData("Show last operations", "showoperations"),
                },
                new[]
                {
                    InlineKeyboardButton.WithCallbackData("Show sum per day", "showsumperday"),
                }
            });
            if (flag)
            {
                await botClient.SendTextMessageAsync(update.Message.Chat, "Choose command:",
                    replyMarkup: inlineKeyboard, cancellationToken: cancellationToken);
            }
            else
            {
                await botClient.SendTextMessageAsync(update.CallbackQuery.Message.Chat.Id, "Choose command:",
                    replyMarkup: inlineKeyboard, cancellationToken: cancellationToken);
            }
        }
    }
}
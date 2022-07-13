using System;
using System.Threading;
using System.Threading.Tasks;
using TB_COMPANET.DataBase;
using Telegram.Bot;
using Telegram.Bot.Extensions.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Types.ReplyMarkups;

namespace TB_COMPANET
{

    class Program
    {
        static ITelegramBotClient bot = new TelegramBotClient("5435335474:AAFZm_O4rvux147BW0nP4U-rcMs8N3JLyc4");
        public static async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            var obj = Newtonsoft.Json.JsonConvert.SerializeObject(update);
            Console.WriteLine(obj);
            Console.WriteLine();
            if(update.Type == Telegram.Bot.Types.Enums.UpdateType.Message)
            {
                var message = update.Message;
                if (message.Text.ToLower() == "/start")
                {
                    await botClient.SendTextMessageAsync(message.Chat, "Welcome to COMPANET BOT");
                    var inlineKeyboard = new InlineKeyboardMarkup(new[]
                    {
                        new[]
                        {
                            InlineKeyboardButton.WithCallbackData("Identification","identification"),
                        },
                        new[]
                        {
                            InlineKeyboardButton.WithCallbackData("123","123"),
                        },
                    });
                    await botClient.SendTextMessageAsync(update.Message.Chat, "Choose command:", replyMarkup: inlineKeyboard, cancellationToken: cancellationToken);
                    return;
                }
                await botClient.SendTextMessageAsync(message.Chat, "There is'nt such command");
            }
            else if (update.Type == Telegram.Bot.Types.Enums.UpdateType.CallbackQuery)
            {
                var message = update.Message;
                await botClient.SendTextMessageAsync(message.Chat, "There is'nt such command");
            }
        }
        

        public static async Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
        {
            // Некоторые действия
            Console.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(exception));
        }


        static void Main(string[] args)
        {
            Console.WriteLine("Telegram bot launch " + bot.GetMeAsync().Result.FirstName);

            //var dataBase = new SQLApplication();
            var cts = new CancellationTokenSource();
            var cancellationToken = cts.Token;
            var receiverOptions = new ReceiverOptions
            {
                AllowedUpdates = { }, // receive all update types
            };
            bot.StartReceiving(
                HandleUpdateAsync,
                HandleErrorAsync,
                receiverOptions,
                cancellationToken
            );
            Console.ReadLine();
        }
    }
}
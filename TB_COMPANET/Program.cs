using System;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using TB_COMPANET.DataBase;
using Telegram.Bot;
using Telegram.Bot.Extensions.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Types.ReplyMarkups;

namespace TB_COMPANET
{
    public static class Program
    {
        private static ITelegramBotClient bot = new TelegramBotClient(Configuration.Token);
        private static SqlApplication _database = new SqlApplication();
        private static string[] _bufferNewPayment = new string[3];
        public static async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            var obj = Newtonsoft.Json.JsonConvert.SerializeObject(update);
            Console.WriteLine(obj);
            Console.WriteLine();
            if(update.Type == Telegram.Bot.Types.Enums.UpdateType.Message)
            {
                var message = update.Message;
                Console.WriteLine("Message\n");
                if (message.Text.ToLower() == "/start" && Configuration.IsIdentificated == false)
                {
                    Console.WriteLine("Start\n");
                    await botClient.SendTextMessageAsync(message.Chat, "🆆🅴🅻🅲🅾🅼🅴 🆃🅾 🅲🅾🅼🅿🅰🅽🅴🆃 🅱🅾🆃");
                    var replyKeyboard = new ReplyKeyboardMarkup(new[]
                    {
                        new KeyboardButton("Identification")
                    })
                    {
                        ResizeKeyboard = true
                    };
                    await botClient.SendTextMessageAsync(update.Message.Chat, "Before continuing, enter your id by clicking\n🅸🅳🅴🅽🆃🅸🅵🅸🅲🅰🆃🅸🅾🅽:", replyMarkup: replyKeyboard, cancellationToken: cancellationToken);
                    return;
                }
                if (message.Text == "Identification" && Configuration.IsIdentificated == false)
                {
                    Console.WriteLine("Identification\n");
                    await botClient.SendTextMessageAsync(update.Message.Chat, "Write your 🅸🅳:", replyMarkup: Configuration.ClearKeyboardRemove, cancellationToken: cancellationToken);
                    Configuration.IsIdDataEntered = true;
                    return;
                }
                if (message.Text.ToLower() == "/exit")
                {
                    Console.WriteLine("Exit\n");
                    await botClient.SendTextMessageAsync(message.Chat, "      𝘎𝘖𝘖𝘋𝘉𝘜𝘠\n♠🍩  ร𝔼є Ƴ𝐎𝓾 ᔕⓞ๏Ｎ!  🎁👊");
                    Configuration.IsIdentificated = false;
                    Configuration.IsIdDataEntered = false;
                    Configuration.CurrentCompany = null;
                    Configuration.OperationInProgress = null;
                    Configuration.StepsOfFunction = 0;
                    return;
                }
                if (Configuration.IsIdDataEntered)
                {
                    Console.WriteLine("DataEntered\n");
                    var companies = _database.Companies.ToList();
                    var result = companies.FirstOrDefault(company => company.CompanyId == message.Text);
                    if (result == null)
                    {
                        await botClient.SendTextMessageAsync(message.Chat, "There is'nt such CompanyId, try again");
                        return;
                    }
                    await botClient.SendTextMessageAsync(message.Chat, $"We are glad to see you again {result.Name}");
                    Configuration.CurrentCompany = result;
                    Configuration.IsIdDataEntered = false;
                    Configuration.IsIdentificated = true;
                }
                if (Configuration.OperationInProgress == "add")
                {
                    Console.WriteLine($"Add New Payment {Configuration.StepsOfFunction}\n");
                    switch (Configuration.StepsOfFunction)
                    {
                        case 1:
                        {
                            _bufferNewPayment[0] = message.Text;
                            await botClient.SendTextMessageAsync(message.Chat, "Write description of the operation");
                            Configuration.StepsOfFunction++;
                            return;
                        }
                        case 2:
                        {
                            _bufferNewPayment[1] = message.Text;
                            await botClient.SendTextMessageAsync(message.Chat, "Write Email");
                            Configuration.StepsOfFunction++;
                            return;
                        }
                        case 3:
                        {
                            _bufferNewPayment[2] = message.Text;
                            await botClient.SendTextMessageAsync(message.Chat, "Data processing, please wait");
                            Configuration.StepsOfFunction++;
                            var newOperation = new Operation(Configuration.CurrentCompany.CompanyId,_bufferNewPayment[2],Convert.ToInt64(_bufferNewPayment[0]),_bufferNewPayment[1],DateTime.Now);
                            _database.Operations.Add(newOperation);
                            _database.SaveChanges();
                            await botClient.SendTextMessageAsync(message.Chat, "New operation was added");
                            Configuration.OperationInProgress = default;
                            Configuration.StepsOfFunction = default;
                            await MenuController.ShowMenu(botClient,update,cancellationToken);
                            return;
                        }
                    }
                }
                if (Configuration.IsIdentificated)
                {
                    await MenuController.ShowMenu(botClient,update,cancellationToken);
                    return;
                }
                await botClient.SendTextMessageAsync(message.Chat, "There is'nt such command");
            }
            else if (update.Type == Telegram.Bot.Types.Enums.UpdateType.CallbackQuery)
            {
                Console.WriteLine("Callback\n");
                var callback = update.CallbackQuery;
                if (callback.Data == "add")
                {
                    Console.WriteLine("Add new payment method\n");
                    await botClient.SendTextMessageAsync(callback.Message.Chat.Id, "Add New Payment method\n Write bill");
                    Configuration.OperationInProgress = callback.Data;
                    Configuration.StepsOfFunction++;
                }
                else if (callback.Data == "showoperations")
                {
                    Console.WriteLine("Show last operations method\n");
                    var companyOperations = _database.Operations.ToList();
                    var result = companyOperations
                        .Where(operation => operation.CompanyId == Configuration.CurrentCompany.CompanyId)
                        .OrderBy(operation => operation.Date)
                        .ToList();
                    result = result.Skip(Math.Max(0, result.Count() - 10)).ToList();
                    Console.WriteLine($"Show last {result.Count} operations\n");
                    var str = "Total operations:\n";
                    foreach (var operation in result)
                    {
                        str +=
                            $"Date: {operation.Date:g}\nFrom: {operation.CompanyId}\nTo: {operation.Email}\nBill: {operation.Bill}\nDescription:\n{operation.Description}\n\n";
                    }
                    await botClient.SendTextMessageAsync(callback.Message.Chat.Id, str);
                    await MenuController.ShowMenu(botClient,update,cancellationToken,false);
                }
                else if (callback.Data == "showsumperday")
                {
                    Console.WriteLine("Showsumperday method\n");
                    var companyOperations = _database.Operations.ToList();
                    var result = companyOperations
                        .Where(operation => operation.CompanyId == Configuration.CurrentCompany.CompanyId)
                        .Where(operation => operation.Date < DateTime.Now && operation.Date > DateTime.Today)
                        .Sum(operation => operation.Bill);
                    await botClient.SendTextMessageAsync(callback.Message.Chat.Id, $"Total sum: {result}");
                    await MenuController.ShowMenu(botClient,update,cancellationToken,false);
                }
            }
        }
        
        public static async Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
        {
            Console.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(exception));
        }


        public static void Main(string[] args)
        {
            Console.WriteLine("Telegram bot was launched " + bot.GetMeAsync().Result.FirstName);

            var cts = new CancellationTokenSource();
            var cancellationToken = cts.Token;
            var receiverOptions = new ReceiverOptions
            {
                AllowedUpdates = { }, // receive all update types
            };
            bot.StartReceiving(HandleUpdateAsync, HandleErrorAsync, receiverOptions, cancellationToken);
            Console.ReadLine();
        }
    }
}


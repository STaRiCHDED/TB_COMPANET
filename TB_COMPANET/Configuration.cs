using Newtonsoft.Json;
using TB_COMPANET.DataBase;
using Telegram.Bot.Types.ReplyMarkups;

namespace TB_COMPANET;

public static class Configuration
{
    public static string Token;
    public static string SqlServerConfiguration;
    public static bool IsIdentificated = false;
    public static bool IsIdDataEntered = false;
    public static Company? CurrentCompany = null;
    public static string? OperationInProgress = null;
    public static byte StepsOfFunction = 0;
    public static ReplyKeyboardRemove ClearKeyboardRemove = new();
    private class SpecialData
    {
        [JsonProperty("Token")]
        public string Token { get; set; }
        [JsonProperty("SqlServerConfiguration")]
        public string SQLConfiguration{ get; set; }
    }
    static Configuration()
    {
        var config = JsonConvert.DeserializeObject<SpecialData>(new StreamReader("Config.json").ReadToEnd());
        Token = config.Token;
        SqlServerConfiguration = config.SQLConfiguration;
        Console.WriteLine(Token);
        Console.WriteLine(SqlServerConfiguration);
    }
}
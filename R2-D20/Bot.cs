using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using DSharpPlus.VoiceNext;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace R2D20
{
  public class Bot
  {
    static public Random s_RNG = new Random();

    public DiscordClient m_Client { get; private set; }
    public CommandsNextExtension m_Commands { get; private set; }
    public VoiceNextExtension m_Voice { get; private set; }

    public async Task RunAsync()
    {
      var json = string.Empty;

      using (var fs = File.OpenRead("config.json"))
      using (var sr = new StreamReader(fs, new UTF8Encoding(false)))
        json = await sr.ReadToEndAsync().ConfigureAwait(false);

      var configJson = JsonConvert.DeserializeObject<ConfigJson>(json);

      var config = new DiscordConfiguration
      {
        Token = configJson.Token,
        TokenType = TokenType.Bot,
        AutoReconnect = true,
        LogLevel = LogLevel.Debug,
        UseInternalLogHandler = true,
      };

      m_Client = new DiscordClient(config);
      m_Client.UseVoiceNext();

      m_Client.Ready += OnClientReady;

      var commandsConfig = new CommandsNextConfiguration
      {
        StringPrefixes = new string[] { configJson.Prefix },
        EnableMentionPrefix = true,
        EnableDms = true,  // maybe change this to allow commands via PM
        DmHelp = true,
        IgnoreExtraArguments = true,
      };

      m_Commands = m_Client.UseCommandsNext(commandsConfig);

      m_Commands.RegisterCommands<R2Command>();

      await m_Client.ConnectAsync();
      await Task.Delay(-1);
    }

    private Task OnClientReady(ReadyEventArgs e)
    {
      return Task.CompletedTask;
    }
  }
}

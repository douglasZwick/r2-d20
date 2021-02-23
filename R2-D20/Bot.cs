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
using System.Runtime.InteropServices.WindowsRuntime;
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
    public string m_Prefix { get; private set; }

    public List<string> m_AutoDeleteCommands = new List<string>()
    {
      "play",
      "emote",
      "join",
      "ping",
      "leave",
      "smash",
      "begincrawl",
    };

    public async Task RunAsync()
    {
      var json = string.Empty;

      using (var fs = File.OpenRead("config.json"))
      using (var sr = new StreamReader(fs, new UTF8Encoding(false)))
        json = await sr.ReadToEndAsync().ConfigureAwait(false);

      var configJson = JsonConvert.DeserializeObject<ConfigJson>(json);
      m_Prefix = configJson.Prefix;

      var config = new DiscordConfiguration
      {
        Token = configJson.Token,
        TokenType = TokenType.Bot,
        AutoReconnect = true,
        //LogLevel = LogLevel.Debug,
        //UseInternalLogHandler = true,
      };

      m_Client = new DiscordClient(config);
      m_Client.UseVoiceNext();

      m_Client.Ready += OnClientReady;
      m_Client.MessageCreated += OnMessageCreated;
      m_Client.MessageCreated += CheckForPlaySyntax;
      m_Client.MessageUpdated += OnMessageUpdated;
      m_Client.MessageUpdated += CheckUpdatedMessageForPlaySyntax;

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

      m_Commands.CommandExecuted += OnCommandExecuted;

      await m_Client.ConnectAsync();
      await Task.Delay(-1);
    }

    private Task OnClientReady(DiscordClient client, ReadyEventArgs e)
    {
      return Task.CompletedTask;
    }

    private Task OnMessageCreated(DiscordClient client, MessageCreateEventArgs e)
    {
      if (!e.Author.IsBot) return Task.CompletedTask;

      _ = Task.Run(async () =>
      {
        var message = e.Message;
        var commandStart = message.GetStringPrefixLength(m_Prefix);
        if (commandStart == -1) return;

        var prefix = message.Content.Substring(0, commandStart);
        var invocation = message.Content.Substring(commandStart);
        var command = m_Commands.FindCommand(invocation, out var args);
        if (command == null) return;

        if (command.Name != "play") return;

        var ctx = m_Commands.CreateContext(message, prefix, command, args);
        await m_Commands.ExecuteCommandAsync(ctx);
      });

      return Task.CompletedTask;
    }

    private Task CheckForPlaySyntax(DiscordClient client, MessageCreateEventArgs e)
    {
      return PlaySyntaxHelper(client, e.Message);
    }

    private Task CheckUpdatedMessageForPlaySyntax(DiscordClient client, MessageUpdateEventArgs e)
    {
      return PlaySyntaxHelper(client, e.Message);
    }

    private Task PlaySyntaxHelper(DiscordClient client, DiscordMessage message)
    {
      _ = Task.Run(async () =>
      {
        var content = message.Content.Trim();
        if (!content.StartsWith('`')) return;

        var firstIndex = content.IndexOf('`');
        var lastIndex = content.LastIndexOf('`');
        if (firstIndex != lastIndex) return;

        var invocation = "play " + content.Substring(firstIndex + 1);
        var command = m_Commands.FindCommand(invocation, out var args);
        if (command == null) return;

        var ctx = m_Commands.CreateContext(message, "!", command, args);
        await m_Commands.ExecuteCommandAsync(ctx);
      });

      return Task.CompletedTask;
    }

    private Task OnMessageUpdated(DiscordClient client, MessageUpdateEventArgs e)
    {
      _ = Task.Run(async () =>
      {
        var message = e.Message;
        var commandStart = message.GetStringPrefixLength(m_Prefix);
        if (commandStart == -1) return;

        var prefix = message.Content.Substring(0, commandStart);
        var invocation = message.Content.Substring(commandStart);
        var command = m_Commands.FindCommand(invocation, out var args);
        if (command == null) return;

        var ctx = m_Commands.CreateContext(message, prefix, command, args);
        await m_Commands.ExecuteCommandAsync(ctx);
      });

      return Task.CompletedTask;
    }

    private async Task OnCommandExecuted(CommandsNextExtension cne, CommandExecutionEventArgs e)
    {
      if (m_AutoDeleteCommands.Contains(e.Command.Name))
        await e.Context.Message.DeleteAsync();
    }
  }
}

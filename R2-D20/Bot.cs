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
      "dance",
      "spinee",
    };

    public List<string> m_CommonChatter = new List<string>()
    {

    };

    public List<string> m_RareChatter = new List<string>()
    {

    };

    public List<string> m_MythicChatter = new List<string>()
    {

    };

    public List<string> AggroResponses = new List<string>()
    {

    };

    public List<string> Responses = new List<string>()
    {

    };

    public List<string> BadWords = new List<string>()
    {

    };

    public int MessageCounter = 0;
    public int ChatterThreshold;
    public int CommonChatterCounter = 0;
    public int RareChatterCounter = 0;
    public int MythicChatterCounter = 0;
    public int RareChatterThreshold;
    public int MythicChatterThreshold;

    public string CommonChatterPath = $"text{Path.DirectorySeparatorChar}CommonChatter.txt";
    public string RareChatterPath = $"text{Path.DirectorySeparatorChar}RareChatter.txt";
    public string MythicChatterPath = $"text{Path.DirectorySeparatorChar}MythicChatter.txt";
    public string AggroResponsesPath = $"text{Path.DirectorySeparatorChar}AggroResponses.txt";
    public string ResponsesPath = $"text{Path.DirectorySeparatorChar}Responses.txt";
    public string BadWordsPath = $"text{Path.DirectorySeparatorChar}BadWords.txt";

    public async Task RunAsync()
    {
      R2Command.s_StartDateTime = DateTime.Now;

      RandomizeChatterThreshold();
      RandomizeRareChatterThreshold();
      RandomizeMythicChatterThreshold();

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
      m_Client.MessageCreated += Chatter;
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

      R2Command.LoadSavedData();

      await Task.Delay(-1);
    }

    private Task OnClientReady(DiscordClient client, ReadyEventArgs e)
    {
      return Task.CompletedTask;
    }

    private Task Chatter(DiscordClient client, MessageCreateEventArgs e)
    {
      if (e.Author.IsBot || e.Channel.Name == "vent") return Task.CompletedTask;

      _ = Task.Run(async () =>
      {
        //Did we get pinged?
        if(e.Message.MentionedUsers.Contains(client.CurrentUser))
        {
          await Task.Delay(2000);
          string message = String.Empty;
          bool messageIsAggro = false;
          try
          {
            using (var sr = new StreamReader(BadWordsPath))
            {
              string chatterLines = sr.ReadToEnd();
              BadWords = chatterLines.Split(System.Environment.NewLine).ToList();
            }
          }
          catch (IOException e)
          {
            Console.WriteLine("The file could not be read: ");
            Console.WriteLine(e.Message);
          }

          for(int i = 0; i < BadWords.Count; ++i)
          {
            if(e.Message.Content.ToLower().Contains(BadWords[i]))
            {
              messageIsAggro = true;
            }
          }

          Console.WriteLine($"Ping received. Message is aggro: ({messageIsAggro})");

          if(messageIsAggro)
          {
            try
            {
              using (var sr = new StreamReader(AggroResponsesPath))
              {
                string chatterLines = sr.ReadToEnd();
                AggroResponses = chatterLines.Split(System.Environment.NewLine).ToList();
              }
            }
            catch (IOException e)
            {
              Console.WriteLine("The file could not be read: ");
              Console.WriteLine(e.Message);
            }
            message = AggroResponses[s_RNG.Next(AggroResponses.Count)];
          }
          else
          {
            try
            {
              using (var sr = new StreamReader(ResponsesPath))
              {
                string chatterLines = sr.ReadToEnd();
                Responses = chatterLines.Split(System.Environment.NewLine).ToList();
              }
            }
            catch (IOException e)
            {
              Console.WriteLine("The file could not be read: ");
              Console.WriteLine(e.Message);
            }
            message = Responses[s_RNG.Next(Responses.Count)];
          }

          await e.Message.RespondAsync(message).ConfigureAwait(false);
        }
        else
        {
          await Task.Delay(1000);
          MessageCounter++;
          if(MessageCounter >= ChatterThreshold)
          {
            string message = string.Empty;
            bool bMythic = false;
            //Which rarity are we saying?
            if(RareChatterCounter >= MythicChatterThreshold)
            {
              //Mythic Chatter
              try
              {
                using (var sr = new StreamReader(MythicChatterPath))
                {
                  string chatterLines = sr.ReadToEnd();
                  m_MythicChatter = chatterLines.Split(System.Environment.NewLine).ToList();
                }
              }
              catch (IOException e)
              {
                Console.WriteLine("The file could not be read: ");
                Console.WriteLine(e.Message);
              }
              message = m_MythicChatter[s_RNG.Next(m_MythicChatter.Count)];
              bMythic = true;
              RareChatterCounter = 0;
              RandomizeMythicChatterThreshold();
              MythicChatterCounter++;
              Console.WriteLine($"Mythic Chatter Counter: {MythicChatterCounter}");
            }
            else if(CommonChatterCounter >= RareChatterThreshold)
            {
              //Rare Chatter
              try
              {
                using (var sr = new StreamReader(RareChatterPath))
                {
                  string chatterLines = sr.ReadToEnd();
                  m_RareChatter = chatterLines.Split(System.Environment.NewLine).ToList();
                }
              }
              catch (IOException e)
              {
                Console.WriteLine("The file could not be read: ");
                Console.WriteLine(e.Message);
              }
              message = m_RareChatter[s_RNG.Next(m_RareChatter.Count)];
              CommonChatterCounter = 0;
              RandomizeRareChatterThreshold();
              RareChatterCounter++;
              Console.WriteLine($"Rare Chatter Counter: {RareChatterCounter}");
            }
            else
            {
              //Common Chatter
              try
              {
                using (var sr = new StreamReader(CommonChatterPath))
                {
                  string chatterLines = sr.ReadToEnd();
                  m_CommonChatter = chatterLines.Split(System.Environment.NewLine).ToList();
                }
              }
              catch (IOException e)
              {
                Console.WriteLine("The file could not be read: ");
                Console.WriteLine(e.Message);
              }
              message = m_CommonChatter[s_RNG.Next(m_CommonChatter.Count)];
              CommonChatterCounter++;
              Console.WriteLine($"Common Chatter Counter: {CommonChatterCounter}");
            }

            RandomizeChatterThreshold();
            if(bMythic)
            {
              await e.Message.RespondAsync(message).ConfigureAwait(false);
              //Console.WriteLine(message);
            }
            else
            {
              await e.Channel.SendMessageAsync(message).ConfigureAwait(false);
              //Console.WriteLine(message);
            }
          }
        }
      });
      return Task.CompletedTask;
    }

    private void RandomizeRareChatterThreshold()
    {
      RareChatterThreshold = s_RNG.Next(3, 7);
      Console.WriteLine($"Current Rare Chatter Threshold: {RareChatterThreshold}");
    }
    private void RandomizeMythicChatterThreshold()
    {
      MythicChatterThreshold = s_RNG.Next(2, 4);
      Console.WriteLine($"Current Mythic Chatter Threshold: {MythicChatterThreshold}");
    }
    
    private void RandomizeChatterThreshold()
    {
      ChatterThreshold = s_RNG.Next(15, 50);
      Console.WriteLine($"Current Chatter Threshold: {ChatterThreshold}");
      MessageCounter = 0;
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

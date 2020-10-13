using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using DSharpPlus.VoiceNext;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace R2D20
{
  public class R2Command : BaseCommandModule
  {
    public static Dictionary<string, FfgDie> s_DiceByString = new Dictionary<string, FfgDie>()
    {
      { "b", FfgDie.s_BoostDie },
      { "boost", FfgDie.s_BoostDie },
      { "boostDie", FfgDie.s_BoostDie },
      { ":boostDie:", FfgDie.s_BoostDie },
      { "<:boostDie:739284154415317023>", FfgDie.s_BoostDie },

      { "s", FfgDie.s_SetbackDie },
      { "setback", FfgDie.s_SetbackDie },
      { "setbackDie", FfgDie.s_SetbackDie },
      { ":setbackDie:", FfgDie.s_SetbackDie },
      { "<:setbackDie:739284154516242482>", FfgDie.s_SetbackDie },

      { "a", FfgDie.s_AbilityDie },
      { "ability", FfgDie.s_AbilityDie },
      { "abilityDie", FfgDie.s_AbilityDie },
      { ":abilityDie:", FfgDie.s_AbilityDie },
      { "<:abilityDie:739284154688077824>", FfgDie.s_AbilityDie },

      { "d", FfgDie.s_DifficultyDie },
      { "difficulty", FfgDie.s_DifficultyDie },
      { "difficultyDie", FfgDie.s_DifficultyDie },
      { ":difficultyDie:", FfgDie.s_DifficultyDie },
      { "<:difficultyDie:739284154436550698>", FfgDie.s_DifficultyDie },

      { "p", FfgDie.s_ProficiencyDie },
      { "proficiency", FfgDie.s_ProficiencyDie },
      { "proficiencyDie", FfgDie.s_ProficiencyDie },
      { ":proficiencyDie:", FfgDie.s_ProficiencyDie },
      { "<:proficiencyDie:739284154696335360>", FfgDie.s_ProficiencyDie },

      { "c", FfgDie.s_ChallengeDie },
      { "challenge", FfgDie.s_ChallengeDie },
      { "challengeDie", FfgDie.s_ChallengeDie },
      { ":challengeDie:", FfgDie.s_ChallengeDie },
      { "<:challengeDie:739284154666975242>", FfgDie.s_ChallengeDie },

      { "f", FfgDie.s_ForceDie },
      { "force", FfgDie.s_ForceDie },
      { "forceDie", FfgDie.s_ForceDie },
      { ":forceDie:", FfgDie.s_ForceDie },
      { "<:forceDie:739284154515980328>", FfgDie.s_ForceDie },
    };

    public static List<string> s_VeryHappySounds = new List<string>()
    {
      "R2.excited",
      "R2.laughing",
      "R2.unbelievable",
    };

    public static List<string> s_HappySounds = new List<string>()
    {
      "R2.cheerful",
      "R2.eureka",
      "R2.veryexcited",
    };

    public static List<string> s_KindaHappySounds = new List<string>()
    {
      "R2.playful",
      "R2.proud",
      "R2.squeaky"
    };

    public static List<string> s_VeryUpsetSounds = new List<string>()
    {
      "R2.snappy",
      "R2.surprised",
      "R2.hit",
      "R2.badmotivator",
    };

    public static List<string> s_UpsetSounds = new List<string>()
    {
      "R2.processing",
      "R2.sad",
      "R2.danger",
    };

    public static List<string> s_KindaUpsetSounds = new List<string>()
    {
      "R2.concerned",
      "R2.look",
      "R2.warning",
    };

    public static List<string> s_UnsureSounds = new List<string>()
    {
      "R2.unsure",
      "R2.determined",
    };

    public static List<string> s_ChaosSounds = new List<string>()
    {
      "R2.shocked",
    };

    public static List<string> s_CommandSounds = new List<string>()
    {
      "R2.cheerful",
      "R2.look",
      "R2.playful",
      "R2.meepwalla",
    };

    public static List<string> s_ErrorSounds = new List<string>()
    {
      "R2.snappy",
      "R2.unsure",
    };

    public static Dictionary<string, List<string>> s_Emotions = new Dictionary<string, List<string>>()
    {
      { "veryhappy", s_VeryHappySounds },
      { "happy", s_HappySounds },
      { "kindahappy", s_KindaHappySounds },
      { "veryupset", s_VeryUpsetSounds },
      { "upset", s_UpsetSounds },
      { "kindaupset", s_KindaUpsetSounds },
      { "unsure", s_UnsureSounds },
      { "chaos", s_ChaosSounds },
      { "command", s_CommandSounds },
      { "error", s_ErrorSounds },
    };

    public static FfgDie.Pool s_CurrentPool = new FfgDie.Pool();

    public static Dictionary<(string, string), string> s_AutoSoundTriggers = new Dictionary<(string, string), string>()
    {
      { ("hedontlikeyou", "sorry"), "idontlikeyou" },
      { ("omgvinny1", "omgvinny2"), "omgvinny3" },
      { ("kame", "hame"), "ha" },
      { ("secret.kame", "secret.hame"), "secret.ha" },
      { ("scouter", "9000"), "what9000" },
      { ("dumbasses", "whatdidhecallus"), "nowaytotalktopeople" },
      //{ ("nut", "nut"), "nut" }, gotta fix this one soon
    };
    public static int s_AutoSoundDelay = 1000; // in ms
    public static string s_MostRecentSound;
    public static string s_SmashSM  = "<:sm:754881714857443419>";
    public static string s_SmashAAA = "<:aaa:754881715595640913>";
    public static string s_SmashSH  = "<:sh:754881714668437535>";

    public static string s_CrawlText =
      "                EPISODE IV\n" +
      " \n" +
      "              A NEW HOPE\n" +
      " \n" +
      " \n" +
      "It   is   a   period   of   civil   war.\n" +
      "Rebel     spaceships,      striking\n" +
      "from a hidden base,  have won\n" +
      "their     first     victory    against\n" +
      "the evil Galactic Empire.\n" +
      " \n" +
      "During     the     battle,      Rebel\n" +
      "spies  managed  to steal secret\n" +
      "plans       to       the       Empire's\n" +
      "ultimate  weapon,  the  DEATH\n" +
      "STAR,     an     armored    space\n" +
      "station  with  enough  power to\n" +
      "destroy an entire planet.\n" +
      " \n" +
      "Pursued     by     the     Empire's\n" +
      "sinister       agents,       Princess\n" +
      "Leia   races   home  aboard  her\n" +
      "starship,     custodian    of    the\n" +
      "stolen plans  that can  save her\n" +
      "people            and            restore\n" +
      "freedom to the galaxy....";
    public static int s_CrawlLineDelay = 1500;  // in ms
    public readonly static int s_MaxCrawlLines = 12;
    public static string s_CrawlMusic = "music.starwars";
    public static int s_CrawlDelay0 = 2000; // before intro text
    public static int s_CrawlDelay1 = 3500; // intro text duration
    public static int s_CrawlDelay2 = 2000; // between intro and game title
    public static int s_CrawlDelay3 = 9000; // game title duration
    public static int s_CrawlDelay4 = 2000; // between game title and crawl
    public static int s_CrawlDelay5 = 2000; // after the last line of the crawl
    public static string s_CrawlIntro0 = "        A long time ago in a galaxy far,";
    public static string s_CrawlIntro1 = "        far away....";
    public static string s_CrawlGameTitle0 = "                    `BEAN`";
    public static string s_CrawlGameTitle1 = "                    `TOWN`";

    static List<string> s_CrawlLines;


    [Command("ping")]
    [Description("Used as a simple acknowledgement that I'm online.")]
    public async Task Ping(CommandContext ctx)
    {
      await Say(ctx, "[ Beep. ]");
      await Play(ctx, PickRandom(s_CommandSounds));
    }

    [Command("echo")]
    [Description("Makes me repeat what you input back to you.")]
    public async Task Echo(CommandContext ctx,
      [Description("The stuff to repeat.")] params string[] args)
    {
      var message = "[ Meep ]" + Environment.NewLine;
      foreach (var arg in args)
      {
        message += "`" + arg + "`" + Environment.NewLine;
      }
      message += "[ zorp. ]";
      await Say(ctx, message);
      await Play(ctx, PickRandom(s_CommandSounds));
    }

    [Command("join")]
    [Description("Asks me to join a voice channel.")]
    public async Task Join(CommandContext ctx,
      [Description("The channel to join. If blank, I'll join the channel you're in.")]
      [RemainingText] string channelName)
    {
      var voiceNext = ctx.Client.GetVoiceNext();

      DiscordChannel channel = null;

      if (string.IsNullOrEmpty(channelName))
      {
        channel = ctx.Member?.VoiceState?.Channel;
        if (channel == null)
          throw new InvalidOperationException("You need to be in a voice channel.");
      }
      else
      {
        var allChannels = ctx.Guild.Channels;
        var values = allChannels.Values;
        foreach (var value in values)
        {
          if (value.Name == channelName)
          {
            channel = value;
            break;
          }
        }

        if (channel == null)
          throw new InvalidOperationException("There doesn't seem to be a channel by that name.");
      }

      var voiceNextConnection = voiceNext.GetConnection(ctx.Guild);
      if (voiceNextConnection != null)
      {
        if (voiceNextConnection.Channel == channel)
        {
          var spec = string.IsNullOrEmpty(channelName) ? "your" : "that";
          throw new InvalidOperationException($"R2-D20 is already connected to {spec} channel.");
        }

        await Leave(ctx);
      }

      await voiceNext.ConnectAsync(channel);
    }

    [Command("leave")]
    [Description("Asks me to leave the voice channel I'm in.")]
    public async Task Leave(CommandContext ctx)
    {
      var voiceNext = ctx.Client.GetVoiceNext();

      var voiceNextConnection = voiceNext.GetConnection(ctx.Guild);
      if (voiceNextConnection == null)
        throw new InvalidOperationException("R2-D20 is not connected to a voice channel in this server.");

      await Task.Run(voiceNextConnection.Disconnect);
    }

    [Command("play")]
    [Description("Asks me to play a sound. Use !soundlist to see what I can play.")]
    public async Task Play(CommandContext ctx,
      [Description("The name of the sound for me to play.")]
      [RemainingText] string soundName)
    {
      var guild = ctx.Guild;
      if (guild == null)
        throw new InvalidOperationException("No guild, no voice channels. Was this a DM...?");

      var path = $"audio{Path.DirectorySeparatorChar}{soundName}.mp3";

      var voiceNext = ctx.Client.GetVoiceNext();

      var voiceNextConnection = voiceNext.GetConnection(guild);
      if (voiceNextConnection == null)
        throw new InvalidOperationException("R2-D20 is not connected to a voice channel in this server.");

      if (!File.Exists(path))
        throw new FileNotFoundException("That file was not found.");

      await voiceNextConnection.SendSpeakingAsync(true);  // this tells the server we're speaking

      var psi = new ProcessStartInfo
      {
        FileName = "ffmpeg",
        Arguments = $@"-i ""{path}"" -ac 2 -f s16le -ar 48000 pipe:1",
        RedirectStandardOutput = true,
        UseShellExecute = false,
      };
      var ffmpeg = Process.Start(psi);
      var ffout = ffmpeg.StandardOutput.BaseStream;

      var stream = voiceNextConnection.GetTransmitStream(20);
      await ffout.CopyToAsync(stream);
      await stream.FlushAsync();
      
      await voiceNextConnection.WaitForPlaybackFinishAsync();

      var tuple = (s_MostRecentSound, soundName);
      s_MostRecentSound = soundName;

      if (s_AutoSoundTriggers.ContainsKey(tuple))
      {
        await Task.Delay(s_AutoSoundDelay);
        var message = $"!play {s_AutoSoundTriggers[tuple]}";
        await ctx.Channel.SendMessageAsync(message).ConfigureAwait(false);
      }
    }

    [Command("stop")]
    [Description("Asks me to stop the sound I'm playing.")]
    public async Task Stop(CommandContext ctx)
    {
      var guild = ctx.Guild;
      if (guild == null)
        throw new InvalidOperationException("No guild, no voice channels. Was this a DM...?");

      var voiceNext = ctx.Client.GetVoiceNext();

      var voiceNextConnection = voiceNext.GetConnection(guild);
      if (voiceNextConnection == null)
        throw new InvalidOperationException("R2-D20 is not connected to a voice channel in this server.");

      if (voiceNextConnection.IsPlaying)
      {
        voiceNextConnection.Pause();
        await voiceNextConnection.SendSpeakingAsync(false);
      }
    }

    [Command("emote")]
    [Description("Asks me to express an emotion. Use !emotelist to see what I can feel.")]
    public async Task Emote(CommandContext ctx,
      [Description("The name of the emotion for me to express.")]
      [RemainingText] string emotion)
    {
      emotion = emotion.ToLower().Trim();
      if (s_Emotions.ContainsKey(emotion))
      {
        var soundName = PickRandom(s_Emotions[emotion]);
        await Play(ctx, soundName);
      }
    }

    [Command("soundlist")]
    [Description("Asks me to recite an alphabetical list of the sounds I can play.")]
    public async Task SoundList(CommandContext ctx,
      [Description("If specified, I'll just show you the sounds that start with this.")]
      [RemainingText] string prefix)
    {
      var prefixSpecified = !string.IsNullOrEmpty(prefix);

      var pathsArray = Directory.GetFiles("audio");
      var names = new List<string>();
      foreach (var path in pathsArray)
        names.Add(Path.GetFileNameWithoutExtension(path));

      if (prefixSpecified)
      {
        var prefixUpper = prefix.ToUpper();
        names = names.Where(path => path.ToUpper().StartsWith(prefixUpper)).ToList();
      }

      var embed = new DiscordEmbedBuilder();

      var defaultList = new List<string>();
      var abcList     = new List<string>();
      var defList     = new List<string>();
      var ghiList     = new List<string>();
      var jklList     = new List<string>();
      var mnoList     = new List<string>();
      var pqrsList    = new List<string>();
      var tuvList     = new List<string>();
      var wxyzList    = new List<string>();

      if (prefixSpecified)
      {
        embed.Title = "[ Here are the sounds that I can play starting with ]";
        var fieldText = string.Empty;
        foreach (var name in names)
        {
          if (name.StartsWith("secret.") || name.StartsWith("music.") || name.StartsWith("R2."))
            continue;
          fieldText += $"{Formatter.InlineCode(name)} ";
        }
        embed.AddField(prefix, fieldText);
      }
      else
      {
        embed.Title = "[ Here are the sounds that I can play: ]";

        foreach (var name in names)
        {
          if (name.StartsWith("secret.") || name.StartsWith("music.") || name.StartsWith("R2."))
            continue;
          var first = name.ToUpper().First();
          switch (first)
          {
            default:
              defaultList.Add(name);
              break;
            case 'A':
            case 'B':
            case 'C':
              abcList.Add(name);
              break;
            case 'D':
            case 'E':
            case 'F':
              defList.Add(name);
              break;
            case 'G':
            case 'H':
            case 'I':
              ghiList.Add(name);
              break;
            case 'J':
            case 'K':
            case 'L':
              jklList.Add(name);
              break;
            case 'M':
            case 'N':
            case 'O':
              mnoList.Add(name);
              break;
            case 'P':
            case 'Q':
            case 'R':
            case 'S':
              pqrsList.Add(name);
              break;
            case 'T':
            case 'U':
            case 'V':
              tuvList.Add(name);
              break;
            case 'W':
            case 'X':
            case 'Y':
            case 'Z':
              wxyzList.Add(name);
              break;
          }
        }

        AddSoundListEmbedField(embed, defaultList, "Digits / Symbols");
        AddSoundListEmbedField(embed, abcList,  "ABC");
        AddSoundListEmbedField(embed, defList,  "DEF");
        AddSoundListEmbedField(embed, ghiList,  "GHI");
        AddSoundListEmbedField(embed, jklList,  "JKL");
        AddSoundListEmbedField(embed, mnoList,  "MNO");
        AddSoundListEmbedField(embed, pqrsList, "PQRS");
        AddSoundListEmbedField(embed, tuvList,  "TUV");
        AddSoundListEmbedField(embed, wxyzList, "WXYZ");
      }

      await ctx.RespondAsync(embed: embed);
    }

    [Command("musiclist")]
    [Description("Asks me to recite an alphabetical list of the music I can play.")]
    public async Task MusicList(CommandContext ctx)
    {
      var names = Directory.GetFiles("audio");
      string _description = "";
      foreach (var name in names)
      {
        if(!Path.GetFileNameWithoutExtension(name).StartsWith("music."))
        {
          continue;
        }
        _description += Formatter.InlineCode(Path.GetFileNameWithoutExtension(name));
        if(name != names.Last())
        _description += ", ";
      }
      
      if(_description.EndsWith(", "))
      {
        _description = _description.Remove(_description.Length - 2);
      }

      var _embed = new DiscordEmbedBuilder
      {
        Title = "[ Here is the music that I can play: ]",
        Description = _description
      };
      await ctx.RespondAsync(embed: _embed);
    }

    [Command("emotelist")]
    [Description("Asks me to recite a list of the emotions I am capable of expressing.")]
    public async Task EmoteList(CommandContext ctx)
    {
      var emotions = s_Emotions.Keys;
      var message = "[ Here are the emotions I can convey via sound: ]";
      var _description = "";

      foreach (var emotion in emotions)
      {
        _description += Formatter.InlineCode(emotion.ToString());
        if(emotion != emotions.Last())
        {
          _description += ", ";
        }
      }

      var _embed = new DiscordEmbedBuilder
      {
        Title = message,
        Description = _description
      };
      await ctx.RespondAsync(embed: _embed);
    }

    //[Command("fcregister")]
    public async Task RegisterFriendCode(CommandContext ctx)
    {
      string path = $"data{Path.DirectorySeparatorChar}friendcodes.txt";
      using (StreamWriter sw = File.CreateText(path))
      {
        using (StreamReader sr = File.OpenText(path))
        {
          if(!File.Exists(path))
          {
            sw.WriteLine("-= Switch Friend Codes =-\n");
          }

          string fileString = sr.ReadToEnd();

          /*if(fileString.Contains(ctx.Member.DisplayName))
          {
            await Reply(ctx, "MY DICKKK");
            Console.WriteLine("MY DICKKK");
          }
          else*/
          {
            string friendCode = ctx.Message.Content.Remove(0, 12);
            sw.WriteLine(ctx.Member.DisplayName + ": " + friendCode);
            await Reply(ctx, "Switch Friend Code has been registered.");
          }
        }
      }
    }
    
    [Command("fclist")]
    [Description("Asks me to list the Switch friend codes that I have registered.")]
    public async Task ListFriendCodes(CommandContext ctx)
    {
      string path = $"data{Path.DirectorySeparatorChar}friendcodes.txt";
      using (StreamReader sr = File.OpenText(path))
      {
        if(!File.Exists(path))
        {
          await Reply(ctx, "Error: No friend code data file found.");
        }
        string _title = "Bean Town Switch Friend Codes";
        string _description = "";
        string[] s;
        string s1;
        while ((s1 = sr.ReadLine()) != null)
        {
          s = s1.Split(": ");
          _description += $"{s[0]}: {Formatter.InlineCode(s[1])}\n";
        }
        var _embed = new DiscordEmbedBuilder
        {
          Title = _title,
          Description = _description
        };
        await Play(ctx, PickRandom(s_CommandSounds));
        await ctx.RespondAsync(embed: _embed);
      }
    }

    [Command("rolln")]
    [Description("Asks me to roll numeric dice.")]
    public async Task RollN(CommandContext ctx,
      [Description("Which and how many dice to roll: [how many dice]d[how many sides]")]
      string diceString)
    {
      diceString = diceString.ToLower().Trim();
      var diceNumbers = diceString.Split("d");
      var splitList = new List<string>();
      foreach (var item in diceNumbers)
        if (!string.IsNullOrEmpty(item))
          splitList.Add(item.Trim());
      if (splitList.Count == 0)
      {
        await Reply(ctx, "[bwzp bweeoo]");
      }
      else if (splitList.Count == 1 && diceString[0] != 'd')
      {
        await Reply(ctx, "[weewoo zwzwzzpp]");
      }
      else
      {
        string message;

        if (splitList.Count == 1)
          message = Roll(uint.Parse(splitList[0]));
        else
          message = Roll(uint.Parse(splitList[0]), uint.Parse(splitList[1]));

        await Reply(ctx, message);
      }
    }

    [Command("pool")]
    [Description("Asks me to create a new empty dice pool, or recite the current pool. "
      + "Use `!dicehelp` for a list of dice that I can add.")]
    public async Task Pool(CommandContext ctx,
      [Description("The dice to add. If blank, I'll just recite the current pool.")]
      params string[] args)
    {
      if (args.Length == 0)
      {
        var emojiString = s_CurrentPool.GetSortedEmojiString();
        if (string.IsNullOrEmpty(emojiString))
          await Reply(ctx, "[ The pool is currently empty. ]");
        else
          await Reply(ctx, $"[ The pool currently contains {emojiString}. ]");
        await Play(ctx, PickRandom(s_CommandSounds));
      }
      else
      {
        s_CurrentPool = new FfgDie.Pool();
        await AddHelper(ctx, args);
      }
    }

    [Command("add")]
    [Description("Asks me to add dice to the current pool. Creates one if necessary. "
      + "Use `!dicehelp` for a list of dice that I can add.")]
    public async Task Add(CommandContext ctx,
      [Description("The dice to add.")]
      params string[] args)
    {
      if (s_CurrentPool == null || s_CurrentPool.m_Counts.Count == 0)
        s_CurrentPool = new FfgDie.Pool();
      await AddHelper(ctx, args);
    }

    [Command("addsecret")]
    [Description("This feature isn't ready yet. Ask Doug if you're curious.")]
    public async Task AddSecret(CommandContext ctx, params string[] args)
    {
      await Reply(ctx, "[ This feature will be added soon. ]");
    }

    [Command("remove")]
    [Description("Asks me to remove dice from the current pool. "
      + "Use `!dicehelp` for a list of dice that I can remove.")]
    public async Task Remove(CommandContext ctx,
      [Description("The dice to remove.")]
      params string[] args)
    {
      if (s_CurrentPool == null || s_CurrentPool.m_Counts.Count == 0)
      {
        await Reply(ctx, "[ You can't remove dice from an empty pool. ]");
        await Play(ctx, PickRandom(s_ErrorSounds));
      }
      else
      {
        var removals = 0;

        foreach (var arg in args)
        {
          if (s_DiceByString.ContainsKey(arg))
          {
            s_CurrentPool.Remove(s_DiceByString[arg]);
            ++removals;
          }
          else
          {
            foreach (var ch in arg)
            {
              var str = ch.ToString();
              if (s_DiceByString.ContainsKey(str))
              {
                s_CurrentPool.Remove(s_DiceByString[str]);
                ++removals;
              }
            }
          }
        }

        if (removals == 0)
        {
          await Reply(ctx, "[ Please specify one or more dice to remove. ]");
          await Play(ctx, PickRandom(s_ErrorSounds));
        }
        else
        {
          var emojiString = s_CurrentPool.GetSortedEmojiString();

          if (string.IsNullOrEmpty(emojiString))
            await Reply(ctx, "[ The pool is now empty. ]");
          else
            await Reply(ctx, $"[ The pool now contains {emojiString}. ]");
          await Play(ctx, PickRandom(s_CommandSounds));
        }
      }
    }

    [Command("dicehelp")]
    [Description("Asks me to recite a list of dice that can be used with various commands.")]
    public async Task DiceHelp(CommandContext ctx)
    {
      var ae = FfgDie.s_DiceEmoji[FfgDie.Type.Ability];
      var pe = FfgDie.s_DiceEmoji[FfgDie.Type.Proficiency];
      var be = FfgDie.s_DiceEmoji[FfgDie.Type.Boost];
      var de = FfgDie.s_DiceEmoji[FfgDie.Type.Difficulty];
      var ce = FfgDie.s_DiceEmoji[FfgDie.Type.Challenge];
      var se = FfgDie.s_DiceEmoji[FfgDie.Type.Setback];
      var fe = FfgDie.s_DiceEmoji[FfgDie.Type.Force];

      var ics = Formatter.InlineCode(" "); // inline code space

      var embed = new DiscordEmbedBuilder();
      embed.Title = "[ Here are the dice that we use in the Star Wars RPGs. ]";
      embed.Description = "Examples:" + Environment.NewLine
        + Formatter.InlineCode("!roll ability ability proficiency boost") + Environment.NewLine
        + Formatter.InlineCode("!pool ") + $"{ae}{ics}{ae}{ics}{pe}{ics}{pe}" + Environment.NewLine
        + Formatter.InlineCode("!add ddss");
      var aText = ae + ", " + Formatter.InlineCode("a") + ", " +
        Formatter.InlineCode("ability") + ", " + Formatter.InlineCode("abilityDie");
      var pText = pe + ", " + Formatter.InlineCode("p") + ", " +
        Formatter.InlineCode("proficiency") + ", " + Formatter.InlineCode("proficiencyDie");
      var bText = be + ", " + Formatter.InlineCode("b") + ", " +
        Formatter.InlineCode("boost") + ", " + Formatter.InlineCode("boostDie");
      var dText = de + ", " + Formatter.InlineCode("d") + ", " +
        Formatter.InlineCode("difficulty") + ", " + Formatter.InlineCode("difficultyDie");
      var cText = ce + ", " + Formatter.InlineCode("c") + ", " +
        Formatter.InlineCode("challenge") + ", " + Formatter.InlineCode("challengeDie");
      var sText = se + ", " + Formatter.InlineCode("s") + ", " +
        Formatter.InlineCode("setback") + ", " + Formatter.InlineCode("setbackDie");
      var fText = fe + ", " + Formatter.InlineCode("f") + ", " +
        Formatter.InlineCode("force") + ", " + Formatter.InlineCode("forceDie");
      embed.AddField("Ability Die", aText);
      embed.AddField("Proficiency Die", pText);
      embed.AddField("Boost Die", bText);
      embed.AddField("Difficulty Die", dText);
      embed.AddField("Challenge Die", cText);
      embed.AddField("Setback Die", sText);
      embed.AddField("Force Die", fText);


      await ctx.RespondAsync(embed: embed);
    }
    
    [Command("symbolhelp")]
    [Description("Asks me to recite a list of the symbols you'll see on the dice.")]
    public async Task SymbolHelp(CommandContext ctx)
    {
      var se = FfgDie.s_SymbolEmoji[FfgDie.Symbol.Success];
      var fe = FfgDie.s_SymbolEmoji[FfgDie.Symbol.Failure];
      var ae = FfgDie.s_SymbolEmoji[FfgDie.Symbol.Advantage];
      var he = FfgDie.s_SymbolEmoji[FfgDie.Symbol.Threat];
      var te = FfgDie.s_SymbolEmoji[FfgDie.Symbol.Triumph];
      var de = FfgDie.s_SymbolEmoji[FfgDie.Symbol.Despair];
      var le = FfgDie.s_SymbolEmoji[FfgDie.Symbol.Light];
      var ke = FfgDie.s_SymbolEmoji[FfgDie.Symbol.Dark];

      var embed = new DiscordEmbedBuilder();
      embed.Title = "[ Here are the symbols you'll see on the Star Wars dice. ]";
      embed.AddField($"Success {se}",
        "You need at least one of these for a skill to work. Cancels with "
        + Formatter.Bold($"Failure {fe}") + ".");
      embed.AddField($"Failure {fe}",
        "You want to avoid seeing this. Cancels with "
        + Formatter.Bold($"Success {se}") + ".");
      embed.AddField($"Advantage {ae}",
        "Small situational benefit. Cancels with "
        + Formatter.Bold($"Threat {he}") + ".");
      embed.AddField($"Threat {he}",
        "Small situational detriment. Cancels with "
        + Formatter.Bold($"Advantage {ae}") + ".");
      embed.AddField($"Triumph {te}",
        $"Large situational benefit. Also adds {se}. "
        + Formatter.Bold($"Does not cancel with Despair {de}."));
      embed.AddField($"Despair {de}",
        $"Large situational detriment. Also adds {fe}. "
        + Formatter.Bold($"Does not cancel with Triumph {te}."));
      embed.AddField($"Light {le}",
        "Represents the Light Side of the Force.");
      embed.AddField($"Dark {ke}",
        "Represents the Dark Side of the Force.");

      await ctx.RespondAsync(embed: embed);
    }

    [Command("clear")]
    [Description("Asks me to clear the current dice pool.")]
    public async Task Clear(CommandContext ctx)
    {
      if (s_CurrentPool == null || s_CurrentPool.m_Counts.Count == 0)
      {
        await Reply(ctx, "[ The pool was already empty. ]");
        await Play(ctx, PickRandom(s_ErrorSounds));
      }
      else
      {
        s_CurrentPool = new FfgDie.Pool();
        await Reply(ctx, "[ The pool is now empty. ]");
        await Play(ctx, PickRandom(s_CommandSounds));
      }
    }

    [Command("roll")]
    [Description("Asks me to roll the dice. "
      + "Use `!dicehelp` for a list of dice that I can roll.")]
    public async Task Roll(CommandContext ctx,
      [Description("The dice for me to roll. If blank, I'll roll the current pool.")]
      params string[] args)
    {
      await RollHelper(ctx, FfgDie.RollType.Normal, args);
    }

    [Command("rollbest")]
    [Description("Asks me to show the best possible roll. "
      + "Use `!dicehelp` for a list of dice that I can roll.")]
    public async Task RollBest(CommandContext ctx,
      [Description("The dice for me to roll. If blank, I'll roll the current pool.")]
      params string[] args)
    {
      await RollHelper(ctx, FfgDie.RollType.Best, args);
    }

    [Command("rollworst")]
    [Description("Asks me to show the worst possible roll. "
      + "Use `!dicehelp` for a list of dice that I can roll.")]
    public async Task RollWorst(CommandContext ctx,
      [Description("The dice for me to roll. If blank, I'll roll the current pool.")]
      params string[] args)
    {
      await RollHelper(ctx, FfgDie.RollType.Worst, args);
    }

    [Command("friendcodes")]
    [Description("...I think this command isn't ready yet. Ask Alek if you're curious.")]
    public async Task FriendCodes(CommandContext ctx)
    {
      await Task.CompletedTask;
    }

    [Command("smash")]
    [Description("Great for when you get a critical hit.")]
    public async Task Smash(CommandContext ctx)
    {
      _ = Play(ctx, "smash");
      var message = ctx.Channel.SendMessageAsync(s_SmashSM).Result;
      await message.ModifyAsync($"{s_SmashSM}{s_SmashAAA}");
      await message.ModifyAsync($"{s_SmashSM}{s_SmashAAA}{s_SmashSH}");
    }

    [Command("crawltext")]
    [Description("Use this to give me a string to show with !begincrawl.")]
    public async Task CrawlText(CommandContext ctx,
      [Description("The text for the crawl. If blank, I'll reply with the current text instead.")]
      [RemainingText] string text)
    {
      if (string.IsNullOrEmpty(text))
      {
        if (string.IsNullOrEmpty(s_CrawlText))
          await Reply(ctx, "[ The crawl text is currently empty. ]");
        else
          await Reply(ctx, "[ Here is the current crawl text: ]" + Environment.NewLine
            + s_CrawlText);
      }
      else
      {
        s_CrawlText = text;
        await Reply(ctx, "[ Okay, here is the new crawl text: ]" + Environment.NewLine
          + s_CrawlText);
      }
    }

    [Command("crawldelay")]
    [Description("Use this to set the delay between lines of the text crawl.")]
    public async Task CrawlSpeed(CommandContext ctx,
      [Description("The delay in milliseconds. If blank, I'll replay with the current delay instead.")]
      [RemainingText] string delayString)
    {
      if (string.IsNullOrEmpty(delayString))
      {
        await Reply(ctx, "[ The next text crawl will use a delay of " +
          Formatter.Bold(s_CrawlLineDelay.ToString() + " ms") + " between lines. ]");
      }
      else
      {
        s_CrawlLineDelay = int.Parse(delayString);
        await Reply(ctx, "[ Okay, the new text crawl delay is " +
          Formatter.Bold(s_CrawlLineDelay.ToString() + " ms") + ". ]");
      }
    }

    [Command("crawlmusic")]
    [Description("Use this to set which music should play during the text crawl.")]
    public async Task CrawlMusic(CommandContext ctx,
      [Description("The music to play. If blank, I'll reply with the current name instead.")]
      [RemainingText] string name)
    {
      if (string.IsNullOrEmpty(name))
      {
        await Reply(ctx, "[ The current crawl music is " +
          Formatter.Bold(s_CrawlMusic) + ". ]");
      }
      else
      {
        s_CrawlMusic = name;
        await Reply(ctx, "[ Okay, the new text crawl music is " +
          Formatter.Bold(s_CrawlMusic) + ". ]");
      }
    }

    [Command("begincrawl")]
    [Description("Asks me to begin the text crawl with the current crawl text.")]
    public async Task BeginCrawl(CommandContext ctx)
    {
      if (string.IsNullOrEmpty(s_CrawlText))
        return;

      var topMiddleIndex = s_MaxCrawlLines / 2 - 1;
      var bottomMiddleIndex = topMiddleIndex + 1;

      s_CrawlLines = new List<string>(s_MaxCrawlLines);
      for (var i = 0; i < s_MaxCrawlLines; ++i)
        s_CrawlLines.Add(string.Empty);

      var page = GetCurrentCrawlPage();
      var message = ctx.Channel.SendMessageAsync(page).Result;
      await Task.Delay(s_CrawlDelay0);

      s_CrawlLines[topMiddleIndex] = s_CrawlIntro0;
      s_CrawlLines[bottomMiddleIndex] = s_CrawlIntro1;
      page = GetCurrentCrawlPage();
      await message.ModifyAsync(page);
      await Task.Delay(s_CrawlDelay1);

      page = ClearCrawlPage();
      await message.ModifyAsync(page);
      await Task.Delay(s_CrawlDelay2);

      s_CrawlLines[topMiddleIndex] = s_CrawlGameTitle0;
      s_CrawlLines[bottomMiddleIndex] = s_CrawlGameTitle1;
      page = GetCurrentCrawlPage();
      _ = Play(ctx, s_CrawlMusic);
      await message.ModifyAsync(page);
      await Task.Delay(s_CrawlDelay3);

      page = ClearCrawlPage();
      await message.ModifyAsync(page);
      await Task.Delay(s_CrawlDelay4);

      var allLines = s_CrawlText.Split("\n");

      for (var i = 1 - s_MaxCrawlLines; i < allLines.Length; ++i)
      {
        for (var j = 0; j < s_MaxCrawlLines; ++j)
        {
          var index = i + j;
          var lineText = index >= 0 && index < allLines.Length ? allLines[index] : string.Empty;
          s_CrawlLines[j] = lineText;
        }
        page = GetCurrentCrawlPage();
        await message.ModifyAsync(page);
        await Task.Delay(s_CrawlLineDelay);
      }

      page = ClearCrawlPage();
      await message.ModifyAsync(page);
      await Task.Delay(s_CrawlDelay5);

      await message.DeleteAsync();
    }

    private string GetCurrentCrawlPage()
    {
      var output = FfgDie.s_SymbolEmoji[FfgDie.Symbol.Triumph] + Environment.NewLine;
      foreach (var line in s_CrawlLines)
      {
        if (string.IsNullOrEmpty(line) || string.IsNullOrWhiteSpace(line))
          output += "\u2022\u2022\u2022    " + Environment.NewLine;
        else
          output += "\u2022\u2022\u2022    " + Formatter.Bold(line) + Environment.NewLine;
      }
      output += FfgDie.s_SymbolEmoji[FfgDie.Symbol.Despair];

      return output;
    }

    private string ClearCrawlPage()
    {
      var output = FfgDie.s_SymbolEmoji[FfgDie.Symbol.Triumph] + Environment.NewLine;
      for (var i = 0; i < s_CrawlLines.Count; ++i)
      {
        s_CrawlLines[i] = string.Empty;
        output += "\u2022\u2022\u2022    " + Environment.NewLine;
      }
      output += FfgDie.s_SymbolEmoji[FfgDie.Symbol.Despair];

      return output;
    }

    private async Task RollHelper(CommandContext ctx, FfgDie.RollType rollType, string[] args)
    {
      FfgDie.Pool pool;

      if (args.Length == 0)
      {
        pool = s_CurrentPool;
      }
      else
      {
        pool = new FfgDie.Pool();
        foreach (var arg in args)
        {
          if (s_DiceByString.ContainsKey(arg))
          {
            pool.Add(s_DiceByString[arg]);
          }
          else
          {
            foreach (var ch in arg)
            {
              var str = ch.ToString();
              if (s_DiceByString.ContainsKey(str))
              {
                pool.Add(s_DiceByString[str]);
              }
            }
          }
        }
      }

      if (pool.m_Counts.Count == 0)
      {
        if (args.Length == 0)
          await Reply(ctx, "[ You can't roll an empty pool. ]");
        else
          await Reply(ctx, "[ I didn't understand what you tried to roll. ]");
        await Play(ctx, PickRandom(s_ErrorSounds));
      }
      else
      {
        FfgDie.Pool.RollResult result;

        switch (rollType)
        {
          default: // normal
            result = pool.Roll();
            break;
          case FfgDie.RollType.Best:
            result = pool.RollBest();
            break;
          case FfgDie.RollType.Worst:
            result = pool.RollWorst();
            break;
        }

        var outcomeSymbols = result.m_Result.Keys.ToList();
        outcomeSymbols.Sort();

        var resultString = string.Empty;

        foreach (var outcomeSymbol in outcomeSymbols)
        {
          var count = result.m_Result[outcomeSymbol];
          for (var i = 0; i < count; ++i)
          {
            var symbolEmoji = FfgDie.s_SymbolEmoji[outcomeSymbol];
            resultString += symbolEmoji;
          }
        }

        string outcomeString;

        if (string.IsNullOrEmpty(resultString))
        {
          resultString = FfgDie.s_SymbolEmoji[FfgDie.Symbol.None];
          outcomeString = "It's a **Wash**";
        }
        else
        {
          var successString = string.Empty;
          var advantageString = string.Empty;
          var forceString = string.Empty;
          var triumphString = string.Empty;
          var despairString = string.Empty;

          if (result.m_SuccessStatus == FfgDie.Pool.RollResult.SuccessStatus.Success)
            successString = "**Success**";
          else if (result.m_SuccessStatus == FfgDie.Pool.RollResult.SuccessStatus.Failure)
            successString = "**Failure**";

          if (result.m_AdvantageStatus == FfgDie.Pool.RollResult.AdvantageStatus.Advantage)
            advantageString = "**Advantage**";
          else if (result.m_AdvantageStatus == FfgDie.Pool.RollResult.AdvantageStatus.Threat)
            advantageString = "**Threat**";

          if (result.m_ForceStatus == FfgDie.Pool.RollResult.ForceStatus.Light ||
            result.m_ForceStatus == FfgDie.Pool.RollResult.ForceStatus.Dark)
            forceString = "**Disturbance in the Force**";
          else if (result.m_ForceStatus == FfgDie.Pool.RollResult.ForceStatus.DoubleLight ||
            result.m_ForceStatus == FfgDie.Pool.RollResult.ForceStatus.DoubleDark)
            forceString = "**Great Disturbance in the Force**";

          if (result.m_Triumph)
            triumphString = "**Triumph**";
          if (result.m_Despair)
            despairString = "**Despair**";

          outcomeString = successString;

          var noOutcome = string.IsNullOrEmpty(outcomeString);
          var noAdvantage = string.IsNullOrEmpty(advantageString);
          var noTriumph = string.IsNullOrEmpty(triumphString);
          var noDespair = string.IsNullOrEmpty(despairString);
          var noForce = string.IsNullOrEmpty(forceString);

          if (!noOutcome && !(noAdvantage && noTriumph && noDespair))
            outcomeString += " with ";

          outcomeString += advantageString;

          if (!noAdvantage && !(noTriumph && noDespair))
            outcomeString += " and ";

          outcomeString += triumphString;

          if (!noTriumph && !noDespair)
            outcomeString += " and ";

          outcomeString += despairString;

          /*if (!noForce)
            if (string.IsNullOrEmpty(outcomeString))
              outcomeString = $"A {forceString}";
            else
              outcomeString += $"... and a {forceString}";*/
        }

        string rollTypeMessage;

        switch (rollType)
        {
          default:
            rollTypeMessage = string.Empty;
            break;
          case FfgDie.RollType.Best:
            rollTypeMessage = "The **best possible roll** for this pool is ";
            break;
          case FfgDie.RollType.Worst:
            rollTypeMessage = "The **worst possible roll** for this pool is ";
            break;
        }

        var line0 = $"[ {rollTypeMessage}{resultString} \u2014 {outcomeString} ]";
        var line1 = string.Empty;

        foreach (var dieResult in result.m_DieList)
          line1 += dieResult;

        var message = $"{line0}{Environment.NewLine}{line1}";

        string soundName;

        if (result.m_Triumph && result.m_Despair)
        {
          soundName = PickRandom(s_ChaosSounds);
        }
        else if (result.m_Triumph)
        {
          if (result.m_SuccessStatus == FfgDie.Pool.RollResult.SuccessStatus.Success ||
            result.m_SuccessStatus == FfgDie.Pool.RollResult.SuccessStatus.Nothing)
          {
            soundName = PickRandom(s_VeryHappySounds);
          }
          else
          {
            soundName = PickRandom(s_UnsureSounds);
          }
        }
        else if (result.m_Despair)
        {
          if (result.m_SuccessStatus == FfgDie.Pool.RollResult.SuccessStatus.Failure ||
            result.m_SuccessStatus == FfgDie.Pool.RollResult.SuccessStatus.Nothing)
          {
            soundName = PickRandom(s_VeryUpsetSounds);
          }
          else
          {
            soundName = PickRandom(s_UnsureSounds);
          }
        }
        else if (result.m_SuccessStatus == FfgDie.Pool.RollResult.SuccessStatus.Success)
          soundName = PickRandom(s_HappySounds);
        else if (result.m_SuccessStatus == FfgDie.Pool.RollResult.SuccessStatus.Failure)
          soundName = PickRandom(s_UpsetSounds);
        else if (result.m_AdvantageStatus == FfgDie.Pool.RollResult.AdvantageStatus.Advantage)
          soundName = PickRandom(s_KindaHappySounds);
        else if (result.m_AdvantageStatus == FfgDie.Pool.RollResult.AdvantageStatus.Threat)
          soundName = PickRandom(s_KindaUpsetSounds);
        else
          //soundName = "bup";
          soundName = PickRandom(s_UnsureSounds);

        await Reply(ctx, message);
        await Play(ctx, soundName);

        /*if (result.m_ForceStatus == FfgDie.Pool.RollResult.ForceStatus.Light)
          await Play(ctx, "light");
        else if (result.m_ForceStatus == FfgDie.Pool.RollResult.ForceStatus.DoubleLight)
          await Play(ctx, "doublelight");
        else if (result.m_ForceStatus == FfgDie.Pool.RollResult.ForceStatus.Dark)
          await Play(ctx, "dark");
        else if (result.m_ForceStatus == FfgDie.Pool.RollResult.ForceStatus.DoubleDark)
          await Play(ctx, "doubledark");*/

        s_CurrentPool = new FfgDie.Pool();
      }
    }

    private async Task AddHelper(CommandContext ctx, string[] args)
    {
      foreach (var arg in args)
      {
        if (s_DiceByString.ContainsKey(arg))
        {
          s_CurrentPool.Add(s_DiceByString[arg]);
        }
        else
        {
          foreach (var ch in arg)
          {
            var str = ch.ToString();
            if (s_DiceByString.ContainsKey(str))
            {
              s_CurrentPool.Add(s_DiceByString[str]);
            }
          }
        }
      }

      var emojiString = s_CurrentPool.GetSortedEmojiString();

      if (string.IsNullOrEmpty(emojiString))
      {
        await Reply(ctx, "[ Please specify one or more dice to add. ]");
        await Play(ctx, PickRandom(s_ErrorSounds));
      }
      else
      {
        await Reply(ctx, $"[ The pool now contains {emojiString}. ]");
        await Play(ctx, PickRandom(s_CommandSounds));
      }
    }



    private string Roll(uint die)
    {
      if (die == 0)
        return "[squuppwuppuppggghl]";

      if (die == 1)
        return "[wzweewzp **1** bzp]";

      var roll = Bot.s_RNG.Next(1, (int)die + 1);
      return $"[beep boop **{roll}**]";
    }

    private string Roll(uint count, uint die)
    {
      if (die == 0)
        return "[whewwwwww wooooooopl]";

      if (count == 0)
        return "[bllzwp whoooop **0** pllbbzt]";

      if (die == 1)
        return $"[wzweewzp **{count}** bzzp]";

      var total = 0u;
      var rollString = string.Empty;
      for (var i = 0u; i < count; ++i)
      {
        var roll = Bot.s_RNG.Next(1, (int)die + 1);
        total += (uint)roll;
        rollString += (i == 0 ? string.Empty : ",") + " " + roll;
      }

      return $"[beep boop **{total}** :{rollString}]";
    }

    private bool RoleCheck(CommandContext ctx, string roleName)
    {
      var guilds = ctx.Client.Guilds.Values.ToList();
      var authorId = ctx.Message.Author.Id;
      var roleFound = false;
      foreach (var guild in guilds)
      {
        if (guild.Members.ContainsKey(authorId))
        {
          var member = guild.Members[authorId];
          var roles = member.Roles;
          foreach (var role in roles)
          {
            if (role.Name == roleName)
            {
              roleFound = true;
              break;
            }
          }
        }
      }

      return roleFound;
    }

    private void AddSoundListEmbedField(DiscordEmbedBuilder embed, List<string> list, string name)
    {
      if (list.Count <= 0)
        return;

      var fieldText = string.Empty;
      foreach (var item in list)
        fieldText += $"{Formatter.InlineCode(item)} ";
      embed.AddField(name, fieldText);
    }

    private async Task Say(CommandContext ctx, string message)
    {
      await ctx.Message.RespondAsync(message).ConfigureAwait(false);
    }

    private async Task Reply(CommandContext ctx, string message)
    {
      var member = ctx.Member;
      if (member != null)
        message = member.Mention + " \u2014 " + message;
      
      await ctx.Message.RespondAsync(message).ConfigureAwait(false);
    }

    private T PickRandom<T>(List<T> list)
    {
      return list[Bot.s_RNG.Next(0, list.Count)];
    }
  }
}

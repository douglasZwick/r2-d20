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
    };
    public static int s_AutoSoundDelay = 1000; // in ms
    public static string s_MostRecentSound;

    [Command("ping")]
    public async Task Ping(CommandContext ctx)
    {
      await Say(ctx, "[ Beep. ]");
      await Play(ctx, PickRandom(s_CommandSounds));
    }

    [Command("echo")]
    public async Task Echo(CommandContext ctx, params string[] args)
    {
      var message = "[ Meep ]\n";
      foreach (var arg in args)
      {
        message += "`" + arg + "`\n";
      }
      message += "[ zorp. ]";
      await Say(ctx, message);
      await Play(ctx, PickRandom(s_CommandSounds));
    }

    [Command("join")]
    public async Task Join(CommandContext ctx, [RemainingText] string channelName)
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
    public async Task Leave(CommandContext ctx)
    {
      var voiceNext = ctx.Client.GetVoiceNext();

      var voiceNextConnection = voiceNext.GetConnection(ctx.Guild);
      if (voiceNextConnection == null)
        throw new InvalidOperationException("R2-D20 is not connected to a voice channel in this server.");

      await Task.Run(voiceNextConnection.Disconnect);
    }

    [Command("play")]
    public async Task Play(CommandContext ctx, [RemainingText] string soundName)
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
        await Play(ctx, s_AutoSoundTriggers[tuple]);
      }
    }

    [Command("emote")]
    public async Task Emote(CommandContext ctx, [RemainingText] string emotion)
    {
      emotion = emotion.ToLower().Trim();
      if (s_Emotions.ContainsKey(emotion))
      {
        var soundName = PickRandom(s_Emotions[emotion]);
        await Play(ctx, soundName);
      }
    }

    [Command("soundlist")]
    public async Task SoundList(CommandContext ctx, [RemainingText] string prefix)
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
          fieldText += $"{Formatter.InlineCode(name)} ";
        embed.AddField(prefix, fieldText);
      }
      else
      {
        embed.Title = "[ Here are the sounds that I can play: ]";

        foreach (var name in names)
        {
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
    public async Task RollN(CommandContext ctx, string diceString)
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
    public async Task Pool(CommandContext ctx, params string[] args)
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
    public async Task Add(CommandContext ctx, params string[] args)
    {
      if (s_CurrentPool == null || s_CurrentPool.m_Counts.Count == 0)
        s_CurrentPool = new FfgDie.Pool();
      await AddHelper(ctx, args);
    }

    [Command("addsecret")]
    public async Task AddSecret(CommandContext ctx, params string[] args)
    {
      await Reply(ctx, "[ This feature will be added soon. ]");
    }

    [Command("remove")]
    public async Task Remove(CommandContext ctx, params string[] args)
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
    public async Task DiceHelp(CommandContext ctx)
    {
      string diceHelpString =   "Ability: " + FfgDie.s_DiceEmoji[FfgDie.Type.Ability] +
                                "\nProficiency: " + FfgDie.s_DiceEmoji[FfgDie.Type.Proficiency] +
                                "\nBoost: " + FfgDie.s_DiceEmoji[FfgDie.Type.Boost] +
                                "\nDifficulty: " + FfgDie.s_DiceEmoji[FfgDie.Type.Difficulty] +
                                "\nChallenge: " + FfgDie.s_DiceEmoji[FfgDie.Type.Challenge] +
                                "\nSetback: " + FfgDie.s_DiceEmoji[FfgDie.Type.Setback] +
                                "\nForce: " + FfgDie.s_DiceEmoji[FfgDie.Type.Force];
      await Say(ctx, diceHelpString);
    }
    
    [Command("symbolhelp")]
    public async Task SymbolHelp(CommandContext ctx)
    {
      string diceHelpString =   "Success: " + FfgDie.s_SymbolEmoji[FfgDie.Symbol.Success] +
                                "\nFailure: " + FfgDie.s_SymbolEmoji[FfgDie.Symbol.Failure] +
                                "\nAdvantage: " + FfgDie.s_SymbolEmoji[FfgDie.Symbol.Advantage] +
                                "\nThreat: " + FfgDie.s_SymbolEmoji[FfgDie.Symbol.Threat] +
                                "\nTriumph: " + FfgDie.s_SymbolEmoji[FfgDie.Symbol.Triumph] +
                                "\nDespair: " + FfgDie.s_SymbolEmoji[FfgDie.Symbol.Despair];
      await Say(ctx, diceHelpString);
    }

    [Command("clear")]
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
    public async Task Roll(CommandContext ctx, params string[] args)
    {
      await RollHelper(ctx, FfgDie.RollType.Normal, args);
    }

    [Command("rollbest")]
    public async Task RollBest(CommandContext ctx, params string[] args)
    {
      await RollHelper(ctx, FfgDie.RollType.Best, args);
    }

    [Command("rollworst")]
    public async Task RollWorst(CommandContext ctx, params string[] args)
    {
      await RollHelper(ctx, FfgDie.RollType.Worst, args);
    }

    [Command("friendcodes")]
    public async Task FriendCodes(CommandContext ctx)
    {
      await Task.CompletedTask;
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

          if (!noForce)
            if (string.IsNullOrEmpty(outcomeString))
              outcomeString = $"A {forceString}";
            else
              outcomeString += $"... and a {forceString}";
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
          soundName = "bup";
          //soundName = PickRandom(s_UnsureSounds);

        await Reply(ctx, message);
        await Play(ctx, soundName);

        if (result.m_ForceStatus == FfgDie.Pool.RollResult.ForceStatus.Light)
          await Play(ctx, "light");
        else if (result.m_ForceStatus == FfgDie.Pool.RollResult.ForceStatus.DoubleLight)
          await Play(ctx, "doublelight");
        else if (result.m_ForceStatus == FfgDie.Pool.RollResult.ForceStatus.Dark)
          await Play(ctx, "dark");
        else if (result.m_ForceStatus == FfgDie.Pool.RollResult.ForceStatus.DoubleDark)
          await Play(ctx, "doubledark");

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

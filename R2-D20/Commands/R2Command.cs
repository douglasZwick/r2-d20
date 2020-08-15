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
      "excited",
      "laughing",
      "unbelievable",
    };

    public static List<string> s_HappySounds = new List<string>()
    {
      "cheerful",
      "eureka",
      "veryexcited",
    };

    public static List<string> s_KindaHappySounds = new List<string>()
    {
      "playful",
      "proud",
      "squeaky"
    };

    public static List<string> s_VeryUpsetSounds = new List<string>()
    {
      "snappy",
      "surprised",
      "r2hit",
      "badmotivator",
    };

    public static List<string> s_UpsetSounds = new List<string>()
    {
      "processing",
      "sad",
      "danger",
    };

    public static List<string> s_KindaUpsetSounds = new List<string>()
    {
      "concerned",
      "look",
      "warning",
    };

    public static List<string> s_UnsureSounds = new List<string>()
    {
      "unsure",
      "determined",
    };

    public static List<string> s_ChaosSounds = new List<string>()
    {
      "shocked",
    };

    public static List<string> s_CommandSounds = new List<string>()
    {
      "cheerful",
      "look",
      "playful",
      "meepwalla",
    };

    public static List<string> s_ErrorSounds = new List<string>()
    {
      "snappy",
      "unsure",
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
    public async Task SoundList(CommandContext ctx)
    {
      var names = Directory.GetFiles("audio");
      var message = "[ Here are the sounds that I can play: ]";

      foreach (var name in names)
        message += $"{Environment.NewLine}• {Path.GetFileNameWithoutExtension(name)}";
 
      await Reply(ctx, message);
      await Play(ctx, PickRandom(s_CommandSounds));
    }

    [Command("emotelist")]
    public async Task EmoteList(CommandContext ctx)
    {
      var emotions = s_Emotions.Keys;
      var message = "[ Here are the emotions I can convey via sound: ]";

      foreach (var emotion in emotions)
        message += $"{Environment.NewLine}• {emotion}";

      await Reply(ctx, message);
      await Play(ctx, PickRandom(s_CommandSounds));
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

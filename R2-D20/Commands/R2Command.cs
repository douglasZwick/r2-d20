using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.EventArgs;
using System.Collections.Generic;
using System.Linq;
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

    public static FfgDie.Pool s_CurrentPool = new FfgDie.Pool();

    [Command("ping")]
    public async Task Ping(CommandContext ctx)
    {
      await Say(ctx, "[beep]");
    }

    [Command("echo")]
    public async Task Echo(CommandContext ctx, params string[] args)
    {
      var message = "[zeep]\n";
      foreach (var arg in args)
      {
        message += "`" + arg + "`\n";
      }
      message += "[burp]";
      await Say(ctx, message);
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
      string message;

      if (args.Length == 0)
      {
        var emojiString = s_CurrentPool.GetSortedEmojiString();
        message = string.IsNullOrEmpty(emojiString) ? "[mlep]" : "[wheeoow " + emojiString + "]";
      }
      else
      {
        s_CurrentPool = new FfgDie.Pool();
        var emojiString = AddHelper(args);
        message = string.IsNullOrEmpty(emojiString) ? "[zurp]" : "[bweep " + emojiString + "]";
      }

      await Reply(ctx, message);
    }

    [Command("add")]
    public async Task Add(CommandContext ctx, params string[] args)
    {
      if (s_CurrentPool == null || s_CurrentPool.m_Counts.Count == 0)
        s_CurrentPool = new FfgDie.Pool();
      var emojiString = AddHelper(args);
      var message = string.IsNullOrEmpty(emojiString) ? "[zurp]" : "[zeeb " + emojiString + "]";

      await Reply(ctx, message);
    }

    [Command("remove")]
    public async Task Remove(CommandContext ctx, params string[] args)
    {
      string message;

      if (s_CurrentPool == null || s_CurrentPool.m_Counts.Count == 0)
      {
        message = "[gzzgwoooo]";
      }
      else
      {
        foreach (var arg in args)
          if (s_DiceByString.ContainsKey(arg))
            s_CurrentPool.Remove(s_DiceByString[arg]);

        var emojiString = s_CurrentPool.GetSortedEmojiString();
        message = string.IsNullOrEmpty(emojiString) ? "[plibt]" : "[gzweeg " + emojiString + "]";
      }

      await Reply(ctx, message);
    }

    [Command("clear")]
    public async Task Clear(CommandContext ctx)
    {
      s_CurrentPool = new FfgDie.Pool();
      await Reply(ctx, "[weeboo weeboo wzzp]");
    }

    [Command("roll")]
    public async Task Roll(CommandContext ctx, params string[] args)
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
          if (s_DiceByString.ContainsKey(arg))
            pool.Add(s_DiceByString[arg]);
      }

      if (pool.m_Counts.Count == 0)
      {
        await Reply(ctx, "[bzzzp woop]");
      }
      else
      {
        var result = pool.Roll();
        var keysList = result.m_Result.Keys.ToList();
        keysList.Sort();

        string message = "[ ";
        var resultMessage = string.Empty;

        foreach (var key in keysList)
        {
          var count = result.m_Result[key];
          for (var i = 0; i < count; ++i)
          {
            var symbolEmoji = FfgDie.s_SymbolEmoji[key];
            resultMessage += symbolEmoji;
          }
        }

        if (resultMessage == string.Empty)
          resultMessage = FfgDie.s_SymbolEmoji[FfgDie.Symbol.None];

        message += resultMessage + " **|** ";

        message += pool.GetSortedEmojiString() + "**:** ";

        foreach (var symbol in result.m_SymbolList)
        {
          var symbolEmoji = FfgDie.s_SymbolEmoji[symbol];
          message += symbolEmoji;
        }

        message += " ]";

        await Reply(ctx, message);
      }
    }

    private string AddHelper(string[] args)
    {
      foreach (var arg in args)
        if (s_DiceByString.ContainsKey(arg))
          s_CurrentPool.Add(s_DiceByString[arg]);

      return s_CurrentPool.GetSortedEmojiString();
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

    private async Task Say(CommandContext ctx, string message)
    {
      await ctx.Message.RespondAsync(message).ConfigureAwait(false);
    }

    private async Task Reply(CommandContext ctx, string message)
    {
      var mention = ctx.Member.Mention;
      message = mention + " \u2014 " + message;
      await ctx.Message.RespondAsync(message).ConfigureAwait(false);
    }
  }
}

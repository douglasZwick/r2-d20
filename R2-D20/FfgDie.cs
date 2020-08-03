using DSharpPlus.Entities;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;

namespace R2D20
{
  public class Die { }

  public class FfgDie : Die
  {
    public static FfgDie s_BoostDie = new FfgDie(new List<List<Symbol>>()
    {
      new List<Symbol>() { Symbol.None },
      new List<Symbol>() { Symbol.None },
      new List<Symbol>() { Symbol.Success },
      new List<Symbol>() { Symbol.Success, Symbol.Advantage },
      new List<Symbol>() { Symbol.Advantage, Symbol.Advantage },
      new List<Symbol>() { Symbol.Advantage },
    });
    public static FfgDie s_SetbackDie = new FfgDie(new List<List<Symbol>>()
    {
      new List<Symbol>() { Symbol.None },
      new List<Symbol>() { Symbol.None },
      new List<Symbol>() { Symbol.Failure },
      new List<Symbol>() { Symbol.Failure },
      new List<Symbol>() { Symbol.Threat },
      new List<Symbol>() { Symbol.Threat },
    });
    public static FfgDie s_AbilityDie = new FfgDie(new List<List<Symbol>>()
    {
      new List<Symbol>() { Symbol.None },
      new List<Symbol>() { Symbol.Success },
      new List<Symbol>() { Symbol.Success },
      new List<Symbol>() { Symbol.Success, Symbol.Success },
      new List<Symbol>() { Symbol.Advantage },
      new List<Symbol>() { Symbol.Advantage },
      new List<Symbol>() { Symbol.Success, Symbol.Advantage },
      new List<Symbol>() { Symbol.Advantage, Symbol.Advantage },
    });
    public static FfgDie s_DifficultyDie = new FfgDie(new List<List<Symbol>>()
    {
      new List<Symbol>() { Symbol.None },
      new List<Symbol>() { Symbol.Failure },
      new List<Symbol>() { Symbol.Failure, Symbol.Failure },
      new List<Symbol>() { Symbol.Threat },
      new List<Symbol>() { Symbol.Threat },
      new List<Symbol>() { Symbol.Threat },
      new List<Symbol>() { Symbol.Threat, Symbol.Threat },
      new List<Symbol>() { Symbol.Failure, Symbol.Threat },
    });
    public static FfgDie s_ProficiencyDie = new FfgDie(new List<List<Symbol>>()
    {
      new List<Symbol>() { Symbol.None },
      new List<Symbol>() { Symbol.Success },
      new List<Symbol>() { Symbol.Success },
      new List<Symbol>() { Symbol.Success, Symbol.Success },
      new List<Symbol>() { Symbol.Success, Symbol.Success },
      new List<Symbol>() { Symbol.Advantage },
      new List<Symbol>() { Symbol.Success, Symbol.Advantage },
      new List<Symbol>() { Symbol.Success, Symbol.Advantage },
      new List<Symbol>() { Symbol.Success, Symbol.Advantage },
      new List<Symbol>() { Symbol.Advantage, Symbol.Advantage },
      new List<Symbol>() { Symbol.Advantage, Symbol.Advantage },
      new List<Symbol>() { Symbol.Triumph, Symbol.Success },
    });
    public static FfgDie s_ChallengeDie = new FfgDie(new List<List<Symbol>>()
    {
      new List<Symbol>() { Symbol.None },
      new List<Symbol>() { Symbol.Failure },
      new List<Symbol>() { Symbol.Failure },
      new List<Symbol>() { Symbol.Failure, Symbol.Failure },
      new List<Symbol>() { Symbol.Failure, Symbol.Failure },
      new List<Symbol>() { Symbol.Threat },
      new List<Symbol>() { Symbol.Threat },
      new List<Symbol>() { Symbol.Failure, Symbol.Threat },
      new List<Symbol>() { Symbol.Failure, Symbol.Threat },
      new List<Symbol>() { Symbol.Threat, Symbol.Threat },
      new List<Symbol>() { Symbol.Threat, Symbol.Threat },
      new List<Symbol>() { Symbol.Despair, Symbol.Failure },
    });
    public static FfgDie s_ForceDie = new FfgDie(new List<List<Symbol>>()
    {
      new List<Symbol>() { Symbol.Dark },
      new List<Symbol>() { Symbol.Dark },
      new List<Symbol>() { Symbol.Dark },
      new List<Symbol>() { Symbol.Dark },
      new List<Symbol>() { Symbol.Dark },
      new List<Symbol>() { Symbol.Dark },
      new List<Symbol>() { Symbol.Dark, Symbol.Dark },
      new List<Symbol>() { Symbol.Light },
      new List<Symbol>() { Symbol.Light },
      new List<Symbol>() { Symbol.Light, Symbol.Light },
      new List<Symbol>() { Symbol.Light, Symbol.Light },
      new List<Symbol>() { Symbol.Light, Symbol.Light },
    });

    public enum Type
    {
      Boost,
      Setback,
      Ability,
      Difficulty,
      Proficiency,
      Challenge,
      Force,
    }

    public enum Symbol
    {
      Success,
      Advantage,
      Triumph,
      Failure,
      Threat,
      Despair,
      Light,
      Dark,
      None,
    }

    public static Dictionary<Symbol, Symbol> s_Opposites = new Dictionary<Symbol, Symbol>()
    {
      { Symbol.Success, Symbol.Failure },
      { Symbol.Advantage, Symbol.Threat },
      { Symbol.Failure, Symbol.Success },
      { Symbol.Threat, Symbol.Advantage },
    };

    public static Dictionary<Type, string> s_DiceEmoji = new Dictionary<Type, string>()
    {
      { Type.Boost, "<:boostDie:739284154415317023>" },
      { Type.Setback, "<:setbackDie:739284154516242482>" },
      { Type.Ability, "<:abilityDie:739284154688077824>" },
      { Type.Difficulty, "<:difficultyDie:739284154436550698>" },
      { Type.Proficiency, "<:proficiencyDie:739284154696335360>" },
      { Type.Challenge, "<:challengeDie:739284154666975242>" },
      { Type.Force, "<:forceDie:739284154515980328>" },
    };

    public static Dictionary<Symbol, string> s_SymbolEmoji = new Dictionary<Symbol, string>()
    {
      { Symbol.Success, "<:success:739286512117612618>" },
      { Symbol.Advantage, "<:advantage:739286512113418242>" },
      { Symbol.Triumph, "<:triumph:739286512587243580>" },
      { Symbol.Failure, "<:failure:739286510808858635>" },
      { Symbol.Threat, "<:threat:739286512482385990>" },
      { Symbol.Despair, "<:despair:739286512289579079>" },
      { Symbol.Light, "<:lightside:739644393836773387>" },
      { Symbol.Dark, "<:darkside:739644393560080466>" },
      { Symbol.None, "<:blank_die:739645851894415452>" },
    };

    public static Dictionary<FfgDie, string> s_EmojiByDie = new Dictionary<FfgDie, string>()
    {
      { s_BoostDie, "<:boostDie:739284154415317023>" },
      { s_SetbackDie, "<:setbackDie:739284154516242482>" },
      { s_AbilityDie, "<:abilityDie:739284154688077824>" },
      { s_DifficultyDie, "<:difficultyDie:739284154436550698>" },
      { s_ProficiencyDie, "<:proficiencyDie:739284154696335360>" },
      { s_ChallengeDie, "<:challengeDie:739284154666975242>" },
      { s_ForceDie, "<:forceDie:739284154515980328>" },
    };

    public static Dictionary<FfgDie, Type> s_DieTypeByDie = new Dictionary<FfgDie, Type>()
    {
      { s_BoostDie, Type.Boost },
      { s_SetbackDie, Type.Setback },
      { s_AbilityDie, Type.Ability },
      { s_DifficultyDie, Type.Difficulty },
      { s_ProficiencyDie, Type.Proficiency },
      { s_ChallengeDie, Type.Challenge },
      { s_ForceDie, Type.Force },
    };

    public class Pool
    {
      public class RollResult
      {
        public Dictionary<Symbol, uint> m_Result = new Dictionary<Symbol, uint>();
        public List<Symbol> m_SymbolList = new List<Symbol>();

        public void Add(Symbol symbol)
        {
          m_SymbolList.Add(symbol);

          if (symbol == Symbol.None)
            return;

          var opposite = s_Opposites.GetValueOrDefault(symbol, Symbol.None);

          if (m_Result.ContainsKey(opposite))
          {
            Subtract(opposite);
            return;
          }

          if (m_Result.ContainsKey(symbol))
            ++m_Result[symbol];
          else
            m_Result[symbol] = 1;
        }

        public void Subtract(Symbol symbol)
        {
          if (m_Result.ContainsKey(symbol))
          {
            --m_Result[symbol];

            if (m_Result[symbol] == 0)
              m_Result.Remove(symbol);
          }
        }
      }

      public Dictionary<FfgDie, uint> m_Counts = new Dictionary<FfgDie, uint>();

      public void Add(FfgDie die)
      {
        if (m_Counts.ContainsKey(die))
          ++m_Counts[die];
        else
          m_Counts[die] = 1;
      }

      public void Remove(FfgDie die)
      {
        if (m_Counts.ContainsKey(die))
        {
          --m_Counts[die];

          if (m_Counts[die] == 0)
            m_Counts.Remove(die);
        }
      }

      public string GetSortedEmojiString()
      {
        var emojiString = string.Empty;
        var typeList = new List<Type>();

        foreach (var entry in m_Counts)
        {
          var die = entry.Key;
          var count = entry.Value;

          for (var i = 0; i < count; ++i)
            typeList.Add(s_DieTypeByDie[die]);
        }

        typeList.Sort();

        foreach (var type in typeList)
          emojiString += s_DiceEmoji[type] + " ";

        return emojiString;
      }

      public RollResult Roll()
      {
        var result = new RollResult();

        foreach (var entry in m_Counts)
        {
          var die = entry.Key;

          for (var i = 0; i < entry.Value; ++i)
          {
            var outcome = die.Roll();

            foreach (var symbol in outcome)
              result.Add(symbol);
          }
        }

        return result;
      }
    }

    public List<List<Symbol>> m_Sides { get; private set; }

    public FfgDie(List<List<Symbol>> sides)
    {
      m_Sides = sides;
    }

    public List<Symbol> Roll()
    {
      var r = Bot.s_RNG.Next(0, m_Sides.Count);
      return m_Sides[r];
    }
  }
}

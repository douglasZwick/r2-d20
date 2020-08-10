using DSharpPlus.Entities;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;

namespace R2D20
{
  public class Die { }

  public class FfgDie : Die
  {
    public static FfgDie s_BoostDie = new FfgDie(new List<Symbol>()
    {
      Symbol.None,
      Symbol.None,
      Symbol.Success,
      Symbol.SuccessAdvantage,
      Symbol.DoubleAdvantage,
      Symbol.Advantage,
    });
    public static FfgDie s_SetbackDie = new FfgDie(new List<Symbol>()
    {
      Symbol.None,
      Symbol.None,
      Symbol.Failure,
      Symbol.Failure,
      Symbol.Threat,
      Symbol.Threat,
    });
    public static FfgDie s_AbilityDie = new FfgDie(new List<Symbol>()
    {
      Symbol.None,
      Symbol.Success,
      Symbol.Success,
      Symbol.DoubleSuccess,
      Symbol.Advantage,
      Symbol.Advantage,
      Symbol.SuccessAdvantage,
      Symbol.DoubleAdvantage,
    });
    public static FfgDie s_DifficultyDie = new FfgDie(new List<Symbol>()
    {
      Symbol.None,
      Symbol.Failure,
      Symbol.DoubleFailure,
      Symbol.Threat,
      Symbol.Threat,
      Symbol.Threat,
      Symbol.DoubleThreat,
      Symbol.FailureThreat,
    });
    public static FfgDie s_ProficiencyDie = new FfgDie(new List<Symbol>()
    {
      Symbol.None,
      Symbol.Success,
      Symbol.Success,
      Symbol.DoubleSuccess,
      Symbol.DoubleSuccess,
      Symbol.Advantage,
      Symbol.SuccessAdvantage,
      Symbol.SuccessAdvantage,
      Symbol.SuccessAdvantage,
      Symbol.DoubleAdvantage,
      Symbol.DoubleAdvantage,
      Symbol.Triumph
    });
    public static FfgDie s_ChallengeDie = new FfgDie(new List<Symbol>()
    {
      Symbol.None,
      Symbol.Failure,
      Symbol.Failure,
      Symbol.DoubleFailure,
      Symbol.DoubleFailure,
      Symbol.Threat,
      Symbol.Threat,
      Symbol.FailureThreat,
      Symbol.FailureThreat,
      Symbol.DoubleThreat,
      Symbol.DoubleThreat,
      Symbol.Despair,
    });
    public static FfgDie s_ForceDie = new FfgDie(new List<Symbol>()
    {
      Symbol.Dark,
      Symbol.Dark,
      Symbol.Dark,
      Symbol.Dark,
      Symbol.Dark,
      Symbol.Dark,
      Symbol.DoubleDark,
      Symbol.Light,
      Symbol.Light,
      Symbol.DoubleLight,
      Symbol.DoubleLight,
      Symbol.DoubleLight,
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
      DoubleSuccess,
      Failure,
      DoubleFailure,
      Advantage,
      DoubleAdvantage,
      SuccessAdvantage,
      Threat,
      DoubleThreat,
      FailureThreat,
      Triumph,
      Despair,
      Light,
      DoubleLight,
      Dark,
      DoubleDark,
      None,
    }

    public static Dictionary<Symbol, List<Symbol>> s_SymbolToList = new Dictionary<Symbol, List<Symbol>>()
    {
      { Symbol.Success, new List<Symbol>() { Symbol.Success } },
      { Symbol.DoubleSuccess, new List<Symbol>() { Symbol.Success, Symbol.Success } },
      { Symbol.Advantage, new List<Symbol>() { Symbol.Advantage } },
      { Symbol.DoubleAdvantage, new List<Symbol>() { Symbol.Advantage, Symbol.Advantage } },
      { Symbol.SuccessAdvantage, new List<Symbol>() { Symbol.Success, Symbol.Advantage } },
      { Symbol.Triumph, new List<Symbol>() { Symbol.Triumph, Symbol.Success } },
      { Symbol.Failure, new List<Symbol>() { Symbol.Failure } },
      { Symbol.DoubleFailure, new List<Symbol>() { Symbol.Failure, Symbol.Failure } },
      { Symbol.Threat, new List<Symbol>() { Symbol.Threat } },
      { Symbol.DoubleThreat, new List<Symbol>() { Symbol.Threat, Symbol.Threat } },
      { Symbol.FailureThreat, new List<Symbol>() { Symbol.Failure, Symbol.Threat } },
      { Symbol.Despair, new List<Symbol>() { Symbol.Despair, Symbol.Failure } },
      { Symbol.Light, new List<Symbol>() { Symbol.Light } },
      { Symbol.DoubleLight, new List<Symbol>() { Symbol.Light, Symbol.Light } },
      { Symbol.Dark, new List<Symbol>() { Symbol.Dark } },
      { Symbol.DoubleDark, new List<Symbol>() { Symbol.Dark, Symbol.Dark } },
      { Symbol.None, new List<Symbol>() { } },
    };

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
      { Symbol.DoubleSuccess, "<:doubleSuccess:739952273735811244>" },
      { Symbol.Advantage, "<:advantage:739286512113418242>" },
      { Symbol.DoubleAdvantage, "<:doubleAdvantage:739952269339918518>" },
      { Symbol.SuccessAdvantage, "<:successAdvantage:739952273735680050>" },
      { Symbol.Triumph, "<:triumph:739286512587243580>" },
      { Symbol.Failure, "<:failure:739286510808858635>" },
      { Symbol.DoubleFailure, "<:doubleFailure:739952273974624266>" },
      { Symbol.Threat, "<:threat:739286512482385990>" },
      { Symbol.DoubleThreat, "<:doubleThreat:739952267427446794>" },
      { Symbol.FailureThreat, "<:failureThreat:739952267729436693>" },
      { Symbol.Despair, "<:despair:739286512289579079>" },
      { Symbol.Light, "<:lightside:739644393836773387>" },
      { Symbol.DoubleLight, "<:doubleLight:742093383950139412>" },
      { Symbol.Dark, "<:darkside:739644393560080466>" },
      { Symbol.DoubleDark, "<:doubleDark:742093383987626135>" },
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
        public enum SuccessStatus
        {
          Nothing,
          Success,
          Failure,
        }

        public enum AdvantageStatus
        {
          Nothing,
          Advantage,
          Threat,
        }

        public enum ForceStatus
        {
          Nothing,
          Light,
          DoubleLight,
          Dark,
          DoubleDark
        }

        public Dictionary<Symbol, uint> m_Result = new Dictionary<Symbol, uint>();
        public List<DieResult> m_DieList = new List<DieResult>();
        public SuccessStatus m_SuccessStatus = SuccessStatus.Nothing;
        public AdvantageStatus m_AdvantageStatus = AdvantageStatus.Nothing;
        public ForceStatus m_ForceStatus = ForceStatus.Nothing;
        public bool m_Triumph = false;
        public bool m_Despair = false;

        public void Add(Symbol symbol)
        {
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

        public void AddDieResult(Type type, Symbol symbol)
        {
          m_DieList.Add(new DieResult(type, symbol));
        }

        public void Finish()
        {
          if (m_Result.ContainsKey(Symbol.Success))
            m_SuccessStatus = SuccessStatus.Success;
          else if (m_Result.ContainsKey(Symbol.Failure))
            m_SuccessStatus = SuccessStatus.Failure;
          else
            m_SuccessStatus = SuccessStatus.Nothing;

          if (m_Result.ContainsKey(Symbol.Advantage))
            m_AdvantageStatus = AdvantageStatus.Advantage;
          else if (m_Result.ContainsKey(Symbol.Threat))
            m_AdvantageStatus = AdvantageStatus.Threat;
          else
            m_AdvantageStatus = AdvantageStatus.Nothing;

          var lightPips = m_Result.GetValueOrDefault(Symbol.Light, 0u);
          var darkPips = m_Result.GetValueOrDefault(Symbol.Dark, 0u);

          if (lightPips == 0u && darkPips == 0u)
          {
            m_ForceStatus = ForceStatus.Nothing;
          }
          else
          {
            if (darkPips > lightPips)
            {
              if (darkPips == 1)
                m_ForceStatus = ForceStatus.Dark;
              else
                m_ForceStatus = ForceStatus.DoubleDark;
            }
            else
            {
              if (lightPips == 1)
                m_ForceStatus = ForceStatus.Light;
              else
                m_ForceStatus = ForceStatus.DoubleLight;
            }
          }

          m_Triumph = m_Result.ContainsKey(Symbol.Triumph);
          m_Despair = m_Result.ContainsKey(Symbol.Despair);

          Shuffle(m_DieList);
        }
      }

      public class DieResult
      {
        public Type m_DieType;
        public Symbol m_Symbol;

        public DieResult(Type type, Symbol symbol)
        {
          m_DieType = type;
          m_Symbol = symbol;
        }

        public override string ToString()
        {
          var typeEmoji = s_DiceEmoji[m_DieType];
          var symbolEmoji = s_SymbolEmoji[m_Symbol];

          return $"[{typeEmoji}{symbolEmoji}]";
        }
      }

      public Dictionary<FfgDie, uint> m_Counts = new Dictionary<FfgDie, uint>();
      public Dictionary<DiscordMember, Dictionary<FfgDie, uint>> m_Secrets =
        new Dictionary<DiscordMember, Dictionary<FfgDie, uint>>();

      public void Add(FfgDie die)
      {
        if (m_Counts.ContainsKey(die))
          ++m_Counts[die];
        else
          m_Counts[die] = 1;
      }

      public void AddSecret(DiscordMember member, FfgDie die)
      {
        if (m_Secrets.ContainsKey(member))
        {
          if (m_Secrets[member].ContainsKey(die))
            ++m_Secrets[member][die];
          else
            m_Secrets[member][die] = 1;
        }
        else
        {
          m_Secrets[member] = new Dictionary<FfgDie, uint>() { { die, 1 } };
        }
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

      public void RemoveSecret(DiscordMember member, FfgDie die)
      {
        if (m_Secrets.ContainsKey(member))
        {
          if (m_Secrets[member].ContainsKey(die))
          {
            --m_Secrets[member][die];

            if (m_Secrets[member][die] == 0)
            {
              m_Secrets[member].Remove(die);

              if (m_Secrets[member].Count == 0)
                m_Secrets.Remove(member);
            }
          }
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
            var type = s_DieTypeByDie[die];
            var symbol = die.Roll();
            result.AddDieResult(type, symbol);

            var outcome = s_SymbolToList[symbol];

            foreach (var outcomeSymbol in outcome)
              result.Add(outcomeSymbol);
          }
        }

        result.Finish();

        return result;
      }
    }

    public List<Symbol> m_Sides { get; private set; }

    public FfgDie(List<Symbol> sides)
    {
      m_Sides = sides;
    }

    public Symbol Roll()
    {
      var r = Bot.s_RNG.Next(0, m_Sides.Count);
      return m_Sides[r];
    }

    private static void Shuffle<T>(List<T> list)
    {
      var count = list.Count;

      for (var i = 0; i < (count - 1); ++i)
      {
        var r = i + Bot.s_RNG.Next(count - i);
        var item = list[r];
        list[r] = list[i];
        list[i] = item;
      }
    }
  }
}

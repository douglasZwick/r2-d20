namespace R2D20
{
  public class FfgDestinyPool
  {
    public int m_Light { get; set; }
    public int m_Dark { get; set; }


    public bool IsEmpty()
    {
      return m_Light <= 0 && m_Dark <= 0;
    }

    public void AddLight(int value)
    {
      m_Light += value;

      if (m_Light < 0)
        m_Light = 0;
    }

    public void AddDark(int value)
    {
      m_Dark += value;

      if (m_Dark < 0)
        m_Dark = 0;
    }

    public void SetLight(int value)
    {
      m_Light = value;
    }

    public void SetDark(int value)
    {
      m_Dark = value;
    }

    public void Set(int lightValue, int darkValue)
    {
      SetLight(lightValue);
      SetDark(darkValue);
    }

    public void Clear()
    {
      m_Light = 0;
      m_Dark = 0;
    }

    public void SpendLight()
    {
      --m_Light;
      ++m_Dark;
    }

    public void SpendDark()
    {
      --m_Dark;
      ++m_Light;
    }
  }
}

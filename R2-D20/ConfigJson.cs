using Newtonsoft.Json;

namespace R2D20
{
  public struct ConfigJson
  {
    [JsonProperty("token")]
    public string Token { get; private set; }
    [JsonProperty("prefix")]
    public string Prefix { get; private set; }
  }
}

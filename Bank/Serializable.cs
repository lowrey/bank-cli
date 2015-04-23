using Newtonsoft.Json;

namespace Bank
{
    internal abstract class Serializable
    {
        public string ToJson()
        {
            return JsonConvert.SerializeObject(this);
        }

        public override string ToString()
        {
            return ToJson();
        }
    }
}
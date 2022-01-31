namespace OntologyCSharpSDK.Common
{
    public class Fixed64
    {
        public string value { get; set; }

        public Fixed64()
        {
            value = "0000000000000000";
        }

        public string serialize()
        {
            return value;
        }

    }
}
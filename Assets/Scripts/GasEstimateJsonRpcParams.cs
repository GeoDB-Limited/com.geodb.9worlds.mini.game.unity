namespace EstimateJSONRPC
{
    public class GasEstimateJsonRpcParams
    {
        public string from;
        public string to;
        public string gasPrice;
        public string gas;
        public string value;
        public string data;

        public GasEstimateJsonRpcParams(string from, string to, string gasPrice, string gas, string value, string data)
        {
            this.from = from;
            this.to = to;
            this.gasPrice = gasPrice;
            this.gas = gas;
            this.value = value;
            this.data = data;
        }
    }
}

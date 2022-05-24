namespace EstimateJSONRPC
{
    public class EstimateErrorJsonRpc
    {
        public string jsonrpc = "2.0";
        public string method = "eth_call";
        public JsonRpcError error;
        public int id = 1;
    }
}

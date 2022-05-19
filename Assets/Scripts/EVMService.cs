using System;

public class EVMService
{
    const string abi = "[ { \"inputs\": [ { \"internalType\": \"uint256\", \"name\": \"_nftMatchCount\", \"type\": \"uint256\" } ], \"name\": \"createMatchAndRequestRandom\", \"outputs\": [], \"stateMutability\": \"nonpayable\", \"type\": \"function\" }, { \"inputs\": [ { \"internalType\": \"address\", \"name\": \"user\", \"type\": \"address\" } ], \"name\": \"initializeMatchFor\", \"outputs\": [], \"stateMutability\": \"nonpayable\", \"type\": \"function\" }, { \"inputs\": [ { \"internalType\": \"contract IVikings\", \"name\": \"_vikingsContract\", \"type\": \"address\" }, { \"internalType\": \"uint256\", \"name\": \"_maxMatchesPerDay\", \"type\": \"uint256\" }, { \"internalType\": \"uint256\", \"name\": \"_maxNftMatchCount\", \"type\": \"uint256\" } ], \"stateMutability\": \"nonpayable\", \"type\": \"constructor\" }, { \"anonymous\": false, \"inputs\": [ { \"indexed\": false, \"internalType\": \"uint256\", \"name\": \"matchId\", \"type\": \"uint256\" }, { \"indexed\": false, \"internalType\": \"address\", \"name\": \"user\", \"type\": \"address\" } ], \"name\": \"MatchCreated\", \"type\": \"event\" }, { \"anonymous\": false, \"inputs\": [ { \"indexed\": false, \"internalType\": \"uint256\", \"name\": \"matchId\", \"type\": \"uint256\" }, { \"indexed\": false, \"internalType\": \"address\", \"name\": \"initializerUser\", \"type\": \"address\" } ], \"name\": \"MatchInitialized\", \"type\": \"event\" }, { \"anonymous\": false, \"inputs\": [ { \"indexed\": false, \"internalType\": \"uint256\", \"name\": \"matchId\", \"type\": \"uint256\" } ], \"name\": \"MatchResolved\", \"type\": \"event\" }, { \"anonymous\": false, \"inputs\": [ { \"indexed\": true, \"internalType\": \"address\", \"name\": \"previousOwner\", \"type\": \"address\" }, { \"indexed\": true, \"internalType\": \"address\", \"name\": \"newOwner\", \"type\": \"address\" } ], \"name\": \"OwnershipTransferred\", \"type\": \"event\" }, { \"anonymous\": false, \"inputs\": [ { \"indexed\": false, \"internalType\": \"bytes32\", \"name\": \"requestId\", \"type\": \"bytes32\" }, { \"indexed\": false, \"internalType\": \"uint256\", \"name\": \"matchId\", \"type\": \"uint256\" } ], \"name\": \"RandomMatchId\", \"type\": \"event\" }, { \"anonymous\": false, \"inputs\": [ { \"indexed\": false, \"internalType\": \"bytes32\", \"name\": \"requestId\", \"type\": \"bytes32\" } ], \"name\": \"RandomnessEvent\", \"type\": \"event\" }, { \"inputs\": [ { \"internalType\": \"bytes32\", \"name\": \"requestId\", \"type\": \"bytes32\" }, { \"internalType\": \"uint256\", \"name\": \"randomness\", \"type\": \"uint256\" } ], \"name\": \"rawFulfillRandomness\", \"outputs\": [], \"stateMutability\": \"nonpayable\", \"type\": \"function\" }, { \"inputs\": [], \"name\": \"renounceOwnership\", \"outputs\": [], \"stateMutability\": \"nonpayable\", \"type\": \"function\" }, { \"anonymous\": false, \"inputs\": [ { \"indexed\": false, \"internalType\": \"bytes32\", \"name\": \"requestId\", \"type\": \"bytes32\" } ], \"name\": \"RequestValues\", \"type\": \"event\" }, { \"inputs\": [], \"name\": \"resolveMatch\", \"outputs\": [], \"stateMutability\": \"nonpayable\", \"type\": \"function\" }, { \"inputs\": [ { \"internalType\": \"uint256\", \"name\": \"indexA\", \"type\": \"uint256\" }, { \"internalType\": \"uint256\", \"name\": \"indexB\", \"type\": \"uint256\" } ], \"name\": \"resolveMatchWithReorder\", \"outputs\": [], \"stateMutability\": \"nonpayable\", \"type\": \"function\" }, { \"inputs\": [ { \"internalType\": \"uint256\", \"name\": \"_maxMatches\", \"type\": \"uint256\" } ], \"name\": \"setMaxMatchesPerDay\", \"outputs\": [], \"stateMutability\": \"nonpayable\", \"type\": \"function\" }, { \"inputs\": [ { \"internalType\": \"uint256\", \"name\": \"_maxNftMatchCount\", \"type\": \"uint256\" } ], \"name\": \"setMaxNftMatchCount\", \"outputs\": [], \"stateMutability\": \"nonpayable\", \"type\": \"function\" }, { \"inputs\": [ { \"internalType\": \"uint256\", \"name\": \"_points\", \"type\": \"uint256\" } ], \"name\": \"setNftPointForComputerWinner\", \"outputs\": [], \"stateMutability\": \"nonpayable\", \"type\": \"function\" }, { \"inputs\": [ { \"internalType\": \"uint256\", \"name\": \"_points\", \"type\": \"uint256\" } ], \"name\": \"setNftPointForPlayerWinner\", \"outputs\": [], \"stateMutability\": \"nonpayable\", \"type\": \"function\" }, { \"inputs\": [ { \"internalType\": \"uint256[]\", \"name\": \"_nftIds\", \"type\": \"uint256[]\" }, { \"internalType\": \"enum NineWorldsMinigame.NftType[]\", \"name\": \"_nftTypes\", \"type\": \"uint8[]\" }, { \"internalType\": \"uint256[]\", \"name\": \"_powers\", \"type\": \"uint256[]\" } ], \"name\": \"setNftTypeAndPower\", \"outputs\": [], \"stateMutability\": \"nonpayable\", \"type\": \"function\" }, { \"inputs\": [ { \"internalType\": \"address\", \"name\": \"newOwner\", \"type\": \"address\" } ], \"name\": \"transferOwnership\", \"outputs\": [], \"stateMutability\": \"nonpayable\", \"type\": \"function\" }, { \"inputs\": [ { \"internalType\": \"uint256\", \"name\": \"matchId\", \"type\": \"uint256\" }, { \"internalType\": \"uint8\", \"name\": \"index\", \"type\": \"uint8\" } ], \"name\": \"getValidcomputerNft\", \"outputs\": [ { \"internalType\": \"uint256\", \"name\": \"\", \"type\": \"uint256\" } ], \"stateMutability\": \"view\", \"type\": \"function\" }, { \"inputs\": [ { \"internalType\": \"uint256\", \"name\": \"matchId\", \"type\": \"uint256\" }, { \"internalType\": \"uint8\", \"name\": \"index\", \"type\": \"uint8\" } ], \"name\": \"getValidNft\", \"outputs\": [ { \"internalType\": \"uint256\", \"name\": \"\", \"type\": \"uint256\" } ], \"stateMutability\": \"view\", \"type\": \"function\" }, { \"inputs\": [ { \"internalType\": \"uint256\", \"name\": \"matchId\", \"type\": \"uint256\" }, { \"internalType\": \"uint8\", \"name\": \"index\", \"type\": \"uint8\" } ], \"name\": \"getValidPlayerNft\", \"outputs\": [ { \"internalType\": \"uint256\", \"name\": \"\", \"type\": \"uint256\" } ], \"stateMutability\": \"view\", \"type\": \"function\" }, { \"inputs\": [ { \"internalType\": \"uint256\", \"name\": \"\", \"type\": \"uint256\" } ], \"name\": \"matchesById\", \"outputs\": [ { \"internalType\": \"uint256\", \"name\": \"matchId\", \"type\": \"uint256\" }, { \"internalType\": \"uint256\", \"name\": \"nftMatchCount\", \"type\": \"uint256\" }, { \"internalType\": \"bool\", \"name\": \"isRerrollEnabled\", \"type\": \"bool\" }, { \"internalType\": \"uint256\", \"name\": \"matchRandomSeed\", \"type\": \"uint256\" }, { \"internalType\": \"bool\", \"name\": \"isMatchFinished\", \"type\": \"bool\" }, { \"internalType\": \"bool\", \"name\": \"didPlayerWin\", \"type\": \"bool\" } ], \"stateMutability\": \"view\", \"type\": \"function\" }, { \"inputs\": [ { \"internalType\": \"bytes32\", \"name\": \"\", \"type\": \"bytes32\" } ], \"name\": \"matchesByRequestId\", \"outputs\": [ { \"internalType\": \"uint256\", \"name\": \"\", \"type\": \"uint256\" } ], \"stateMutability\": \"view\", \"type\": \"function\" }, { \"inputs\": [], \"name\": \"maxMatchesPerDay\", \"outputs\": [ { \"internalType\": \"uint256\", \"name\": \"\", \"type\": \"uint256\" } ], \"stateMutability\": \"view\", \"type\": \"function\" }, { \"inputs\": [], \"name\": \"maxNftMatchCount\", \"outputs\": [ { \"internalType\": \"uint256\", \"name\": \"\", \"type\": \"uint256\" } ], \"stateMutability\": \"view\", \"type\": \"function\" }, { \"inputs\": [ { \"internalType\": \"uint256\", \"name\": \"\", \"type\": \"uint256\" } ], \"name\": \"nftMatchesByNftId\", \"outputs\": [ { \"internalType\": \"uint256\", \"name\": \"matchId\", \"type\": \"uint256\" }, { \"internalType\": \"uint256\", \"name\": \"nftMatchCount\", \"type\": \"uint256\" }, { \"internalType\": \"bool\", \"name\": \"isRerrollEnabled\", \"type\": \"bool\" }, { \"internalType\": \"uint256\", \"name\": \"matchRandomSeed\", \"type\": \"uint256\" }, { \"internalType\": \"bool\", \"name\": \"isMatchFinished\", \"type\": \"bool\" }, { \"internalType\": \"bool\", \"name\": \"didPlayerWin\", \"type\": \"bool\" } ], \"stateMutability\": \"view\", \"type\": \"function\" }, { \"inputs\": [], \"name\": \"nftPointForComputerWinner\", \"outputs\": [ { \"internalType\": \"uint256\", \"name\": \"\", \"type\": \"uint256\" } ], \"stateMutability\": \"view\", \"type\": \"function\" }, { \"inputs\": [], \"name\": \"nftPointForPlayerWinner\", \"outputs\": [ { \"internalType\": \"uint256\", \"name\": \"\", \"type\": \"uint256\" } ], \"stateMutability\": \"view\", \"type\": \"function\" }, { \"inputs\": [ { \"internalType\": \"uint256\", \"name\": \"\", \"type\": \"uint256\" } ], \"name\": \"nftStatusById\", \"outputs\": [ { \"internalType\": \"enum NineWorldsMinigame.NftType\", \"name\": \"nftType\", \"type\": \"uint8\" }, { \"internalType\": \"uint256\", \"name\": \"totalPower\", \"type\": \"uint256\" }, { \"internalType\": \"uint8\", \"name\": \"dailyMatchCounter\", \"type\": \"uint8\" }, { \"internalType\": \"uint8\", \"name\": \"reRollCounter\", \"type\": \"uint8\" }, { \"internalType\": \"uint256\", \"name\": \"dailyExpirationTimestamp\", \"type\": \"uint256\" }, { \"internalType\": \"uint256\", \"name\": \"points\", \"type\": \"uint256\" }, { \"internalType\": \"uint256\", \"name\": \"currentMatchId\", \"type\": \"uint256\" } ], \"stateMutability\": \"view\", \"type\": \"function\" }, { \"inputs\": [], \"name\": \"ONE_DAY_IN_SECONDS\", \"outputs\": [ { \"internalType\": \"uint256\", \"name\": \"\", \"type\": \"uint256\" } ], \"stateMutability\": \"view\", \"type\": \"function\" }, { \"inputs\": [], \"name\": \"owner\", \"outputs\": [ { \"internalType\": \"address\", \"name\": \"\", \"type\": \"address\" } ], \"stateMutability\": \"view\", \"type\": \"function\" }, { \"inputs\": [], \"name\": \"totalNfts\", \"outputs\": [ { \"internalType\": \"uint256\", \"name\": \"\", \"type\": \"uint256\" } ], \"stateMutability\": \"view\", \"type\": \"function\" }, { \"inputs\": [ { \"internalType\": \"address\", \"name\": \"\", \"type\": \"address\" } ], \"name\": \"usersLastMatchId\", \"outputs\": [ { \"internalType\": \"uint256\", \"name\": \"\", \"type\": \"uint256\" } ], \"stateMutability\": \"view\", \"type\": \"function\" }, { \"inputs\": [], \"name\": \"vikingsContract\", \"outputs\": [ { \"internalType\": \"contract IVikings\", \"name\": \"\", \"type\": \"address\" } ], \"stateMutability\": \"view\", \"type\": \"function\" } ]";
    const string rpc = "https://polygon-mumbai.g.alchemy.com/v2/g4D8RdKg4vYhyeZYCzME28DXSU2yo744";
    const string chain = "polygon";
    const string network = "testnet";
    const string contract = "0x9c247046295E6052815184e52106297a69bC1A15";

    const string value = "0";
    const string gasLimit = "";
    const string gasPrice = "";

    public async System.Threading.Tasks.Task<string> TxStatus(string txHash)
    {
        return await EVM.TxStatus(chain, network, txHash);
    }

    public async System.Threading.Tasks.Task<string> GetUserLastMatch(string account)
    {
        string method = "usersLastMatchId";
        string args = "[\"" + account + "\"]";
        try {
            return await EVM.Call(chain, network, contract, abi, method, args);
        } catch (Exception e) {
            throw e;
        }
    }

    public async System.Threading.Tasks.Task<string> GetMatchInfoById(string id)
    {
        string method = "matchesById";
        string args = "[\"" + id + "\"]";
        try {
            return await EVM.Call(chain, network, contract, abi, method, args);
        } catch (Exception e) {
            throw e;
        }
    }

    public async System.Threading.Tasks.Task<string> GetValidPlayerNft(string matchId, string index)
    {
        string method = "getValidPlayerNft";
        string args = "[\"" + matchId + "\", \"" + index + "\"]";
        try {
            return await EVM.Call(chain, network, contract, abi, method, args);
        } catch (Exception e) {
            throw e;
        }
    }

    public async System.Threading.Tasks.Task<string> GetValidComputerNft(string matchId, string index)
    {
        string method = "getValidcomputerNft";
        string args = "[\"" + matchId + "\", \"" + index + "\"]";
        try {
            return await EVM.Call(chain, network, contract, abi, method, args);
        } catch (Exception e) {
            throw e;
        }
    }

    public async System.Threading.Tasks.Task<string> CreateMatch(int numberOfNfts)
    {
        string method = "createMatchAndRequestRandom";
        string args = "[\"" + numberOfNfts + "\"]";

        try {
            return await Web3GL.SendContract(method, abi, contract, args, value, gasLimit, gasPrice);
        } catch (Exception e) {
            throw e;
        }
    }

    public async System.Threading.Tasks.Task<string> InitializeMatch(string account)
    {
        string method = "initializeMatchFor";
        string args = "[\"" + account + "\"]";
        
        try {
            return await Web3GL.SendContract(method, abi, contract, args, value, gasLimit, gasPrice);
        } catch (Exception e) {
            throw e;
        }
    }

}

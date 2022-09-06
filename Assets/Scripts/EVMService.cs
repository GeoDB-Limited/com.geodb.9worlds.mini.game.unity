using System;
using UnityEngine.Networking;
using UnityEngine;
using Newtonsoft.Json;
using EstimateJSONRPC;

public class EVMService
{
    const string abi = "[ { \"inputs\": [ { \"internalType\": \"uint256\", \"name\": \"_nftMatchCount\", \"type\": \"uint256\" } ], \"name\": \"createMatchAndRequestRandom\", \"outputs\": [], \"stateMutability\": \"nonpayable\", \"type\": \"function\" }, { \"inputs\": [ { \"internalType\": \"address\", \"name\": \"user\", \"type\": \"address\" } ], \"name\": \"initializeMatchFor\", \"outputs\": [], \"stateMutability\": \"nonpayable\", \"type\": \"function\" }, { \"inputs\": [ { \"internalType\": \"contract IVikings\", \"name\": \"_vikingsContract\", \"type\": \"address\" }, { \"internalType\": \"uint256\", \"name\": \"_maxMatchesPerDay\", \"type\": \"uint256\" }, { \"internalType\": \"uint256\", \"name\": \"_maxNftMatchCount\", \"type\": \"uint256\" }, { \"internalType\": \"uint256\", \"name\": \"_nftPointForPlayerWinner\", \"type\": \"uint256\" }, { \"internalType\": \"uint256\", \"name\": \"_nftPointForComputerWinner\", \"type\": \"uint256\" }, { \"internalType\": \"uint256\", \"name\": \"_nftPointForPlayerTie\", \"type\": \"uint256\" }, { \"internalType\": \"uint256\", \"name\": \"_nftPointForComputerTie\", \"type\": \"uint256\" }, { \"internalType\": \"uint256\", \"name\": \"_maxValidId\", \"type\": \"uint256\" } ], \"stateMutability\": \"nonpayable\", \"type\": \"constructor\" }, { \"anonymous\": false, \"inputs\": [ { \"indexed\": false, \"internalType\": \"uint256\", \"name\": \"matchId\", \"type\": \"uint256\" }, { \"indexed\": false, \"internalType\": \"address\", \"name\": \"user\", \"type\": \"address\" } ], \"name\": \"MatchCreated\", \"type\": \"event\" }, { \"anonymous\": false, \"inputs\": [ { \"indexed\": false, \"internalType\": \"uint256\", \"name\": \"matchId\", \"type\": \"uint256\" }, { \"indexed\": false, \"internalType\": \"address\", \"name\": \"initializerUser\", \"type\": \"address\" } ], \"name\": \"MatchInitialized\", \"type\": \"event\" }, { \"anonymous\": false, \"inputs\": [ { \"indexed\": false, \"internalType\": \"uint256\", \"name\": \"matchId\", \"type\": \"uint256\" } ], \"name\": \"MatchResolved\", \"type\": \"event\" }, { \"anonymous\": false, \"inputs\": [ { \"indexed\": true, \"internalType\": \"address\", \"name\": \"previousOwner\", \"type\": \"address\" }, { \"indexed\": true, \"internalType\": \"address\", \"name\": \"newOwner\", \"type\": \"address\" } ], \"name\": \"OwnershipTransferred\", \"type\": \"event\" }, { \"anonymous\": false, \"inputs\": [ { \"indexed\": false, \"internalType\": \"bytes32\", \"name\": \"requestId\", \"type\": \"bytes32\" }, { \"indexed\": false, \"internalType\": \"uint256\", \"name\": \"matchId\", \"type\": \"uint256\" } ], \"name\": \"RandomnessEvent\", \"type\": \"event\" }, { \"inputs\": [ { \"internalType\": \"bytes32\", \"name\": \"requestId\", \"type\": \"bytes32\" }, { \"internalType\": \"uint256\", \"name\": \"randomness\", \"type\": \"uint256\" } ], \"name\": \"rawFulfillRandomness\", \"outputs\": [], \"stateMutability\": \"nonpayable\", \"type\": \"function\" }, { \"inputs\": [], \"name\": \"renounceOwnership\", \"outputs\": [], \"stateMutability\": \"nonpayable\", \"type\": \"function\" }, { \"anonymous\": false, \"inputs\": [ { \"indexed\": false, \"internalType\": \"bytes32\", \"name\": \"requestId\", \"type\": \"bytes32\" } ], \"name\": \"RequestValues\", \"type\": \"event\" }, { \"inputs\": [], \"name\": \"resolveMatch\", \"outputs\": [], \"stateMutability\": \"nonpayable\", \"type\": \"function\" }, { \"inputs\": [ { \"internalType\": \"uint256\", \"name\": \"_indexA\", \"type\": \"uint256\" }, { \"internalType\": \"uint256\", \"name\": \"_indexB\", \"type\": \"uint256\" } ], \"name\": \"resolveMatchWithReorder\", \"outputs\": [], \"stateMutability\": \"nonpayable\", \"type\": \"function\" }, { \"inputs\": [ { \"internalType\": \"uint256\", \"name\": \"_maxMatches\", \"type\": \"uint256\" } ], \"name\": \"setMaxMatchesPerDay\", \"outputs\": [], \"stateMutability\": \"nonpayable\", \"type\": \"function\" }, { \"inputs\": [ { \"internalType\": \"uint256\", \"name\": \"_maxNftMatchCount\", \"type\": \"uint256\" } ], \"name\": \"setMaxNftMatchCount\", \"outputs\": [], \"stateMutability\": \"nonpayable\", \"type\": \"function\" }, { \"inputs\": [ { \"internalType\": \"uint256\", \"name\": \"_maxValidId\", \"type\": \"uint256\" } ], \"name\": \"setMaxValidId\", \"outputs\": [], \"stateMutability\": \"nonpayable\", \"type\": \"function\" }, { \"inputs\": [ { \"internalType\": \"uint256\", \"name\": \"_nftPointForComputerTie\", \"type\": \"uint256\" } ], \"name\": \"setNftPointForComputerTie\", \"outputs\": [], \"stateMutability\": \"nonpayable\", \"type\": \"function\" }, { \"inputs\": [ { \"internalType\": \"uint256\", \"name\": \"_points\", \"type\": \"uint256\" } ], \"name\": \"setNftPointForComputerWinner\", \"outputs\": [], \"stateMutability\": \"nonpayable\", \"type\": \"function\" }, { \"inputs\": [ { \"internalType\": \"uint256\", \"name\": \"_nftPointForPlayerTie\", \"type\": \"uint256\" } ], \"name\": \"setNftPointForPlayerTie\", \"outputs\": [], \"stateMutability\": \"nonpayable\", \"type\": \"function\" }, { \"inputs\": [ { \"internalType\": \"uint256\", \"name\": \"_points\", \"type\": \"uint256\" } ], \"name\": \"setNftPointForPlayerWinner\", \"outputs\": [], \"stateMutability\": \"nonpayable\", \"type\": \"function\" }, { \"inputs\": [ { \"internalType\": \"uint256[]\", \"name\": \"_nftIds\", \"type\": \"uint256[]\" }, { \"internalType\": \"enum NineWorldsMinigame.NftType[]\", \"name\": \"_nftTypes\", \"type\": \"uint8[]\" }, { \"internalType\": \"uint256[]\", \"name\": \"_powers\", \"type\": \"uint256[]\" } ], \"name\": \"setNftTypeAndPower\", \"outputs\": [], \"stateMutability\": \"nonpayable\", \"type\": \"function\" }, { \"inputs\": [ { \"internalType\": \"address\", \"name\": \"newOwner\", \"type\": \"address\" } ], \"name\": \"transferOwnership\", \"outputs\": [], \"stateMutability\": \"nonpayable\", \"type\": \"function\" }, { \"inputs\": [ { \"internalType\": \"uint256\", \"name\": \"_matchId\", \"type\": \"uint256\" }, { \"internalType\": \"uint8\", \"name\": \"_index\", \"type\": \"uint8\" } ], \"name\": \"getValidComputerNft\", \"outputs\": [ { \"internalType\": \"uint256\", \"name\": \"\", \"type\": \"uint256\" } ], \"stateMutability\": \"view\", \"type\": \"function\" }, { \"inputs\": [ { \"internalType\": \"uint256\", \"name\": \"_matchId\", \"type\": \"uint256\" }, { \"internalType\": \"uint8\", \"name\": \"_index\", \"type\": \"uint8\" } ], \"name\": \"getValidNft\", \"outputs\": [ { \"internalType\": \"uint256\", \"name\": \"\", \"type\": \"uint256\" } ], \"stateMutability\": \"view\", \"type\": \"function\" }, { \"inputs\": [ { \"internalType\": \"uint256\", \"name\": \"_matchId\", \"type\": \"uint256\" }, { \"internalType\": \"uint8\", \"name\": \"_index\", \"type\": \"uint8\" } ], \"name\": \"getValidPlayerNft\", \"outputs\": [ { \"internalType\": \"uint256\", \"name\": \"\", \"type\": \"uint256\" } ], \"stateMutability\": \"view\", \"type\": \"function\" }, { \"inputs\": [ { \"internalType\": \"uint256\", \"name\": \"\", \"type\": \"uint256\" } ], \"name\": \"matchesById\", \"outputs\": [ { \"internalType\": \"uint256\", \"name\": \"matchId\", \"type\": \"uint256\" }, { \"internalType\": \"uint256\", \"name\": \"nftMatchCount\", \"type\": \"uint256\" }, { \"internalType\": \"bool\", \"name\": \"isRerrollEnabled\", \"type\": \"bool\" }, { \"internalType\": \"uint256\", \"name\": \"matchRandomSeed\", \"type\": \"uint256\" }, { \"internalType\": \"bool\", \"name\": \"isMatchFinished\", \"type\": \"bool\" }, { \"internalType\": \"enum NineWorldsMinigame.MatchResult\", \"name\": \"matchResult\", \"type\": \"uint8\" } ], \"stateMutability\": \"view\", \"type\": \"function\" }, { \"inputs\": [ { \"internalType\": \"bytes32\", \"name\": \"\", \"type\": \"bytes32\" } ], \"name\": \"matchesByRequestId\", \"outputs\": [ { \"internalType\": \"uint256\", \"name\": \"\", \"type\": \"uint256\" } ], \"stateMutability\": \"view\", \"type\": \"function\" }, { \"inputs\": [], \"name\": \"maxMatchesPerDay\", \"outputs\": [ { \"internalType\": \"uint256\", \"name\": \"\", \"type\": \"uint256\" } ], \"stateMutability\": \"view\", \"type\": \"function\" }, { \"inputs\": [], \"name\": \"maxNftMatchCount\", \"outputs\": [ { \"internalType\": \"uint256\", \"name\": \"\", \"type\": \"uint256\" } ], \"stateMutability\": \"view\", \"type\": \"function\" }, { \"inputs\": [], \"name\": \"maxValidId\", \"outputs\": [ { \"internalType\": \"uint256\", \"name\": \"\", \"type\": \"uint256\" } ], \"stateMutability\": \"view\", \"type\": \"function\" }, { \"inputs\": [ { \"internalType\": \"uint256\", \"name\": \"\", \"type\": \"uint256\" } ], \"name\": \"nftMatchesByNftId\", \"outputs\": [ { \"internalType\": \"uint256\", \"name\": \"matchId\", \"type\": \"uint256\" }, { \"internalType\": \"uint256\", \"name\": \"nftMatchCount\", \"type\": \"uint256\" }, { \"internalType\": \"bool\", \"name\": \"isRerrollEnabled\", \"type\": \"bool\" }, { \"internalType\": \"uint256\", \"name\": \"matchRandomSeed\", \"type\": \"uint256\" }, { \"internalType\": \"bool\", \"name\": \"isMatchFinished\", \"type\": \"bool\" }, { \"internalType\": \"enum NineWorldsMinigame.MatchResult\", \"name\": \"matchResult\", \"type\": \"uint8\" } ], \"stateMutability\": \"view\", \"type\": \"function\" }, { \"inputs\": [], \"name\": \"nftPointForComputerTie\", \"outputs\": [ { \"internalType\": \"uint256\", \"name\": \"\", \"type\": \"uint256\" } ], \"stateMutability\": \"view\", \"type\": \"function\" }, { \"inputs\": [], \"name\": \"nftPointForComputerWinner\", \"outputs\": [ { \"internalType\": \"uint256\", \"name\": \"\", \"type\": \"uint256\" } ], \"stateMutability\": \"view\", \"type\": \"function\" }, { \"inputs\": [], \"name\": \"nftPointForPlayerTie\", \"outputs\": [ { \"internalType\": \"uint256\", \"name\": \"\", \"type\": \"uint256\" } ], \"stateMutability\": \"view\", \"type\": \"function\" }, { \"inputs\": [], \"name\": \"nftPointForPlayerWinner\", \"outputs\": [ { \"internalType\": \"uint256\", \"name\": \"\", \"type\": \"uint256\" } ], \"stateMutability\": \"view\", \"type\": \"function\" }, { \"inputs\": [ { \"internalType\": \"uint256\", \"name\": \"\", \"type\": \"uint256\" } ], \"name\": \"nftStatusById\", \"outputs\": [ { \"internalType\": \"enum NineWorldsMinigame.NftType\", \"name\": \"nftType\", \"type\": \"uint8\" }, { \"internalType\": \"uint256\", \"name\": \"totalPower\", \"type\": \"uint256\" }, { \"internalType\": \"uint8\", \"name\": \"dailyMatchCounter\", \"type\": \"uint8\" }, { \"internalType\": \"uint8\", \"name\": \"reRollCounter\", \"type\": \"uint8\" }, { \"internalType\": \"uint256\", \"name\": \"dailyExpirationTimestamp\", \"type\": \"uint256\" }, { \"internalType\": \"uint256\", \"name\": \"points\", \"type\": \"uint256\" }, { \"internalType\": \"uint256\", \"name\": \"currentMatchId\", \"type\": \"uint256\" } ], \"stateMutability\": \"view\", \"type\": \"function\" }, { \"inputs\": [], \"name\": \"ONE_DAY_IN_SECONDS\", \"outputs\": [ { \"internalType\": \"uint256\", \"name\": \"\", \"type\": \"uint256\" } ], \"stateMutability\": \"view\", \"type\": \"function\" }, { \"inputs\": [], \"name\": \"owner\", \"outputs\": [ { \"internalType\": \"address\", \"name\": \"\", \"type\": \"address\" } ], \"stateMutability\": \"view\", \"type\": \"function\" }, { \"inputs\": [ { \"internalType\": \"address\", \"name\": \"user\", \"type\": \"address\" } ], \"name\": \"pointBalanceOf\", \"outputs\": [ { \"internalType\": \"uint256\", \"name\": \"\", \"type\": \"uint256\" } ], \"stateMutability\": \"view\", \"type\": \"function\" }, { \"inputs\": [], \"name\": \"totalNfts\", \"outputs\": [ { \"internalType\": \"uint256\", \"name\": \"\", \"type\": \"uint256\" } ], \"stateMutability\": \"view\", \"type\": \"function\" }, { \"inputs\": [ { \"internalType\": \"address\", \"name\": \"\", \"type\": \"address\" } ], \"name\": \"usersLastMatchId\", \"outputs\": [ { \"internalType\": \"uint256\", \"name\": \"\", \"type\": \"uint256\" } ], \"stateMutability\": \"view\", \"type\": \"function\" }, { \"inputs\": [], \"name\": \"vikingsContract\", \"outputs\": [ { \"internalType\": \"contract IVikings\", \"name\": \"\", \"type\": \"address\" } ], \"stateMutability\": \"view\", \"type\": \"function\" } ]";

    const string nftABI = "[{\"inputs\":[{\"internalType\":\"uint256[]\",\"name\":\"_totalSupply\",\"type\":\"uint256[]\"},{\"internalType\":\"uint256[]\",\"name\":\"_initialIndex\",\"type\":\"uint256[]\"},{\"internalType\":\"uint256[]\",\"name\":\"_startTime\",\"type\":\"uint256[]\"},{\"internalType\":\"uint256[]\",\"name\":\"_endTime\",\"type\":\"uint256[]\"},{\"internalType\":\"uint256[]\",\"name\":\"_strength\",\"type\":\"uint256[]\"},{\"internalType\":\"uint256[]\",\"name\":\"_price\",\"type\":\"uint256[]\"},{\"internalType\":\"string[]\",\"name\":\"_baseUri\",\"type\":\"string[]\"},{\"internalType\":\"contract IERC20\",\"name\":\"_saleToken\",\"type\":\"address\"},{\"internalType\":\"address\",\"name\":\"_feeReceiver\",\"type\":\"address\"}],\"stateMutability\":\"nonpayable\",\"type\":\"constructor\"},{\"anonymous\":false,\"inputs\":[{\"indexed\":true,\"internalType\":\"address\",\"name\":\"owner\",\"type\":\"address\"},{\"indexed\":true,\"internalType\":\"address\",\"name\":\"approved\",\"type\":\"address\"},{\"indexed\":true,\"internalType\":\"uint256\",\"name\":\"tokenId\",\"type\":\"uint256\"}],\"name\":\"Approval\",\"type\":\"event\"},{\"anonymous\":false,\"inputs\":[{\"indexed\":true,\"internalType\":\"address\",\"name\":\"owner\",\"type\":\"address\"},{\"indexed\":true,\"internalType\":\"address\",\"name\":\"operator\",\"type\":\"address\"},{\"indexed\":false,\"internalType\":\"bool\",\"name\":\"approved\",\"type\":\"bool\"}],\"name\":\"ApprovalForAll\",\"type\":\"event\"},{\"anonymous\":false,\"inputs\":[{\"indexed\":true,\"internalType\":\"address\",\"name\":\"previousOwner\",\"type\":\"address\"},{\"indexed\":true,\"internalType\":\"address\",\"name\":\"newOwner\",\"type\":\"address\"}],\"name\":\"OwnershipTransferred\",\"type\":\"event\"},{\"anonymous\":false,\"inputs\":[{\"indexed\":false,\"internalType\":\"address\",\"name\":\"account\",\"type\":\"address\"}],\"name\":\"Paused\",\"type\":\"event\"},{\"anonymous\":false,\"inputs\":[{\"indexed\":true,\"internalType\":\"address\",\"name\":\"from\",\"type\":\"address\"},{\"indexed\":true,\"internalType\":\"address\",\"name\":\"to\",\"type\":\"address\"},{\"indexed\":true,\"internalType\":\"uint256\",\"name\":\"tokenId\",\"type\":\"uint256\"}],\"name\":\"Transfer\",\"type\":\"event\"},{\"anonymous\":false,\"inputs\":[{\"indexed\":false,\"internalType\":\"address\",\"name\":\"account\",\"type\":\"address\"}],\"name\":\"Unpaused\",\"type\":\"event\"},{\"anonymous\":false,\"inputs\":[{\"indexed\":true,\"internalType\":\"uint256\",\"name\":\"vikingId\",\"type\":\"uint256\"},{\"indexed\":false,\"internalType\":\"address\",\"name\":\"owner\",\"type\":\"address\"}],\"name\":\"VikingGenerated\",\"type\":\"event\"},{\"anonymous\":false,\"inputs\":[{\"indexed\":false,\"internalType\":\"uint256\",\"name\":\"vikingId\",\"type\":\"uint256\"},{\"indexed\":false,\"internalType\":\"bool\",\"name\":\"vikingStatus\",\"type\":\"bool\"}],\"name\":\"VikingLocked\",\"type\":\"event\"},{\"anonymous\":false,\"inputs\":[{\"indexed\":true,\"internalType\":\"uint256\",\"name\":\"vikingId\",\"type\":\"uint256\"},{\"indexed\":false,\"internalType\":\"address\",\"name\":\"owner\",\"type\":\"address\"},{\"indexed\":false,\"internalType\":\"uint256\",\"name\":\"price\",\"type\":\"uint256\"}],\"name\":\"VikingSale\",\"type\":\"event\"},{\"inputs\":[{\"internalType\":\"address\",\"name\":\"\",\"type\":\"address\"}],\"name\":\"alreadyMintedAmount\",\"outputs\":[{\"internalType\":\"uint256\",\"name\":\"\",\"type\":\"uint256\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"address\",\"name\":\"to\",\"type\":\"address\"},{\"internalType\":\"uint256\",\"name\":\"tokenId\",\"type\":\"uint256\"}],\"name\":\"approve\",\"outputs\":[],\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"address\",\"name\":\"owner\",\"type\":\"address\"}],\"name\":\"balanceOf\",\"outputs\":[{\"internalType\":\"uint256\",\"name\":\"\",\"type\":\"uint256\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[],\"name\":\"baseExtension\",\"outputs\":[{\"internalType\":\"string\",\"name\":\"\",\"type\":\"string\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"address[]\",\"name\":\"_accounts\",\"type\":\"address[]\"}],\"name\":\"deleteWhitelistBatch\",\"outputs\":[],\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"inputs\":[],\"name\":\"feeReceiver\",\"outputs\":[{\"internalType\":\"address\",\"name\":\"\",\"type\":\"address\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"uint256\",\"name\":\"tokenId\",\"type\":\"uint256\"}],\"name\":\"getApproved\",\"outputs\":[{\"internalType\":\"address\",\"name\":\"\",\"type\":\"address\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[],\"name\":\"getCurrentMaxSupply\",\"outputs\":[{\"internalType\":\"uint256\",\"name\":\"\",\"type\":\"uint256\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"address\",\"name\":\"owner\",\"type\":\"address\"},{\"internalType\":\"address\",\"name\":\"operator\",\"type\":\"address\"}],\"name\":\"isApprovedForAll\",\"outputs\":[{\"internalType\":\"bool\",\"name\":\"\",\"type\":\"bool\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"uint256\",\"name\":\"_tokenId\",\"type\":\"uint256\"}],\"name\":\"isVikingLocked\",\"outputs\":[{\"internalType\":\"bool\",\"name\":\"\",\"type\":\"bool\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"uint256\",\"name\":\"_tokenId\",\"type\":\"uint256\"},{\"internalType\":\"bool\",\"name\":\"status\",\"type\":\"bool\"}],\"name\":\"lockToken\",\"outputs\":[],\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"inputs\":[],\"name\":\"maxAmountMintTx\",\"outputs\":[{\"internalType\":\"uint256\",\"name\":\"\",\"type\":\"uint256\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"address\",\"name\":\"_tokenOwner\",\"type\":\"address\"},{\"internalType\":\"uint256\",\"name\":\"_type\",\"type\":\"uint256\"}],\"name\":\"mintByType\",\"outputs\":[],\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"uint256\",\"name\":\"\",\"type\":\"uint256\"}],\"name\":\"mintDataType\",\"outputs\":[{\"internalType\":\"uint256\",\"name\":\"totalSupply\",\"type\":\"uint256\"},{\"internalType\":\"uint256\",\"name\":\"currentSupply\",\"type\":\"uint256\"},{\"internalType\":\"uint256\",\"name\":\"initialIndex\",\"type\":\"uint256\"},{\"internalType\":\"uint256\",\"name\":\"startTime\",\"type\":\"uint256\"},{\"internalType\":\"uint256\",\"name\":\"endTime\",\"type\":\"uint256\"},{\"internalType\":\"uint256\",\"name\":\"strength\",\"type\":\"uint256\"},{\"internalType\":\"uint256\",\"name\":\"price\",\"type\":\"uint256\"},{\"internalType\":\"string\",\"name\":\"baseTokenUri\",\"type\":\"string\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"uint256\",\"name\":\"_type\",\"type\":\"uint256\"},{\"internalType\":\"uint256\",\"name\":\"_amount\",\"type\":\"uint256\"}],\"name\":\"mintSupplyLeft\",\"outputs\":[],\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"inputs\":[],\"name\":\"name\",\"outputs\":[{\"internalType\":\"string\",\"name\":\"\",\"type\":\"string\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[],\"name\":\"owner\",\"outputs\":[{\"internalType\":\"address\",\"name\":\"\",\"type\":\"address\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"uint256\",\"name\":\"tokenId\",\"type\":\"uint256\"}],\"name\":\"ownerOf\",\"outputs\":[{\"internalType\":\"address\",\"name\":\"\",\"type\":\"address\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[],\"name\":\"pause\",\"outputs\":[],\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"inputs\":[],\"name\":\"paused\",\"outputs\":[{\"internalType\":\"bool\",\"name\":\"\",\"type\":\"bool\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[],\"name\":\"renounceOwnership\",\"outputs\":[],\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"address\",\"name\":\"from\",\"type\":\"address\"},{\"internalType\":\"address\",\"name\":\"to\",\"type\":\"address\"},{\"internalType\":\"uint256\",\"name\":\"tokenId\",\"type\":\"uint256\"}],\"name\":\"safeTransferFrom\",\"outputs\":[],\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"address\",\"name\":\"from\",\"type\":\"address\"},{\"internalType\":\"address\",\"name\":\"to\",\"type\":\"address\"},{\"internalType\":\"uint256\",\"name\":\"tokenId\",\"type\":\"uint256\"},{\"internalType\":\"bytes\",\"name\":\"_data\",\"type\":\"bytes\"}],\"name\":\"safeTransferFrom\",\"outputs\":[],\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"uint256\",\"name\":\"_type\",\"type\":\"uint256\"},{\"internalType\":\"uint256\",\"name\":\"_amount\",\"type\":\"uint256\"}],\"name\":\"saleMint\",\"outputs\":[],\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"inputs\":[],\"name\":\"saleToken\",\"outputs\":[{\"internalType\":\"contract IERC20\",\"name\":\"\",\"type\":\"address\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"address\",\"name\":\"operator\",\"type\":\"address\"},{\"internalType\":\"bool\",\"name\":\"approved\",\"type\":\"bool\"}],\"name\":\"setApprovalForAll\",\"outputs\":[],\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"string\",\"name\":\"_baseExtension\",\"type\":\"string\"}],\"name\":\"setBaseExtension\",\"outputs\":[],\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"uint256\",\"name\":\"_mintType\",\"type\":\"uint256\"},{\"internalType\":\"string\",\"name\":\"_baseUri\",\"type\":\"string\"}],\"name\":\"setBaseURI\",\"outputs\":[],\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"address\",\"name\":\"_feeReceiver\",\"type\":\"address\"}],\"name\":\"setFeeReceiver\",\"outputs\":[],\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"uint256\",\"name\":\"_maxAmountMintTx\",\"type\":\"uint256\"}],\"name\":\"setMaxAmountMintTx\",\"outputs\":[],\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"uint256\",\"name\":\"_mintType\",\"type\":\"uint256\"},{\"internalType\":\"uint256\",\"name\":\"_totalSupply\",\"type\":\"uint256\"},{\"internalType\":\"uint256\",\"name\":\"_initialIndex\",\"type\":\"uint256\"},{\"internalType\":\"uint256\",\"name\":\"_startTime\",\"type\":\"uint256\"},{\"internalType\":\"uint256\",\"name\":\"_endTime\",\"type\":\"uint256\"},{\"internalType\":\"uint256\",\"name\":\"_strength\",\"type\":\"uint256\"},{\"internalType\":\"uint256\",\"name\":\"_price\",\"type\":\"uint256\"},{\"internalType\":\"string\",\"name\":\"_baseUri\",\"type\":\"string\"}],\"name\":\"setMintType\",\"outputs\":[],\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"address[]\",\"name\":\"_minters\",\"type\":\"address[]\"},{\"internalType\":\"bool[]\",\"name\":\"status\",\"type\":\"bool[]\"}],\"name\":\"setMinters\",\"outputs\":[],\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"contract IERC20\",\"name\":\"_saleToken\",\"type\":\"address\"}],\"name\":\"setSaleToken\",\"outputs\":[],\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"uint256\",\"name\":\"_whiteListStartTime\",\"type\":\"uint256\"},{\"internalType\":\"uint256\",\"name\":\"_whiteListEndTime\",\"type\":\"uint256\"},{\"internalType\":\"address[]\",\"name\":\"_accounts\",\"type\":\"address[]\"},{\"internalType\":\"uint256[]\",\"name\":\"_mintAllowed\",\"type\":\"uint256[]\"}],\"name\":\"setWhitelist\",\"outputs\":[],\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"address[]\",\"name\":\"_accounts\",\"type\":\"address[]\"},{\"internalType\":\"uint256[]\",\"name\":\"_mintAllowed\",\"type\":\"uint256[]\"}],\"name\":\"setWhitelistBatch\",\"outputs\":[],\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"bytes4\",\"name\":\"interfaceId\",\"type\":\"bytes4\"}],\"name\":\"supportsInterface\",\"outputs\":[{\"internalType\":\"bool\",\"name\":\"\",\"type\":\"bool\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[],\"name\":\"symbol\",\"outputs\":[{\"internalType\":\"string\",\"name\":\"\",\"type\":\"string\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"uint256\",\"name\":\"index\",\"type\":\"uint256\"}],\"name\":\"tokenByIndex\",\"outputs\":[{\"internalType\":\"uint256\",\"name\":\"\",\"type\":\"uint256\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"address\",\"name\":\"owner\",\"type\":\"address\"},{\"internalType\":\"uint256\",\"name\":\"index\",\"type\":\"uint256\"}],\"name\":\"tokenOfOwnerByIndex\",\"outputs\":[{\"internalType\":\"uint256\",\"name\":\"\",\"type\":\"uint256\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"uint256\",\"name\":\"_tokenId\",\"type\":\"uint256\"}],\"name\":\"tokenStrength\",\"outputs\":[{\"internalType\":\"uint256\",\"name\":\"\",\"type\":\"uint256\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"uint256\",\"name\":\"tokenId\",\"type\":\"uint256\"}],\"name\":\"tokenURI\",\"outputs\":[{\"internalType\":\"string\",\"name\":\"\",\"type\":\"string\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[],\"name\":\"totalSupply\",\"outputs\":[{\"internalType\":\"uint256\",\"name\":\"\",\"type\":\"uint256\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[],\"name\":\"totalTypes\",\"outputs\":[{\"internalType\":\"uint256\",\"name\":\"\",\"type\":\"uint256\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"address\",\"name\":\"from\",\"type\":\"address\"},{\"internalType\":\"address\",\"name\":\"to\",\"type\":\"address\"},{\"internalType\":\"uint256\",\"name\":\"tokenId\",\"type\":\"uint256\"}],\"name\":\"transferFrom\",\"outputs\":[],\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"address\",\"name\":\"newOwner\",\"type\":\"address\"}],\"name\":\"transferOwnership\",\"outputs\":[],\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"inputs\":[],\"name\":\"unpause\",\"outputs\":[],\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"uint256[]\",\"name\":\"_vikingIds\",\"type\":\"uint256[]\"},{\"internalType\":\"uint256[]\",\"name\":\"_strengths\",\"type\":\"uint256[]\"}],\"name\":\"updateStrength\",\"outputs\":[],\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"uint256\",\"name\":\"\",\"type\":\"uint256\"}],\"name\":\"vikings\",\"outputs\":[{\"internalType\":\"uint256\",\"name\":\"birthTime\",\"type\":\"uint256\"},{\"internalType\":\"uint256\",\"name\":\"strength\",\"type\":\"uint256\"},{\"internalType\":\"uint256\",\"name\":\"tokenType\",\"type\":\"uint256\"},{\"internalType\":\"bool\",\"name\":\"isLocked\",\"type\":\"bool\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"address\",\"name\":\"_owner\",\"type\":\"address\"}],\"name\":\"walletOfOwner\",\"outputs\":[{\"internalType\":\"uint256[]\",\"name\":\"\",\"type\":\"uint256[]\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"address\",\"name\":\"\",\"type\":\"address\"}],\"name\":\"whiteListAmountLeft\",\"outputs\":[{\"internalType\":\"uint256\",\"name\":\"\",\"type\":\"uint256\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[],\"name\":\"whiteListEndTime\",\"outputs\":[{\"internalType\":\"uint256\",\"name\":\"\",\"type\":\"uint256\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[],\"name\":\"whiteListStartTime\",\"outputs\":[{\"internalType\":\"uint256\",\"name\":\"\",\"type\":\"uint256\"}],\"stateMutability\":\"view\",\"type\":\"function\"}]";
    const string rpc = "https://polygon-mumbai.g.alchemy.com/v2/YK2vDo3Ieko9YEI1xldJmTmRkVPk7bmp";
    const string chain = "polygon";
    const string network = "testnet";
    const string contract = "0xB9679ce5248Ea71b66b97880c141F7F4AA6555c3";
    const string nftContract = "0xF2Cf7C7E757EEA9896e39ec50148df66DBea92ae";


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
        try
        {
            return await EVM.Call(chain, network, contract, abi, method, args, rpc);
        }
        catch (Exception e)
        {
            throw e;
        }
    }

    public async System.Threading.Tasks.Task<string> pointBalanceOf(string account)
    {
        string method = "pointBalanceOf";
        string args = "[\"" + account + "\"]";
        try
        {
            return await EVM.Call(chain, network, contract, abi, method, args, rpc);
        }
        catch (Exception e)
        {
            throw e;
        }
    }

    public async System.Threading.Tasks.Task<string> GetMatchInfoById(string id)
    {
        string method = "matchesById";
        string args = "[\"" + id + "\"]";
        try
        {
            return await EVM.Call(chain, network, contract, abi, method, args, rpc);
        }
        catch (Exception e)
        {
            throw e;
        }
    }

    public async System.Threading.Tasks.Task<string> GetValidPlayerNft(string matchId, string index)
    {
        string method = "getValidPlayerNft";
        string args = "[\"" + matchId + "\", \"" + index + "\"]";
        try
        {
            return await EVM.Call(chain, network, contract, abi, method, args, rpc);
        }
        catch (Exception e)
        {
            throw e;
        }
    }

    public async System.Threading.Tasks.Task<string> BalanceOf(string account)
    {
        string method = "balanceOf";
        string args = "[\"" + account + "\"]";
        try
        {
            return await EVM.Call(chain, network, nftContract, nftABI, method, args, rpc);
        }
        catch (Exception e)
        {
            throw e;
        }
    }

    public async System.Threading.Tasks.Task<string> TokenOfOwnerByIndex(string account, int index)
    {
        string method = "tokenOfOwnerByIndex";
        string args = "[\"" + account + "\", \"" + index + "\"]";
        try
        {
            return await EVM.Call(chain, network, nftContract, nftABI, method, args, rpc);
        }
        catch (Exception e)
        {
            throw e;
        }
    }

    public async System.Threading.Tasks.Task<string> GetNftStatusById(int index)
    {
        string method = "nftStatusById";
        string args = "[\"" + index + "\"]";
        try
        {
            return await EVM.Call(chain, network, contract, abi, method, args, rpc);
        }
        catch (Exception e)
        {
            throw e;
        }
    }

    public async System.Threading.Tasks.Task<string> GetNftPointForPlayerWinner()
    {
        string method = "nftPointForPlayerWinner";
        string args = "[]";
        try
        {
            return await EVM.Call(chain, network, contract, abi, method, args, rpc);
        }
        catch (Exception e)
        {
            throw e;
        }
    }

    public async System.Threading.Tasks.Task<string> GetNftPointForPlayerTie()
    {
        string method = "nftPointForPlayerTie";
        string args = "[]";
        try
        {
            return await EVM.Call(chain, network, contract, abi, method, args, rpc);
        }
        catch (Exception e)
        {
            throw e;
        }
    }


    public async System.Threading.Tasks.Task<string> GetValidComputerNft(string matchId, string index)
    {
        string method = "getValidComputerNft";
        string args = "[\"" + matchId + "\", \"" + index + "\"]";
        try
        {
            return await EVM.Call(chain, network, contract, abi, method, args, rpc);
        }
        catch (Exception e)
        {
            throw e;
        }
    }

    public async System.Threading.Tasks.Task<string> CreateMatch(int numberOfNfts)
    {
        string method = "createMatchAndRequestRandom";
        string args = "[\"" + numberOfNfts + "\"]";

        try
        {
            return await Web3GL.SendContract(method, abi, contract, args, value, gasLimit, gasPrice);
        }
        catch (Exception e)
        {
            throw e;
        }
    }

    public async System.Threading.Tasks.Task<string> DryCreateMatch(int numberOfNfts, string account)
    {
        string method = "createMatchAndRequestRandom";
        string args = "[\"" + numberOfNfts + "\"]";
        string data = await EVM.CreateContractData(abi, method, args);
        EstimateJsonRpc jrpc = new EstimateJsonRpc();
        jrpc.@params = new System.Object[] { new GasEstimateJsonRpcParams(account, contract, "0xA7A358200", "0xAA358", "0x0", data), "latest" };
        string body = JsonConvert.SerializeObject(jrpc);

        try
        {
            using (UnityWebRequest webRequest = UnityWebRequest.Post(rpc, ""))
            {
                webRequest.uploadHandler = new UploadHandlerRaw(new System.Text.UTF8Encoding().GetBytes(body));
                webRequest.SetRequestHeader("Content-Type", "application/json");
                await webRequest.SendWebRequest();
                EstimateErrorJsonRpc rpcError = new EstimateErrorJsonRpc();
                rpcError = JsonConvert.DeserializeObject<EstimateErrorJsonRpc>(webRequest.downloadHandler.text);
                return rpcError.error.message;
            }
        }
        catch (Exception e)
        {
            throw e;
        }
    }

    public async System.Threading.Tasks.Task<string> InitializeMatch(string account)
    {
        string method = "initializeMatchFor";
        string args = "[\"" + account + "\"]";

        try
        {
            return await Web3GL.SendContract(method, abi, contract, args, value, gasLimit, gasPrice);
        }
        catch (Exception e)
        {
            throw e;
        }
    }

    public async System.Threading.Tasks.Task<string> ResolveMatch()
    {
        string method = "resolveMatch";
        string args = "[]";

        try
        {
            return await Web3GL.SendContract(method, abi, contract, args, value, gasLimit, gasPrice);
        }
        catch (Exception e)
        {
            throw e;
        }
    }

    public async System.Threading.Tasks.Task<string> ResolveMatchReorder(int firstIndex, int secondIndex)
    {
        string method = "resolveMatchWithReorder";
        string args = "[\"" + firstIndex + "\", \"" + secondIndex + "\"]";

        try
        {
            return await Web3GL.SendContract(method, abi, contract, args, value, gasLimit, gasPrice);
        }
        catch (Exception e)
        {
            throw e;
        }
    }

}

# Odin 9 worlds: Erlog

## Description
Erlog is a NFT based mingame which uses Odin 9 worlds: Midgard weapon collention NFTs. To be able to play you need to own at least one of them. To obtain them go to: https://opensea.io/collection/o9wmidgard

## Rules
### Intro
This game is based on Rock, Paper, Scissors but with the 9worlds weapons which are Swords, Axes and Shields.

Swords wins axes, axes wins shields and shields wins swords.

Each match can be played with 1 to 5 NFTs. When a user creates a match he selects how many nfts he wants to play with. The max number of NFTs available to use depends on the number of NFTs the user owns.

### The basics

Once the match is created the game will ask for a random number to CHAINLINK VRF. This number is the seed to generate which NFTs of the user and the AI are selected and in which order.

Once you are in match you have to take into account that NFTs battle resolve vertically, one by one. This means in a config like this one:

```
               1      2      3
AI:          SWORD - AXE - SHIELD
               |      |      |
Playere:      AXE - SWORD - SHIELD
```
The results should be a tie, beacause AI wins battle 1, player wins battle 2, and battle 3 is tie.

Another example

```
               1      2      3
AI:          SWORD - AXE - SHIELD
               |      |      |
Playere:     AXE - SHIELD - SWORD
```
In this second one the result is AI winner because AI wins battle 1 and 2, and player only wins battle 3.

The agent(AI or player) which wins more battle win the match.

### Reorder matches
All Odin 9 worlds nft have a `Power` attribute, which is the sum of all its traits.

When you start a match each agent(AI or player) have a `Total power` attribute on the right of the game board. This number is the sum of all the `Power` attributes of each NFT on its hand.

If player `Total power` is bigger than AI `Total power` the player is able to make a reorder.

This reaorder mean swapping the position of 2 of your NFTs.

Let`s se the same example above:

```
               1      2      3
AI:          SWORD - AXE - SHIELD
               |      |      |
Playere:     AXE - SHIELD - SWORD
```
As we already said here AI wins because AI wins battle 1 and 2, and player only wins battle 3. But if we add the `Total power` we can make a reorder and change the result.

```
               1      2      3
AI:          SWORD - AXE - SHIELD     AI total power: 120
               |      |      |               |
Playere:     AXE - SHIELD - SWORD    Player total power: 200
```
User has more `Total power` then he can make a reorder. Lets swap player's axe by shield, the the match would be:
```
               1      2      3
AI:          SWORD - AXE - SHIELD     AI total power: 120
               |      |      |               |
Playere:    SHIELD - AXE - SWORD    Player total power: 200
```
Now player wins because AI doesn't win anything, and player wins battles 1 and 3. 

IMPORTANT: With reorder you changed match result! To be able to use reorder most of the time, make sure that you have the most powerful NFTs by checking: https://opensea.io/collection/o9wmidgard

## Prizes
Every time a NFT is on the winner side of a match, no matter if is in players hand or AI hand, it wins GEMS. This means when your NFT appears in the AI hand in a match of some random player, if AI wins that battle, your NFT also wins GEMs.

GEMs will have be one of the main currencies of Odins 9 worlds final game, so collect as much as posible.

## Technical info
This game was developed with Unity 2021.3.1f1 using Chainsafe unity web3 library: https://github.com/ChainSafe/web3.unity

## Future steps
This game will be launched on polygon mainnet point to mainnet Odin 9 Worlds Nfts: https://opensea.io/collection/o9wmidgard

There will be a more advanced UI design of the game with more information about GEMs, owned NFTs.

New gaem mechanis, there will be multiple machanics added to this main one on thw future mainnet game.

## Screenshots
![connect](/screenshots/connect.png "Connect")
![play](/screenshots/play.png "Play")
![Selector](/screenshots/selector.png "Selector")
![Game](/screenshots/game.png "Game")
![Reorder](/screenshots/reorder.png "Reorder")
![Result](/screenshots/result.png "Result")

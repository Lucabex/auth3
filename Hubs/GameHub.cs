using System.Collections.Concurrent;
using System.Security.Cryptography.X509Certificates;
using Microsoft.AspNetCore.SignalR;

namespace auth3.Hubs;

public class GameHub :Hub
{
    private static readonly ConcurrentQueue<string> _awaitingPlayer = new ConcurrentQueue<string>();
    private static readonly ConcurrentDictionary<string,string> _connectionToRoom = new ConcurrentDictionary<string,string>();

    public async Task SearchNewMatch(string username)
    {
        if (_awaitingPlayer.TryDequeue(out string? opponentConnectionId))
        {   
            // add the 2 player in the connectionToROmm dictionary
            string roomId = Guid.NewGuid().ToString();
            _connectionToRoom[Context.ConnectionId]= roomId;
            _connectionToRoom[opponentConnectionId] = roomId;

            //Add both player to signlaR group for future comunication
            await Groups.AddToGroupAsync(Context.ConnectionId,roomId);
            await Groups.AddToGroupAsync(opponentConnectionId, roomId);

            //NotifyFilters oppponent game found
            await Clients.Client(opponentConnectionId).SendAsync("MatchFound",roomId,username);

            //notify your FE
            await Clients.Caller.SendAsync("MatchFound",roomId,"opponent");


        }
        else
        {
            _awaitingPlayer.Enqueue(Context.ConnectionId);
            await Clients.Caller.SendAsync("WaitingForMatch");
        }
        }
        
        public async Task MakeOnLineMove(string roomId,object moveData)
    {
        await Clients.OthersInGroup(roomId).SendAsync("OpponentMove",moveData);
    }
    
    public async Task CancelSerch()
    {
        _connectionToRoom.TryRemove(Context.ConnectionId, out _);
    }

}
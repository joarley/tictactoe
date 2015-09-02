using Microsoft.AspNet.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace tictactoy.Hubs
{
    public class TicTacToyHub : Hub
    {
        Dictionary<int, object> jogadas = new Dictionary<int, object>();

        public void Jogar(int pos)
        {
            


        }
    }
}
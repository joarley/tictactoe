using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace tictactoy.TicTacToy
{
    public enum EstadoJogo
    {
        NaoIniciado,
        TurnoJogador1,
        TurnoJogador2,
        FinalizadoVitoriaJogador1,
        FinalizadoVitoriaJogador2
        FinalizadoEmpate
    }
}
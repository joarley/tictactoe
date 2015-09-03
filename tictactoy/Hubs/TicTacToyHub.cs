using Microsoft.AspNet.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using tictactoy.TicTacToy;

namespace tictactoy.Hubs
{
    public class TicTacToyHub : Hub
    {
        List<Jogo> jogosAtuais = new List<Jogo>();

        public void Jogar(int jogada)
        {
            var jogo = BuscaJogoDaConexao(Context.ConnectionId);

            if (!jogo.Iniciado)
                throw new Exception("Jogo não iniciado");

            var jogador = BuscaJogadorDaConexao(Context.ConnectionId);

            if (!jogo.EhTurnoDoJogador(jogador))
                throw new Exception("Não é o turno do jogador");

            if (jogo.JogadaDisponivel(jogada))
                throw new Exception("Jogada Invalida");

            jogo.Jogar(jogador, jogada);

            if (jogo.Finalizado)
            {
                if (jogo.Estado == EstadoJogo.FinalizadoEmpate)
                    EnviarEmpateParaTodos(jogo);
                else
                    EnviarVitoriaParaTodos(jogo);
                
                SalvarRank(jogo);
            }
            else
                EnviarJogadaOponente(jogo, jogo.BuscaOponente(jogador), jogada);
        }

        private Jogador BuscaJogadorDaConexao(string connectionId)
        {
            throw new NotImplementedException();
        }

        private void EnviarEmpateParaTodos(Jogo jogo)
        {
            throw new NotImplementedException();
        }

        private void EnviarJogadaOponente(Jogo jogo, Jogador oponente, int jogada)
        {
        }

        private Jogo BuscaJogoDaConexao(string connectionId)
        {
            return null;
        }

        private void EnviarJogadaOponente(int jogada)
        {
            
        }

        private void SalvarRank(Jogo jogo)
        {
        }

        private void EnviarVitoriaParaTodos(Jogo jogo)
        {
            var padraoVitoria = jogo.BuscaPadraoVitoria();
        }
    }
}
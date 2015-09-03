using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tictactoy.TicTacToy
{
    public class Jogo
    {
        private const int MAXIMO_JOGADAS = 9;

        Dictionary<int, Jogador> jogadas = new Dictionary<int, Jogador>();
        static int[][] padroesGanhador = new[] {
            new []{ 0, 1, 2 }, new []{ 3, 4, 5 }, new []{ 6, 7, 8 }, new []{ 0, 3, 6 },
            new []{ 1, 4, 7 }, new []{ 2, 5, 8 }, new []{ 0, 4, 8 }, new []{ 2, 4, 6 } };

        public bool Iniciado
        {
            get
            {
                return !(Estado == EstadoJogo.NaoIniciado || Finalizado);
            }
        }

        public bool Finalizado
        {
            get
            {
                return Estado == EstadoJogo.FinalizadoVitoriaJogador1 ||
                    Estado == EstadoJogo.FinalizadoVitoriaJogador2 ||
                    Estado == EstadoJogo.FinalizadoEmpate;
            }
        }

        public EstadoJogo Estado { get; set; }
        public Jogador Jogador1 { get; set; }
        public Jogador Jogador2 { get; set; }


        public bool JogadaDisponivel(int jogada)
        {
            return !jogadas.ContainsKey(jogada);
        }

        public void Jogar(Jogador jogador, int jogada)
        {
            if (!EhTurnoDoJogador(jogador))
                throw new Exception("Não é o turno do jogador");

            if (!JogadaDisponivel(jogada))
                throw new Exception("Jogada não disponivel");

            jogadas.Add(jogada, jogador);

            var padrao = BuscaPadraoVitoria();

            if (padrao != null)
            {
                if (jogador == Jogador1)
                    Estado = EstadoJogo.FinalizadoVitoriaJogador1;
                else if (jogador == Jogador2)
                    Estado = EstadoJogo.FinalizadoVitoriaJogador2;
            }
            else if (jogadas.Count == MAXIMO_JOGADAS)
            {
                Estado = EstadoJogo.FinalizadoEmpate;
            }
            else
                FinalizarTurno();
        }

        public int[] BuscaPadraoVitoria()
        {
            foreach (var padrao in padroesGanhador)
            {
                var existePadraoVitoria = jogadas.GroupBy(x => x.Value)
                    .Any(x => x.Select(y => y.Key).Intersect(padrao).Count() == padrao.Length);
                if (existePadraoVitoria)
                    return padrao;
            }
            return null;
        }

        public Jogador BuscaOponente(Jogador jogador)
        {
            if (Jogador1 == jogador)
                return Jogador2;
            if (Jogador2 == jogador)
                return Jogador1;

            throw new Exception("Jogador não faz parte do jogo");
        }

        public bool EhTurnoDoJogador(Jogador jogador)
        {
            if (Estado == EstadoJogo.TurnoJogador1 && Jogador1 == jogador)
                return true;
            if (Estado == EstadoJogo.TurnoJogador2 && Jogador2 == jogador)
                return true;
            return false;
        }

        private void FinalizarTurno()
        {
            if (Estado == EstadoJogo.TurnoJogador1)
                Estado = EstadoJogo.TurnoJogador2;
            else if (Estado == EstadoJogo.TurnoJogador2)
                Estado = EstadoJogo.TurnoJogador1;
        }
    }
}

using LiteDB;
using Microsoft.AspNet.SignalR;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Web;
using tictactoy.TicTacToy;

namespace tictactoy.Hubs
{
    public class TicTacToyHub : Hub
    {
        static Dictionary<Jogador, string> jogadoresConnectionId = new Dictionary<Jogador, string>();
        static List<Jogo> jogos = new List<Jogo>();
        static string Database { get { return Path.Combine(AssemblyDirectory, "data.db"); } }

        public override Task OnReconnected()
        {
            var jogo = BuscaJogoDaConexao(Context.ConnectionId);

            if (jogo != null)
                EnviarStatusJogo(jogo);

            return base.OnReconnected();
        }

        public override Task OnDisconnected(bool stopCalled)
        {
            SairJogo();
            return base.OnDisconnected(stopCalled);
        }

        public void NovoJogo(string nomeJogador)
        {
            SairJogo();

            lock (jogos)
            {
                var jogo = jogos.FirstOrDefault(x => x.Jogador2 == null);
                if (jogo == null)
                {
                    jogo = new Jogo()
                    {
                        Estado = EstadoJogo.NaoIniciado,
                        Jogador1 = new Jogador()
                        {
                            Nome = nomeJogador,
                            Simbolo = Simbolo.Xis
                        }
                    };

                    jogos.Add(jogo);
                    jogadoresConnectionId[jogo.Jogador1] = Context.ConnectionId;
                }
                else
                {
                    jogo.Jogador2 = new Jogador()
                    {
                        Nome = nomeJogador,
                        Simbolo = Simbolo.Circulo
                    };
                    jogo.Estado = EstadoJogo.TurnoJogador1;
                    jogadoresConnectionId[jogo.Jogador2] = Context.ConnectionId;

                }

                EnviarStatusJogo(jogo);
            }
        }

        public void Jogar(int jogada)
        {
            var jogo = BuscaJogoDaConexao(Context.ConnectionId);
            if (jogo == null)
                return;

            if (!jogo.Iniciado)
                throw new Exception("Jogo não iniciado");

            var jogador = BuscaJogadorDaConexao(Context.ConnectionId);

            if (!jogo.EhTurnoDoJogador(jogador))
                throw new Exception("Não é o turno do jogador");

            if (!jogo.JogadaDisponivel(jogada))
                throw new Exception("Jogada Invalida");

            jogo.Jogar(jogador, jogada);

            if (jogo.Finalizado)
                try
                {
                    Task.Run(() =>
                    {
                        SalvarRank(jogo);
                    });
                }
                catch (Exception) { }

            EnviarStatusJogo(jogo);
        }

        public IEnumerable<Rank> BuscarRank()
        {
            var rank = new Dictionary<string, Rank>();
            using (var db = new LiteDatabase(Database))
            {
                var col = db.GetCollection<Jogo>("jogos");
                var todosJogos = col.FindAll();

                foreach (var jogo in todosJogos)
                {
                    if (!rank.ContainsKey(jogo.Jogador1.Nome))
                        rank[jogo.Jogador1.Nome] = new Rank()
                        {
                            Jogador = jogo.Jogador1.Nome
                        };
                    if (!rank.ContainsKey(jogo.Jogador2.Nome))
                        rank[jogo.Jogador2.Nome] = new Rank()
                        {
                            Jogador = jogo.Jogador2.Nome
                        };


                    if (jogo.Finalizado)
                    {
                        if (jogo.Estado == EstadoJogo.FinalizadoVitoriaJogador1)
                        {
                            rank[jogo.Jogador1.Nome].Ganhados++;
                            rank[jogo.Jogador2.Nome].Perdidos++;
                            rank[jogo.Jogador1.Nome].Pontos += 3;
                        }

                        if (jogo.Estado == EstadoJogo.FinalizadoVitoriaJogador2)
                        {
                            rank[jogo.Jogador2.Nome].Ganhados++;
                            rank[jogo.Jogador1.Nome].Perdidos++;
                            rank[jogo.Jogador2.Nome].Pontos += 3;
                        }

                        if (jogo.Estado == EstadoJogo.FinalizadoEmpate)
                        {
                            rank[jogo.Jogador2.Nome].Empatados++;
                            rank[jogo.Jogador1.Nome].Empatados++;
                            rank[jogo.Jogador2.Nome].Pontos += 1;
                            rank[jogo.Jogador1.Nome].Pontos += 1;
                        }
                    }
                }
            }

            return rank.Values.OrderByDescending(x => x.Pontos).
                ThenByDescending(x => x.Ganhados).
                ThenByDescending(x => x.Empatados);
        }

        public void SairJogo()
        {
            lock (jogos)
            {
                var jogo = BuscaJogoDaConexao(Context.ConnectionId);
                if (jogo == null)
                    return;
                jogo.Estado = EstadoJogo.Cancelado;
                EnviarStatusJogo(jogo);
                jogos.Remove(jogo);
                jogadoresConnectionId.Remove(jogo.Jogador1);
                if (jogo.Jogador2 != null)
                    jogadoresConnectionId.Remove(jogo.Jogador2);
            }
        }

        #region private

        private Jogador BuscaJogadorDaConexao(string connectionId)
        {
            var jogador = jogadoresConnectionId.FirstOrDefault(x => x.Value == connectionId);
            return jogador.Key;
        }

        private void EnviarStatusJogo(Jogo jogo)
        {
            try
            {
                var connectionIdJogador1 = BuscarIdConexaoJogador(jogo.Jogador1);

                var conexaoJogador1 = this.Clients.Client(connectionIdJogador1);

                conexaoJogador1.AtualizarJogo(jogo);
                if (jogo.Jogador2 != null)
                {
                    var connectionIdJogador2 = BuscarIdConexaoJogador(jogo.Jogador2);
                    var conexaoJogador2 = this.Clients.Client(connectionIdJogador2);
                    conexaoJogador2.AtualizarJogo(jogo);
                }
            }
            catch (Exception) { }
        }

        private string BuscarIdConexaoJogador(Jogador jogador)
        {
            return jogadoresConnectionId[jogador];
        }

        private Jogo BuscaJogoDaConexao(string connectionId)
        {
            var jogador = BuscaJogadorDaConexao(connectionId);
            if (jogador == null)
                return null;

            return jogos.FirstOrDefault(x => x.Jogador1 == jogador || x.Jogador2 == jogador);
        }

        private void SalvarRank(Jogo jogo)
        {
            using (var db = new LiteDatabase(Database))
            {
                var col = db.GetCollection<Jogo>("jogos");
                col.Insert(jogo);
            }
        }

        private static string AssemblyDirectory
        {
            get
            {
                string codeBase = this.GetType().Assembly.CodeBase;
                UriBuilder uri = new UriBuilder(codeBase);
                string path = Uri.UnescapeDataString(uri.Path);
                return Path.GetDirectoryName(path);
            }
        }
        #endregion
    }
}
var svg = null;
var ticTacToyHub = null;
var jogo = null;
var meuNome = null;
var meuTurno = false;

Snap.load("/App/img/tabuleiro.svg", function (f) {
    Snap("#svg").append(f);

    svg = Snap("#svg svg");
    limparVisualizacao();

    ticTacToyHub = $.connection.ticTacToyHub;

    ticTacToyHub.client.atualizarJogo = function (jogoAtualizado) {
        jogo = jogoAtualizado;
        limparVisualizacao();
        meuTurno = false;


        if (jogo == null)
            return;

        for (var i = 0; i < 9; i++) {
            var jogada = jogo.Jogadas[i];
            if (jogada) {
                if (jogada.Simbolo == 1)
                    svg.select("#cir_" + i).attr({ visibility: "visible" });
                else
                    svg.select("#xis_" + i).attr({ visibility: "visible" });
            }
        }

        if (jogo.PadraoVitoria) {
            svg.select("#win_" + jogo.PadraoVitoria[0] +
                "_" + jogo.PadraoVitoria[1] +
                "_" + jogo.PadraoVitoria[2]).attr({ visibility: "visible" });

            if ((jogo.Jogador1.Nome == meuNome && jogo.Estado == 3) ||
                (jogo.Jogador2.Nome == meuNome && jogo.Estado == 4))
                $("#status").text("Você Ganhou")
            else
                $("#status").text("Você Perdeu")
            jogo = null;
            return;
        }

        if (jogo.Estado == 6) {
            $("#status").text("Jogo finalizado por um dos jogadores");
            jogo = null;
            return;
        }

        if (jogo.Jogador2 == null) {
            $("#status").text("Aguardando oponente");
            return;
        }

        if (jogo.Iniciado) {
            if ((jogo.Jogador1.Nome == meuNome && jogo.Estado == 1) ||
                (jogo.Jogador2.Nome == meuNome && jogo.Estado == 2)) {
                meuTurno = true;
            }

            $("#status").html(jogo.Jogador1.Nome + " x " + jogo.Jogador2.Nome + "<br>(Turno " +
                (jogo.Estado == 1 ? jogo.Jogador1.Nome : jogo.Jogador2.Nome)
                + ")");
        }
    };

    $.connection.hub.start();

    $("#iniciarNovoJogo").click(function () {
        if ($("#nomeInput").val() == "")
            alert("Nome Invalido");
        else {
            ticTacToyHub.server.novoJogo($("#nomeInput").val());
            meuNome = $("#nomeInput").val();
        }
    });
    $("#novoJogo").click(function (e) {
        if (jogo != null && !jogo.Finalizado && confirm("Deseja parar o jogo atual?")) {
            ticTacToyHub.server.sairJogo();
        } else if (jogo != null) {
            e.stopPropagation();
        }
    });
    $("#rank").click(function () {
        ticTacToyHub.server.buscarRank().done(function (rank) {
            $("#rankTable").html("");

            for (var i = 0; i < rank.length; i++) {
                var tr = $("<tr>");
                tr.append($("<td>").text(i + 1));
                tr.append($("<td>").text(rank[i].Jogador));
                tr.append($("<td>").text(rank[i].Ganhados));
                tr.append($("<td>").text(rank[i].Empatados));
                tr.append($("<td>").text(rank[i].Perdidos));
                tr.append($("<td>").text(rank[i].Pontos));

                $("#rankTable").append(tr);
            }
        });
    });
    $("#sairJogo").click(function () {
        if (jogo != null && confirm("Deseja parar o jogo atual?")) {
            ticTacToyHub.server.sairJogo();
        }
    });
    $("#click_0").click(function () {
        if (meuTurno && !jogo.Jogadas[0]) {
            ticTacToyHub.server.jogar(0);
        }
    });
    $("#click_1").click(function () {
        if (meuTurno && !jogo.Jogadas[1]) {
            ticTacToyHub.server.jogar(1);
        }
    });
    $("#click_2").click(function () {
        if (meuTurno && !jogo.Jogadas[2]) {
            ticTacToyHub.server.jogar(2);
        }
    });
    $("#click_3").click(function () {
        if (meuTurno && !jogo.Jogadas[3]) {
            ticTacToyHub.server.jogar(3);
        }
    });
    $("#click_4").click(function () {
        if (meuTurno && !jogo.Jogadas[4]) {
            ticTacToyHub.server.jogar(4);
        }
    });
    $("#click_5").click(function () {
        if (meuTurno && !jogo.Jogadas[5]) {
            ticTacToyHub.server.jogar(5);
        }
    });
    $("#click_6").click(function () {
        if (meuTurno && !jogo.Jogadas[6]) {
            ticTacToyHub.server.jogar(6);
        }
    });
    $("#click_7").click(function () {
        if (meuTurno && !jogo.Jogadas[7]) {
            ticTacToyHub.server.jogar(7);
        }
    });
    $("#click_8").click(function () {
        if (meuTurno && !jogo.Jogadas[8]) {
            ticTacToyHub.server.jogar(8);
        }
    });
});



function limparVisualizacao() {
    $("#status").text("Jogo não iniciado");
    svg.select("#xis_0").attr({ visibility: "hidden" });
    svg.select("#xis_1").attr({ visibility: "hidden" });
    svg.select("#xis_2").attr({ visibility: "hidden" });
    svg.select("#xis_3").attr({ visibility: "hidden" });
    svg.select("#xis_4").attr({ visibility: "hidden" });
    svg.select("#xis_5").attr({ visibility: "hidden" });
    svg.select("#xis_6").attr({ visibility: "hidden" });
    svg.select("#xis_7").attr({ visibility: "hidden" });
    svg.select("#xis_8").attr({ visibility: "hidden" });
    svg.select("#cir_0").attr({ visibility: "hidden" });
    svg.select("#cir_1").attr({ visibility: "hidden" });
    svg.select("#cir_2").attr({ visibility: "hidden" });
    svg.select("#cir_3").attr({ visibility: "hidden" });
    svg.select("#cir_4").attr({ visibility: "hidden" });
    svg.select("#cir_5").attr({ visibility: "hidden" });
    svg.select("#cir_6").attr({ visibility: "hidden" });
    svg.select("#cir_7").attr({ visibility: "hidden" });
    svg.select("#cir_8").attr({ visibility: "hidden" });
    svg.select("#win_0_1_2").attr({ visibility: "hidden" });
    svg.select("#win_3_4_5").attr({ visibility: "hidden" });
    svg.select("#win_6_7_8").attr({ visibility: "hidden" });
    svg.select("#win_0_3_6").attr({ visibility: "hidden" });
    svg.select("#win_1_4_7").attr({ visibility: "hidden" });
    svg.select("#win_2_5_8").attr({ visibility: "hidden" });
    svg.select("#win_0_4_8").attr({ visibility: "hidden" });
    svg.select("#win_2_4_6").attr({ visibility: "hidden" });
    svg.select("#clicks").attr({ opacity: "0" });
}
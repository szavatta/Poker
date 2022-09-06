// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

var timer = null;
$(document).ready(function () {

    GetPartitaCorrente();
    DisegnaHtml();

    $("#btnNuovaPartita").click(function () {
        NuovaPartita();
    });

    $("#btnPescaCartaTavolo").click(function () {
        $.ajax({
            type: "GET",
            url: "/home/PescaCartaTavolo",
            dataType: "json",
            success: function (partita) {
                AggiornaTavolo(partita);
            },
            error: function (xhr) {
                alert(xhr.responseText.replace("System.Exception: ", "").split("\r\n")[0]);
            }
        });
    });

    $("#btnDistribuisciCarte").click(function () {
        $.ajax({
            type: "GET",
            url: "/home/DistribuisciCarte",
            dataType: "json",
            success: function (partita) {
                AggiornaTavolo(partita);
            },
            error: function (xhr) {
                alert(xhr.responseText.replace("System.Exception: ", "").split("\r\n")[0]);
            }
        });
    });

    $(document).on("change", "#inputnome", function () {
        $.ajax({
            type: "GET",
            url: "/home/ModificaNomeGiocatore",
            dataType: "json",
            data: {
                id: $("#IdGiocatore").val(),
                nome: $("#inputnome").val()
            },
            success: function (partita) {
                AggiornaTavolo(partita);
            },
            error: function (xhr) {
                alert(xhr.responseText.replace("System.Exception: ", "").split("\r\n")[0]);
            }
        });
    });

    $(document).on("click", ".btnPuntata", function () {
        var importo = $(this).siblings(".txPuntata").val();
        $(this).siblings(".txPuntata").val("");
        if (importo == "") {
            alert("Inserire un importo");
            return;
        }
        $.ajax({
            type: "POST",
            url: "/home/Puntata",
            dataType: "json",
            data: {
                id: $("#IdGiocatore").val(),
                importo: importo
            },
            success: function (ret) {
                AggiornaTavolo(ret.partita);
            },
            error: function (xhr) {
                alert(xhr.responseText.replace("System.Exception: ", "").split("\r\n")[0]);
            }
        });
    });

    $(document).on("click", ".btnPassa", function () {
        $.ajax({
            type: "GET",
            url: "/home/Passa",
            dataType: "json",
            data: {
                id: $("#IdGiocatore").val()
            },
            success: function (ret) {
                AggiornaTavolo(ret.partita);
            },
            error: function (xhr) {
                alert(xhr.responseText.replace("System.Exception: ", "").split("\r\n")[0]);
            }
        });
    });

    $(document).on("click", ".btnCheck", function () {
        $.ajax({
            type: "GET",
            url: "/home/Check",
            dataType: "json",
            data: {
                id: $("#IdGiocatore").val()
            },
            success: function (ret) {
                AggiornaTavolo(ret.partita);
            },
            error: function (xhr) {
                alert(xhr.responseText.replace("System.Exception: ", "").split("\r\n")[0]);
            }
        });
    });

    $(document).on("click", ".btnAllIn", function () {
        $.ajax({
            type: "GET",
            url: "/home/Allin",
            dataType: "json",
            data: {
                id: $("#IdGiocatore").val()
            },
            success: function (ret) {
                AggiornaTavolo(ret.partita);
            },
            error: function (xhr) {
                alert(xhr.responseText.replace("System.Exception: ", "").split("\r\n")[0]);
            }
        });
    });

    $(document).on("click", ".btnVedi", function () {
        var obj = $(this);
        $.ajax({
            type: "GET",
            url: "/home/Vedi",
            dataType: "json",
            data: {
                id: $("#IdGiocatore").val()
            },
            success: function (ret) {
                AggiornaTavolo(ret.partita);
            },
            error: function (xhr) {
                alert(xhr.responseText.replace("System.Exception: ", "").split("\r\n")[0]);
            }
        });
    });

    $("#btnGetVincitore").click(function () {
        GetVincitore();
    });

    $("#btnAggiorna").click(function () {
        GetPartitaCorrente();
    });

    SetInterval();
})

function SetInterval() {
    if (timer == null) {
        timer = setInterval(function () {
            GetPartitaCorrente();
        }, 5000);
    }
}

function GetPartitaCorrente() {

    $.ajax({
        type: "GET",
        url: "/home/GetPartita",
        dataType: "json",
        success: function (partita) {
            AggiornaTavolo(partita);
        },
        error: function (xhr) {
            alert(xhr.responseText.replace("System.Exception: ", "").split("\r\n")[0]);
        }
    });
}

function NuovaPartita() {

    $.ajax({
        type: "GET",
        url: "/home/NuovaPartita",
        dataType: "json",
        success: function (partita) {
            AggiornaTavolo(partita);
        },
        error: function (xhr) {
            alert(xhr.responseText.replace("System.Exception: ", "").split("\r\n")[0]);
        }
    });
}

function GetVincitore() {

    $.ajax({
        type: "GET",
        url: "/home/GetVincitore",
        dataType: "json",
        success: function (id) {
            $(".bred").removeClass("bred");
            $("#carte-g" + id + " .nomeg").addClass("bred");
        },
        error: function (xhr) {
            alert(xhr.responseText.replace("System.Exception: ", "").split("\r\n")[0]);
        }
    });
}

function DisegnaHtml() {
    $("#carte-tavolo").append("<div class='carte'></div>")
        .append("<div class='divcredito'></div>");

    for (var i = 0; i < 4; i++) {
        $("#carte-g" + i).append("<div class='carte'></div>")
            .append("<div class='nomeg'>" + (i == 0 && $("#IdGiocatore").val() != "-1" ? "<input id='inputnome' type='text' value=''>" : "") + "</div>")
            .append("<div class='divcredito'></div>")
            .append("<div class='divpuntata'></div>");
        if (i == 0) {
            $("#carte-g" + i).append("<div class='divpunta d-none'>"
                + "<input type='text' class='txPuntata' value='' style='width: 50px' />"
                + "<button class='btnPuntata'>Punta</button>"
                + "<button class='btnVedi'>Vedi</button>"
                + "<button class='btnPassa'>Passa</button>"
                + "<button class='btnCheck'>Check</button>"
                + "<button class='btnAllIn'>All in</button>"
                + "</div>");
            $("#carte-g" + i).append("<div class='divPulsantiMazziere d-none'>"
                + "<button id='btnDistribuisciCarte'>Distribuisci carte</button>"
                + "</div>");
        }
    }
}

function VisualizzaCarte(giocatore, tavolo, divid, idg, idm, partita) {
    $("#" + divid).find(".carte").empty();

    //if (divid == "carte-tavolo" && tavolo.carte.length == 0) {
    //    var div = $("<div class='carta'></div>");
    //    div.css("background-image", "url('carte/retroblu.png')");
    //    $("#" + divid).find(".carte").append(div);
    //}
    if (giocatore != null && giocatore != undefined) {
        carte = giocatore.carte;
    } else if (tavolo != null && tavolo != undefined) {
        carte = tavolo.carte;
    }
    carte.forEach(function (item, index) {
        var div = $("<div class='carta'></div>");
        div.attr("title", item.numeroString + " di " + item.semeString).css("border", "1px solid").css("background-color", "white")/*.css("left", -65 * index)*/;
        //div.css("background-image", "url('data:image/png;base64," + item.immagineBase64 + "')");

        //var path = item.pathImage;
        //if (divid != "carte-tavolo") {
        //    if (idg != $("#IdGiocatore").val() && partita.stato == 1) {
        //        path = "carte/retroblu.png";
        //    }
        //}
        //div.css("background-image", "url('" + path + "')").attr("title", item.numeroString + " di " + item.semeString)/*.css("left", -65*index)*/;

        $("#" + divid).find(".carte").append(div);
    });
    if (giocatore != null && giocatore != undefined && giocatore.nome != "") {
        if (divid == "carte-g0" && $("#IdGiocatore").val() != "-1") {
            if ($("#" + divid).find("#inputnome").is(":focus") == false) {
                $("#" + divid).find("#inputnome").val(giocatore.nome);
            }
        } else {
            $("#" + divid).find(".nomeg").html(giocatore.nome);
        }
    }
    if (giocatore != null && giocatore != undefined) {
        $("#" + divid).find(".divcredito").html("Credito: " + giocatore.credito);
        if (giocatore.puntata != 0) {
            $("#" + divid).find(".divpuntata").html("Puntata: " + giocatore.puntata);
        } else if (giocatore.isCheck == true) {
            $("#" + divid).find(".divpuntata").html("Check");
        } else if (giocatore.isAllIn == true) {
            $("#" + divid).find(".divpuntata").html(giocatore.puntataAllIn + " All-In");
        } else {
            $("#" + divid).find(".divpuntata").html("");
        }
        if (idm == $("#IdGiocatore").val() && giocatore.credito > 0 && partita.stato == 1) {
            $("#" + divid).find(".divpunta").removeClass("d-none");
        } else {
            $("#" + divid).find(".divpunta").addClass("d-none");
        }
        if (giocatore.uscito || giocatore.terminato) {
            $("#" + divid).find(".nomeg").addClass("bred");
        } else {
            $("#" + divid).find(".nomeg").removeClass("bred");
        }
    }

    if (tavolo != null && tavolo != undefined) {
        if (tavolo.credito != 0) {
            $("#" + divid).find(".divcredito").html("Posta: " + tavolo.credito);
        } else {
            $("#" + divid).find(".divcredito").html("");
        }
    }
}

function VisualizzaPunteggio(punteggio, divid) {
    var div = $("<div>" + punteggio + "</div>");
    $("#" + divid).append(div);
}

function AggiornaTavolo(partita) {

    var idg = $("#IdGiocatore").val();
    document.title = partita.giocatori[idg].nome;

    if (partita.idMazziere == idg && (partita.stato == 0 || partita.stato == 2)) {
        $(".divPulsantiMazziere").removeClass("d-none");
    } else {
        $(".divPulsantiMazziere").addClass("d-none");
    }

    VisualizzaCarte(null, partita.tavolo, "carte-tavolo");
    if (idg < 0) {
        idg = 0;
    }

    $(".nomeg").removeClass("bgreen");
    for (var i = 0; i < partita.giocatori.length; i++) {
        var ind = i - idg;
        if (ind < 0) {
            ind = 4 + ind;
        }

        VisualizzaCarte(partita.giocatori[i], null, "carte-g" + ind, i, partita.mano, partita);
        //VisualizzaPunteggio(partita.giocatori[i].punteggio.tipoString, "carte-g" + i)

        if (i == partita.mano) {
            $("#carte-g" + ind + " .nomeg").addClass("bgreen");
        }

    }

    $("#divlogs").empty().append("<ul class='list-group'>");
    partita.logs.forEach(function (log) {
        $("#divlogs").append("<li class='list-group-item'>" + log.testo + "</li>");
    });
    $("#divlogs").append("</ul>");
}





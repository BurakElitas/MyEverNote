$(function () {
    var noteids = [];
    $("div[data-note-id]").each(function (i, e) {
        noteids.push($(e).data("note-id"));

    });
    console.log(noteids);
    $.ajax({
        method: "POST",
        url: "/Note/GetLiked/",
        data: { ids: noteids }

    }).done(function (data) {

        if (data.state) {
            for (var i = 0; i < data.sonuc.length; i++) {
                var id = data.sonuc[i];
                var div = $("div[data-note-id=" + id + "]");
                var button = div.find("button[data-liked]");
                var icon = button.children().first();
                icon.removeClass("far");
                icon.addClass("fas");
                button.data("liked", true);

            }
        }


    });
    $("button[data-liked]").click(function () {
        var btn = $(this);
        var state = $(this).data("liked");
        var noteid = $(this).data("note-id");
        var icon = $(this).children().first();
        $.ajax({
            method: "POST",
            url: "/Note/SetlikedUser",
            data: { "noteid": noteid, "state": !state }

        }).done(function (data) {
            if (data.hasError) {
                alert(data.errorMessage);
            }
            else {
                icon.removeClass("far");
                icon.removeClass("fas");
                icon.addClass("" + data.style);
                icon.empty();
                icon.text(" " + data.likeCount);
                btn.data("liked", data.newstate);

            }
        });


    });




});
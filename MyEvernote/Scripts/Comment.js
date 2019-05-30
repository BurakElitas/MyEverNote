var model_comment_body = '#modal_comment_body';
var id = -1;
$(function () {

    $('#modal_comment').on('show.bs.modal', function (event) {


        var button = $(event.relatedTarget);
        var noteid = button.data("note-id");
        $(model_comment_body).load("/Comment/ShowNoteComments/" + noteid);
        id = noteid;
    })
});
function doComment(btn, e, commentid, spanid) {
    var button = $(btn);
    var mode = button.data("edit-mode");
    if (e === "edit_clicked") {
        if (!mode) {
            button.removeClass("btn-outline-warning");
            button.addClass("btn-outline-success");
            button.data("edit-mode", true);
            var icon = button.find("i");
            icon.removeClass("fa-edit");
            icon.addClass("fa-check");
            $(spanid).attr("contenteditable", true);
            $(spanid).addClass("edit_comment");
            $(spanid).focus();
        }
        else {
            button.removeClass("btn-outline-success");
            button.addClass("btn-outline-warning");
            button.data("edit-mode", false);
            var icon = button.find("i");
            icon.removeClass("fa-check");
            icon.addClass("fa-edit");
            $(spanid).attr("contenteditable", false);
            $(spanid).removeClass("edit_comment");
            var txt = $(spanid).text();

            $.ajax({
                method: "POST",
                url: "/Comment/Edit/" + commentid,
                data: { text: txt }

            }).done(function (data) {
                if (data.result) {

                    $(model_comment_body).load("/Comment/ShowNoteComments/" + id);
                }
                else {
                    alert("Yorum Güncellenemdi.");
                }

            }).fai(function () {
                alert("Sunucu ile bağlantı kurulamadı.");
            });

        }


    }
    else if (e === "delete_clicked") {
        var result = confirm("Yorumunuzu Silmek İstediğinizden Emin Misiniz?");
        if (result) {
            $.ajax({
                method: "GET",
                url: "/Comment/Delete/" + commentid
            }).done(function (data) {
                if (data.result) {
                    $(model_comment_body).load("/Comment/ShowNoteComments/" + id);
                }
                else {
                    alert("Yorum Silinemedi");
                }

            }).fail(function () {
                alert("Sunucu ile bağlantı kurulamadı.");
            });
        }
        else
            return false;

    }
    else if (e === "add_comment") {
        var txt = $("#add_com").val();
            $.ajax({
                method: "POST",
                url: "/Comment/AddComment/",
                data: { Text:txt, "noteid":id}
            }).done(function (data) {
                if (data.result) {
                    $(model_comment_body).load("/Comment/ShowNoteComments/" + id);
                }
                else {
                    alert("Yorum Eklenemedi");
                }

            }).fail(function () {
                alert("Sunucu ile bağlantı kurulamadı.");
            });

        }
    }





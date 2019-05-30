
    $(function () {
        $('#Detail').on('show.bs.modal', function (e) {
            var noteid = $(e.relatedTarget).data("id");
            $("#Detail_body").load("/Note/ShowDetail/" + noteid);

        });




    });

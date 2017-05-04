$(function () {
    $("#username-text").on("blur", function () {
        $.post("/home/checkuser", { name: $("#username-text").text() }, function (valid) {
            if (!valid) {
                $("#nonvalid-user").text("Sorry not a valid user name");
            }
        })
    })
});
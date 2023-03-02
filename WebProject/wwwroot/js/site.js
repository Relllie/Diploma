function readURL(input, a) {
    if (input.files && input.files[0]) {
        var reader = new FileReader();
        reader.onload = function (e) {
            $(a).attr('src', e.target.result).width(350).height(300);
        };
        reader.readAsDataURL(input.files[0]);
    }
}
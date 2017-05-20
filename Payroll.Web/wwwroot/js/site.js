// Write your Javascript code.
$(document).on('change', '.btn-file :file', function() {
    var input = $(this);
    var numFiles = input.get(0).files ? input.get(0).files.length : 1;
    var label = input.val().replace(/\\/g, '/').replace(/.*\//, '');
    input.trigger('file-change', [numFiles, label]);
});

$('.btn-file :file').on('file-change', function(event, numfiles, label) {
    $('.btn-label').text(label);
    _.isEmpty(label)
        ? $('.payslip .btn-success').attr('disabled', 'disabled')
        : $('.payslip .btn-success').removeAttr('disabled');
});
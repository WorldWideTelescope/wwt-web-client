var assert = chai.assert;


describe('Layer', function() {

  it('Layer.getStringFromGzipBlob should keep non-compressed strings as-is', function(done) {

    var csv = 'a,b,c\r\n266,-29,1\r\n267,-29,3\r\n267,-30,5\r\n266,-30,10\r\n';
    var blob = new Blob([csv])

    var layer = new wwtlib.SpreadSheetLayer()
    layer.getStringFromGzipBlob(blob, function(data) {
        assert.equal(data, csv);
        done()
    });

  });

  it('Layer.getStringFromGzipBlob should decompress compressed strings', function(done) {

    var csv = 'a,b,c\r\n266,-29,1\r\n267,-29,3\r\n267,-30,5\r\n266,-30,10\r\n';

    // The above string was compressed and converted to base64 - we then decode the base64
    // string into a byte array that we construct a blob with
    var byteCharacters = atob('eJxL1EnSSeblMjIz09E1stQxBDHNwUxjKNPYQMcUqgDINDTg5QIAAcIJFg==');
    const byteNumbers = new Array(byteCharacters.length);
    for (let i = 0; i < byteCharacters.length; i++) {
        byteNumbers[i] = byteCharacters.charCodeAt(i);
    }
    const byteArray = new Uint8Array(byteNumbers);
    var blob = new Blob([byteArray], {type: 'application/gzip'})

    var layer = new wwtlib.SpreadSheetLayer()
    layer.getStringFromGzipBlob(blob, function(data) {
        assert.equal(data, csv);
        done()
    });

  });

});

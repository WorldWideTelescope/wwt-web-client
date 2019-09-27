var assert = chai.assert;


describe('SpreadSheetLayer', function() {

  it('should export XML with backward-compatibility for size normalization', function(done) {

    // Set up a SpreadSheetLayer and make use of the size normalization functionality
    var csv = 'a,b,c\r\n266,-29,1\r\n267,-29,3\r\n267,-30,5\r\n266,-30,10\r\n';
    var layer = new wwtlib.SpreadSheetLayer()
    layer.updateData(csv, true, true, true)
    layer.set_sizeColumn(1);
    layer.set_normalizeSize(true);
    layer.set_normalizeSizeMin(-31);
    layer.set_normalizeSizeMax(-27);
    layer.set_normalizeSizeClip(false);

    // Export to XML
    var xmlWriter = new wwtlib.XmlTextWriter();
    layer.saveToXml(xmlWriter);
    xmlWriter._pending = true;
    xmlWriter._writePending(true);

    // We call addFilesToCabinet after getting the XML of the layer since this
    // is the order in which things happen when saving a tour.
    var fc = new wwtlib.FileCabinet();
    layer.addFilesToCabinet(fc);
    assert.equal(fc.fileList.length, 1);

    // Now use a standard XML parser to load and check for attributes
    parser = new DOMParser();
    xmlDoc = parser.parseFromString(xmlWriter.body, "text/xml");
    layer_xml = xmlDoc.getElementsByTagName('Layer')[0]

    // sizeColumn should be set to a new fourth column (index=3 as 0-based)
    // while NormalizeSizeColumn is set to the actual column to scale
    assert.equal(layer_xml.getAttribute('SizeColumn'), 3);
    assert.equal(layer_xml.getAttribute('NormalizeSizeColumn'), 1);

    // Check that the normalization attributes are set correctly
    assert.equal(JSON.parse(layer_xml.getAttribute('NormalizeSize')), true);
    assert.equal(JSON.parse(layer_xml.getAttribute('NormalizeSizeMin')), -31);
    assert.equal(JSON.parse(layer_xml.getAttribute('NormalizeSizeMax')), -27);
    assert.equal(JSON.parse(layer_xml.getAttribute('NormalizeSizeClip')), false);

    // Now check that when loading back we get the same as the initial values
    var new_layer = new wwtlib.Layer.fromXml(layer_xml);
    assert.equal(new_layer.sizeColumn, 1);
    assert.equal(new_layer.normalizeSize, true);
    assert.equal(new_layer.normalizeSizeMin, -31);
    assert.equal(new_layer.normalizeSizeMax, -27);
    assert.equal(new_layer.normalizeSizeClip, false);

    // Finally check content of table as serialized in the FileCabinet, and
    // make sure it has the extra column with the normalized sizes. We need
    // to do this last since we need to pass the done() function down into
    // the callback.
    const reader = new FileReader();
    reader.addEventListener('loadend', (e) => {
        const text = e.srcElement.result;
        assert.equal(text, "a\tb\tc\tdfe78b4c-f972-4796-b04f-68c5efd4ecb0\r\n266\t-29\t1\t0.5\r\n267\t-29\t3\t0.5\r\n267\t-30\t5\t0.25\r\n266\t-30\t10\t0.25\r\n");
        done()
    });
    reader.readAsText(fc.fileList[0].blob);

  });

});

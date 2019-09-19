var assert = chai.assert;


describe('SpreadSheetLayer', function() {

  it('should export XML with backward-compatibility for size normalization', function() {

    // Set up a SpreadSheetLayer and make use of the size normalization functionality
    var csv = 'a,b,c\r\n266,-29,1\r\n267,-29,3\r\n267,-30,5\r\n266,-30,10\r\n';    
    var layer = new wwtlib.SpreadSheetLayer()
    layer.updateData(csv, true, true, true)
    layer.set_sizeColumn(1);
    layer.set_normalizeSize(true);
    layer.set_normalizeSizeMin(2);
    layer.set_normalizeSizeMax(10);
    layer.set_normalizeSizeClip(false);

    // Make sure we add the table to a file cabinet, as this triggers the addition
    // of the normalized size column
    var fc = new wwtlib.FileCabinet()
    layer.addFilesToCabinet(fc)

    // Export to XML
    var xmlWriter = new wwtlib.XmlTextWriter();
    layer.saveToXml(xmlWriter);
    xmlWriter._pending = true;
    xmlWriter._writePending(true);

    // Now use a standard XML parser to load and check for attributes
    parser = new DOMParser();
    xmlDoc = parser.parseFromString(xmlWriter.body, "text/xml");
    layer_xml = xmlDoc.getElementsByTagName('Layer')[0]

    // sizeColumn should be set to a new fourth column (index=3 as 0-based)
    // while NormalizeSizeColumn is set to the actual column to scale
    assert.equal(layer_xml.getAttribute('SizeColumn'), 3);
    assert.equal(layer_xml.getAttribute('NormalizeColumn'), 1);

    // Check that the normalization attributes are set correctly
    assert.equal(JSON.parse(layer_xml.getAttribute('NormalizeSize')), true);
    assert.equal(JSON.parse(layer_xml.getAttribute('NormalizeSizeMin')), 2.);
    assert.equal(JSON.parse(layer_xml.getAttribute('NormalizeSizeMax')), 10.);
    assert.equal(JSON.parse(layer_xml.getAttribute('NormalizeSizeClip')), false);

  });

});
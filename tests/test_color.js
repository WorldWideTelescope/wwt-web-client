var assert = chai.assert;

describe('Color', function() {

  it('should parse strings', function() {
    var color = wwtlib.Color.fromName("red");
    assert.equal(color.a, 255);
    assert.equal(color.r, 255);
    assert.equal(color.g, 0);
    assert.equal(color.b, 0);
  });

  it('should parse simple hex', function() {
    var color = wwtlib.Color.fromSimpleHex("ffaa8811");
    assert.equal(color.a, 255);
    assert.equal(color.r, 170);
    assert.equal(color.g, 136);
    assert.equal(color.b, 17);
  });

  it('should convert to simple hex', function() {
    var color = wwtlib.Color.fromName("red");
    assert.equal(color.toSimpleHex(), "FFFF0000")
    assert.equal(color.a, 255);
    assert.equal(color.r, 255);
    assert.equal(color.g, 0);
    assert.equal(color.b, 0);
  });


});
var assert = chai.assert;

describe('Color', function() {
  it('should parse strings', function() {
    var color = wwtlib.Color.fromName("red");
    assert.equal(color.a, 255);
    assert.equal(color.r, 255);
    assert.equal(color.g, 0);
    assert.equal(color.b, 0);
  });
});
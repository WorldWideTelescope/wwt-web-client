var assert = chai.assert;

describe('ColorMapContainer', function() {

  it('should initialize from nested ARGB lists', function() {
    var lists = [[10, 20, 30, 40], [20, 30, 20, 10]];
    var cmap = wwtlib.ColorMapContainer.fromArgbList(lists);
    assert.equal(cmap.colors.length, 2)
    assert.equal(cmap.colors[0].toString(), '#141E28');
    assert.equal(cmap.colors[0].a, 10);
    assert.equal(cmap.colors[1].toString(), '#1E140A');
    assert.equal(cmap.colors[1].a, 20);
  });

  it('should initialize from a list of strings', function() {
    var colors = ['red', 'green', 'blue']
    var cmap = wwtlib.ColorMapContainer.fromStringList(colors);
    assert.equal(cmap.colors.length, 3)
    assert.equal(cmap.colors[0].toString(), '#FF0000');
    assert.equal(cmap.colors[1].toString(), '#008000');
    assert.equal(cmap.colors[2].toString(), '#0000FF');
});

  it('should find the closest color', function() {
    var colors = ['red', 'green', 'blue']
    var cmap = wwtlib.ColorMapContainer.fromStringList(colors);
    assert.equal(cmap.findClosestColor(0.00).toString(), '#FF0000');
    assert.equal(cmap.findClosestColor(0.10).toString(), '#FF0000');
    assert.equal(cmap.findClosestColor(0.32).toString(), '#FF0000');
    assert.equal(cmap.findClosestColor(0.34).toString(), '#008000');
    assert.equal(cmap.findClosestColor(0.50).toString(), '#008000');
    assert.equal(cmap.findClosestColor(0.66).toString(), '#008000');
    assert.equal(cmap.findClosestColor(0.67).toString(), '#0000FF');
    assert.equal(cmap.findClosestColor(0.80).toString(), '#0000FF');
    assert.equal(cmap.findClosestColor(1.00).toString(), '#0000FF');
  });

  it('should get a colormap by name', function() {
    var cmap = wwtlib.ColorMapContainer.fromNamedColormap('greys');
    assert.equal(cmap.findClosestColor(0.51).toString(), '#999999');
    var cmap = wwtlib.ColorMapContainer.fromNamedColormap('GREYS');
    assert.equal(cmap.findClosestColor(0.51).toString(), '#999999');
    var cmap = wwtlib.ColorMapContainer.fromNamedColormap('Viridis');
    assert.equal(cmap.findClosestColor(0.51).toString(), '#218E8D');
  });

});

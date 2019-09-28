var assert = chai.assert;

describe('Table', function() {

  it('should load data from CSV strings', function() {
    var table = new wwtlib.Table();
    table.loadFromString('a,b,c\r\n1,2,3.5\r\n4,5,6.5\r\n', false, true, true);
    assert.deepEqual(table.header, ['a', 'b', 'c']);
    assert.equal(table.rows.length, 2)
    assert.deepEqual(table.rows[0], ['1', '2', '3.5']);
    assert.deepEqual(table.rows[1], ['4', '5', '6.5']);
  });

  it('should save an existing table correctly', function() {
    var table = new wwtlib.Table();
    table.header = ['d', 'e', 'f'];
    table.rows = [[4, 6, 'l'], [2, 2, 'k']];
    assert.equal(table.save(), 'd\te\tf\r\n4\t6\tl\r\n2\t2\tk\r\n');
  });

  it('should copy cleanly when using clone()', function() {
    var table1 = new wwtlib.Table();
    table1.header = ['d', 'e', 'f'];
    table1.rows = [[4, 6, 'l'], [2, 2, 'k']];
    var table2 = table1.clone()
    table2.rows[0][1] = 9
    assert.equal(table1.save(), 'd\te\tf\r\n4\t6\tl\r\n2\t2\tk\r\n');
    assert.equal(table2.save(), 'd\te\tf\r\n4\t9\tl\r\n2\t2\tk\r\n');
  });

  it('should include a new column when calling addColumn', function() {
    var table = new wwtlib.Table();
    table.header = ['d', 'e', 'f'];
    table.rows = [[4, 6, 'l'], [2, 2, 'k']];
    table.addColumn('p', [4.5, 9])
    assert.equal(table.save(), 'd\te\tf\tp\r\n4\t6\tl\t4.5\r\n2\t2\tk\t9\r\n');
  });

  it('should remove a column when calling removeColumn', function() {
    var table = new wwtlib.Table();
    table.header = ['d', 'e', 'f'];
    table.rows = [[4, 6, 'l'], [2, 2, 'k']];
    table.removeColumn('e');
    assert.equal(table.save(), 'd\tf\r\n4\tl\r\n2\tk\r\n');
  });

});
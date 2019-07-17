var chai = require('chai');
var expect = chai.expect;

import {filterList, getFilterOptions} from '../../../client/src/js/filter';

const mockBetList = [
    {
      bet: 3.2,
      name: "a test bet",
      bet_won: true
    },
    {
      bet: 1,
      name: "another test bet",
      bet_won: null
    },
    {
      bet: 5,
      name: "third test bet",
      bet_won: false
    }
  ];

describe('filterList', function(){

  it('includes limit value in results', function(done){
    let filterOptions = [{
      key: "bet",
      type: "number",
      lowerLimit: 1,
      upperLimit: 5
    }];

    let resultList = filterList(mockBetList, filterOptions);

    expect(resultList.length).to.equal(3);

    done();
  });

  it('filters correctly with lowerLimit', function(done){
    let filterOptions = [{
      key: "bet",
      type: "number",
      lowerLimit: 3.3
    }];

    let resultList = filterList(mockBetList, filterOptions);

    expect(resultList.length).to.equal(1);
    expect(resultList[0].bet).to.equal(5);

    done();
  });

  it('filters correctly with upperLimit', function(done){
    let filterOptions = [{
      key: "bet",
      type: "number",
      upperLimit: 3.1
    }];

    let resultList = filterList(mockBetList, filterOptions);

    expect(resultList.length).to.equal(1);
    expect(resultList[0].bet).to.equal(1);

    done();
  });

  it('filters correctly on string', function(done){
    let filterOptions = [{
      key: "name",
      type: "string",
      value: "other"
    }];

    let resultList = filterList(mockBetList, filterOptions);

    expect(resultList.length).to.equal(1);
    expect(resultList[0].bet).to.equal(1);

    done();
  });

  it('filters correctly on boolean', function(done){
    let filterOptions = [{
      key: "bet_won",
      type: "boolean",
      value: false
    }];

    let resultList = filterList(mockBetList, filterOptions);

    expect(resultList.length).to.equal(1);
    expect(resultList[0].bet).to.equal(5);

    done();
  });

  it('filters correctly on multiple filters', function(done){
    let filterOptions = [
      {
      key: "bet_won",
      type: "boolean",
      value: null
    },
    {
      key: "bet",
      type: "number",
      upperLimit: 4
    }];

    let resultList = filterList(mockBetList, filterOptions);

    expect(resultList.length).to.equal(1);
    expect(resultList[0].bet).to.equal(1);
    expect(resultList[0].name).to.equal("another test bet");

    done();
  });
});

describe('getFilterOptions', function(){

  it('returns excpcted filterOption for number, first array value is lowerLimit',
  function(done){

    let filter1 = getFilterOptions('number', 'bet', [0, 1]);
    let filter2 = getFilterOptions('number', 'bet', [1, 0]);

    expect(typeof filter1).to.equal('object');
    expect(filter1.lowerLimit).to.equal(0);
    expect(filter1.upperLimit).to.equal(1);

    expect(typeof filter2).to.equal('object');
    expect(filter2.lowerLimit).to.equal(1);
    expect(filter2.upperLimit).to.equal(0);
    done();
  });

  it('returns expected option with string', function(done){
    let filter = getFilterOptions('string', 'key', ['searchWord']);

    expect(typeof filter).to.equal('object');
    expect(filter.type).to.equal('string');
    expect(filter.value).to.equal('searchWord');
    done();
  });

  it('returns expected option with boolean filter', function(done){
    let filter = getFilterOptions('boolean', 'key', [true]);

    expect(typeof filter).to.equal('object');
    expect(filter.type).to.equal('boolean');
    expect(filter.value).to.equal(true);
    done();
  });
});

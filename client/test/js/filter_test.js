var chai = require('chai');
var expect = chai.expect;
var moment = require('moment');
var _ = require('lodash');

import {filterList, getFilterOptions} from '../../src/js/filter';

const mockBetList = [
    {
      bet: 3.2,
      name: "a test bet",
      bet_won: true,
      datetime: '2019-01-01 12:00'
    },
    {
      bet: 1,
      name: "another test bet",
      bet_won: null,
      datetime: '2019-01-01 13:00'
    },
    {
      bet: 5,
      name: "third test bet",
      bet_won: false,
      datetime: '2019-01-01 14:00'
    }
  ];

describe('filterList', function(){

  it('filters correctly based on datetime', function(done){
    let filterOptions = [{
      key: "datetime",
      type: "dateTime",
      before: moment('2019-01-01 13:30'),
      after: moment('2019-01-01 12:30'),
    }];

    const resultList = filterList(mockBetList, filterOptions);

    expect(resultList.length).to.equal(1);
    expect(resultList[0].bet).to.equal(1);

    done();
  });

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

  it('ignores case for string filter', function(done){
    let filterOptions = [{
      key: "name",
      type: "string",
      value: "OTHER"
    }];

    let resultList = filterList(mockBetList, filterOptions);

    expect(resultList.length).to.equal(1);
    expect(resultList[0].bet).to.equal(1);

    done();
  });

  it('filters correctly on valueList', function(done){
    let filterOptions = [{
      key: "bet_won",
      type: "valueList",
      value: [false]
    }];

    let resultList = filterList(mockBetList, filterOptions);

    expect(resultList.length).to.equal(1);
    expect(resultList[0].bet).to.equal(5);

    done();
  });

  it ('filters none based on valueList 0-length valuelist', function(done){
    let filterOptions = [{
      key: "bet_won",
      type: "valueList",
      value: []
    }];

    const resultList = filterList(mockBetList, filterOptions);

    expect(resultList.length).to.equal(3);

    done();
  });

  it('filters all given parameters in valueList filter', function(done){
    let filterOptions = [{
      key: "bet_won",
      type: "valueList",
      value: [false, null]
    }];

    let resultList = filterList(mockBetList, filterOptions);

    expect(resultList.length).to.equal(2);

    done();
  });

  it('filters correctly on multiple filters', function(done){
    let filterOptions = [
      {
      key: "bet_won",
      type: "valueList",
      value: [null]
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

  it('returns expected option for dateTime', function(done){
    const before = '2019-01-02 13:30';
    const after = '2019-01-01 12:30';
    const filter = getFilterOptions('dateTime', 'testKey', [after, before]);

    expect(filter.after.isSame(moment(after))).to.equal(true);
    expect(filter.before.isSame(moment(before))).to.equal(true);

    done();
  });

  it('returns null value for invalid datetime', function(done){
    const before = '';
    const after = 'invalid date';
    const filter = getFilterOptions('dateTime', 'testKey', [after, before]);

    expect(_.isNil(filter.after)).to.equal(true);
    expect(_.isNil(filter.before)).to.equal(true);

    done();
  });

  it('boolean filterOption contains array of values',
    function(done){
      const filter = getFilterOptions('valueList', 'test', [true, false]);

      expect(filter.value.length).to.equal(2);
      expect(filter.value[0]).to.equal(true);
      expect(filter.value[1]).to.equal(false);
      expect(filter.type).to.equal('valueList');

      done();
    });

  it('returns expected filterOption for number, first array value is lowerLimit',
  function(done){

    const filter1 = getFilterOptions('number', 'bet', [0, 1]);
    const filter2 = getFilterOptions('number', 'bet', [1, 0]);

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
});
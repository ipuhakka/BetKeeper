var chai = require('chai');
var expect = chai.expect;

import {filterList, getFilterOptions} from '../../../client/src/js/filter';

function mockBetList(){

  return [
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
}

describe('filterList', function(){

  it('filters correctly on over', function(done){
    let filterOptions = [{
      key: "bet",
      option: "over",
      value: 3.2
    }];

    let resultList = filterList(mockBetList(), filterOptions);

    expect(resultList.length).to.equal(1);
    expect(resultList[0].bet).to.equal(5);

    done();
  });

  it('filters correctly on under', function(done){
    let filterOptions = [{
      key: "bet",
      option: "under",
      value: 3.2
    }];

    let resultList = filterList(mockBetList(), filterOptions);

    expect(resultList.length).to.equal(1);
    expect(resultList[0].bet).to.equal(1);

    done();
  });

  it('filters correctly on contains', function(done){
    let filterOptions = [{
      key: "name",
      option: "contains",
      value: "other"
    }];

    let resultList = filterList(mockBetList(), filterOptions);

    expect(resultList.length).to.equal(1);
    expect(resultList[0].bet).to.equal(1);

    done();
  });

  it('filters correctly on is', function(done){
    let filterOptions = [{
      key: "bet_won",
      option: "is",
      value: false
    }];

    let resultList = filterList(mockBetList(), filterOptions);

    expect(resultList.length).to.equal(1);
    expect(resultList[0].bet).to.equal(5);

    done();
  });

  it('filters correctly on multiple filters', function(done){
    let filterOptions = [
      {
      key: "bet_won",
      option: "is",
      value: null
    },
    {
      key: "bet",
      option: "under",
      value: 4
    }];

    let resultList = filterList(mockBetList(), filterOptions);

    expect(resultList.length).to.equal(1);
    expect(resultList[0].bet).to.equal(1);
    expect(resultList[0].name).to.equal("another test bet");

    done();
  });
});

describe('getFilterOptions', function(){

  it('returns two filterOptions on between and options are identical regardless of value order',
  function(done){

    let filters1 = getFilterOptions('number', [0, 1], 'key', 'between');
    let filters2 = getFilterOptions('number', [1, 0], 'key', 'between');

    expect(filters1).to.deep.equal(filters2);
    expect(filters1.length).to.equal(2);

    done();
  });

  it('returns one filterOption with contains when filter is type text', function(done){
    let filters = getFilterOptions('text', ['searchWord'], 'key'); //option does not matter

    expect(filters.length).to.equal(1);
    expect(filters[0].option).to.equal('contains');

    done();
  });

  it('returns one filterOption with is when filter is type bool', function(done){
    let filters = getFilterOptions('bool', [true], 'key'); //option does not matter

    expect(filters.length).to.equal(1);
    expect(filters[0].option).to.equal('is');

    done();
  });
});

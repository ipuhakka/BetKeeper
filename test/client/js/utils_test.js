var chai = require('chai');
var expect = chai.expect;
var chaiHttp = require('chai-http');
chai.use(chaiHttp);
var should = chai.should();

import {isValidDouble, isValidString, filterList} from '../../../client/src/js/utils';

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

describe('isValidString', function(){

  it ('returns true on not containing non-allowed characters', function(done){
    expect(isValidString('Tämän pitäisi olla validi')).to.equal(true);
    done();
  })

  it('returns false on containing non-allowed character', function(done){
    expect(isValidString('This ; should not pass')).to.equal(false);
    done();
  });
});

describe('isValidDouble', function(){

  it('returns false on empty value', function(done){
    expect(isValidDouble("")).to.equals(false);
    done();
  })

  it('returns false when value begins with decimal point', function(done){
    expect(isValidDouble(".2")).to.equals(false);
    done();
  })

  it('returns false when value ends with decimal point', function(done){
    expect(isValidDouble("2.")).to.equals(false);
    done();
  })

  it('returns true on valid double given as string', function(done){
    expect(isValidDouble("2.2")).to.equals(true);
    done();
  })

  it('returns true on valid double', function(done){
    expect(isValidDouble(2.2)).to.equals(true);
    done();
  })
})

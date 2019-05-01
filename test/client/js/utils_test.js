var chai = require('chai');
var expect = chai.expect;

import {isValidDouble, isValidString} from '../../../client/src/js/utils';

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
